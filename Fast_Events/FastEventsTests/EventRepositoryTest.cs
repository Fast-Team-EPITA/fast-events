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

        private class EventClassData : BaseClassData
        {
            public override IEnumerator<object[]> GetEnumerator()
            {
                var stats = new List<Stat>
                {
                    new() { EventId = 1 },
                    new() { EventId = 2 },
                    new() { EventId = 2 },
                    new() { EventId = 4 }
                };

                var tickets = new List<Ticket>
                {
                    new() { EventId = 1 },
                    new() { EventId = 2 },
                    new() { EventId = 2 },
                    new() { EventId = 4 }
                };

               

                var events1 = new List<Event>
                {
                    new() { Id = 1, Stats = stats, Tickets = tickets },
                    new() { Id = 2, Stats = stats, Tickets = tickets },
                    new() { Id = 3, Stats = stats, Tickets = tickets },
                    new() { Id = 4, Stats = stats, Tickets = tickets }
                };
                var events2 = new List<Event>
                {
                    new() { Id = 1, Stats = stats, Tickets = tickets },
                    new() { Id = 2, Stats = stats, Tickets = tickets },
                    new() { Id = 3, Stats = stats, Tickets = tickets },
                    new() { Id = 4, Stats = stats, Tickets = tickets }
                };
                var events3 = new List<Event>
                {
                    new() { Id = 1, Stats = stats, Tickets = tickets },
                    new() { Id = 2, Stats = stats, Tickets = tickets },
                    new() { Id = 3, Stats = stats, Tickets = tickets },
                    new() { Id = 4, Stats = stats, Tickets = tickets }
                };
                var events4 = new List<Event>
                {
                    new() { Id = 1, Stats = stats, Tickets = tickets },
                    new() { Id = 2, Stats = stats, Tickets = tickets },
                    new() { Id = 3, Stats = stats, Tickets = tickets },
                    new() { Id = 4, Stats = stats, Tickets = tickets }
                };

                yield return new object[] { events1, stats, tickets, 1, true };
                yield return new object[] { events2, stats, tickets, 2, true };
                yield return new object[] { events3, stats, tickets, 5, false };
                yield return new object[] { events4, stats, tickets, 3, true };

            }
        }
        [Theory]
        [ClassData(typeof(EventClassData))]
        public async void DeleteAlongWithReferencesTest(List<Event> events, List<Stat> stats, List<Ticket> tickets,long EventId, bool expected)
        {
            var contextMock = new Mock<FastEventContext>();
            var setMock = DbSetMock.GetQueryableMockDbSet(events);


            var loggerMock = new Mock<ILogger<EventRepository>>();
            var mapperMock = new Mock<IMapper>();

            contextMock.Setup(c => c.Events).Returns(setMock);


            var sut = new EventRepository(contextMock.Object, loggerMock.Object, mapperMock.Object);
            Assert.Equal(await sut.DeleteAlongWithReferences(EventId), expected);
        }
    }
}
