using System;
using System.Collections.Generic;
using System.Linq;
using Stored.Tests.Models;
using Xunit;

namespace Stored.Tests.Memory
{
    public class MemorySessionTests : MemoryTest
    {
        [Fact]
        public void CanCreate()
        {
            // Arrange
            var car = new Car
            {
                Make = "Toyota",
                Model = "Rav4"
            };

            // Act
            car = Session.Create(car);
            Session.Commit();

            // Asssert
            Assert.NotNull(car);
            Assert.NotEqual(Guid.Empty, car.Id);
        }

        [Fact]
        public void CanBulkCreate()
        {
            // Arrange
            var items = new List<Car>
            {
                new Car {Make = "Toyota", Model = "Rav4"},
                new Car {Make = "Astin Martin", Model = "DB9 Volante"}
            };

            // Act
            Session.Advanced.BulkCreate(items);

            // Assert
            Assert.Equal(2, Store[typeof(Car)].Count);
        }

        [Fact]
        public void CanGet()
        {
            // Arrange
            var car = new Car
            {
                Make = "Toyota",
                Model = "Rav4"
            };

            car = Session.Create(car);

            // Act
            var again = Session.Get<Car>(car.Id);

            // Asssert
            Assert.NotNull(again);
            Assert.Equal(car.Id, again.Id);
            Assert.Equal("Rav4", again.Model);
        }

        [Fact]
        public void CanUpdate()
        {
            // Arrange
            var car = new Car
            {
                Make = "Toyota",
                Model = "Rav4"
            };

            car = Session.Create(car);

            car.Model = "Corolla";

            // Act
            var updated = Session.Modify(car);

            // Asssert
            Assert.NotNull(updated);
            Assert.Equal(car.Id, updated.Id);
            Assert.Equal("Corolla", updated.Model);
        }

        [Fact]
        public void CanDelete()
        {
            // Arrange
            var car = new Car
            {
                Make = "Toyota",
                Model = "Rav4"
            };

            car = Session.Create(car);

            car.Model = "Corolla";

            // Act / Asssert
            Assert.DoesNotThrow(() => Session.Delete(car));
        }

        [Fact]
        public void CanQuery()
        {
            // Arrange
            Session.Create(new Car { Make = "Toyota", Model = "Rav4" });
            Session.Create(new Car { Make = "Astin Martin", Model = "DB9 Volante" });
            Session.Create(new Car { Make = "Toyota", Model = "Corolla" });
            Session.Commit();

            var query = new Query { Take = 100 };
            query.Filters.WithEqual("Make", "Toyota");

            // Act
            var items = Session.Query<Car>(query).ToList();

            // Asssert
            Assert.Equal(2, items.Count());

        }

        [Fact]
        public void CreateWithModelThatHasNoIdShouldThrow()
        {
            // Assert
            Assert.Throws<EntityException>(() => Session.Create(new Bad()));
        }

        [Fact]
        public void CreateWithModelThatHasAnIdShouldntOverwrite()
        {
            // Arrange
            var car = new Car
            {
                Id = Guid.NewGuid(),
                Make = "Toyota",
                Model = "Rav4"
            };

            // Act
            car = Session.Create(car);
            Session.Commit();

            // Asssert
            Assert.NotNull(car);
            Assert.Equal(car.Id, car.Id);
        }
    }
}
