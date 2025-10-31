
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Finbuckle.MultiTenant;
using PropAgent.Auth.Infrastructure.Data;
using Shared;
using Finbuckle.MultiTenant.Abstractions;
using PropAgent.Auth.MultiTenancy;
using Microsoft.AspNetCore.Mvc;
using HttpJsonOptions =  Microsoft.AspNetCore.Http.Json.JsonOptions;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddMultiTenant<AppTenantInfo>()
    .WithStore<AppTenantStore>(ServiceLifetime.Scoped)
    .WithRouteStrategy("tenantKey")
    .WithHostStrategy();

builder.Services.AddScoped<ITenantDb, AppTenantDb>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<HttpJsonOptions>(o => o.SerializerOptions.WriteIndented = true);

var app = builder.Build();

app.UseHttpsRedirection();

app.Use(async (ctx, next) =>
{
    var tic = ctx.GetMultiTenantContext<AppTenantInfo>();
    if (tic?.TenantInfo is { } t)
        app.Logger.LogInformation("Tenant resolved: {Identifier} ({Id})", t.Identifier, t.Id);
    await next();
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseMultiTenant();

app.MapGet("/api/health", () => Results.Ok(new { status = "Healthy" }));

app.MapGet("/{tenantKey}/api/dbping",
    async ([FromServices] ITenantDb db,
           [FromServices] IMultiTenantContextAccessor<AppTenantInfo> ctx) =>
{
    var ti = ctx.MultiTenantContext?.TenantInfo;
    var ok = await db.PingAsync();
    return Results.Ok(new { tenant = new { ti?.Id, ti?.Identifier, ti?.Name }, db = new { reachable = ok } });
});


app.Run();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

