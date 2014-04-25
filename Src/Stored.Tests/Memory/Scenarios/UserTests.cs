using System;
using System.Collections.Generic;
using Stored.Tests.Models;
using Xunit;

namespace Stored.Tests.Memory.Scenarios
{
    public class UserTests : MemoryTest
    {
        static readonly string[] _names = { "Tara Nore", "Sally Fields", "Byron Sims", "Wally Walkers", "Jane Saw" };

        [Fact]
        public void CanRandomlyQueryUser()
        {
            // Arrange
            var users = CreateUsers();

            foreach (var user in users)
            {
                Session.Create(user);    
            }
            
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
            var users = CreateUsers();

            foreach (var user in users)
            {
                Session.Create(user);
            }

            Session.Commit();
            
            // Act
            var result = Session.Query<User>()
                .Where(x => x.Id).Equal(users[1].Id)
                .FirstOrDefault();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(users[1].Id, result.Id);
        }

        static List<User> CreateUsers()
        {
            var list = new List<User>();

            for (var i = 0; i < 3; i++)
            {
                list.Add(new User
                {
                    Id = Guid.NewGuid(),
                    ApiKey = CreateKey(),
                    Name = _names[new Random().Next(0, _names.Length - 1)]
                });
            }

            return list;
        }

        static string CreateKey()
        {
            return Guid.NewGuid()
                .ToString("N")
                .ToUpper()
                .Substring(0, 15);
        }
    }
}
