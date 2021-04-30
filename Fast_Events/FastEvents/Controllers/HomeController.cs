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

        private async Task<List<EventUi>> GetEvents()
        {
            var event1 = new EventUi
            {
                Id = 1,
                Name = "A Fast Event 1",
                Organizer = "B Fast Team",
                StartDate = DateTime.Now.AddDays(1),
                EndDate = DateTime.Now.AddDays(2),
                Capacity = 100,
                Location = "1 Rue Voltaire, 94270, Le Kremlin Bicetre",
                Description =
                    "This event is a special techno party to have fun and listen to techno. The most famous DJs will be here, comme check it out it's free. We hope to see you there !",
                PictureFilename = "event_place_holder.jpg",
                OwnerUuid = _userId,
                Category = Category.Concert,
                NumberTickets = 30
            };

            var event2 = new EventUi
            {
                Id = 2,
                Name = "C Fast Event 2",
                Organizer = "A Fast Team",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                Capacity = 50,
                Location = "1 Rue Voltaire, 94270, Le Kremlin Bicetre",
                Description =
                    "This event is a special techno party to have fun and listen to techno. The most famous DJs will be here, comme check it out it's free. We hope to see you there !",
                PictureFilename = "event_place_holder.jpg",
                OwnerUuid = "", //_userId,
                Category = Category.Conference,
                NumberTickets = 0
            };

            var event3 = new EventUi
            {
                Id = 3,
                Name = "B dast Event 3",
                Organizer = "C dast Team",
                StartDate = DateTime.Now.AddDays(3),
                EndDate = DateTime.Now.AddDays(4),
                Capacity = 100,
                Location = "1 Rue Voltaire, 94270, Le Kremlin Bicetre",
                Description =
                    "This event is a special techno party to have fun and listen to techno. The most famous DJs will be here, comme check it out it's free. We hope to see you there !",
                PictureFilename = "event_place_holder.jpg",
                OwnerUuid = "", //_userId,
                Category = Category.OpenAir,
                NumberTickets = 10
            };
            //return new List<EventUi> {event1, event2, event3};
            return (await _eventUiRepository.Get()).ToList();
        }


        /**
         *  View Navigation
         */
        public async Task<IActionResult> Index(Category? sortCategory = null, string sortType = null, bool ownedEvents = false, string searchPattern = null)
        {
            GetUserIdFromCookies();
            var events = await GetEvents();
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
            var model = eventId.HasValue
                ? new CreateOrEditViewModel(_eventUiRepository.GetById(eventId.Value), false)
                : new CreateOrEditViewModel(new EventUi(), true);
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
            return await Index();
            // TODO add one ticket to event in db
        }

        public async Task<IActionResult> CancelReservation(long eventId)
        {
            //await _ticketRepository.Delete(_ticketRepository.GetByOwnerId());
            return await Index();
            // TODO remove one ticket to event in db
        }

        public async Task<IActionResult> CreateEvent(string eventName, string organiserName, DateTime startDate, DateTime endDate, string category, int numberPlaces, string location, string description, string image)
        {
            GetUserIdFromCookies();
            Category category1 = Category.Concert;
            switch (category)
            {
                case "OpenAir":
                    category1 = Category.OpenAir;
                    break;
                case "Conference":
                    category1 = Category.Conference;
                    break;
            }

            var ev = new Event
            {
                Name = eventName ?? "",
                Organizer = organiserName ?? "",
                StartDate = startDate,
                EndDate = endDate,
                Category = category1,
                Capacity = numberPlaces,
                Location = location ?? "",
                Description = description ?? "",
                PictureFilename = image ?? "event_place_holder.jpg",
                OwnerUuid = _userId ?? ""
            };

            var insertedEvent = await _eventRepository.Insert(ev);
            
            var model = await PrepareDetailModel(insertedEvent.Id);
            return View("Detail", model);
        }

        public async Task<IActionResult> EditEvent(string eventName, string organiserName, DateTime startDate, DateTime endDate, string category, int numberPlaces, string location, string description, string image, int eventId)
        {
            Category category1 = Category.Concert;
            switch (category)
            {
                case "OpenAir":
                    category1 = Category.OpenAir;
                    break;
                case "Conference":
                    category1 = Category.Conference;
                    break;
            }

            var ev = new Event
            {
                Id = eventId,
                Name = eventName,
                Organizer = organiserName,
                StartDate = startDate,
                EndDate = endDate,
                Category = category1,
                Capacity = numberPlaces,
                Location = location,
                Description = description,
                PictureFilename = image,
            };

            await _eventRepository.Update(ev);
            
            
            var model = await PrepareDetailModel(eventId);
            return View("Detail", model);
        }

        /**
         *  Prepare models
         */

        private async Task<DetailViewModel> PrepareDetailModel(long eventId)
        {
            var stat = new Stat {Date = DateTime.Now, EventId = eventId};
            await _statRepository.Insert(stat);

            var selectedEvent = _eventUiRepository.GetById(eventId);
            var isOwner = selectedEvent.OwnerUuid == _userId;
            var hasTicket = _ticketRepository.GetByOwnerId(_userId).FirstOrDefault(ticket => ticket.EventUi.Id == eventId) != null;

            return new DetailViewModel(selectedEvent, isOwner, hasTicket);
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
            return events.Where(ev => ev.Name.ToLower().Contains(searchPattern.ToLower()) || ev.Organizer.ToLower().Contains(searchPattern.ToLower())).ToList();
        }


        /**
         *  QR Code Management
         */
        public IActionResult GenerateAndDownloadQrCode(long eventId)
        {
            var filename = GenerateQrCode();
            var ticket = new Ticket { EventId = eventId, OwnerUuid = _userId, QrcFilename = filename };
            _ticketRepository.Insert(ticket);
            return DownloadQrCode(filename);
        }

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