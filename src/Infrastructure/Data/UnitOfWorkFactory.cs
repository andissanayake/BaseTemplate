using BaseTemplate.Application.Common.Interfaces;
using Microsoft.Data.SqlClient;

namespace BaseTemplate.Infrastructure.Data;
public class UnitOfWorkFactory : IUnitOfWorkFactory
{
    private readonly string _connectionString;
    private readonly IUser _currentUserService;

    public UnitOfWorkFactory(string connectionString, IUser currentUserService)
    {
        _connectionString = connectionString;
        _currentUserService = currentUserService;
    }

    public IUnitOfWork CreateUOW()
    {
        var connection = new SqlConnection(_connectionString);
        return new UnitOfWork(connection, _currentUserService);
    }
}

