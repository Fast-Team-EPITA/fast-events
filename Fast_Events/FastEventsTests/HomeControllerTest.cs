using System.IO;
using System.Threading.Tasks;
using Castle.Core.Logging;
using FastEvents.Controllers;
using FastEvents.DataAccess;
using FastEvents.dbo;
using FastEvents.DataAccess.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace FastEventsTests
{
    public class HomeControllerTest
    {
        [Fact]
        public void GenerateQrCodeTest()
        {
            var path = Path.Join(Directory.GetCurrentDirectory(), "..", "FastEvents", "wwwroot", "Resources",
                "QRCodes");
        }

        [Theory]
        [InlineData(1)]
        [InlineData(5435)]
        [InlineData(5769203)]
        public async Task GenerateAndDownloadQrCodeTest(long id)
        {
            var loggerMock = new Mock<ILogger<HomeController>>();
            var eventRepoMock = new Mock<IEventRepository>();
            var eventUiRepoMock = new Mock<IEventUiRepository>();
            var ticketRepoMock = new Mock<ITicketRepository>();
            var statRepoMock = new Mock<IStatRepository>();
            var sut = new HomeController(loggerMock.Object, eventUiRepoMock.Object, ticketRepoMock.Object, statRepoMock.Object, eventRepoMock.Object);

            await sut.GenerateAndDownloadQrCode(id);
            ticketRepoMock.Verify(t => t.Insert(It.IsAny<Ticket>()), Times.Once);
        }
    }
}