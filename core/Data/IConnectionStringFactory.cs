// core/Data/IConnectionStringFactory.cs
using System;

namespace PropAgent.Core.Data
{
    public interface IConnectionStringFactory
    {
        string BuildNpgsql(
            string host,
            string database,
            string username,
            string password,
            bool sslRequired = true,
            bool trustServerCertificate = false);

        string BuildNpgsqlPreview(
            string host,
            string database,
            string username,
            bool sslRequired = true,
            bool trustServerCertificate = false);
    }
}
