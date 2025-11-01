// src/Core/Data/ConnectionStringFactory.cs
using System;

namespace PropAgent.Core.Data
{
    public sealed class ConnectionStringFactory
    {
        public string BuildNpgsql(string host, int port, string database, string username, string password, bool sslRequired = true, bool trustServerCertificate = false)
            => throw new NotImplementedException();
    }
}
