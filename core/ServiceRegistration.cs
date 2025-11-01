// core/ServiceRegistration.cs
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PropAgent.Core.Data;
using PropAgent.Core.Runtime;
using PropAgent.Core.Security;

namespace PropAgent.Core
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddPropAgentCore(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMemoryCache();
            services.AddHttpClient();

            services.AddSingleton<IAppVaultClient, AppVaultClient>();
            services.AddSingleton<IConnectionStringFactory, ConnectionStringFactory>();
            services.AddSingleton<ISessionConnectionCache, SessionConnectionCache>();
            services.AddSingleton<IVaultLeaseManager, VaultLeaseManager>();
            services.AddSingleton(TimeProvider.System);

            return services;
        }
    }
}
