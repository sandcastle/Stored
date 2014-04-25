using System;
using System.Collections.Generic;
using System.Diagnostics;
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

            // Assert
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

            // Assert
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

            // Assert
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

            // Act
            Session.Delete(car);
            Session.Commit();

            // Assert
            Assert.Null(Session.Get<Car>(car.Id));
        }

        [Fact]
        public void CanDeleteWhenNotCreatedInSameSession()
        {
            Guid carId;

            // Arrange
            using (var session1 = Store.CreateSession())
            {
                var car = new Car
                {
                    Make = "Toyota",
                    Model = "Rav4"
                };

                carId = session1.Create(car).Id;
                session1.Commit();
            }

            // Act
            using (var session2 = Store.CreateSession())
            {
                var car = session2.Get<Car>(carId);
                session2.Delete(car);
                session2.Commit();
            }

            // Assert
            using (var session3 = Store.CreateSession())
            {
                Assert.Null(Session.Get<Car>(carId));
            }
        }

        [Fact]
        public void CanCreateAll()
        {
            // Arrange
            Session.CreateAll(new[] {
                new Car { Make = "Toyota", Model = "Rav4" },
                new Car { Make = "Astin Martin", Model = "DB9 Volante" },
                new Car { Make = "Toyota", Model = "Corolla" }
            });
            Session.Commit();

            // Act
            var items = Session.All<Car>();

            // Assert
            Assert.Equal(3, items.Count);
        }

        [Fact]
        public void CanGetAll()
        {
            // Arrange
            Session.Create(new Car { Make = "Toyota", Model = "Rav4" });
            Session.Create(new Car { Make = "Astin Martin", Model = "DB9 Volante" });
            Session.Create(new Car { Make = "Toyota", Model = "Corolla" });
            Session.Commit();

            // Act
            var items = Session.All<Car>();

            // Assert
            Assert.Equal(3, items.Count());
        }

        [Fact]
        public void CanQuery()
        {
            // Arrange
            Session.Create(new Car { Make = "Toyota", Model = "Rav4" });
            Session.Create(new Car { Make = "Astin Martin", Model = "DB9 Volante" });
            Session.Create(new Car { Make = "Toyota", Model = "Corolla" });
            Session.Commit();

            // Act
            var items = Session.Query<Car>()
                .Where(x => x.Make).Equal("Toyota")
                .ToList();

            // Assert
            Assert.Equal(2, items.Count());

        }

        [Fact]
        public void CanQueryWithoutRestrictions()
        {
            // Arrange
            Session.Create(new Car { Make = "Toyota", Model = "Rav4" });
            Session.Create(new Car { Make = "Astin Martin", Model = "DB9 Volante" });
            Session.Create(new Car { Make = "Toyota", Model = "Corolla" });
            Session.Commit();

            // Act
            var items = Session.Query<Car>()
                .ToList();

            // Assert
            Assert.Equal(3, items.Count);

        }

        [Fact]
        public void CanQueryOne()
        {
            // Arrange
            Session.Create(new Car { Make = "Toyota", Model = "Rav4" });
            Session.Create(new Car { Make = "Astin Martin", Model = "DB9 Volante" });
            Session.Create(new Car { Make = "Toyota", Model = "Corolla" });
            Session.Commit();

            // Act
            var car = Session.Query<Car>()
                .Where(x => x.Make).Equal("Astin Martin")
                .FirstOrDefault();

            // Assert
            Assert.NotNull(car);
            Assert.Equal("DB9 Volante", car.Model);
        }

        [Fact]
        public void CanQueryWithSkip()
        {
            // Arrange
            Session.Create(new Car { Make = "Toyota", Model = "Rav4" });
            Session.Create(new Car { Make = "Astin Martin", Model = "DB9 Volante" });
            Session.Create(new Car { Make = "Toyota", Model = "Corolla" });
            Session.Commit();

            // Act
            var items = Session.Query<Car>()
                .Where(x => x.Make).Equal("Toyota")
                .Skip(1)
                .ToList();

            // Assert
            Assert.Equal(1, items.Count());
        }

        [Fact]
        public void CanQueryWithTake()
        {
            // Arrange
            Session.Create(new Car { Make = "Toyota", Model = "Rav4" });
            Session.Create(new Car { Make = "Astin Martin", Model = "DB9 Volante" });
            Session.Create(new Car { Make = "Toyota", Model = "Corolla" });
            Session.Commit();

            // Act
            var items = Session.Query<Car>()
                .Where(x => x.Make).Equal("Toyota")
                .Take(1)
                .ToList();

            // Assert
            Assert.Equal(1, items.Count());
        }

        [Fact]
        public void CanQueryFast()
        {
            // Arrange
            Session.Create(new Car { Make = "Toyota", Model = "Carolla" });
            for (int i = 0; i < 200000; i++)
            {
                Session.Create(new Car {Make = "Ford", Model = "Focus"});
            }
            Session.Create(new Car { Make = "Toyota", Model = "Carolla" });

            Session.Commit();

            // Act
            var watch = new Stopwatch();
            watch.Start();

            var items = Session.Query<Car>()
                .Where(x => x.Make).Equal("Toyota")
                .ToList();

            watch.Stop();

            // Assert - should be sub 100ms for 200k full scan
            Assert.Equal(2, items.Count);
            Assert.True(watch.ElapsedMilliseconds < 150); 
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

            // Assert
            Assert.NotNull(car);
            Assert.Equal(car.Id, car.Id);
        }
    }
}
