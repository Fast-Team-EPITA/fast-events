using System.IO;
using System.Linq;
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
        Mock<ILogger<HomeController>> loggerMock;
        Mock<IEventRepository> eventRepoMock;
        Mock<IEventUiRepository> eventUiRepoMock;
        Mock<ITicketRepository> ticketRepoMock;
        Mock<IStatRepository> statRepoMock;
        HomeController sut;


        private void InitController()
        {
            loggerMock = new Mock<ILogger<HomeController>>();
            eventRepoMock = new Mock<IEventRepository>();
            eventUiRepoMock = new Mock<IEventUiRepository>();
            ticketRepoMock = new Mock<ITicketRepository>();
            statRepoMock = new Mock<IStatRepository>();
            sut = new HomeController(loggerMock.Object, eventUiRepoMock.Object, ticketRepoMock.Object, statRepoMock.Object, eventRepoMock.Object);
            sut.QrCodesPath = ".";
            sut.ImagesPath = ".";
        }

        private void RemoveTempFiles()
        {
            var di = new DirectoryInfo(".");
            var files = di.GetFiles("*.jpg").Where(p => p.Extension == ".jpg").ToArray();
            foreach (var file in files)
            {
                file.Attributes = FileAttributes.Normal;
                File.Delete(file.FullName);
            }
        }
        
        [Fact]
        public void GenerateQrCodeTest()
        {
            InitController();
            var filename = sut.GenerateQrCode();
            Assert.True(File.Exists(Path.Join(sut.QrCodesPath, filename)));
            RemoveTempFiles();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(5435)]
        [InlineData(5769203)]
        public async Task GenerateAndDownloadQrCodeTest(long id)
        {
            InitController();
            ticketRepoMock.Setup(t => t.Insert(It.IsAny<Ticket>()))
                .Callback<Ticket>(t => Assert.Equal(id, t.EventId));
            await sut.GenerateAndDownloadQrCode(id);
            ticketRepoMock.Verify(t => t.Insert(It.IsAny<Ticket>()), Times.Once);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(5435)]
        [InlineData(5769203)]
        public async Task DetailTest(long id)
        {
            InitController();
            statRepoMock.Setup(t => t.Insert(It.IsAny<Stat>()))
                .Callback<Stat>(t => Assert.Equal(id, t.EventId));
            var action = await sut.Detail(id);
            statRepoMock.Verify(t => t.Insert(It.IsAny<Stat>()), Times.Once);
            // Might need to test viewmodel but how ?
        }
    }
}