Stored
======

A lightweight data store for saving entities as JSON in Postgres with support for unit of work.

```
var store = new PostgresStore(connectionString);

using (var session = store.CreateSession())
{
    session.Create(new Car { Make = "Astin Martin", Model = "DB9 Volante" });
    session.Create(new Car { Make = "Toyota", Model = "Rav4" });
    
    session.Commit();
}
```
