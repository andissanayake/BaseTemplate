using System.Data;
using BaseTemplate.Application.Common.Interfaces;
using Microsoft.Data.SqlClient;

namespace BaseTemplate.Infrastructure.Data;
public class SqlConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public SqlConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IDbConnection CreateConnection()
    {
        var connection = new SqlConnection(_connectionString);
        return connection;
    }
}
