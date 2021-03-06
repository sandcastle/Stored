﻿using System;

namespace Stored.Postgres
{
    public class PostgresTableMetadata : ITableMetadata
    {
        public PostgresTableMetadata(Type type)
        {
            Type = type;
            Name = type.Name.ToLower();
        }

        public Type Type { get; }
        public string Name { get; }
    }
}