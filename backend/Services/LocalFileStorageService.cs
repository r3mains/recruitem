namespace backend.Services;

public class LocalFileStorageService(IWebHostEnvironment environment, ILogger<LocalFileStorageService> logger) : IServices.IFileStorageService
{
  private readonly IWebHostEnvironment _environment = environment;
  private readonly ILogger<LocalFileStorageService> _logger = logger;

  public async Task<string> SaveFileAsync(IFormFile file, string folder)
  {
    try
    {
      if (file == null || file.Length == 0)
        throw new ArgumentException("File is empty");

      // Create uploads directory structure
      var uploadsPath = Path.Combine(_environment.ContentRootPath, "uploads", folder);
      Directory.CreateDirectory(uploadsPath);

      // Generate unique filename
      var fileExtension = Path.GetExtension(file.FileName);
      var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
      var filePath = Path.Combine(uploadsPath, uniqueFileName);

      // Save file
      using (var stream = new FileStream(filePath, FileMode.Create))
      {
        await file.CopyToAsync(stream);
      }

      // Return relative path
      return Path.Combine("uploads", folder, uniqueFileName);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error saving file: {FileName}", file.FileName);
      throw;
    }
  }

  public Task<bool> DeleteFileAsync(string filePath)
  {
    try
    {
      var fullPath = Path.Combine(_environment.ContentRootPath, filePath);
      
      if (File.Exists(fullPath))
      {
        File.Delete(fullPath);
        return Task.FromResult(true);
      }
      
      return Task.FromResult(false);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error deleting file: {FilePath}", filePath);
      return Task.FromResult(false);
    }
  }

  public Task<byte[]> GetFileAsync(string filePath)
  {
    try
    {
      var fullPath = Path.Combine(_environment.ContentRootPath, filePath);
      
      if (!File.Exists(fullPath))
        throw new FileNotFoundException("File not found", filePath);

      return File.ReadAllBytesAsync(fullPath);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error reading file: {FilePath}", filePath);
      throw;
    }
  }

  public Task<Stream> GetFileStreamAsync(string filePath)
  {
    try
    {
      var fullPath = Path.Combine(_environment.ContentRootPath, filePath);
      
      if (!File.Exists(fullPath))
        throw new FileNotFoundException("File not found", filePath);

      Stream stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
      return Task.FromResult(stream);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error reading file stream: {FilePath}", filePath);
      throw;
    }
  }

  public bool FileExists(string filePath)
  {
    var fullPath = Path.Combine(_environment.ContentRootPath, filePath);
    return File.Exists(fullPath);
  }

  public string GetContentType(string filePath)
  {
    var extension = Path.GetExtension(filePath).ToLowerInvariant();
    return extension switch
    {
      ".pdf" => "application/pdf",
      ".doc" => "application/msword",
      ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
      ".jpg" or ".jpeg" => "image/jpeg",
      ".png" => "image/png",
      ".gif" => "image/gif",
      ".txt" => "text/plain",
      ".zip" => "application/zip",
      _ => "application/octet-stream"
    };
  }
}
