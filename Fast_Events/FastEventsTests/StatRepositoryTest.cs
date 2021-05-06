using System.Collections.Generic;
using AutoMapper;
using FastEvents.DataAccess;
using FastEvents.DataAccess.EfModels;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using StatByEvent = FastEvents.DataAccess.EfModels.StatByEvent;

namespace FastEventsTests
{
    public class StatRepositoryTest
    {
        private class StatByEventIdClassData : BaseClassData
        {

            /**
             * ClassData
             */

            public override IEnumerator<object[]> GetEnumerator()
            {
                var statByEvent1 = new List<StatByEvent>
                {
                    new () {Id = 1, NumberView = 1},
                    new () {Id = 2, NumberView = 5},
                    new () {Id = 3, NumberView = 10},
                    new () {Id = 4, NumberView = 15}
                };

                var statByEvent2 = new List<StatByEvent>
                {
                    new () {Id = 1, NumberView = 15},
                    new () {Id = 2, NumberView = 10},
                    new () {Id = 3, NumberView = 5},
                    new () {Id = 4, NumberView = 1}
                };

                var statByEvent3 = new List<StatByEvent>
                {
                    new () {Id = 1, NumberView = 10},
                    new () {Id = 2, NumberView = 15},
                    new () {Id = 3, NumberView = 1},
                    new () {Id = 4, NumberView = 5}
                };

                var statByEvent4 = new List<StatByEvent>
                {
                    new () {Id = 1, NumberView = 5},
                    new () {Id = 2, NumberView = 1},
                    new () {Id = 3, NumberView = 15},
                    new () {Id = 4, NumberView = 10}
                };

                yield return new object[] { new List<StatByEvent>(), 1, 0 };
                yield return new object[] { statByEvent1, 1, 1 };
                yield return new object[] { statByEvent2, 2, 10 };
                yield return new object[] { statByEvent3, 3, 1 };
                yield return new object[] { statByEvent4, 4, 10 };
            }
        }

        /**
         * Tests
         */

        [Theory]
        [ClassData(typeof(StatByEventIdClassData))]

        public void GetStatByEventId(List<StatByEvent> stats, long eventId, int expected)
        {
            var contextMock = new Mock<FastEventContext>();
            var setMock = DbSetMock.GetQueryableMockDbSet(stats);
            var loggerMock = new Mock<ILogger<StatRepository>>();
            var mapperMock = new Mock<IMapper>();

            contextMock.Setup(c => c.StatByEvents).Returns(setMock);
            mapperMock.Setup(m => m.Map<StatByEvent,FastEvents.dbo.StatByEvent>(It.IsAny<StatByEvent>())).
                Callback<StatByEvent>(s => Assert.Equal(expected, s.NumberView));

            var sut = new StatRepository(contextMock.Object, loggerMock.Object, mapperMock.Object);
            sut.GetByEvent(eventId);
        }
    }
}