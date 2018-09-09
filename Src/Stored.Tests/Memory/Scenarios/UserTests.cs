using System.Linq;
using Stored.Tests.Models;
using Xunit;

namespace Stored.Tests.Memory.Scenarios
{
    public class UserTests : MemoryTest
    {
        [Fact]
        public void CanRandomlyQueryUser()
        {
            // Arrange
            var users = UserFactory.CreateUsers(3);

            Session.CreateAll(users);
            Session.Commit();

            string key = users[1].ApiKey;

            // Act
            var results = Session.Query<User>()
                .Where(x => x.ApiKey).Equal(key.ToUpper())
                .ToList();

            // Assert
            Assert.Single(results);
            Assert.Equal(key, results[0].ApiKey);
        }

        [Fact]
        public void CanQueryWithEnum()
        {
            // Arrange
            var users = UserFactory.CreateUsers(10);

            Session.CreateAll(users);
            Session.Commit();

            // Act
            var result = Session.Query<User>()
                .Where(x => x.Type).Equal(UserType.Vip)
                .ToList();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(users.Count(x => x.Type == UserType.Vip), result.Count);
        }

        [Fact]
        public void CanQueryWithEnumByUnderlyingType()
        {
            // Arrange
            var users = UserFactory.CreateUsers(10);

            Session.CreateAll(users);
            Session.Commit();

            // Act
            var result = Session.Query<User>()
                .Where(x => x.Type).Equal(1)
                .ToList();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(users.Count(x => x.Type == UserType.Vip), result.Count);
        }

        [Fact]
        public void CanReturnTheSecondUser()
        {
            // Arrange
            var users = UserFactory.CreateUsers(3);

            Session.CreateAll(users);
            Session.Commit();

            // Act
            var result = Session.Query<User>()
                .Where(x => x.Id).Equal(users[1].Id)
                .FirstOrDefault();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(users[1].Id, result.Id);
        }
    }
}
