using Moq;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace MVCMusicStore.Tests.Extensions
{
    public static class DbSetExtensions
    {
        public static Mock<DbSet<T>> GetQueryableMockDbSet<T>(this IEnumerable<T> sourceList) where T : class
        {
            var queryable = sourceList.AsQueryable();
            var dbSet = new Mock<DbSet<T>>();
            dbSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            dbSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            dbSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            dbSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());
            return dbSet;
        }
    }
}
