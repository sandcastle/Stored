﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Stored.Tests.Models;
using Xunit;

namespace Stored.Tests.Postgres
{
    public class PostgresStoreTests : PostgresTest
    {
        const string TraitName = "Integration";

        [Fact]
        [Trait(TraitName, "")]
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
        [Trait(TraitName, "")]
        public void CanBulkCreate()
        {
            // Arrange
            var items = new List<Car>
            {
                new Car {Make = "Toyota", Model = "Rav4"},
                new Car {Make = "Aston Martin", Model = "DB9 Volante"}
            };

            // Act
            Session.Advanced.BulkCreate(items);
        }

        [Fact]
        [Trait(TraitName, "")]
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
        [Trait(TraitName, "")]
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
        [Trait(TraitName, "")]
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

            // Act
            Session.Delete(car);
        }

        [Fact]
        [Trait(TraitName, "")]
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
                Assert.Null(session3.Get<Car>(carId));
            }
        }

        [Fact]
        [Trait(TraitName, "")]
        public void CanQuery()
        {
            // Arrange
            Session.Create(new Car { Make = "Toyota", Model = "Rav4" });
            Session.Create(new Car { Make = "Aston Martin", Model = "DB9 Volante" });
            Session.Create(new Car { Make = "Toyota", Model = "Corolla" });
            Session.Commit();

            // Act
            var items = Session.Query<Car>()
                .Where(x => x.Make).Equal("Toyota")
                .ToList();

            // Assert
            Assert.Equal(2, items.Count);
        }

        [Fact]
        [Trait(TraitName, "")]
        public void CanQueryNullable()
        {
            // Arrange
            var one = new Other
            {
                Id = Guid.NewGuid(),
                GameId = Guid.NewGuid()
            };
            var two = new Other
            {
                Id = Guid.NewGuid(),
                GameId = null
            };

            Session.Create(one);
            Session.Create(two);
            Session.Commit();

            // Act
            var others = Session.Query<Other>()
                .Where(x => x.GameId).Equal(null)
                .ToList();

            // Assert
            Assert.Equal(1, others.Count);
            Assert.Null(others[0].GameId);
        }

        [Fact]
        [Trait(TraitName, "")]
        public void CanQueryBoolTrue()
        {
            // Arrange
            Session.Create(new Car { Make = "Toyota", Model = "Rav4", CarType = CarType.Suv, IsAutomatic = true});
            Session.Create(new Car { Make = "Aston Martin", Model = "DB9 Volante", CarType = CarType.Sedan });
            Session.Create(new Car { Make = "Toyota", Model = "Corolla", CarType = CarType.Sedan, IsAutomatic = true});
            Session.Commit();

            // Act
            var items = Session.Query<Car>()
                .Where(x => x.IsAutomatic).Equal(true)
                .ToList();

            // Assert
            Assert.Equal(2, items.Count);
        }

        [Fact]
        [Trait(TraitName, "")]
        public void CanQueryBoolFalse()
        {
            // Arrange
            Session.Create(new Car { Make = "Toyota", Model = "Rav4", CarType = CarType.Suv, IsAutomatic = true });
            Session.Create(new Car { Make = "Aston Martin", Model = "DB9 Volante", CarType = CarType.Sedan });
            Session.Create(new Car { Make = "Toyota", Model = "Corolla", CarType = CarType.Sedan, IsAutomatic = true });
            Session.Commit();

            // Act
            var items = Session.Query<Car>()
                .Where(x => x.IsAutomatic).Equal(false)
                .ToList();

            // Assert
            Assert.Single(items);
        }

        [Fact]
        [Trait(TraitName, "")]
        public void CanQueryEnum()
        {
            // Arrange
            Session.Create(new Car { Make = "Toyota", Model = "Rav4", CarType = CarType.Suv });
            Session.Create(new Car { Make = "Aston Martin", Model = "DB9 Volante", CarType = CarType.Sedan });
            Session.Create(new Car { Make = "Toyota", Model = "Corolla", CarType = CarType.Hatch });
            Session.Commit();

            // Act
            var items = Session.Query<Car>()
                .Where(x => x.CarType).Equal(CarType.Hatch)
                .ToList();

            // Assert
            Assert.Single(items);
        }

        [Fact]
        [Trait(TraitName, "")]
        public void CanQueryEnumWithUnderlyingType()
        {
            // Arrange
            Session.Create(new Car { Make = "Toyota", Model = "Rav4", CarType = CarType.Suv });
            Session.Create(new Car { Make = "Aston Martin", Model = "DB9 Volante", CarType = CarType.Sedan });
            Session.Create(new Car { Make = "Toyota", Model = "Corolla", CarType = CarType.Hatch });
            Session.Commit();

            // Act
            var items = Session.Query<Car>()
                .Where(x => x.CarType).Equal(1)
                .ToList();

            // Assert
            Assert.Single(items);
        }

        [Fact]
        [Trait(TraitName, "")]
        public void CanQueryStringFilter()
        {
            // Arrange
            Session.Create(new Car { Make = "Toyota", Model = "Rav4" });
            Session.Create(new Car { Make = "Aston Martin", Model = "DB9 Volante" });
            Session.Create(new Car { Make = "Toyota", Model = "Corolla" });
            Session.Commit();

            // Act
            var items = Session.Query<Car>()
                .Where("Make").Equal("Toyota")
                .ToList();

            // Assert
            Assert.Equal(2, items.Count);
        }

        [Fact]
        [Trait(TraitName, "")]
        public void CanQueryWithoutRestrictions()
        {
            // Arrange
            Session.Create(new Car { Make = "Toyota", Model = "Rav4" });
            Session.Create(new Car { Make = "Aston Martin", Model = "DB9 Volante" });
            Session.Create(new Car { Make = "Toyota", Model = "Corolla" });
            Session.Commit();

            // Act
            var items = Session.Query<Car>()
                .ToList();

            // Assert
            Assert.Equal(3, items.Count);
        }

        [Fact]
        [Trait(TraitName, "")]
        public void CanQuerySortedWithoutRestrictions()
        {
            // Arrange
            Session.Create(new Car { Make = "Toyota", Model = "Rav4" });
            Session.Create(new Car { Make = "Aston Martin", Model = "DB9 Volante" });
            Session.Create(new Car { Make = "Toyota", Model = "Corolla" });
            Session.Commit();

            // Act
            var item = Session.Query<Car>()
                .OrderBy(x => x.Make)
                .FirstOrDefault();

            // Assert
            Assert.Equal("Aston Martin", item.Make);
        }

        [Fact]
        [Trait(TraitName, "")]
        public void CanQuerySorted()
        {
            // Arrange
            Session.Create(new Car { Make = "Toyota", Model = "Rav4" });
            Session.Create(new Car { Make = "Aston Martin", Model = "DB9 Volante" });
            Session.Create(new Car { Make = "Toyota", Model = "Corolla" });
            Session.Commit();

            // Act
            var items = Session.Query<Car>()
                .Where(x => x.Make).Equal("Toyota")
                .OrderBy(x => x.Model)
                .ToList();

            // Assert
            Assert.Equal(2, items.Count);
            Assert.Equal("Corolla", items[0].Model);
        }

        [Fact]
        [Trait(TraitName, "")]
        public void CanQuerySortedDescending()
        {
            // Arrange
            Session.Create(new Car { Make = "Toyota", Model = "Rav4" });
            Session.Create(new Car { Make = "Aston Martin", Model = "DB9 Volante" });
            Session.Create(new Car { Make = "Toyota", Model = "Corolla" });
            Session.Commit();

            // Act
            var items = Session.Query<Car>()
                .Where(x => x.Make).Equal("Toyota")
                .OrderBy(x => x.Model, order: SortOrder.Descending)
                .ToList();

            // Assert
            Assert.Equal(2, items.Count);
            Assert.Equal("Rav4", items[0].Model);
        }

        [Fact]
        [Trait(TraitName, "")]
        public void CanQuerySortedWithoutRestrictionsWithPropertyName()
        {
            // Arrange
            Session.Create(new Car { Make = "Toyota", Model = "Rav4" });
            Session.Create(new Car { Make = "Aston Martin", Model = "DB9 Volante" });
            Session.Create(new Car { Make = "Toyota", Model = "Corolla" });
            Session.Commit();

            // Act
            var item = Session.Query<Car>()
                .OrderBy("Make")
                .FirstOrDefault();

            // Assert
            Assert.Equal("Aston Martin", item.Make);

        }

        [Fact]
        [Trait(TraitName, "")]
        public void CanCreateAll()
        {
            // Arrange
            Session.CreateAll(new[] {
                new Car { Make = "Toyota", Model = "Rav4" },
                new Car { Make = "Aston Martin", Model = "DB9 Volante" },
                new Car { Make = "Toyota", Model = "Corolla" }
            });
            Session.Commit();

            // Act
            var items = Session.All<Car>();

            // Assert
            Assert.Equal(3, items.Count);
        }

        [Fact]
        [Trait(TraitName, "")]
        public void CanGetAll()
        {
            // Arrange
            Session.Create(new Car { Make = "Toyota", Model = "Rav4" });
            Session.Create(new Car { Make = "Aston Martin", Model = "DB9 Volante" });
            Session.Create(new Car { Make = "Toyota", Model = "Corolla" });
            Session.Commit();

            // Act
            var items = Session.All<Car>();

            // Assert
            Assert.Equal(3, items.Count);
        }

        [Fact]
        [Trait(TraitName, "")]
        public void CanQueryWithSkip()
        {
            // Arrange
            Session.Create(new Car { Make = "Toyota", Model = "Rav4" });
            Session.Create(new Car { Make = "Aston Martin", Model = "DB9 Volante" });
            Session.Create(new Car { Make = "Toyota", Model = "Corolla" });
            Session.Commit();

            // Act
            var items = Session.Query<Car>()
                .Where(x => x.Make).Equal("Toyota")
                .Skip(1)
                .ToList();

            // Assert
            Assert.Single(items);
        }

        [Fact]
        [Trait(TraitName, "")]
        public void CanQueryWithTake()
        {
            // Arrange
            Session.Create(new Car { Make = "Toyota", Model = "Rav4" });
            Session.Create(new Car { Make = "Aston Martin", Model = "DB9 Volante" });
            Session.Create(new Car { Make = "Toyota", Model = "Corolla" });
            Session.Commit();

            // Act
            var items = Session.Query<Car>()
                .Where(x => x.Make).Equal("Toyota")
                .Take(1)
                .ToList();

            // Assert
            Assert.Single(items);
        }

        [Fact]
        [Trait(TraitName, "")]
        public void CanQueryOne()
        {
            // Arrange
            Session.Create(new Car { Make = "Toyota", Model = "Rav4" });
            Session.Create(new Car { Make = "Aston Martin", Model = "DB9 Volante" });
            Session.Create(new Car { Make = "Toyota", Model = "Corolla" });
            Session.Commit();

            // Act
            var car = Session.Query<Car>()
                .Where(x => x.Make).Equal("Aston Martin")
                .FirstOrDefault();

            // Assert
            Assert.NotNull(car);
            Assert.Equal("DB9 Volante", car.Model);
        }

        [Fact]
        [Trait(TraitName, "")]
        public void DoesReturnCorrectTotalCount()
        {
            // Arrange
            Session.Create(new Car { Make = "Toyota", Model = "Rav4" });
            Session.Create(new Car { Make = "Aston Martin", Model = "DB9 Volante" });
            Session.Create(new Car { Make = "Toyota", Model = "Corolla" });
            Session.Commit();

            // Act
            var items = Session.Query<Car>()
                .Statistics(out var stats)
                .Where(x => x.Make).Equal("Toyota")
                .Skip(1)
                .ToList();

            // Assert
            Assert.Single(items);
            Assert.Equal(2, stats.TotalCount.Value);
            Assert.Equal(1, stats.Skip);
            Assert.Equal(1024, stats.Take);
        }

        [Fact]
        public void CanCreateInParallelWithoutIssue()
        {
            const int itemCount = 500;

            // Arrange
            var items = new List<int>();
            for (int i = 0; i < itemCount; i++)
            {
                items.Add(i);
            }

            // Act
            Parallel.ForEach(items, i =>
            {
                var car = new Car
                {
                    Id = Guid.NewGuid(),
                    Make = $"Toyota {i}",
                    Model = $"Rav4 {i}"
                };
                Session.Create(car);
            });
            Session.Commit();

            Session.Query<Car>()
                .Take(2)
                .Statistics(out var stats)
                .ToList();

            Assert.Equal(itemCount, stats.TotalCount.Value);
        }
    }
}
