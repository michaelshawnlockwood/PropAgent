using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace PropAgent.Core.Runtime
{
    public sealed class SessionConnectionCache : ISessionConnectionCache
    {
        private readonly IMemoryCache _cache;
        private readonly TimeProvider _time;

        public SessionConnectionCache(IMemoryCache cache, TimeProvider timeProvider)
        {
            _cache = cache;
            _time = timeProvider;
        }

        private static string Key(string sessionId, string tenantId) => $"session:{sessionId}:tenant:{tenantId}:conn";

        public Task SetAsync(string sessionId, string tenantId, string connectionString, DateTimeOffset absoluteExpiration, TimeSpan slidingExpiration, string? leaseId, DateTimeOffset? leaseExpiresAt, CancellationToken ct = default)
        {
            var key = Key(sessionId, tenantId);
            var value = new CacheItem(connectionString, leaseId, leaseExpiresAt);
            var opts = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = absoluteExpiration,
                SlidingExpiration = slidingExpiration
            };
            _cache.Set(key, value, opts);
            return Task.CompletedTask;
        }

        public Task<(bool Found, string? ConnectionString, string? LeaseId, DateTimeOffset? LeaseExpiresAt)> GetAsync(string sessionId, string tenantId, CancellationToken ct = default)
        {
            var key = Key(sessionId, tenantId);
            if (_cache.TryGetValue(key, out CacheItem? value) && value != null)
                return Task.FromResult<(bool, string?, string?, DateTimeOffset?)>((true, value.ConnectionString, value.LeaseId, value.LeaseExpiresAt));
            return Task.FromResult<(bool, string?, string?, DateTimeOffset?)>((false, null, null, null));
        }

        public Task RemoveAsync(string sessionId, string tenantId, CancellationToken ct = default)
        {
            _cache.Remove(Key(sessionId, tenantId));
            return Task.CompletedTask;
        }

        private sealed record CacheItem(string ConnectionString, string? LeaseId, DateTimeOffset? LeaseExpiresAt);
    }
}
