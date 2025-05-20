using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Appointment_System.Application.Tests.Helpers
{
    public static class MockHelpers
    {
        public static Mock<UserManager<TUser>> MockUserManager<TUser>() where TUser : class
        {
            var store = new Mock<IUserStore<TUser>>();
            return new Mock<UserManager<TUser>>(store.Object,
                null, null, null, null, null, null, null, null);
        }

        public static Mock<SignInManager<TUser>> MockSignInManager<TUser>() where TUser : class
        {
            return new Mock<SignInManager<TUser>>(
                MockUserManager<TUser>().Object,
                new Mock<IHttpContextAccessor>().Object,
                new Mock<IUserClaimsPrincipalFactory<TUser>>().Object,
                null, null, null, null);
        }

        public static Mock<DbSet<T>> GetQueryableMockDbSet<T>(List<T> sourceList) where T : class
        {
            var queryable = sourceList.AsQueryable();
            var dbSet = new Mock<DbSet<T>>();
            dbSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            dbSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            dbSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            dbSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());

            dbSet.Setup(d => d.Add(It.IsAny<T>())).Callback<T>((s) => sourceList.Add(s));

            return dbSet;
        }
    }
}
