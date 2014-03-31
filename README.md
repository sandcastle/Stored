Stored
======

A light weight data store for storing entities with support for unit of work. 

There are currently 2 supported providers in-memory (really fast) and Postgres.

### Getting an entity

```csharp
var store = new PostgresStore(connectionString);

using (var session = store.CreateSession())
{
    var id = new Guid("3E137227-2BEF-451F-B392-482CDC2216B0");

    var car = session.Get<Car>(id);
}
```

### Saving an entity

```csharp
var store = new PostgresStore(connectionString);

using (var session = store.CreateSession())
{
    session.Create(new Car { Make = "Astin Martin", Model = "DB9 Volante" });
    session.Commit();
}
```

### Modifying an entity

```csharp
var store = new PostgresStore(connectionString);

using (var session = store.CreateSession())
{
    var car = session.Get<Car>(carId);
	car.Model = "Mustang";

    session.Modify(car);
    session.Commit();
}
```

### Deleting an entity

```csharp
var store = new PostgresStore(connectionString);

using (var session = store.CreateSession())
{
    var car = session.Get<Car>(carId);

    session.Delete(car);
    session.Commit();
}
```

### Querying for entities

I plan to move this to a basic linq provider which will simplify this dramatically.

```csharp
var store = new PostgresStore(connectionString);

using (var session = store.CreateSession())
{
	session.Create(new Car { Make = "Toyota", Model = "Rav4" });
    session.Create(new Car { Make = "Astin Martin", Model = "DB9 Volante" });
    session.Create(new Car { Make = "Toyota", Model = "Corolla" });
    session.Commit();

    var query = new Query { Take = 100 };
    query.Filters.WithEqual("Make", "Toyota");

    var items = session.Query<Car>(query).ToList();
}
```

### Bulk create

```csharp
var store = new PostgresStore(connectionString);

using (var session = store.CreateSession())
{
    var items = new List<Car>
    {
        new Car { Make = "Astin Martin", Model = "DB9 Volante" },
        new Car { Make = "Toyota", Model = "Rav4" }
    };

    // does not buffer in unit of work
    session.Advanced.BulkCreate(items);
}
```

Inspiration
===

Inspriation from this project has come from Raven DB and Biggy.
