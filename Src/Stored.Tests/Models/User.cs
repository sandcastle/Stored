using System;

namespace Stored.Tests.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ApiKey { get; set; }
        public UserType Type { get; set; }
    }

    public enum UserType
    {
        Normal = 0,
        Vip = 1
    }
}
