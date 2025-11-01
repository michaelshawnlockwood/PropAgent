using System;
using Finbuckle.MultiTenant.Abstractions;

namespace PropAgent.Shared
{
    public sealed class AppTenantInfo : ITenantInfo
    {
        public string? Id { get; set; }
        public string? Identifier { get; set; }
        public string? Name { get; set; }
        public string? ConnectionString { get; set; }
        public string? ItemsJson { get; set; }
        public DateTimeOffset? CreatedUtc { get; set; }
        public DateTimeOffset? UpdatedUtc { get; set; }
        public bool? IsActive { get; set; } = true;
    }
}
