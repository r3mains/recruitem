namespace backend.Services.IServices;

public interface IFileStorageService
{
  Task<string> SaveFileAsync(IFormFile file, string folder);
  Task<bool> DeleteFileAsync(string filePath);
  Task<byte[]> GetFileAsync(string filePath);
  Task<Stream> GetFileStreamAsync(string filePath);
  bool FileExists(string filePath);
  string GetContentType(string filePath);
}
