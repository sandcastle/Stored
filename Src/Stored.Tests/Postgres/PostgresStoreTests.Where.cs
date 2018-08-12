using System.Collections.Generic;
using Stored.Query;
using Stored.Tests.Models;
using Xunit;

namespace Stored.Tests.Postgres
{
    public partial class PostgresStoreTests
    {
        [Fact]
        [Trait(TraitName, "")]
        public void CanRestrictByTrueBoolean()
        {
            // Arrange
            var cars = new List<Car>
            {
                new Car
                {
                    Make = "Toyota",
                    Model = "Carolla",
                    IsActive = true
                },
                new Car
                {
                    Make = "Ford",
                    Model = "Falcon",
                    IsActive = true
                },
                new Car
                {
                    Make = "Ford",
                    Model = "Mustang",
                    IsActive = false
                }
            };

            Session.CreateAll(cars);
            Session.Commit();

            // Act
            var activeCars = Session.Query<Car>()
                .Where(x => x.IsActive).Equal(true)
                .ToList();

            // Assert
            Assert.NotEmpty(activeCars);
            Assert.Equal(2, activeCars.Count);
            Assert.Contains(activeCars, c => c.Model == cars[0].Model);
            Assert.Contains(activeCars, c => c.Model == cars[1].Model);
        }

        [Fact]
        [Trait(TraitName, "")]
        public void CanRestrictByFalseBoolean()
        {
            // Arrange
            var cars = new List<Car>
            {
                new Car
                {
                    Make = "Toyota",
                    Model = "Carolla",
                    IsActive = true
                },
                new Car
                {
                    Make = "Ford",
                    Model = "Falcon",
                    IsActive = false
                },
                new Car
                {
                    Make = "Ford",
                    Model = "Mustang",
                    IsActive = false
                }
            };

            Session.CreateAll(cars);
            Session.Commit();

            // Act
            var activeCars = Session.Query<Car>()
                .Where(x => x.IsActive).Equal(false)
                .ToList();

            // Assert
            Assert.NotEmpty(activeCars);
            Assert.Equal(2, activeCars.Count);
            Assert.Contains(activeCars, c => c.Model == cars[1].Model);
            Assert.Contains(activeCars, c => c.Model == cars[2].Model);
        }

        // --


        [Fact]
        [Trait(TraitName, "")]
        public void WhereTrue_ReturnsCorrectValues()
        {
            // Arrange
            var cars = new List<Car>
            {
                new Car
                {
                    Make = "Toyota",
                    Model = "Carolla",
                    IsActive = true
                },
                new Car
                {
                    Make = "Ford",
                    Model = "Falcon",
                    IsActive = true
                },
                new Car
                {
                    Make = "Ford",
                    Model = "Mustang",
                    IsActive = false
                }
            };

            Session.CreateAll(cars);
            Session.Commit();

            // Act
            var activeCars = Session.Query<Car>()
                .WhereTrue(x => x.IsActive)
                .ToList();

            // Assert
            Assert.NotEmpty(activeCars);
            Assert.Equal(2, activeCars.Count);
            Assert.Contains(activeCars, c => c.Model == cars[0].Model);
            Assert.Contains(activeCars, c => c.Model == cars[1].Model);
        }
    }
}
