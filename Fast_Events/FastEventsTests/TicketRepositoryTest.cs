using System.Collections.Generic;
using AutoMapper;
using FastEvents.DataAccess;
using FastEvents.DataAccess.EfModels;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using FastEvents.dbo;
using Ticket = FastEvents.DataAccess.EfModels.Ticket;

namespace FastEventsTests
{
    public class TicketRepositoryTest
    {
        /**
         *  ClassData
         */

        private class TicketByEventCountClassData : BaseClassData
        {
            public override IEnumerator<object[]> GetEnumerator()
            {
                var tickets1 = new List<Ticket>
                {
                    new() { EventId = 1 },
                    new() { EventId = 1 },
                    new() { EventId = 1 },
                    new() { EventId = 1 }
                };
                var tickets2 = new List<Ticket>
                {
                    new() { EventId = 2 },
                    new() { EventId = 1 },
                    new() { EventId = 1 },
                    new() { EventId = 1 }
                };
                var tickets3 = new List<Ticket>
                {
                    new() { EventId = 2 },
                    new() { EventId = 2 },
                    new() { EventId = 1 },
                    new() { EventId = 1 }
                };
                var tickets4 = new List<Ticket>
                {
                    new() { EventId = 1 },
                    new() { EventId = 1 },
                    new() { EventId = 1 },
                    new() { EventId = 1 }
                };

                yield return new object[] { new List<Ticket>(), 1, 0 };
                yield return new object[] { tickets1, 2, 0 };
                yield return new object[] { tickets2, 2, 1 };
                yield return new object[] { tickets3, 2, 2 };
                yield return new object[] { tickets4, 1, 4 };
            }
        }
        
        private class TicketByOwnerIdClassData : BaseClassData
        {
            public override IEnumerator<object[]> GetEnumerator()
            {
                var tickets1 = new List<Ticket>
                {
                    new() { OwnerUuid = "1" },
                    new() { OwnerUuid = "1" },
                    new() { OwnerUuid = "1" },
                    new() { OwnerUuid = "1" }
                };
                var tickets2 = new List<Ticket>
                {
                    new() { OwnerUuid = "2" },
                    new() { OwnerUuid = "1" },
                    new() { OwnerUuid = "1" },
                    new() { OwnerUuid = "1" }
                };
                var tickets3 = new List<Ticket>
                {
                    new() { OwnerUuid = "2" },
                    new() { OwnerUuid = "2" },
                    new() { OwnerUuid = "1" },
                    new() { OwnerUuid = "1" }
                };
                var tickets4 = new List<Ticket>
                {
                    new() { OwnerUuid = "1" },
                    new() { OwnerUuid = "1" },
                    new() { OwnerUuid = "1" },
                    new() { OwnerUuid = "1" }
                };

                yield return new object[] { new List<Ticket>(), "1", 0 };
                yield return new object[] { tickets1, "2", 0 };
                yield return new object[] { tickets2, "2", 1 };
                yield return new object[] { tickets3, "2", 2 };
                yield return new object[] { tickets4, "1", 4 };
            }
        }
        
        /**
         *  Tests
         */
        
        [Theory]
        [ClassData(typeof(TicketByEventCountClassData))]
        public void GetNbBookedByEventIdTest(List<Ticket> tickets, long eventId, int expected)
        {
            var contextMock = new Mock<FastEventContext>();
            var setMock = DbSetMock.GetQueryableMockDbSet(tickets);
            var loggerMock = new Mock<ILogger<TicketRepository>>();
            var mapperMock = new Mock<IMapper>();
            contextMock.Setup(c => c.Tickets).Returns(setMock);
            
            var sut = new TicketRepository(contextMock.Object, loggerMock.Object, mapperMock.Object);
            
            var count = sut.GetNbBookedByEventId(eventId);
            Assert.Equal(expected, count);
        }

        [Theory]
        [ClassData(typeof(TicketByOwnerIdClassData))]
        public void GetByOwnerId(List<Ticket> tickets, string ownerId, int expected)
        {
            var contextMock = new Mock<FastEventContext>();
            var setMock = DbSetMock.GetQueryableMockDbSet(tickets);
            var loggerMock = new Mock<ILogger<TicketRepository>>();
            var mapperMock = new Mock<IMapper>();
            
            contextMock.Setup(c => c.Tickets).Returns(setMock);
            mapperMock.Setup(m => m.Map<List<Ticket>, FastEvents.dbo.Ticket>(It.IsAny<List<Ticket>>()))
                .Callback<List<Ticket>>(l => Assert.Equal(expected, l?.Count ?? 0));
            
            var sut = new TicketRepository(contextMock.Object, loggerMock.Object, mapperMock.Object);

            sut.GetByOwnerId(ownerId);
        }
    }
}