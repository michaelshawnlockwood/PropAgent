// core/Runtime/VaultLeaseManager.cs
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PropAgent.Core.Security;

namespace PropAgent.Core.Runtime
{
    public sealed class VaultLeaseManager : IVaultLeaseManager
    {
        private readonly IAppVaultClient _vault;
        private readonly ISessionConnectionCache _cache;
        private readonly TimeProvider _time;
        private readonly ILogger<VaultLeaseManager> _logger;

        public VaultLeaseManager(IAppVaultClient vault, ISessionConnectionCache cache, TimeProvider timeProvider, ILogger<VaultLeaseManager> logger)
        {
            _vault = vault;
            _cache = cache;
            _time = timeProvider;
            _logger = logger;
        }

        public Task RegisterAsync(string sessionId, string tenantId, string leaseId, TimeSpan leaseDuration, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task UnregisterAsync(string sessionId, string tenantId, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task TickAsync(CancellationToken ct = default)
            => throw new NotImplementedException();
    }
}
