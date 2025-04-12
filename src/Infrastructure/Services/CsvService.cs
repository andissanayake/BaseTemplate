using BaseTemplate.Application.Common.Interfaces;
using CsvHelper;
using System.Globalization;

namespace BaseTemplate.Infrastructure.Services;

public class CsvService : ICsvService
{
    public async Task<byte[]> WriteToCsvAsync<T>(IEnumerable<T> items, string[] headers, Func<T, object[]> rowSelector)
    {
        using var memoryStream = new MemoryStream();
        using var writer = new StreamWriter(memoryStream);
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

        // Write headers
        foreach (var header in headers)
        {
            csv.WriteField(header);
        }
        csv.NextRecord();

        // Write data
        foreach (var item in items)
        {
            var values = rowSelector(item);
            foreach (var value in values)
            {
                csv.WriteField(value);
            }
            csv.NextRecord();
        }

        await writer.FlushAsync();
        return memoryStream.ToArray();
    }
} 