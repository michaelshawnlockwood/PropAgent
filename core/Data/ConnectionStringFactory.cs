// core/Data/ConnectionStringFactory.cs
using System;
using Microsoft.Extensions.Configuration;

namespace PropAgent.Core.Data
{
    public sealed class ConnectionStringFactory : IConnectionStringFactory
    {
        private readonly IConfiguration _config;

        public ConnectionStringFactory(IConfiguration config)
        {
            _config = config;
        }

        public string BuildNpgsqlPreview(
            string host,
            // int port,
            string database,
            string username,
            bool sslRequired = true,
            bool trustServerCertificate = false)
        {
            return $"Host={host};Database={database};Username={username};Password=***;SSL Mode={(sslRequired ? "Require" : "Disable")};Trust Server Certificate={(trustServerCertificate ? "true" : "false")};Pooling=true;Maximum Pool Size=100;Minimum Pool Size=0;";
        }

        public string BuildNpgsql
            (string host,
            // int port,
            string database,
            string username,
            string password,
            bool sslRequired = true,
            bool trustServerCertificate = false)
        {
            return $"Host={host};Database={database};Username={username};Password=*{password};SSL Mode={(sslRequired ? "Require" : "Disable")};Trust Server Certificate={(trustServerCertificate ? "true" : "false")};Pooling=true;Maximum Pool Size=100;Minimum Pool Size=0;";
        }
    }
}
