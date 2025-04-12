namespace BaseTemplate.Application.Common.Interfaces;

public interface ICsvService
{
    Task<byte[]> WriteToCsvAsync<T>(IEnumerable<T> items, string[] headers, Func<T, string[]> rowSelector);
}
