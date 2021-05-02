using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FastEvents.DataAccess;
using FastEvents.DataAccess.EfModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace FastEventsTests
{
    public class TicketRepositoryTest
    {
        [Fact]
        public void GetNbBookedByEventIdTest()
        {
            var contextMock = new Mock<FastEventContext>();
            var setMock = new Mock<DbSet<Ticket>>();
            var loggerMock = new Mock<ILogger>();
            var mapperMock = new Mock<IMapper>();
            
            contextMock.Setup(c => c.Tickets).Returns(setMock.Object);
            setMock.Verify(s => s.AsNoTracking(), Times.Once());

            var repo = new Repository<Ticket, FastEvents.dbo.Ticket>(contextMock.Object, loggerMock.Object,
                mapperMock.Object);

            repo.Get();
        }
    }
}