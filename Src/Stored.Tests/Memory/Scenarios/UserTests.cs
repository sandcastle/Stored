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

            var key = users[1].ApiKey;

            // Act
            var results = Session.Query<User>()
                .Where(x => x.ApiKey).Equal(key.ToUpper())
                .ToList();

            // Assert
            Assert.Equal(1, results.Count);
            Assert.Equal(key, results[0].ApiKey);
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
