using System.Data;

namespace BaseTemplate.Application.Common.Interfaces;
public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}
