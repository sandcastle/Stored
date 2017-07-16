using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Stored.Tests.Models;
using Xunit;
using Xunit.Extensions;

namespace Stored.Tests.Postgres
{
    public class PostgresStotePerfTests : PostgresTest
    {
        const string TraitName = "Performance";

        static readonly Random _random = new Random();
        static readonly string[] _products = { "Apples", "Pears", "Chips", "Corn" };

        [Theory]
        [InlineData(10)]
        [InlineData(1000)]
        [InlineData(1000000)]
        [Trait(TraitName, "")]
        public void CanQueryLargeDataSets(int count)
        {
            var purchases = new List<Purchase>();

            for (var i = 0; i < count; i++)
            {
                purchases.Add(new Purchase
                {
                    Product = SelectProduct(),
                    Price = (decimal)_random.Next(100, 1000) / 10,
                    Qty = _random.Next(1, 10)
                });
            }

            var watch = new Stopwatch();
            watch.Start();

            Session.Advanced.BulkCreate(purchases);

            watch.Stop();
            Console.WriteLine("Write count {0}", count);
            Console.WriteLine("Write time {0}", watch.Elapsed);

            watch.Reset();
            watch.Start();

            var items = Session.Query<Purchase>()
                .Where(x => x.Product).Equal("Apples")
                .Take(512)
                .ToList();

            watch.Stop();
            Console.WriteLine("Results {0}", items.Count);
            Console.WriteLine("Query time {0}", watch.Elapsed);

            // Assert
            Assert.True(items.Count <= count);
            Assert.True(items.All(x => x.Product == "Apples"));
            Assert.True(watch.ElapsedMilliseconds < 2000);
        }

        static string SelectProduct()
        {
            return _products[_random.Next(0, _products.Length)];
        }
    }
}
