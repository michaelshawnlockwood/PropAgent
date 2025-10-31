using Microsoft.AspNetCore.Mvc;
using Finbuckle.MultiTenant.Abstractions;
using PropAgent.Auth.Infrastructure.Data;
using System.Threading.Tasks;

namespace PropAgent.Auth.Controllers;

[ApiController]
[Route("{tenantKey}/api/[controller]")] // <- REQUIRED for RouteStrategy
public sealed class TenantController : ControllerBase
{
    private readonly ITenantDb _db;
    private readonly ITenantInfo _ti;

    public TenantController(ITenantDb db, ITenantInfo ti)
    {
        _db = db; _ti = ti;
    }

    [HttpGet("dbping")]
    public async Task<IActionResult> DbPing()
    {
        var ok = await _db.PingAsync();
        return Ok(new { tenant = new { _ti.Id, _ti.Identifier, _ti.Name }, db = new { reachable = ok } });
    }
}
