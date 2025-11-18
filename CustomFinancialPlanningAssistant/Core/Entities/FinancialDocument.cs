using System.ComponentModel.DataAnnotations;

namespace CustomFinancialPlanningAssistant.Core.Entities;

/// <summary>
/// Represents a financial document uploaded to the system
/// </summary>
public class FinancialDocument
{
    /// <summary>
    /// Primary key identifier
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Name of the uploaded file
    /// </summary>
    [Required]
    [MaxLength(255)]
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Type of file (Excel, CSV, PDF)
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string FileType { get; set; } = string.Empty;

    /// <summary>
    /// Date and time when the file was uploaded
    /// </summary>
    [Required]
    public DateTime UploadDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Size of the file in bytes
    /// </summary>
    [Required]
    public long FileSize { get; set; }

    /// <summary>
    /// Storage path of the uploaded file
    /// </summary>
    [Required]
    [MaxLength(500)]
    public string FilePath { get; set; } = string.Empty;

    /// <summary>
    /// Current status of the document (Uploaded, Processing, Analyzed, Error)
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// User who uploaded the document
    /// </summary>
    [MaxLength(100)]
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Collection of financial data records extracted from this document
    /// </summary>
    public ICollection<FinancialData> FinancialDataRecords { get; set; }

    /// <summary>
    /// Collection of AI analyses performed on this document
    /// </summary>
    public ICollection<AIAnalysis> AIAnalyses { get; set; }

    /// <summary>
    /// Initializes a new instance of the FinancialDocument class
    /// </summary>
    public FinancialDocument()
    {
        FinancialDataRecords = new List<FinancialData>();
        AIAnalyses = new List<AIAnalysis>();
    }
}
