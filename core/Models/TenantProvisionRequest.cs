namespace PropAgent.Core.Models;

public sealed class TenantProvisionRequest
{
    // Finbuckle slug, e.g., "alpha" (must be URL/host-safe)
    public required string Identifier { get; init; }

    // Human-readable name, e.g., "Alpha Properties"
    public required string Name { get; init; }

    // Optional contact for bootstrap notifications
    public string? AdminEmail { get; init; }

    // Optional: initial DB/app credentials for the tenant
    public TenantCreds? Creds { get; init; }
}
