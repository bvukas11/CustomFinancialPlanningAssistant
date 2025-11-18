namespace CustomFinancialPlanningAssistant.Core.DTOs;

/// <summary>
/// Data Transfer Object for document upload results
/// </summary>
public class UploadResultDto
{
    /// <summary>
    /// Indicates whether the upload was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// ID of the created document (null if upload failed)
    /// </summary>
    public int? DocumentId { get; set; }

    /// <summary>
    /// Name of the uploaded file
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Number of financial data records imported
    /// </summary>
    public int RecordsImported { get; set; }

    /// <summary>
    /// List of error messages encountered during processing
    /// </summary>
    public List<string> ErrorMessages { get; set; }

    /// <summary>
    /// List of warnings (non-critical issues)
    /// </summary>
    public List<string> Warnings { get; set; }

    /// <summary>
    /// Time taken to process the upload in milliseconds
    /// </summary>
    public int ProcessingTime { get; set; }

    /// <summary>
    /// Indicates whether any errors were encountered
    /// </summary>
    public bool HasErrors => ErrorMessages.Any();

    /// <summary>
    /// Indicates whether any warnings were generated
    /// </summary>
    public bool HasWarnings => Warnings.Any();

    /// <summary>
    /// Initializes a new instance with empty collections
    /// </summary>
    public UploadResultDto()
    {
        ErrorMessages = new List<string>();
        Warnings = new List<string>();
    }

    /// <summary>
    /// Adds an error message to the result
    /// </summary>
    /// <param name="message">Error message to add</param>
    public void AddError(string message)
    {
        if (!string.IsNullOrWhiteSpace(message))
        {
            ErrorMessages.Add(message);
        }
    }

    /// <summary>
    /// Adds a warning message to the result
    /// </summary>
    /// <param name="message">Warning message to add</param>
    public void AddWarning(string message)
    {
        if (!string.IsNullOrWhiteSpace(message))
        {
            Warnings.Add(message);
        }
    }
}
