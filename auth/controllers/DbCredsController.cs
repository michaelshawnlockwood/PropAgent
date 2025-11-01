using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PropAgent.Core.Data;
using PropAgent.Core.Runtime;
using PropAgent.Core.Tenancy.Secrets;
using System.Text.Json.Serialization;

namespace PropAgent.Auth.Controllers
{
    [ApiController]
    [Route("{tenantKey}/api/dbcreds")]
    public sealed class DbCredsController : ControllerBase
    {
        private readonly IConnectionStringFactory connFactory;
        private readonly ISessionConnectionCache sessionCache;
        private readonly TimeProvider timeProvider;

        private readonly IVaultSecrets _secrets;

        public DbCredsController(
            IVaultSecrets secrets,
            IConnectionStringFactory connFactory,
            ISessionConnectionCache sessionCache,
            TimeProvider timeProvider)
        {
            _secrets = secrets;
            this.connFactory = connFactory;
            this.sessionCache = sessionCache;
            this.timeProvider = timeProvider;
        }


        [HttpGet] // GET /{tenantKey}/api/dbcreds
        public async Task<IActionResult> Get(string tenantKey)
        {

            var now = timeProvider.GetUtcNow();
            var absExp = now.AddHours(2);
            var sliding = TimeSpan.FromMinutes(20);

            var c = await _secrets.GetDbCredsAsync(tenantKey);

            var _connStr = connFactory.BuildNpgsql(c.Host, c.Db, c.User, c.Password!, sslRequired: true, trustServerCertificate: false);

            // var realConn = connFactory.BuildNpgsql(c.Host, 5432, c.Db, c.User, c.Password, sslRequired: true, trustServerCertificate: false);
            var preview = connFactory.BuildNpgsqlPreview(c.Host, c.Db, c.User, sslRequired: true, trustServerCertificate: false);

            var sessionId = HttpContext.TraceIdentifier;
            await sessionCache.SetAsync(sessionId, tenantKey, _connStr, absExp, sliding, leaseId: null, leaseExpiresAt: null);

            return Ok(new
            {
                tenantKey = c.TenantKey,
                user = c.User,
                host = c.Host,
                db = c.Db,
                hasPassword = c.HasPassword,
                connPreview = preview
            });
        }
    }
}
