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

// TODO Tests unitaires Repo
// TODO Deploy

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
         *  Utils
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

        private static string TruncateFilename(string filename)
        {
            return filename.Length <= 50 ? filename : filename.Substring(filename.Length - 49, 49);
        }

        /**
         *  View Navigation
         */
        public async Task<IActionResult> Index(IndexViewModel model, int page = 1)
        {
            GetUserIdFromCookies();
            var events = (await _eventUiRepository.Get()).ToList();

            if (model.SortCategory != null)
                events = SortByCategory(model.SortCategory, events);

            if (model.SortType != null)
                events = SortByType(model.SortType, events);

            if (model.OwnedEvents)
                events = SortOwnedEvents(events);

            if (model.SearchPattern != null)
                events = SortSearchPattern(model.SearchPattern, events);

            model.PageNumber = page;
            model.EventUis = events;
            return View(model);
        }

        [Route("Home/Detail/{eventId:long}")]
        public async Task<IActionResult> Detail(long eventId)
        {
            GetUserIdFromCookies();

            var selectedEvent = _eventUiRepository.GetById(eventId);
            if (selectedEvent == null)
                return RedirectToAction("Index");

            var isOwner = selectedEvent.OwnerUuid == _userId;
            var tickets = _ticketRepository.GetByOwnerId(_userId);
            var hasTicket = tickets?.FirstOrDefault(ticket => ticket.EventId == eventId) != null;

            var stat = new Stat {Date = DateTime.Now, EventId = eventId};
            await _statRepository.Insert(stat);

            var model = new DetailViewModel {EventUi = selectedEvent, IsOwner = isOwner, HasTicket = hasTicket};
            return View(model);
        }

        public IActionResult CreateOrEdit(long? eventId = null)
        {
            var eventUi = eventId.HasValue
                ? _eventUiRepository.GetById(eventId.Value)
                : new EventUi { StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(1) };
            
            GetUserIdFromCookies();
            if (eventUi.OwnerUuid != null && _userId != eventUi.OwnerUuid)
                return RedirectToAction("Index");
            var model = new CreateOrEditViewModel {EventUi = eventUi, IsCreate = !eventId.HasValue};
            return View(model);
        }

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
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }


        /**
         *  Button Actions
         */
        public async Task<IActionResult> CancelEvent(long eventId)
        {
            await _eventRepository.DeleteAlongWithReferences(eventId);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> CancelReservation(long ticketId)
        {
            await _ticketRepository.Delete(ticketId);
            return RedirectToAction("Tickets");
        }

        public async Task<IActionResult> GenerateAndDownloadQrCode(long eventId)
        {
            GetUserIdFromCookies();
            var filename = GenerateQrCode();
            var ticket = new Ticket {EventId = eventId, OwnerUuid = _userId, QrcFilename = filename};
            await _ticketRepository.Insert(ticket);
            return DownloadQrCode(filename);
        }

        public async Task<IActionResult> SaveEvent(CreateOrEditViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View("CreateOrEdit", viewModel);

            var fileName = viewModel.PictureFile != null
                ? TruncateFilename(viewModel.PictureFile.FileName)
                : viewModel.EventUi.PictureFilename ?? "event_place_holder.jpg";
            if (viewModel.PictureFile != null)
                await using (var stream = System.IO.File.Create(Path.Join(ImagesPath, fileName)))
                    await viewModel.PictureFile.CopyToAsync(stream);

            GetUserIdFromCookies();
            var insertedEvent = viewModel.IsCreate
                ? await _eventRepository.Insert(new Event(viewModel.EventUi)
                    {OwnerUuid = _userId, PictureFilename = fileName})
                : await _eventRepository.Update(new Event(viewModel.EventUi)
                    {OwnerUuid = _userId, PictureFilename = fileName});

            return RedirectToAction("Detail", new {insertedEvent.Id});
        }


        /**
         *  Sorting
         */
        public List<EventUi> SortByCategory(Category? category, List<EventUi> events)
        {
            //return events.Where((ev) => ev.Category != category).ToList(); //TODO broken version test
            return events.Where((ev) => ev.Category == category).ToList();
        }

        public List<EventUi> SortByType(string type, List<EventUi> events)
        {
            return type switch
            {
                "Name" => events.OrderBy(ev => ev.Name).ToList(),
                "Organizer" => events.OrderBy(ev => ev.Organizer).ToList(),
                "Date" => events.OrderBy(ev => ev.StartDate).ToList(),
                _ => events
            };
        }

        public List<EventUi> SortOwnedEvents(List<EventUi> events)
        {
            return events.Where((ev) => ev.OwnerUuid == _userId).ToList();
        }

        public List<EventUi> SortSearchPattern(string searchPattern, List<EventUi> events)
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