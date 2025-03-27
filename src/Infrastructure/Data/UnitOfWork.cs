using System.Data;
using System.Reflection;
using BaseTemplate.Application.Common.Interfaces;
using BaseTemplate.Domain.Common;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Data.SqlClient;

namespace BaseTemplate.Infrastructure.Data;
public class UnitOfWork : IUnitOfWork
{
    public IDbConnection Connection { get; }
    private readonly IDbTransaction? _transaction;
    private bool _isCompleted;
    private readonly IUser _user;

    public UnitOfWork(IDbConnection connection, IUser user, bool transactional = true)
    {
        SqlMapperExtensions.TableNameMapper = (type) => type.Name;
        _user = user;
        Connection = connection;
        Connection.Open();
        if (transactional)
        {
            _transaction = Connection.BeginTransaction();
        }
        else
        {
            _isCompleted = true;
        }
    }

    public async Task InsertAsync<T>(T entity) where T : class
    {
        if (entity is BaseAuditableEntity auditable)
        {
            var now = DateTimeOffset.UtcNow;
            auditable.Created = now;
            auditable.CreatedBy = _user.Id;
        }
        await Connection.InsertAsync(entity, _transaction);
    }

    public async Task UpdateAsync<T>(T entity) where T : class
    {
        if (entity is BaseAuditableEntity auditable)
        {
            auditable.LastModified = DateTimeOffset.UtcNow;
            auditable.LastModifiedBy = _user.Id;
        }
        await Connection.UpdateAsync(entity, _transaction);
    }

    public async Task DeleteAsync<T>(T entity) where T : class =>
        await Connection.DeleteAsync(entity, _transaction);

    public async Task<T?> GetAsync<T>(object id) where T : class =>
        await Connection.GetAsync<T>(id, _transaction);
    public async Task<IEnumerable<T>> GetAllAsync<T>() where T : class =>
        await Connection.GetAllAsync<T>(_transaction);
    public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null) =>
        await Connection.QueryAsync<T>(sql, param, _transaction);

    public async Task<T> QuerySingleAsync<T>(string sql, object? param = null) =>
        await Connection.QuerySingleAsync<T>(sql, param, _transaction);

    public async Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? param = null) =>
        await Connection.QueryFirstOrDefaultAsync<T>(sql, param, _transaction);

    public async Task<int> ExecuteAsync(string sql, object? param = null) =>
        await Connection.ExecuteAsync(sql, param, _transaction);

    public async Task BulkCopyAsync<T>(IEnumerable<T> items, string? tableName = null)
    {
        if (Connection is not SqlConnection sqlConnection)
            throw new NotSupportedException("Bulk insert is only supported with SqlConnection.");

        var actualTableName = tableName ?? typeof(T).Name;

        var props = typeof(T)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanWrite)
            .ToArray();

        var dataTable = new DataTable();
        foreach (var prop in props)
        {
            var type = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
            if (type == typeof(DateTimeOffset))
                type = typeof(DateTime);
            dataTable.Columns.Add(prop.Name, type);
        }

        var now = DateTimeOffset.UtcNow;
        var userId = _user.Id ?? "SYSTEM";

        foreach (var item in items)
        {
            // ✅ Assign new Guid if Id is Guid.Empty
            var idProp = typeof(T).GetProperty("Id");
            if (idProp != null && idProp.PropertyType == typeof(Guid))
            {
                var idValue = (Guid?)idProp.GetValue(item);
                if (idValue == null || idValue == Guid.Empty)
                {
                    idProp.SetValue(item, Guid.NewGuid());
                }
            }

            // ✅ Set audit fields
            if (item is BaseAuditableEntity auditable)
            {
                auditable.Created = now;
                auditable.CreatedBy = userId;
            }

            // ✅ Collect values for each row
            var values = props.Select(p =>
            {
                var value = p.GetValue(item);
                if (value is DateTimeOffset dto)
                    return dto.UtcDateTime;
                return value ?? DBNull.Value;
            }).ToArray();

            dataTable.Rows.Add(values);
        }

        using var bulkCopy = new SqlBulkCopy(sqlConnection, SqlBulkCopyOptions.Default, (SqlTransaction?)_transaction)
        {
            DestinationTableName = actualTableName
        };

        foreach (DataColumn column in dataTable.Columns)
        {
            bulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
        }

        await bulkCopy.WriteToServerAsync(dataTable);
    }

    public void Commit()
    {
        if (_isCompleted) return;
        _transaction?.Commit();
        _isCompleted = true;
    }

    public void Rollback()
    {
        if (_isCompleted) return;
        _transaction?.Rollback();
        _isCompleted = true;
    }

    public void Dispose()
    {
        if (!_isCompleted)
        {
            try { _transaction?.Rollback(); } catch { }
        }
        _transaction?.Dispose();
        Connection.Dispose();
    }
}


