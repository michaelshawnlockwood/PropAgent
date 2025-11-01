// core/Runtime/IVaultLeaseManager.cs
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PropAgent.Core.Runtime
{
    public interface IVaultLeaseManager
    {
        Task RegisterAsync(string sessionId, string tenantId, string leaseId, TimeSpan leaseDuration, CancellationToken ct = default);
        Task UnregisterAsync(string sessionId, string tenantId, CancellationToken ct = default);
        Task TickAsync(CancellationToken ct = default);
    }
}
