Stored
======

A lightweight data store for saving entities as JSON in Postgres with support for unit of work.

### Saving an entity

```csharp
var store = new PostgresStore(connectionString);

using (var session = store.CreateSession())
{
    session.Create(new Car { Make = "Astin Martin", Model = "DB9 Volante" });
    session.Commit();
}
```

### Getting an entity

```csharp
var store = new PostgresStore(connectionString);

using (var session = store.CreateSession())
{
    var id = new Guid("3E137227-2BEF-451F-B392-482CDC2216B0");

    var car = session.Get<Car>(id);
}
```

### Bulk insert

```csharp
var store = new PostgresStore(connectionString);

using (var session = store.CreateSession())
{
    var items = new List<Car>
    {
        ew Car { Make = "Astin Martin", Model = "DB9 Volante" },
        ew Car { Make = "Toyota", Model = "Rav4" }
    };

    // does not buffer in unit of work
    session.Advanced.BulkCreate(items);
}
```
