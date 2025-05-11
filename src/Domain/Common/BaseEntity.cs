using Dapper.Contrib.Extensions;

namespace BaseTemplate.Domain.Common;

public abstract class BaseEntity
{
    // This can easily be modified to be BaseEntity<T> and public T Id to support different key types.
    // Using non-generic integer types for simplicity
    [Key]
    public int Id { get; set; }
}
