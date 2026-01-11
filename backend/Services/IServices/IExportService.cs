namespace backend.Services.IServices;

public interface IExportService
{
  Task<byte[]> ExportToCsvAsync<T>(IEnumerable<T> data, string[]? headers = null);
  Task<Stream> ExportToCsvStreamAsync<T>(IEnumerable<T> data, string[]? headers = null);
}
