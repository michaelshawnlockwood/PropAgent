using System.Threading;
using System.Threading.Tasks;
using Npgsql;
using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using Shared;

namespace PropAgent.Auth.Infrastructure.Data
{
    public interface ITenantDb
    {
        Task<bool> PingAsync(CancellationToken ct = default);
    }

    public sealed class AppTenantDb : ITenantDb
    {
        private readonly IMultiTenantContextAccessor<AppTenantInfo> _ctx;

        public AppTenantDb(IMultiTenantContextAccessor<AppTenantInfo> ctx)
        {
            _ctx = ctx;
        }

        public async Task<bool> PingAsync(CancellationToken ct = default)
        {
            var tenant = _ctx.MultiTenantContext?.TenantInfo as AppTenantInfo;
            if (tenant is null || string.IsNullOrWhiteSpace(tenant.ConnectionString))
                return false;

            await using var conn = new NpgsqlConnection(tenant.ConnectionString);
            await conn.OpenAsync(ct);
            await using var cmd = new NpgsqlCommand("select 1", conn);
            var result = await cmd.ExecuteScalarAsync(ct);
            return result is int i && i == 1;
        }
    }
}
