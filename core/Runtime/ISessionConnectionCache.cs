// core/Runtime/ISessionConnectionCache.cs
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PropAgent.Core.Runtime
{
    public interface ISessionConnectionCache
    {
        Task SetAsync(
            string sessionId,
            string tenantId,
            string connectionString,
                DateTimeOffset absoluteExpiration,
                TimeSpan slidingExpiration,
                string? leaseId,
                DateTimeOffset? leaseExpiresAt,
                CancellationToken ct = default);
        Task<(
            bool Found,
            string? ConnectionString,
            string? LeaseId,
            DateTimeOffset? LeaseExpiresAt)> GetAsync(string sessionId, string tenantId, CancellationToken ct = default);
        Task RemoveAsync(
            string sessionId,
            string tenantId,
            CancellationToken ct = default);
    }
}
