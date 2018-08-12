using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Stored
{
    public interface ISessionAdvanced
    {
        Task BulkCreateAsync<T>(IEnumerable<T> items, CancellationToken token = default);
    }
}
