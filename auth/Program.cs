
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Finbuckle.MultiTenant;
using PropAgent.Auth.Infrastructure.Data;
using PropAgent.Core.Tenancy;
using Finbuckle.MultiTenant.Abstractions;
using PropAgent.Auth.MultiTenancy;
using Microsoft.AspNetCore.Mvc;
using HttpJsonOptions = Microsoft.AspNetCore.Http.Json.JsonOptions;
using PropAgent.Core.Models;
using PropAgent.Core.Tenancy.Secrets;

var builder = WebApplication.CreateBuilder(args);
DotNetEnv.Env.TraversePath().Load();


builder.Services
    .AddMultiTenant<AppTenantInfo>()
    .WithStore<AppTenantStore>(ServiceLifetime.Scoped)
    .WithRouteStrategy("tenantKey")
    .WithHostStrategy();

builder.Services.AddHttpClient();
builder.Services.AddMemoryCache();
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton<PropAgent.Core.Data.IConnectionStringFactory, PropAgent.Core.Data.ConnectionStringFactory>();
builder.Services.AddSingleton<PropAgent.Core.Runtime.ISessionConnectionCache, PropAgent.Core.Runtime.SessionConnectionCache>();
builder.Services.AddSingleton<PropAgent.Core.Security.IAppVaultClient, PropAgent.Core.Security.AppVaultClient>();
builder.Services.AddSingleton<PropAgent.Core.Runtime.IVaultLeaseManager, PropAgent.Core.Runtime.VaultLeaseManager>();
builder.Services.AddScoped<ITenantDb, AppTenantDb>();
builder.Services.AddSingleton<IVaultSecrets, VaultSecrets>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<HttpJsonOptions>(o => o.SerializerOptions.WriteIndented = true);

var app = builder.Build();

app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.MapControllers();
app.UseMultiTenant();

app.Use(async (ctx, next) =>
{
    var tic = ctx.GetMultiTenantContext<AppTenantInfo>();
    if (tic?.TenantInfo is { } t)
        app.Logger.LogInformation("Tenant resolved: {Identifier} ({Id})", t.Identifier, t.Id);
    await next();
});

app.MapGet("/api/health", () => Results.Ok(new { status = "Healthy" }));

app.MapGet("/{tenantKey}/api/dbping",
    async ([FromServices] ITenantDb db,
           [FromServices] IMultiTenantContextAccessor<AppTenantInfo> ctx) =>
{
    var ti = ctx.MultiTenantContext?.TenantInfo;
    var ok = await db.PingAsync();
    return Results.Ok(new { tenant = new { ti?.Id, ti?.Identifier, ti?.Name }, db = new { reachable = ok } });
});


app.MapGet("/{tenantKey}/__whoami",
    ([FromRoute] string tenantKey,
     IMultiTenantContextAccessor<AppTenantInfo> ctx,
     HttpContext http) =>
{
    var ti = ctx.MultiTenantContext?.TenantInfo;
    return Results.Ok(new {
        routeTenant = tenantKey,
        resolvedTenant = ti?.Identifier,
        hasContext = ti is not null,
        routeValues = http.Request.RouteValues
    });
});


app.MapPost("/{tenantKey}/api/provision",
    async ([FromRoute] string tenantKey,
           TenantProvisionRequest req,
           IMultiTenantStore<AppTenantInfo> store) =>
{
    // Look up the tenant by identifier directly in the store (DB-backed)
    var ti = await store.TryGetByIdentifierAsync(tenantKey);
    if (ti is null)
        return Results.NotFound(new { error = "Tenant not found", tenantKey });

    // OK: tenant exists in DB/store
    return Results.Accepted($"/{ti.Identifier}/api/status/{req.Identifier}");
});

app.Run();



