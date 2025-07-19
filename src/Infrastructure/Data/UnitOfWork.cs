using System.Data;
using BaseTemplate.Application.Common.Interfaces;
using BaseTemplate.Domain.Common;
using Dapper;

namespace BaseTemplate.Infrastructure.Data;

public class UOWTransaction : ITransaction
{
    private readonly IDbTransaction _transaction;

    public UOWTransaction(IDbTransaction transaction)
    {
        _transaction = transaction;
    }

    public void Commit()
    {
        _transaction.Commit();
    }

    public void Rollback()
    {
        _transaction.Rollback();
    }

    public void Dispose()
    {
        _transaction.Dispose();
    }
}
public class UnitOfWork : IUnitOfWork
{

    private readonly IDbConnection _connection;
    private IDbTransaction? _transaction;
    private bool _disposed;
    private readonly IUser _user;
    public UnitOfWork(IDbConnectionFactory dbfactory, IUser user)
    {
        _connection = dbfactory.CreateConnection();
        _user = user;

    }
    public ITransaction BeginTransaction()
    {
        if (_connection.State == ConnectionState.Closed) { _connection.Open(); }
        _transaction = _connection.BeginTransaction();
        return new UOWTransaction(_transaction);
    }

    public async Task<int> InsertAsync<T>(T entity) where T : class
    {
        if (entity is BaseAuditableEntity auditable)
        {
            var now = DateTimeOffset.UtcNow;
            auditable.Created = now;
            auditable.CreatedBy = _user.Identifier;
        }
        var data = await _connection.InsertAsync(entity, _transaction);
        return data ?? 0;
    }

    public async Task UpdateAsync<T>(T entity) where T : class
    {
        if (entity is BaseAuditableEntity auditable)
        {
            auditable.LastModified = DateTimeOffset.UtcNow;
            auditable.LastModifiedBy = _user.Identifier;
        }
        await _connection.UpdateAsync(entity, _transaction);
    }

    public async Task DeleteAsync<T>(T entity) where T : class =>
        await _connection.DeleteAsync(entity, _transaction);

    public async Task<T> GetAsync<T>(object id) where T : class =>
        await _connection.GetAsync<T>(id, _transaction);

    public async Task<IEnumerable<T>> GetAllAsync<T>() where T : class =>
        await _connection.GetListAsync<T>(_transaction);

    public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null) =>
        await _connection.QueryAsync<T>(sql, param, _transaction);

    public async Task<T> QuerySingleAsync<T>(string sql, object? param = null) =>
        await _connection.QuerySingleAsync<T>(sql, param, _transaction);

    public async Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? param = null) =>
        await _connection.QueryFirstOrDefaultAsync<T>(sql, param, _transaction);

    public async Task<int> ExecuteAsync(string sql, object? param = null) =>
        await _connection.ExecuteAsync(sql, param, _transaction);

    public void Dispose()
    {
        if (!_disposed)
        {
            _transaction?.Dispose();
            _connection.Close();
            _connection.Dispose();
            _disposed = true;
        }
    }
}

public class UnitOfWorkFactory : IUnitOfWorkFactory
{
    private readonly IDbConnectionFactory _dbfactory;
    private readonly IUser _user;
    public UnitOfWorkFactory(IDbConnectionFactory dbfactory, IUser user)
    {
        _dbfactory = dbfactory;
        _user = user;
    }
    public IUnitOfWork Create() => new UnitOfWork(_dbfactory, _user);
}
