// core/Security/IAppVaultClient.cs
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PropAgent.Core.Security
{
    public interface IAppVaultClient
    {
        Task<(string Username, string Password, string LeaseId, TimeSpan LeaseDuration)> FetchDbCredentialsAsync(string tenantId, CancellationToken ct = default);
        Task<bool> RenewLeaseAsync(string leaseId, CancellationToken ct = default);
        Task RevokeLeaseAsync(string leaseId, CancellationToken ct = default);
    }
}
