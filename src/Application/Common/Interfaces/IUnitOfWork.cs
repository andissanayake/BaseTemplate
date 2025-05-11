namespace BaseTemplate.Application.Common.Interfaces;
public interface IUnitOfWork : IDisposable
{
    Task InsertAsync<T>(T entity) where T : class;
    Task UpdateAsync<T>(T entity) where T : class;
    Task DeleteAsync<T>(T entity) where T : class;
    Task<T?> GetAsync<T>(object id) where T : class;
    Task<IEnumerable<T>> GetAllAsync<T>() where T : class;
    Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null);
    Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? param = null);
    Task<T> QuerySingleAsync<T>(string sql, object? param = null);
    Task<int> ExecuteAsync(string sql, object? param = null);
    ITransaction BeginTransaction();
}
public interface ITransaction
{
    public void Commit();
    public void Rollback();
}

public interface IUnitOfWorkFactory
{
    IUnitOfWork Create();
}
