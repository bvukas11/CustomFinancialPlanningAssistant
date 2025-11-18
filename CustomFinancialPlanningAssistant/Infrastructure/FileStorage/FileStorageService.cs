using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CustomFinancialPlanningAssistant.Infrastructure.FileStorage;

/// <summary>
/// Implementation of file storage service for managing uploaded files
/// </summary>
public class FileStorageService : IFileStorageService
{
    private readonly string _baseStoragePath;
    private readonly ILogger<FileStorageService> _logger;

    /// <summary>
    /// Initializes a new instance of the FileStorageService class
    /// </summary>
    public FileStorageService(
        IConfiguration configuration,
        ILogger<FileStorageService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        // Get base storage path from configuration or use default
        var configPath = configuration["FileStorage:BasePath"];
        _baseStoragePath = string.IsNullOrEmpty(configPath)
            ? Path.Combine(Directory.GetCurrentDirectory(), "FileStorage")
            : Path.Combine(Directory.GetCurrentDirectory(), configPath);

        // Ensure base directory exists
        if (!Directory.Exists(_baseStoragePath))
        {
            Directory.CreateDirectory(_baseStoragePath);
            _logger.LogInformation("Created file storage directory: {Path}", _baseStoragePath);
        }
    }

    public async Task<string> SaveFileAsync(Stream fileStream, string fileName, string subFolder = "uploads")
    {
        try
        {
            // Sanitize filename
            var sanitizedFileName = SanitizeFileName(fileName);

            // Build full directory path
            var directoryPath = Path.Combine(_baseStoragePath, subFolder);

            // Ensure directory exists
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
                _logger.LogInformation("Created subdirectory: {Path}", directoryPath);
            }

            // Build full file path
            var filePath = Path.Combine(directoryPath, sanitizedFileName);

            // If file exists, append timestamp to make unique
            if (File.Exists(filePath))
            {
                var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                var fileNameWithoutExt = Path.GetFileNameWithoutExtension(sanitizedFileName);
                var extension = Path.GetExtension(sanitizedFileName);
                sanitizedFileName = $"{fileNameWithoutExt}_{timestamp}{extension}";
                filePath = Path.Combine(directoryPath, sanitizedFileName);
            }

            // Save file
            using (var fileStreamOutput = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                await fileStream.CopyToAsync(fileStreamOutput);
            }

            _logger.LogInformation("File saved successfully: {FilePath}", filePath);
            return filePath;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving file: {FileName}", fileName);
            throw;
        }
    }

    public async Task<Stream> GetFileAsync(string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"File not found: {filePath}");
            }

            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            _logger.LogInformation("File retrieved: {FilePath}", filePath);
            return await Task.FromResult(fileStream);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving file: {FilePath}", filePath);
            throw;
        }
    }

    public async Task<byte[]> GetFileBytesAsync(string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"File not found: {filePath}");
            }

            var bytes = await File.ReadAllBytesAsync(filePath);
            _logger.LogInformation("File bytes retrieved: {FilePath}, Size: {Size}", filePath, bytes.Length);
            return bytes;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving file bytes: {FilePath}", filePath);
            throw;
        }
    }

    public async Task<bool> DeleteFileAsync(string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                _logger.LogWarning("File does not exist, cannot delete: {FilePath}", filePath);
                return false;
            }

            File.Delete(filePath);
            _logger.LogInformation("File deleted: {FilePath}", filePath);
            return await Task.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file: {FilePath}", filePath);
            return false;
        }
    }

    public Task<bool> FileExistsAsync(string filePath)
    {
        var exists = File.Exists(filePath);
        return Task.FromResult(exists);
    }

    public Task<long> GetFileSizeAsync(string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"File not found: {filePath}");
            }

            var fileInfo = new FileInfo(filePath);
            return Task.FromResult(fileInfo.Length);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting file size: {FilePath}", filePath);
            throw;
        }
    }

    public Task<string> GetFileExtensionAsync(string fileName)
    {
        var extension = Path.GetExtension(fileName);
        return Task.FromResult(extension);
    }

    public Task<List<string>> GetAllFilesAsync(string subFolder = "uploads")
    {
        try
        {
            var directoryPath = Path.Combine(_baseStoragePath, subFolder);

            if (!Directory.Exists(directoryPath))
            {
                _logger.LogWarning("Directory does not exist: {Path}", directoryPath);
                return Task.FromResult(new List<string>());
            }

            var files = Directory.GetFiles(directoryPath).ToList();
            _logger.LogInformation("Retrieved {Count} files from {Path}", files.Count, directoryPath);
            return Task.FromResult(files);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing files in: {SubFolder}", subFolder);
            throw;
        }
    }

    /// <summary>
    /// Sanitizes filename by removing invalid characters
    /// </summary>
    private string SanitizeFileName(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException("Filename cannot be null or empty", nameof(fileName));
        }

        // Remove invalid path characters
        var invalidChars = Path.GetInvalidFileNameChars();
        var sanitized = string.Join("_", fileName.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries));

        // Replace spaces with underscores
        sanitized = sanitized.Replace(" ", "_");

        // Limit length to 255 characters
        if (sanitized.Length > 255)
        {
            var extension = Path.GetExtension(sanitized);
            var nameWithoutExt = Path.GetFileNameWithoutExtension(sanitized);
            sanitized = nameWithoutExt.Substring(0, 255 - extension.Length) + extension;
        }

        return sanitized;
    }
}
