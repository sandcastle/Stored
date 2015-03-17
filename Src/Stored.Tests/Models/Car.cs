using System;

namespace Stored.Tests.Models
{
    public class Car
    {
        public Guid Id { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public bool IsAutomatic { get; set; }
        public CarType CarType { get; set; }
    }

    public enum CarType
    {
        Sedan = 0,
        Hatch = 1,
        Suv = 2
    }
}