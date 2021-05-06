using System;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;
using System.Linq;
using FastEvents.dbo.Interfaces;

namespace FastEventsTests
{
    public static class DbSetMock
    {
        public static DbSet <T> GetQueryableMockDbSet <T> (List <T> sourceList) where T: class, IObjectWithId
        {
            var queryable = sourceList.AsQueryable();
            var dbSet = new Mock<DbSet<T>>();
            dbSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            dbSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            dbSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            dbSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());
            dbSet.Setup(d => d.RemoveRange());
            dbSet.Setup(d => d.Add(It.IsAny<T>())).Callback<T>((s) => sourceList.Add(s));
            dbSet.Setup(d => d.Find(It.IsAny<object[]>())).Returns<object[]>(x => sourceList.FirstOrDefault(y => y.Id == (long) x[0]));
            return dbSet.Object;
        }
    }
}