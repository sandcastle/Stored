using Stored.Memory;

namespace Stored.Tests
{
    public class MemoryTest
    {
        readonly MemoryStore _store;
        readonly MemorySession _session;

        public MemoryTest()
        {
            _store = new MemoryStore();
            _session = _store.CreateSession();
        }

        protected MemoryStore Store
        {
            get { return _store; }
        }

        protected MemorySession Session
        {
            get { return _session; }
        }
    }
}
