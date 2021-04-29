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

namespace FastEvents.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IEventRepository _eventRepository;
        private readonly ITicketRepository _ticketRepository;
        private readonly IStatRepository _statRepository;
        private readonly QRCodeGenerator _qrCodeGenerator = new();

        private string _userId;

        private static readonly string QrCodesPath =
            Path.Join(Directory.GetCurrentDirectory(), "wwwroot", "Resources", "QRCodes");

        public HomeController(ILogger<HomeController> logger, IEventRepository eventRepository,
            ITicketRepository ticketRepository, IStatRepository statRepository)
        {
            _logger = logger;
            _eventRepository = eventRepository;
            _ticketRepository = ticketRepository;
            _statRepository = statRepository;
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

        private async Task<List<Event>> GetEvents()
        {
            var event1 = new Event
            {
                id = 1,
                name = "A Fast Event 1",
                organizer = "B Fast Team",
                startDate = DateTime.Now.AddDays(1),
                endDate = DateTime.Now.AddDays(2),
                capacity = 100,
                location = "1 Rue Voltaire, 94270, Le Kremlin Bicetre",
                description =
                    "This event is a special techno party to have fun and listen to techno. The most famous DJs will be here, comme check it out it's free. We hope to see you there !",
                pictureFilename = "event_place_holder.jpg",
                ownerUuid = "", //_userId,
                category = Category.Concert,
                nbAvailableTickets = 30
            };

            var event2 = new Event
            {
                id = 2,
                name = "C Fast Event 2",
                organizer = "A Fast Team",
                startDate = DateTime.Now,
                endDate = DateTime.Now.AddDays(1),
                capacity = 50,
                location = "1 Rue Voltaire, 94270, Le Kremlin Bicetre",
                description =
                    "This event is a special techno party to have fun and listen to techno. The most famous DJs will be here, comme check it out it's free. We hope to see you there !",
                pictureFilename = "event_place_holder.jpg",
                ownerUuid = "", //_userId,
                category = Category.Conference,
                nbAvailableTickets = 0
            };

            var event3 = new Event
            {
                id = 3,
                name = "B dast Event 3",
                organizer = "C dast Team",
                startDate = DateTime.Now.AddDays(3),
                endDate = DateTime.Now.AddDays(4),
                capacity = 100,
                location = "1 Rue Voltaire, 94270, Le Kremlin Bicetre",
                description =
                    "This event is a special techno party to have fun and listen to techno. The most famous DJs will be here, comme check it out it's free. We hope to see you there !",
                pictureFilename = "event_place_holder.jpg",
                ownerUuid = "", //_userId,
                category = Category.OpenAir,
                nbAvailableTickets = 10
            };
            //return (await _eventRepository.Get()).ToList(); TODO UNCOMMENT
            return new List<Event> {event1, event2, event3};
        }


        /**
         *  View Navigation
         */
        public async Task<IActionResult> Index(Category? sortCategory = null, string sortType = null, bool ownedEvents = false, string searchPattern = null)
        {
            GetUserIdFromCookies();
            var events = await GetEvents();
            if (sortCategory != null)
                events = sortByCategory(sortCategory, events);

            if (sortType != null)
                events = sortByType(sortType, events);

            if (ownedEvents)
                events = sortOwnedEvents(events);

            if (searchPattern != null)
                events = sortSearchPattern(searchPattern, events);

            var model = new IndexViewModel(events);
            return View(model);
        }

        public List<Event> sortByCategory(Category? category, List<Event> events)
        {
            return events.Where((ev) => ev.category == category).ToList();
        }

        public List<Event> sortByType(string type, List<Event> events)
        {
            switch (type)
            {
                case "Name":
                    return events.OrderBy(ev => ev.name).ToList();
                case "Organizer":
                    return events.OrderBy(ev => ev.organizer).ToList();
                case "Date":
                    return events.OrderBy(ev => ev.startDate).ToList();
                default:
                    return events;
            }
        }

        public List<Event> sortOwnedEvents(List<Event> events)
        {
            return events.Where((ev) => ev.ownerUuid == _userId).ToList();
        }

        public List<Event> sortSearchPattern(string searchPattern, List<Event> events)
        {
            return events.Where(ev => ev.name.ToLower().Contains(searchPattern.ToLower()) || ev.organizer.ToLower().Contains(searchPattern.ToLower())).ToList();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Detail(long eventId)
        {
            var stat = new Stat {date = DateTime.Now, eventId = eventId};
            _statRepository.Insert(stat);

            var selectedEvent = _eventRepository.GetById(eventId);
            var isOwner = selectedEvent.ownerUuid == _userId;
            var hasTicket =
                _ticketRepository.GetByOwnerId(_userId).FirstOrDefault(ticket => ticket.eventId == eventId) != null;

            var model = new DetailViewModel(selectedEvent, isOwner, hasTicket);
            return View(model);
        }

        public IActionResult CreateOrEdit(long? eventId = null)
        {
            var model = eventId.HasValue
                ? new CreateOrEditViewModel(_eventRepository.GetById(eventId.Value), false)
                : new CreateOrEditViewModel(new Event(), true);
            return View(model);
        }

        public IActionResult Tickets()
        {
            var model = new TicketsViewModel(_ticketRepository.GetByOwnerId(_userId));
            return View(model);
        }


        public IActionResult Stat(long eventId)
        {
            var stat = _statRepository.GetByEvent(eventId);
            return Json(stat);
        }

        public async Task<IActionResult> CancelEvent(long eventId)
        {
            await _eventRepository.Delete(eventId);
            return await Index();
            // TODO add one ticket to event in db
        }

        public async Task<IActionResult> CancelReservation(long eventId)
        {
            //await _ticketRepository.Delete(_ticketRepository.GetByOwnerId());
            return await Index();
            // TODO remove one ticket to event in db
        }


        /**
         *  QR Code Management
         */
        public IActionResult GenerateAndDownloadQrCode(long eventId)
        {
            var filename = GenerateQRCode();
            var ticket = new Ticket { eventId = eventId, ownerUuid = _userId, qrcFilename = filename };
            _ticketRepository.Insert(ticket);
            return DownloadQrCode(filename);
        }

        private string GenerateQRCode()
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