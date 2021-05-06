using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FastEvents.DataAccess;
using FastEvents.DataAccess.EfModels;
using Microsoft.Extensions.Logging;
using Xunit;
using Moq;
using Event = FastEvents.DataAccess.EfModels.Event;
using AutoMapper;

namespace FastEventsTests
{
    public class EventRepositoryTest
    {

        /**
         * ClassData
         */

        private class EventByOwnerId : BaseClassData
        {
            public override IEnumerator<object[]> GetEnumerator()
            {
                var tickets1 = new List<Event>
                    {
                        new() { Id = 1 },
                        new() { Id = 1 },
                        new() { Id = 1 },
                        new() { Id = 1 }
                    };
                var tickets2 = new List<Event>
                    {
                        new() { Id = 2 },
                        new() { Id = 1 },
                        new() { Id = 1 },
                        new() { Id = 1 }
                    };
                var tickets3 = new List<Event>
                    {
                        new() { Id = 2 },
                        new() { Id = 2 },
                        new() { Id = 1 },
                        new() { Id = 1 }
                    };
                var tickets4 = new List<Event>
                    {
                        new() { Id = 1 },
                        new() { Id = 1 },
                        new() { Id = 1 },
                        new() { Id = 1 }
                    };

                yield return new object[] { new List<Event>(), 1, 0 };
                yield return new object[] { tickets1, 2, 0 };
                yield return new object[] { tickets2, 2, 1 };
                yield return new object[] { tickets3, 2, 2 };
                yield return new object[] { tickets4, 1, 4 };
            }
        }
        //[Theory]
        //[ClassData(typeof(EventByOwnerId))]
        public void DeleteAlongWithReferencesTest(List<Event> events, long EventId, int expected)
        {
            var contextMock = new Mock<FastEventContext>();
            var setMock = DbSetMock.GetQueryableMockDbSet(events);
            var loggerMock = new Mock<ILogger<EventRepository>>();
            var mapperMock = new Mock<IMapper>();

            contextMock.Setup(c => c.Events).Returns(setMock);
            var sut = new EventRepository(contextMock.Object, loggerMock.Object, mapperMock.Object);
            var _ = sut.DeleteAlongWithReferences(EventId); // TODO

        }

    }
}
