// core/Security/AppVaultClient.cs
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace PropAgent.Core.Security
{
    public sealed class AppVaultClient : IAppVaultClient
    {
        private readonly HttpClient _http;
        private readonly IConfiguration _config;

        public AppVaultClient(HttpClient httpClient, IConfiguration config)
        {
            _http = httpClient;
            _config = config;
        }

        public Task<(string Username, string Password, string LeaseId, TimeSpan LeaseDuration)> FetchDbCredentialsAsync(string tenantId, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task<bool> RenewLeaseAsync(string leaseId, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task RevokeLeaseAsync(string leaseId, CancellationToken ct = default)
            => throw new NotImplementedException();
    }
}
