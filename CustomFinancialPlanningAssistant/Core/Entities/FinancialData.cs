using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CustomFinancialPlanningAssistant.Core.Entities;

/// <summary>
/// Represents a single financial data record extracted from a document
/// </summary>
public class FinancialData
{
    /// <summary>
    /// Primary key identifier
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Foreign key to the parent financial document
    /// </summary>
    [Required]
    public int DocumentId { get; set; }

    /// <summary>
    /// Name of the financial account
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string AccountName { get; set; } = string.Empty;

    /// <summary>
    /// Account code or number
    /// </summary>
    [MaxLength(50)]
    public string? AccountCode { get; set; }

    /// <summary>
    /// Time period for this data (e.g., "2024-Q1" or "2024-01")
    /// </summary>
    [Required]
    [MaxLength(20)]
    public string Period { get; set; } = string.Empty;

    /// <summary>
    /// Financial amount with precision of 18 digits and 2 decimal places
    /// </summary>
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    /// <summary>
    /// Currency code (e.g., "USD", "EUR")
    /// </summary>
    [Required]
    [MaxLength(3)]
    public string Currency { get; set; } = "USD";

    /// <summary>
    /// Category of the financial data (Revenue, Expense, Asset, Liability, Equity)
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Subcategory for more detailed classification
    /// </summary>
    [MaxLength(100)]
    public string? SubCategory { get; set; }

    /// <summary>
    /// Date when this financial data was recorded
    /// </summary>
    [Required]
    public DateTime DateRecorded { get; set; }

    /// <summary>
    /// Navigation property to the parent document
    /// </summary>
    [ForeignKey(nameof(DocumentId))]
    public FinancialDocument? Document { get; set; }
}
