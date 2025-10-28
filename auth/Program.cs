using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Finbuckle.MultiTenant;
using Shared;

var builder = WebApplication.CreateBuilder(args);

var tenants = new List<AppTenantInfo>
{
    new()
    {
        Id = Guid.NewGuid().ToString("n"),
        Identifier = "alpha",
        Name = "Alpha Properties",
        ConnectionString = "Host=localhost;Database=alpha_app;Username=app;Password=dev" // placeholder
    },
    new()
    {
        Id = Guid.NewGuid().ToString("n"),
        Identifier = "bravo",
        Name = "Bravo Maintenance",
        ConnectionString = "Host=localhost;Database=bravo_app;Username=app;Password=dev" // placeholder
    }
};

builder.Services
    .AddMultiTenant<AppTenantInfo>()
    .WithInMemoryStore(options => options.Tenants = tenants)
    .WithRouteStrategy()
    .WithHostStrategy();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<JsonOptions>(o => o.SerializerOptions.WriteIndented = true);

var app = builder.Build();
app.UseMultiTenant();
app.UseHttpsRedirection();

app.Use(async (ctx, next) =>
{
    var tic = ctx.GetMultiTenantContext<TenantInfo>();
    if (tic?.TenantInfo is { } t)
        app.Logger.LogInformation("Tenant resolved: {Identifier} ({Id})", t.Identifier, t.Id);
    await next();
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/api/health", () => Results.Ok(new { status = "Healthy" }));

app.Run();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

