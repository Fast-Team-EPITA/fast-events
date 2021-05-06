using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FastEvents.Controllers;
using FastEvents.dbo;
using FastEvents.DataAccess.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using FastEvents.Models;
using System;
using Microsoft.AspNetCore.Mvc;

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
            var directory = new DirectoryInfo(".");
            var files = directory.GetFiles("*.jpg").Where(p => p.Extension == ".jpg").ToArray();
            foreach (var file in files)
            {
                file.Attributes = FileAttributes.Normal;
                File.Delete(file.FullName);
            }
        }
        
        /**
         *  ClassData
         */

        private class SortEventsCategoryClassData : BaseClassData
        {
            public override IEnumerator<object[]> GetEnumerator()
            {
                var event1 = new EventUi { Category = Category.Concert };

                var event2 = new EventUi { Category = Category.Conference };

                var event3 = new EventUi { Category = Category.OpenAir };

                yield return new object[] { Category.Concert, new List<EventUi> { event1, event2, event3 } };
                yield return new object[] { Category.Conference, new List<EventUi> { event1, event2, event3 } };
                yield return new object[] { Category.OpenAir, new List<EventUi> { event1, event2, event3 } };
            }
        }

        private class SortEventsTypeClassData : BaseClassData
        {
            public override IEnumerator<object[]> GetEnumerator()
            {
                var event1 = new EventUi
                {
                    Name = "A", Organizer = "A organizer", StartDate = DateTime.Now
                };

                var event2 = new EventUi
                {
                    Name = "B", Organizer = "B organizer", StartDate = DateTime.Now.AddDays(1)
                };

                var event3 = new EventUi
                {
                    Name = "C", Organizer = "C organizer", StartDate = DateTime.Now.AddDays(2)
                };

                yield return new object[] { "Name" , new List<EventUi> { event3, event2, event1 } };
                yield return new object[] { "Organizer" , new List<EventUi> { event3, event2, event1 } };
                yield return new object[] { "Date" , new List<EventUi> { event3, event2, event1 } };
            }
        }


        private class SortEventsOwnedClassData : BaseClassData
        {
            public override IEnumerator<object[]> GetEnumerator()
            {
                var event1 = new EventUi { Name = "A", OwnerUuid = null };

                var event2 = new EventUi { Name = "B", OwnerUuid = "2" };

                var event3 = new EventUi { Name = "C", OwnerUuid = null };

                yield return new object[] { new List<EventUi> { event1, event2, event3 }, 2 };
                yield return new object[] { new List<EventUi> { event1, event2, event3, event1 }, 3 };
                yield return new object[] { new List<EventUi> { event1, event2, event3, event1, event3 }, 4 };
            }
        }

        

        private class SortEventsSearchClassData : BaseClassData
        {
            public override IEnumerator<object[]> GetEnumerator()
            {
                var event1 = new EventUi
                {
                    Name = "A", Organizer = "C organizer", StartDate = DateTime.Now
                };

                var event2 = new EventUi
                {
                    Name = "B", Organizer = "D organizer", StartDate = DateTime.Now.AddDays(1)
                };

                var event3 = new EventUi
                {
                    Name = "C", Organizer = "E organizer", StartDate = DateTime.Now.AddDays(2)
                };

                yield return new object[] { "A", new List<EventUi> { event1, event2, event3 }, 3 };
                yield return new object[] { "B", new List<EventUi> { event1, event2, event3 }, 1 };
                yield return new object[] { "C", new List<EventUi> { event1, event2, event3 }, 2 };
            }
        }

        private class SaveEventsClassData : BaseClassData
        {
            public override IEnumerator<object[]> GetEnumerator()
            {
                var event1 = new EventUi
                {
                    Id = 1,
                    Name = "Fast Event",
                    Organizer = "me",
                    StartDate = DateTime.Today,
                    EndDate = DateTime.Today.AddDays(1),
                    Capacity = 100,
                    Location = "here",
                    Description = "description",
                    PictureFilename = "none",
                    OwnerUuid = "1",
                    Category = Category.Concert,
                    NumberTickets = 0
                };

                yield return new object[] { new CreateOrEditViewModel { EventUi = event1, IsCreate = true } };
            }
        }
        
        /**
         *  Actions
         */

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
            RemoveTempFiles();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(5435)]
        [InlineData(5769203)]
        public async Task CancelEventTest(long id)
        {
            InitController();
            eventRepoMock.Setup(e => e.DeleteAlongWithReferences(It.IsAny<long>()));
            await sut.CancelEvent(id);
            eventRepoMock.Verify(e => e.DeleteAlongWithReferences(It.IsAny<long>()), Times.Once);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(5435)]
        [InlineData(5769203)]
        public async Task CancelReservationTest(long id)
        {
            InitController();
            ticketRepoMock.Setup(t => t.Delete(It.IsAny<long>()));
            await sut.CancelReservation(id);
            ticketRepoMock.Verify(t => t.Delete(It.IsAny<long>()), Times.Once);
        }

        [Theory]
        [ClassData(typeof(SaveEventsClassData))]
        public async Task SaveEventTest(CreateOrEditViewModel viewModel)
        {
            InitController();
            eventRepoMock.Setup(e => e.Insert(It.IsAny<Event>())).Returns(Task.FromResult(new Event()));
            eventRepoMock.Setup(e => e.Update(It.IsAny<Event>())).Returns(Task.FromResult(new Event()));
            
            var action = await sut.SaveEvent(viewModel);
            eventRepoMock.Verify(s => s.Insert(It.IsAny<Event>()), Times.Once);
        }
        
        /**
         *  Views
         */

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
            var viewResult = Assert.IsType<ViewResult>(action);
            var model = Assert.IsType<DetailViewModel>(viewResult.ViewData.Model);
            
            Assert.NotNull(model.EventUi);
            statRepoMock.Verify(s => s.Insert(It.IsAny<Stat>()), Times.Once);
            RemoveTempFiles();
        }

        [Fact]
        public void TicketsTest()
        {
            InitController();
            ticketRepoMock.Setup(t => t.GetByOwnerId(It.IsAny<string>())).Returns(new List<Ticket>());

            var action = sut.Tickets();
            var viewResult = Assert.IsType<ViewResult>(action);
            var model = Assert.IsType<TicketsViewModel>(viewResult.ViewData.Model);
            
            Assert.NotNull(model);
            ticketRepoMock.Verify(t => t.GetByOwnerId(It.IsAny<string>()), Times.Once);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(5435)]
        [InlineData(5769203)]
        public void StatTest(long id)
        {
            InitController();
            statRepoMock.Setup(s => s.GetByEvent(It.IsAny<long>())).Returns(new StatByEvent());

            var action = sut.Stat(id);
            Assert.IsType<JsonResult>(action);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(5435)]
        public void CreateOrEditTest(long id)
        {
            InitController();
            eventUiRepoMock.Setup(e => e.GetById(It.IsAny<long>())).Returns(new EventUi());
            
            var action = id == 0 ? sut.CreateOrEdit() : sut.CreateOrEdit(id);
            var viewResult = Assert.IsType<ViewResult>(action);
            var model = Assert.IsType<CreateOrEditViewModel>(viewResult.ViewData.Model);

            Assert.NotNull(model);
            Assert.Equal(id == 0, model.IsCreate);
        }

        [Fact]
        public async Task IndexTest()
        {
            InitController();
            eventUiRepoMock.Setup(s => s.Get("")).Returns(Task.FromResult<IEnumerable<EventUi>>(new List<EventUi>().ToArray()));
            var action = await sut.Index(new IndexViewModel());
            var viewResult = Assert.IsType<ViewResult>(action);
            var model = Assert.IsType<IndexViewModel>(viewResult.ViewData.Model);
            Assert.NotNull(model);
        }

        /**
         *  Sort
         */

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

        [Theory]
        [ClassData(typeof(SortEventsOwnedClassData))]
        public void SortOwnedEvents(List<EventUi> events, int count)
        {
            InitController();
            var result = sut.SortOwnedEvents(events);
            Assert.Equal(count, result.Count);
        }

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