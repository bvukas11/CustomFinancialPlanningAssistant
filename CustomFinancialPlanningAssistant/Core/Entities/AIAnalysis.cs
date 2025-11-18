using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CustomFinancialPlanningAssistant.Core.Entities;

/// <summary>
/// Represents an AI-powered analysis performed on a financial document
/// </summary>
public class AIAnalysis
{
    /// <summary>
    /// Primary key identifier
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Foreign key to the analyzed financial document
    /// </summary>
    [Required]
    public int DocumentId { get; set; }

    /// <summary>
    /// Type of analysis performed (Summary, TrendAnalysis, AnomalyDetection, etc.)
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string AnalysisType { get; set; } = string.Empty;

    /// <summary>
    /// The prompt sent to the AI model
    /// </summary>
    [Required]
    public string Prompt { get; set; } = string.Empty;

    /// <summary>
    /// The AI model's response
    /// </summary>
    [Required]
    public string Response { get; set; } = string.Empty;

    /// <summary>
    /// Name of the AI model used (e.g., "llama3.2", "qwen2.5:8b")
    /// </summary>
    [MaxLength(50)]
    public string? ModelUsed { get; set; }

    /// <summary>
    /// Time taken to execute the analysis in milliseconds
    /// </summary>
    [Required]
    public int ExecutionTime { get; set; }

    /// <summary>
    /// Date and time when the analysis was created
    /// </summary>
    [Required]
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// User rating of the analysis quality (1-5 stars)
    /// </summary>
    [Range(1, 5)]
    public int? Rating { get; set; }

    /// <summary>
    /// Navigation property to the parent document
    /// </summary>
    [ForeignKey(nameof(DocumentId))]
    public FinancialDocument? Document { get; set; }
}
