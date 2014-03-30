using System;

namespace Stored.Tests.Models
{
    public class Purchase
    {
        public Guid Id { get; set; }
        public string Product { get; set; }
        public decimal Price { get; set; }
        public int Qty { get; set; }
    }
}