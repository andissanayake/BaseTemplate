using System.Reflection;
using static Dapper.SimpleCRUD;

namespace BaseTemplate.Infrastructure.Data;
public static class SnakeCaseMapper
{
    public static string ToSnakeCase(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;

        var builder = new System.Text.StringBuilder();
        for (int i = 0; i < input.Length; i++)
        {
            if (char.IsUpper(input[i]))
            {
                if (i > 0) builder.Append("_");
                builder.Append(char.ToLower(input[i]));
            }
            else
            {
                builder.Append(input[i]);
            }
        }
        return builder.ToString();
    }
}
public class SnakeCaseTableNameResolver : ITableNameResolver
{

    public string ResolveTableName(Type type)
    {
        return SnakeCaseMapper.ToSnakeCase(type.Name);
    }
}
public class SnakeCaseColumnNameResolver : IColumnNameResolver
{
    public string ResolveColumnName(PropertyInfo type)
    {
        return SnakeCaseMapper.ToSnakeCase(type.Name);
    }
}
