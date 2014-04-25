using System;
using System.Collections.Generic;
using Stored.Tests.Models;
using Xunit;

namespace Stored.Tests.Memory.Scenarios
{
    public class MultiFilterTests : MemoryTest
    {
        [Fact]
        public void CanQueryWithOneFilter()
        {
            // Arrange
            Session.CreateAll(GetCars());
            Session.Commit();

            // Act
            var results = Session.Query<Car>()
                .Where(x => x.Make).Equal("Ford")
                .ToList();

            // Assert
            Assert.Equal(4, results.Count);
        }

        [Fact]
        public void CanQueryWithTwoFilters()
        {
            // Arrange
            Session.CreateAll(GetCars());
            Session.Commit();

            // Act
            var results = Session.Query<Car>()
                .Where(x => x.Make).Equal("Ford")
                .Where(x => x.Model).Equal("Shelby Mustang")
                .ToList();

            // Assert
            Assert.Equal(2, results.Count);
        }
        
        [Fact]
        public void CanQueryWithThreeFilters()
        {
            // Arrange
            Session.CreateAll(GetCars());
            Session.Commit();

            // Act
            var results = Session.Query<Car>()
                .Where(x => x.Make).Equal("Ford")
                .Where(x => x.Model).Equal("Shelby Mustang")
                .Where(x => x.Id).Equal(_knownId)
                .ToList();

            // Assert
            Assert.Equal(1, results.Count);
        }

        static readonly Guid _knownId = new Guid("2798B862-9909-472A-8FDA-A0741ED2026B");

        static IEnumerable<Car> GetCars()
        {
            return new[]
            {
                new Car { Make = "Toyota", Model = "Rav 4" },
                new Car { Make = "Ferarri", Model = "F50" },
                new Car { Make = "Ford", Model = "Focus" },
                new Car { Make = "Ford", Model = "Festiva" },
                new Car { Make = "Ford", Model = "Shelby Mustang" },
                new Car { Make = "Ford", Model = "Shelby Mustang", Id = _knownId }
            };
        }
    }
}
