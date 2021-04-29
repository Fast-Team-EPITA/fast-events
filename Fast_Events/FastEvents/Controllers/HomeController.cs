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
                name = "Fast Event 1",
                organizer = "Fast Team",
                startDate = DateTime.Now,
                endDate = DateTime.Now.AddDays(1),
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
                name = "Fast Event 2",
                organizer = "Fast Team",
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
                name = "Fast Event 3",
                organizer = "Fast Team",
                startDate = DateTime.Now,
                endDate = DateTime.Now.AddDays(1),
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

        private List<Ticket> GetTickets(string ownerId)
        {
            //return _ticketRepository.GetByOwnerId(ownerId); TODO UNCOMMENT
            return new List<Ticket> {new() {eventId = 1L, qrcFilename = "1.jpg", eventName = "Fast Event 1"}};
        }


        /**
         *  View Navigation
         */
        public async Task<IActionResult> Index()
        {
            GetUserIdFromCookies();
            var events = await GetEvents();
            var model = new IndexViewModel(events);
            return View(model);
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
            var model = new DetailViewModel(selectedEvent, selectedEvent.ownerUuid == _userId);
            return View(model);
        }

        public IActionResult CreateOrEdit(long? eventId = null)
        {
            var model = eventId.HasValue
                ? new CreateOrEditViewModel(_eventRepository.GetById(eventId.Value), false)
                : new CreateOrEditViewModel(new Event(), true);
            return View(model);
        }

        public IActionResult Tickets(string userId)
        {
            GetTickets(userId);
            var model = new TicketsViewModel(_ticketRepository.GetByOwnerId(userId));
            return View(model);
        }


        public IActionResult Stat(long eventId)
        {
            var stat = _statRepository.GetByEvent(eventId);
            return Json(stat);
        }


        /**
         *  QR Code Management
         */
        public IActionResult GenerateAndDownloadQrCode()
        {
            //TODO check if already downloaded one
            var filename = GenerateQRCode();
            //TODO add to DB
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