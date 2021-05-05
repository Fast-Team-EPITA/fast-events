using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FastEvents.DataAccess.Interfaces;
using FastEvents.dbo;
using FastEvents.Models;
using QRCoder;
using Event = FastEvents.dbo.Event;
using Stat = FastEvents.dbo.Stat;
using Ticket = FastEvents.dbo.Ticket;

// TODO Remove event if finished
// TODO Tests unitaires
// TODO Add page number in index
// TODO Ajouter des logs

namespace FastEvents.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IEventRepository _eventRepository;
        private readonly IEventUiRepository _eventUiRepository;
        private readonly ITicketRepository _ticketRepository;
        private readonly IStatRepository _statRepository;
        private readonly QRCodeGenerator _qrCodeGenerator;

        private string _userId;

        public string QrCodesPath =
            Path.Join(Directory.GetCurrentDirectory(), "wwwroot", "Resources", "QRCodes");
        public string ImagesPath =
            Path.Join(Directory.GetCurrentDirectory(), "wwwroot", "Resources", "Images");

        public HomeController(ILogger<HomeController> logger, IEventUiRepository eventUiRepository,
            ITicketRepository ticketRepository, IStatRepository statRepository, IEventRepository eventRepository)
        {
            _logger = logger;
            _eventUiRepository = eventUiRepository;
            _ticketRepository = ticketRepository;
            _statRepository = statRepository;
            _eventRepository = eventRepository;
            _qrCodeGenerator = new QRCodeGenerator();
        }


        /**
         *  Data access
         */
        private void GetUserIdFromCookies()
        {
            if (HttpContext?.Request.Cookies == null)
                return;
            
            var (key, value) = HttpContext.Request.Cookies.FirstOrDefault(x => x.Key == "userId");
            if (key == null)
            {
                _userId = Guid.NewGuid().ToString();
                HttpContext.Response.Cookies.Append("userId", _userId);
            }
            else
                _userId = value;
        }


        /**
         *  View Navigation
         */
        public async Task<IActionResult> Index(Category? sortCategory = null, string sortType = null, bool ownedEvents = false, string searchPattern = null, int pageNumber = 0)
        {
            GetUserIdFromCookies();
            var events = (await _eventUiRepository.Get()).ToList();
            

            if (sortCategory != null)
                events = SortByCategory(sortCategory, events);

            if (sortType != null)
                events = SortByType(sortType, events);

            if (ownedEvents)
                events = SortOwnedEvents(events);

            if (searchPattern != null)
                events = SortSearchPattern(searchPattern, events);


            var eventsToDisplay = events.Skip(pageNumber * 10).Take(10).ToList();


            var model = new IndexViewModel(eventsToDisplay, pageNumber);
            return View(model);
        }

        [Route("detail/{eventId:long}")]
        public async Task<IActionResult> Detail(long eventId)
        {
            GetUserIdFromCookies();
            var stat = new Stat { Date = DateTime.Now, EventId = eventId };
            await _statRepository.Insert(stat);

            var selectedEvent = _eventUiRepository.GetById(eventId);
            var isOwner = selectedEvent.OwnerUuid == _userId;
            var tickets = _ticketRepository.GetByOwnerId(_userId);
            var hasTicket = tickets?.FirstOrDefault(ticket => ticket.EventId == eventId) != null;

            var model = new DetailViewModel { EventUi = selectedEvent, IsOwner = isOwner, HasTicket = hasTicket };
            return View(model);
        }

        [Route("edit/{eventId:long?}")]
        public IActionResult CreateOrEdit(long? eventId = null)
        {
            var eventUi = eventId.HasValue ? _eventUiRepository.GetById(eventId.Value) : new EventUi();
            var model = new CreateOrEditViewModel { EventUi = eventUi, IsCreate = !eventId.HasValue };
            return View(model);
        }

        [Route("tickets")]
        public IActionResult Tickets()
        {
            GetUserIdFromCookies();
            var model = new TicketsViewModel(_ticketRepository.GetByOwnerId(_userId));
            return View(model);
        }

        [Route("stat/{eventId:long}")]
        public IActionResult Stat(long eventId)
        {
            var stat = _statRepository.GetByEvent(eventId);
            return Json(stat);
        }

        public IActionResult Privacy()
        {
            return View();
        }
        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        /**
         *  Button Actions
         */
        [HttpPost]
        public async Task<IActionResult> CancelEvent(long eventId)
        {
            await _eventRepository.DeleteAlongWithReferences(eventId);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> CancelReservation(long ticketId)
        {
            await _ticketRepository.Delete(ticketId);
            return RedirectToAction("Tickets");
        }
        
        [HttpPost]
        public async Task<IActionResult> GenerateAndDownloadQrCode(long eventId)
        {
            GetUserIdFromCookies();
            var filename = GenerateQrCode();
            var ticket = new Ticket { EventId = eventId, OwnerUuid = _userId, QrcFilename = filename };
            await _ticketRepository.Insert(ticket);
            return DownloadQrCode(filename);
        }

        [HttpPost]
        public async Task<IActionResult> SaveEvent(CreateOrEditViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View("CreateOrEdit", viewModel);
            
            var fileName = viewModel.PictureFile != null ? viewModel.PictureFile.FileName : viewModel.EventUi.PictureFilename ?? "event_place_holder.jpg";
            if (viewModel.PictureFile != null)
                await using (var stream = System.IO.File.Create(Path.Join(ImagesPath, fileName)))
                    await viewModel.PictureFile.CopyToAsync(stream);

            GetUserIdFromCookies();
            var insertedEvent = viewModel.IsCreate
                ? await _eventRepository.Insert(new Event(viewModel.EventUi) {OwnerUuid = _userId, PictureFilename = fileName})
                : await _eventRepository.Update(new Event(viewModel.EventUi) {OwnerUuid = _userId, PictureFilename = fileName});
           
            return RedirectToAction("Detail", viewModel.EventUi.Id);
        }


        /**
         *  Sorting
         */
        private static List<EventUi> SortByCategory(Category? category, List<EventUi> events)
        {
            return events.Where((ev) => ev.Category == category).ToList();
        }

        private static List<EventUi> SortByType(string type, List<EventUi> events)
        {
            return type switch
            {
                "Name" => events.OrderBy(ev => ev.Name).ToList(),
                "Organizer" => events.OrderBy(ev => ev.Organizer).ToList(),
                "Date" => events.OrderBy(ev => ev.StartDate).ToList(),
                _ => events
            };
        }

        private List<EventUi> SortOwnedEvents(List<EventUi> events)
        {
            return events.Where((ev) => ev.OwnerUuid == _userId).ToList();
        }

        private static List<EventUi> SortSearchPattern(string searchPattern, List<EventUi> events)
        {
            return events.Where(ev =>
                ev.Name.ToLower().Contains(searchPattern.ToLower()) ||
                ev.Organizer.ToLower().Contains(searchPattern.ToLower())).ToList();
        }


        /**
         *  QR Code Management
         */
        public string GenerateQrCode()
        {
            var uuid = Guid.NewGuid().ToString();
            var qrCodeData = _qrCodeGenerator.CreateQrCode(uuid, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new QRCode(qrCodeData).GetGraphic(20);
            var filename = $"{uuid}.jpg";
            qrCode.Save(Path.Join(QrCodesPath, filename));
            return filename;
        }

        public FileResult DownloadQrCode(string qrCodeFilename)
        {
            byte[] fileBytes = System.IO.File.ReadAllBytes(Path.Join(QrCodesPath, qrCodeFilename));
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, qrCodeFilename);
        }
    }
}