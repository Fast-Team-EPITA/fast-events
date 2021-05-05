using System.Collections.Generic;
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
using FastEvents.Models;
using System;

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

        private class SortEventsCategoryClassData : BaseClassData
        {
            public override IEnumerator<object[]> GetEnumerator()
            {
                EventUi event1 = new();
                event1.Category = Category.Concert;

                EventUi event2 = new();
                event2.Category = Category.Conference;

                EventUi event3 = new();
                event3.Category = Category.OpenAir;

                yield return new object[] { Category.Concert, new List<EventUi> { event1, event2, event3 } };
                yield return new object[] { Category.Conference, new List<EventUi> { event1, event2, event3 } };
                yield return new object[] { Category.OpenAir, new List<EventUi> { event1, event2, event3 } };
            }
        }

        private class SortEventsTypeClassData : BaseClassData
        {
            public override IEnumerator<object[]> GetEnumerator()
            {
                EventUi event1 = new();
                event1.Name = "A";
                event1.Organizer = "A organizer";
                event1.StartDate = DateTime.Now;

                EventUi event2 = new();
                event2.Name = "B";
                event2.Organizer = "B organizer";
                event2.StartDate = DateTime.Now.AddDays(1);

                EventUi event3 = new();
                event3.Name = "C";
                event3.Organizer = "C organizer";
                event3.StartDate = DateTime.Now.AddDays(2);

                yield return new object[] { "Name" , new List<EventUi> { event3, event2, event1 } };
                yield return new object[] { "Organizer" , new List<EventUi> { event3, event2, event1 } };
                yield return new object[] { "Date" , new List<EventUi> { event3, event2, event1 } };
            }
        }


        private class SortEventsOwnedClassData : BaseClassData
        {
            public override IEnumerator<object[]> GetEnumerator()
            {
                EventUi event1 = new();
                event1.Name = "A";
                event1.OwnerUuid = "1";

                EventUi event2 = new();
                event2.Name = "B";
                event2.OwnerUuid = "2";

                EventUi event3 = new();
                event3.Name = "C";
                event3.OwnerUuid = "1";

                yield return new object[] { new List<EventUi> { event1, event2, event3 } };
                yield return new object[] { new List<EventUi> { event1, event2, event3, event1 } };
                yield return new object[] { new List<EventUi> { event1, event2, event3, event1, event3 } };
            }
        }

        

        private class SortEventsSearchClassData : BaseClassData
        {
            public override IEnumerator<object[]> GetEnumerator()
            {
                EventUi event1 = new();
                event1.Name = "A";
                event1.Organizer = "C organizer";
                event1.StartDate = DateTime.Now;

                EventUi event2 = new();
                event2.Name = "B";
                event2.Organizer = "D organizer";
                event2.StartDate = DateTime.Now.AddDays(1);

                EventUi event3 = new();
                event3.Name = "C";
                event3.Organizer = "E organizer";
                event3.StartDate = DateTime.Now.AddDays(2);

                yield return new object[] { "A", new List<EventUi> { event1, event2, event3 }, 3 };
                yield return new object[] { "B", new List<EventUi> { event1, event2, event3 }, 1 };
                yield return new object[] { "C", new List<EventUi> { event1, event2, event3 }, 2 };
            }
        }

        private class SaveEventsClassData : BaseClassData
        {
            public override IEnumerator<object[]> GetEnumerator()
            {
                EventUi event1 = new();
                event1.Id = 1;
                event1.Name = "Fast Event";
                event1.Organizer = "me";
                event1.StartDate = DateTime.Today;
                event1.EndDate = DateTime.Today.AddDays(1);
                event1.Capacity = 100;
                event1.Location = "here";
                event1.Description = "description";
                event1.PictureFilename = "none";
                event1.OwnerUuid = "1";
                event1.Category = Category.Concert;
                event1.NumberTickets = 0;

                yield return new object[] { new CreateOrEditViewModel { EventUi = event1, IsCreate = true } };
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
            statRepoMock.Setup(s => s.Insert(It.IsAny<Stat>()))
                .Callback<Stat>(s => Assert.Equal(id, s.EventId));
            eventUiRepoMock.Setup(e => e.GetById(It.IsAny<long>())).Returns(new EventUi());
            ticketRepoMock.Setup(t => t.GetByOwnerId(It.IsAny<string>())).Returns(new List<Ticket>());
            
            var action = await sut.Detail(id);
            statRepoMock.Verify(s => s.Insert(It.IsAny<Stat>()), Times.Once);
            // Might need to test viewmodel but how ?
        }

        [Theory]
        [ClassData(typeof(SaveEventsClassData))]
        public async Task SaveEventTest(CreateOrEditViewModel viewModel)
        {
            InitController();
            var action = await sut.SaveEvent(viewModel);
            eventRepoMock.Verify(s => s.Insert(It.IsAny<Event>()), Times.Once);
        }

        [Theory]
        [ClassData(typeof(SortEventsCategoryClassData))]
        public void SortByCategoryTest(Category? category, List<EventUi> events)
        {     
            InitController();
            var results = sut.SortByCategory(category, events);
            Assert.Single(results);
            Assert.Equal(category, results[0].Category);
        }

        [Theory]
        [ClassData(typeof(SortEventsTypeClassData))]
        public void SortByTypeTest(string type, List<EventUi> events)
        {
            InitController();
            var result = sut.SortByType(type, events);
            events.Reverse();
            Assert.Equal(events, result);
        }

        /*[Theory]
        [ClassData(typeof(SortEventsOwnedClassData))]
        public void SortOwnedEvents(List<EventUi> events)
        {
            InitController();
            // How to get user_id;
            var result = sut.SortOwnedEvents(events);
            Assert.Equal(2, result.Count);
        }*/

        [Theory]
        [ClassData(typeof(SortEventsSearchClassData))]
        public void SortSearchPattern(string searchPattern, List<EventUi> events, int count)
        {
            InitController();
            var result = sut.SortSearchPattern(searchPattern, events);
            Assert.Equal(count, result.Count);
        }
    }
}