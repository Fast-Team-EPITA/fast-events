using System.Collections.Generic;
using AutoMapper;
using FastEvents.DataAccess;
using FastEvents.DataAccess.EfModels;
using FastEvents.dbo;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

using EventUi = FastEvents.DataAccess.EfModels.EventView;

namespace FastEventsTests
{
    public class EventUiRepositoryTest
    {
        private class EventUiByIdClassData : BaseClassData
        {
            public override IEnumerator<object[]> GetEnumerator() {
                var event1 = new List<EventUi>
                {
                    new () { Id = 1, Name = "ok"},
                    new () { Id = 2, Name = "ko"},
                    new () { Id = 3, Name = "ko"},
                    new () { Id = 4, Name = "ko"}
                };
                var event2 = new List<EventUi>
                {
                    new () { Id = 1, Name = "ko"},
                    new () { Id = 2, Name = "ok"},
                    new () { Id = 3, Name = "ko"},
                    new () { Id = 4, Name = "ko"}
                };
                var event3 = new List<EventUi>
                {
                    new () { Id = 1, Name = "ok"},
                    new () { Id = 2, Name = "ko"},
                    new () { Id = 1, Name = "ko"},
                    new () { Id = 3, Name = "ko"}
                };
                
                yield return new object[] { event1, 1, "ok"};
                yield return new object[] { event2, 2, "ok" };
                yield return new object[] { event3, 1, "ok" };
                
            }
        }
        private class EventUiByCategoryClassData : BaseClassData
        {
            public override IEnumerator<object[]> GetEnumerator()
            {
                var event1 = new List<EventUi>
                {
                    new () { Category = "Concert"},
                    new () { Category = "Conference"},
                    new () { Category = "OpenAir"},
                };

                var event2 = new List<EventUi>
                {
                    new () { Category = "Concert"},
                    new () { Category = "Conference"},
                    new () { Category = "OpenAir"},
                    new () { Category = "Concert"},
                    new () { Category = "Concert"},
                    new () { Category = "Concert"},
                };
                var event3 = new List<EventUi>
                {
                    new () { Category = "Concert"},
                    new () { Category = "OpenAir"},
                    new () { Category = "Conference"},
                    new () { Category = "OpenAir"},
                    new () { Category = "OpenAir"},
                   
                };
                var event4 = new List<EventUi>
                {
                    new () { Category = "Concert"},
                    new () { Category = "OpenAir"},
                    new () { Category = "Conference"},
                    new () { Category = "Conference"},
                };
                

                yield return new object[] { event1, Category.Concert, 1};
                yield return new object[] { event2, Category.Concert, 4};
                yield return new object[] { event3, Category.OpenAir, 3};
                yield return new object[] { event4, Category.Conference, 2};
            }
        }

        private class EventUiByOwnerIdClassData : BaseClassData
        {
            public override IEnumerator<object[]> GetEnumerator() 
            {
                var event1 = new List<EventUi>
                {
                    new () { OwnerUuid = "owner1"},
                    new () { OwnerUuid = "owner2"},
                    new () { OwnerUuid = "owner3"},
                    new () { OwnerUuid = "owner4"},

                };
                var event2 = new List<EventUi>
                {
                    new () { OwnerUuid = "owner1"},
                    new () { OwnerUuid = "owner2"},
                    new () { OwnerUuid = "owner3"},
                    new () { OwnerUuid = "owner2"},

                };
                var event3 = new List<EventUi>
                {
                    new () { OwnerUuid = "owner1"},
                    new () { OwnerUuid = "owner2"},
                    new () { OwnerUuid = "owner3"},
                    new () { OwnerUuid = "owner4"},

                };
                
                
                
                yield return new object[] { new EventUi { OwnerUuid = "test" }, "owner1", 0 };
                yield return new object[] { event1, "owner1", 1 };
                yield return new object[] { event2, "owner2", 2 };
                yield return new object[] { event3, "owner5", 0 };

            }
        }

       [Theory]
        [ClassData(typeof(EventUiByIdClassData))]
        public void GetByIdTest(List<EventUi> events, long eventId, string expected)
        {
            var contextMock = new Mock<FastEventContext>();
            var setMock = DbSetMock.GetQueryableMockDbSet(events);
            var loggerMock = new Mock<ILogger<EventUiRepository>>();
            var mapperMock = new Mock<IMapper>();

            contextMock.Setup(c => c.EventViews).Returns(setMock);
            mapperMock.Setup(m => m.Map<EventUi, FastEvents.dbo.EventUi>(It.IsAny<EventUi>())).
                Callback<EventUi>(s => Assert.Equal(expected, s.Name));

            var sut = new EventUiRepository(contextMock.Object, loggerMock.Object, mapperMock.Object);
            sut.GetById(eventId);
        }

        [Theory]
        [ClassData(typeof(EventUiByCategoryClassData))]
        public void GetByCategoryTest(List<EventUi> events, Category category, int expected)
        {
            var contextMock = new Mock<FastEventContext>();
            var setMock = DbSetMock.GetQueryableMockDbSet(events);
            var loggerMock = new Mock<ILogger<EventUiRepository>>();
            var mapperMock = new Mock<IMapper>();

            contextMock.Setup(c => c.EventViews).Returns(setMock);
            mapperMock.Setup(m => m.Map<List<EventUi>, FastEvents.dbo.EventUi>(It.IsAny<List<EventUi>>()))
                .Callback<List<EventUi>>(l => Assert.Equal(expected, l?.Count ?? 0));

            var sut = new EventUiRepository(contextMock.Object, loggerMock.Object, mapperMock.Object);
            sut.GetByCategory(category);
        }

        [Theory]
        [ClassData(typeof(EventUiByCategoryClassData))]
        public void GetByOwnerIdTest(List<EventUi> events, string ownerId, int expected)
        {
            var contextMock = new Mock<FastEventContext>();
            var setMock = DbSetMock.GetQueryableMockDbSet(events);
            var loggerMock = new Mock<ILogger<EventUiRepository>>();
            var mapperMock = new Mock<IMapper>();

            contextMock.Setup(c => c.EventViews).Returns(setMock);
            mapperMock.Setup(m => m.Map<List<EventUi>, FastEvents.dbo.EventUi>(It.IsAny<List<EventUi>>()))
                .Callback<List<EventUi>>(l => Assert.Equal(expected, l?.Count ?? 0));

            var sut = new EventUiRepository(contextMock.Object, loggerMock.Object, mapperMock.Object);
            sut.GetByOwnerId(ownerId);
        }
    }
}
