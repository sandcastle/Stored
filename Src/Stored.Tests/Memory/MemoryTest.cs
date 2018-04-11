using Stored.Memory;

namespace Stored.Tests.Memory
{
    public class MemoryTest
    {
        protected MemoryTest()
        {
            Store = new MemoryStore();
            Session = Store.CreateSession();

            // Ensure the identity cache is fresh
            IdentityFactory.Clear();
        }

        protected MemoryStore Store { get; }

        protected MemorySession Session { get; }
    }
}
