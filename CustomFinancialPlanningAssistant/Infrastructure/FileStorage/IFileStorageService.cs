namespace CustomFinancialPlanningAssistant.Infrastructure.FileStorage;

/// <summary>
/// Interface for file storage operations
/// </summary>
public interface IFileStorageService
{
    /// <summary>
    /// Saves uploaded file to disk storage
    /// </summary>
    /// <param name="fileStream">File stream to save</param>
    /// <param name="fileName">Name of the file</param>
    /// <param name="subFolder">Optional subfolder (default: "uploads")</param>
    /// <returns>Full file path where file was saved</returns>
    Task<string> SaveFileAsync(Stream fileStream, string fileName, string subFolder = "uploads");

    /// <summary>
    /// Retrieves file as stream
    /// </summary>
    /// <param name="filePath">Path to the file</param>
    /// <returns>File stream</returns>
    Task<Stream> GetFileAsync(string filePath);

    /// <summary>
    /// Retrieves file as byte array for image processing
    /// </summary>
    /// <param name="filePath">Path to the file</param>
    /// <returns>File contents as byte array</returns>
    Task<byte[]> GetFileBytesAsync(string filePath);

    /// <summary>
    /// Deletes file from storage
    /// </summary>
    /// <param name="filePath">Path to the file</param>
    /// <returns>True if successful, false otherwise</returns>
    Task<bool> DeleteFileAsync(string filePath);

    /// <summary>
    /// Checks if file exists in storage
    /// </summary>
    /// <param name="filePath">Path to check</param>
    /// <returns>True if file exists, false otherwise</returns>
    Task<bool> FileExistsAsync(string filePath);

    /// <summary>
    /// Gets file size in bytes
    /// </summary>
    /// <param name="filePath">Path to the file</param>
    /// <returns>File size in bytes</returns>
    Task<long> GetFileSizeAsync(string filePath);

    /// <summary>
    /// Extracts and returns file extension
    /// </summary>
    /// <param name="fileName">Name of the file</param>
    /// <returns>File extension (e.g., ".xlsx")</returns>
    Task<string> GetFileExtensionAsync(string fileName);

    /// <summary>
    /// Lists all files in specified folder
    /// </summary>
    /// <param name="subFolder">Subfolder to list (default: "uploads")</param>
    /// <returns>List of file paths</returns>
    Task<List<string>> GetAllFilesAsync(string subFolder = "uploads");
}
