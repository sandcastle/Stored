﻿using System;
using System.Collections.Generic;
using Stored.Tests.Models;

namespace Stored.Tests.Memory.Scenarios
{
    public class UserFactory
    {
        static readonly string[] _names =
        {
            "Tara Nore",
            "Sally Fields",
            "Byron Sims",
            "Wally Walkers",
            "Jane Saw",
            "Damon Tully",
            "Glenn Aesop",
            "Brian Pims"
        };

        public static List<User> CreateUsers(int count)
        {
            var list = new List<User>();
            var random = new Random();

            for (var i = 0; i < count; i++)
            {
                list.Add(new User
                {
                    Id = Guid.NewGuid(),
                    ApiKey = CreateKey(),
                    Name = _names[random.Next(0, _names.Length - 1)],
                    Type = ((UserType) (random.Next(0, 100) % 2))
                });
            }

            return list;
        }

        public static string CreateKey()
        {
            return Guid.NewGuid()
                .ToString("N")
                .ToUpper()
                .Substring(0, 15);
        }
    }
}