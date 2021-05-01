using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FastEvents.DataAccess;
using FastEvents.DataAccess.EfModels;
using FastEvents.DataAccess.Interfaces;
using FastEvents.dbo;
using FastEvents.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using QRCoder;
using Event = FastEvents.dbo.Event;
using Stat = FastEvents.dbo.Stat;
using Ticket = FastEvents.dbo.Ticket;

namespace FastEvents.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IEventRepository _eventRepository;
        private readonly IEventUiRepository _eventUiRepository;
        private readonly ITicketRepository _ticketRepository;
        private readonly IStatRepository _statRepository;
        private readonly QRCodeGenerator _qrCodeGenerator = new();

        private string _userId;

        private static readonly string QrCodesPath =
            Path.Join(Directory.GetCurrentDirectory(), "wwwroot", "Resources", "QRCodes");

        public HomeController(ILogger<HomeController> logger, IEventUiRepository eventUiRepository,
            ITicketRepository ticketRepository, IStatRepository statRepository, IEventRepository eventRepository)
        {
            _logger = logger;
            _eventUiRepository = eventUiRepository;
            _ticketRepository = ticketRepository;
            _statRepository = statRepository;
            _eventRepository = eventRepository;
        }


        /**
         *  Data access
         */
        private void GetUserIdFromCookies()
        {
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
        public async Task<IActionResult> Index(Category? sortCategory = null, string sortType = null, bool ownedEvents = false, string searchPattern = null)
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

            var model = new IndexViewModel(events);
            return View(model);
        }

        [Route("detail/{eventId:long}")]
        public async Task<IActionResult> Detail(long eventId)
        {
            var model = await PrepareDetailModel(eventId);
            return View(model);
        }

        [Route("edit/{eventId:long?}")]
        public IActionResult CreateOrEdit(long? eventId = null)
        {
            var eventUi = eventId.HasValue ? _eventUiRepository.GetById(eventId.Value) : new EventUi { StartDate = DateTime.Now, EndDate = DateTime.Now};
            var model = new CreateOrEditViewModel { EventUi = eventUi, IsCreate = !eventId.HasValue };
            return View(model);
        }

        [Route("tickets")]
        public IActionResult Tickets()
        {
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


        /**
         *  Button Actions
         */
        public async Task<IActionResult> CancelEvent(long eventId)
        {
            await _eventUiRepository.Delete(eventId);
            var events = (await _eventUiRepository.Get()).ToList();
            var model = new IndexViewModel(events);
            return View("Index", model);
        }

        public async Task<IActionResult> CancelReservation(long ticketId)
        {
            await _ticketRepository.Delete(ticketId);
            var model = new TicketsViewModel(_ticketRepository.GetByOwnerId(_userId));
            return View("Tickets", model);
        }
        
        public IActionResult GenerateAndDownloadQrCode(long eventId)
        {
            var filename = GenerateQrCode();
            var ticket = new Ticket {EventId = eventId, OwnerUuid = _userId, QrcFilename = filename};
            _ticketRepository.Insert(ticket);
            return DownloadQrCode(filename);
        }

        [HttpPost]
        public async Task<IActionResult> SaveEvent(CreateOrEditViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.Error = $"You didn't fill {ModelState.ErrorCount} forms";
                return View("CreateOrEdit", viewModel);
            }

            GetUserIdFromCookies();
            var insertedEvent = viewModel.IsCreate
                ? await _eventRepository.Insert(new Event(viewModel.EventUi) {OwnerUuid = _userId})
                : await _eventRepository.Update(new Event(viewModel.EventUi) {OwnerUuid = _userId});
            var model = await PrepareDetailModel(insertedEvent.Id);
            return View("Detail", model);
        }

        /**
         *  Prepare models
         */
        private async Task<DetailViewModel> PrepareDetailModel(long eventId)
        {
            GetUserIdFromCookies();
            var stat = new Stat {Date = DateTime.Now, EventId = eventId};
            await _statRepository.Insert(stat);

            var selectedEvent = _eventUiRepository.GetById(eventId);
            var isOwner = selectedEvent.OwnerUuid == _userId;
            var hasTicket = _ticketRepository.GetByOwnerId(_userId)
                .FirstOrDefault(ticket => ticket.EventUi.Id == eventId) != null;

            return new DetailViewModel { EventUi = selectedEvent, IsOwner = isOwner, HasTicket = hasTicket };
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

        private string GenerateQrCode()
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


        /**
         *  Error
         */
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}