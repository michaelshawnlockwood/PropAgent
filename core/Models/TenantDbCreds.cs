using System;
using System.Text.Json.Serialization;

namespace PropAgent.Core.Models
{
    public sealed class TenantDbCreds
    {
        public required string TenantKey { get; init; }
        public required string User { get; init; }
        public required string Host { get; init; }
        // public int Port { get; init; }
        public required string Db { get; init; }
        public bool HasPassword { get; init; }

        [JsonIgnore]
        public string? Password { get; init; }

        public string? LeaseId { get; init; }
        public DateTimeOffset? LeaseExpiresAtUtc { get; init; }
    }
}
