using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Finbuckle.MultiTenant.Abstractions;
using Shared;

namespace PropAgent.Auth.MultiTenancy
{
    public sealed class AppTenantStore : IMultiTenantStore<AppTenantInfo>
    {
        private readonly string _connString;

        public AppTenantStore(IConfiguration config)
        {
            _connString = config.GetConnectionString("ConfigDb") ?? string.Empty;
        }

        public async Task<AppTenantInfo?> TryGetAsync(string id)
        {
            const string sql = @"select id, identifier, name, connection_string
                                 from propagent.public.tenants where id = @id limit 1;";
            await using var conn = new NpgsqlConnection(_connString);
            await conn.OpenAsync();
            await using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("id", id);
            await using var reader = await cmd.ExecuteReaderAsync();
            if (!await reader.ReadAsync()) return null;

            return new AppTenantInfo
            {
                Id = reader.GetString(0),
                Identifier = reader.GetString(1),
                Name = reader.GetString(2),
                ConnectionString = reader.GetString(3)
            };
        }

        public async Task<AppTenantInfo?> TryGetByIdentifierAsync(string identifier)
        {
            const string sql = @"select id, identifier, name, connection_string
                                 from propagent.public.tenants where identifier = @identifier limit 1;";
            await using var conn = new NpgsqlConnection(_connString);
            await conn.OpenAsync();
            await using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("identifier", identifier);
            await using var reader = await cmd.ExecuteReaderAsync();
            if (!await reader.ReadAsync()) return null;

            return new AppTenantInfo
            {
                Id = reader.GetString(0),
                Identifier = reader.GetString(1),
                Name = reader.GetString(2),
                ConnectionString = reader.GetString(3)
            };
        }

        public async Task<IEnumerable<AppTenantInfo>> GetAllAsync()
        {
            const string sql = @"select id, identifier, name, connection_string
                                 from propagent.public.tenants order by identifier;";
            var list = new List<AppTenantInfo>();
            await using var conn = new NpgsqlConnection(_connString);
            await conn.OpenAsync();
            await using var cmd = new NpgsqlCommand(sql, conn);
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new AppTenantInfo
                {
                    Id = reader.GetString(0),
                    Identifier = reader.GetString(1),
                    Name = reader.GetString(2),
                    ConnectionString = reader.GetString(3)
                });
            }
            return list;
        }

        public async Task<IEnumerable<AppTenantInfo>> GetAllAsync(int take, int skip)
        {
            const string sql = @"select id, identifier, name, connection_string
                                 from propagent.public.tenants
                                 order by identifier
                                 limit @take offset @skip;";
            var list = new List<AppTenantInfo>();
            await using var conn = new NpgsqlConnection(_connString);
            await conn.OpenAsync();
            await using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("take", take);
            cmd.Parameters.AddWithValue("skip", skip);
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new AppTenantInfo
                {
                    Id = reader.GetString(0),
                    Identifier = reader.GetString(1),
                    Name = reader.GetString(2),
                    ConnectionString = reader.GetString(3)
                });
            }
            return list;
        }

        public Task<bool> TryAddAsync(AppTenantInfo tenantInfo) => Task.FromResult(false);
        public Task<bool> TryUpdateAsync(AppTenantInfo tenantInfo) => Task.FromResult(false);
        public Task<bool> TryRemoveAsync(string identifier) => Task.FromResult(false);
    }
}
