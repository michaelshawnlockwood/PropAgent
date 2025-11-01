namespace PropAgent.Core.Models;

public sealed class TenantCreds
{
    public required string Username { get; init; }
    public required string Password { get; init; }
}
