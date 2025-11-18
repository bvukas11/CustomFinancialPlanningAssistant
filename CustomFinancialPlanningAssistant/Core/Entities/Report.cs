using System.ComponentModel.DataAnnotations;

namespace CustomFinancialPlanningAssistant.Core.Entities;

/// <summary>
/// Represents a generated financial report
/// </summary>
public class Report
{
    /// <summary>
    /// Primary key identifier
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Title of the report
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Description of the report contents
    /// </summary>
    [MaxLength(1000)]
    public string? Description { get; set; }

    /// <summary>
    /// Type of report (Summary, Detailed, Comparison, etc.)
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string ReportType { get; set; } = string.Empty;

    /// <summary>
    /// Date and time when the report was generated
    /// </summary>
    [Required]
    public DateTime GeneratedDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Report content stored as JSON or HTML
    /// </summary>
    [Required]
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// JSON string containing parameters used to generate the report
    /// </summary>
    public string? Parameters { get; set; }
}
