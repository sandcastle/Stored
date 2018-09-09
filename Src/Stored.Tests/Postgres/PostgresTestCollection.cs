using Xunit;

namespace Stored.Tests.Postgres
{
    [CollectionDefinition(Tests.Postgres)]
    public class PostgresTestCollection : ICollectionFixture<PostgresTestFixture> { }
}