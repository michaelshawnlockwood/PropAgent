using Finbuckle.MultiTenant.Abstractions;

namespace PropAgent.Core.Tenancy;

public class AppTenantInfo : ITenantInfo
{
    public string? Id { get; set; }
    public string? Identifier { get; set; }
    public string? Name { get; set; }
    public string? ConnectionString { get; set; } // kept for Finbuckle compatibility
    public string? ItemsJson { get; set; }

    // metadata fields
    public string? DbName { get; set; }
    public string? DbHost { get; set; }
    public int? DbPort { get; set; }
    public string? SslMode { get; set; }
    public string? VaultRole { get; set; }
    public DateTimeOffset CreatedUtc { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? UpdatedUtc { get; set; }
    public bool? IsActive { get; set; } = true;
}
