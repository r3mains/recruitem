using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.Text;

namespace backend.Services;

public class ExportService : IServices.IExportService
{
  public async Task<byte[]> ExportToCsvAsync<T>(IEnumerable<T> data, string[]? headers = null)
  {
    using var memoryStream = new MemoryStream();
    await using var writer = new StreamWriter(memoryStream, Encoding.UTF8);
    await using var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)
    {
      HasHeaderRecord = true,
    });

    if (headers != null && headers.Any())
    {
      foreach (var header in headers)
      {
        csv.WriteField(header);
      }
      await csv.NextRecordAsync();
    }

    await csv.WriteRecordsAsync(data);
    await writer.FlushAsync();

    return memoryStream.ToArray();
  }

  public async Task<Stream> ExportToCsvStreamAsync<T>(IEnumerable<T> data, string[]? headers = null)
  {
    var memoryStream = new MemoryStream();
    await using var writer = new StreamWriter(memoryStream, Encoding.UTF8, leaveOpen: true);
    await using var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)
    {
      HasHeaderRecord = true,
    });

    if (headers != null && headers.Any())
    {
      foreach (var header in headers)
      {
        csv.WriteField(header);
      }
      await csv.NextRecordAsync();
    }

    await csv.WriteRecordsAsync(data);
    await writer.FlushAsync();

    memoryStream.Position = 0;
    return memoryStream;
  }
}
