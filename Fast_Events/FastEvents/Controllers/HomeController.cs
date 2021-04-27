using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FastEvents.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QRCoder;

namespace FastEvents.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private string _userId;
        
        private static readonly QRCodeGenerator QrCodeGenerator = new();
        private static readonly string QrCodesPath = Path.Join(Directory.GetCurrentDirectory(), "wwwroot", "Resources", "QRCodes");

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        
        
        /**
         *  Misc
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
        public IActionResult Index()
        {
            GetUserIdFromCookies();
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Detail(string eventId)
        {
            //TODO Generate model using eventId
            var selectedEvent = new Event()
            {
                name = "Fast Event 1",
                organizer = "Fast Team",
                startDate = DateTime.Now,
                endDate = DateTime.Now.AddDays(1),
                capacity = 100,
                location = "1 Rue Voltaire, 94270, Le Kremlin Bicetre",
                description = "This event is a special techno party to have fun and listen to techno. The most famous DJs will be here, comme check it out it's free. We hope to see you there !",
                pictureFilename = "event_place_holder.jpg",
                ownerUuid = "",//_userId,
                category = Category.Concert,
                nbAvailableTickets = 30
                
            };
            var model = new DetailViewModel(selectedEvent, selectedEvent.ownerUuid == _userId);
            return View(model);
        }

        public IActionResult CreateOrEdit(string eventId)
        {
            CreateOrEditViewModel model;
            if (eventId != null)
                //TODO Generate model for edit screen
                model = new CreateOrEditViewModel();
            else
                //TODO Generate model for create screen
                model = new CreateOrEditViewModel();
            return View(model);
        }

        public IActionResult Tickets(string userId)
        {
            var tickets = new List<Ticket> {new() {eventId = 1L, qrcFilename = "1.jpg", eventName = "Fast Event 1"}};
            //TODO Add all tickets for this userId
            var model = new TicketsViewModel(tickets);
            return View(model);
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
            var qrCodeData = QrCodeGenerator.CreateQrCode(uuid, QRCodeGenerator.ECCLevel.Q);
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
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
