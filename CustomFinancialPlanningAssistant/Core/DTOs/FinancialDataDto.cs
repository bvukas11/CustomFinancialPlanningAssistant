using CustomFinancialPlanningAssistant.Core.Entities;

namespace CustomFinancialPlanningAssistant.Core.DTOs;

/// <summary>
/// Data Transfer Object for financial data records
/// </summary>
public class FinancialDataDto
{
    /// <summary>
    /// Record identifier
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// ID of the parent document
    /// </summary>
    public int DocumentId { get; set; }

    /// <summary>
    /// Name of the financial account
    /// </summary>
    public string AccountName { get; set; } = string.Empty;

    /// <summary>
    /// Account code or number
    /// </summary>
    public string? AccountCode { get; set; }

    /// <summary>
    /// Time period (e.g., "2024-Q1")
    /// </summary>
    public string Period { get; set; } = string.Empty;

    /// <summary>
    /// Financial amount
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Currency code
    /// </summary>
    public string Currency { get; set; } = "USD";

    /// <summary>
    /// Category of financial data
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Subcategory for detailed classification
    /// </summary>
    public string? SubCategory { get; set; }

    /// <summary>
    /// Date when the data was recorded
    /// </summary>
    public DateTime DateRecorded { get; set; }

    /// <summary>
    /// Name of the parent document (for display purposes)
    /// </summary>
    public string DocumentFileName { get; set; } = string.Empty;

    /// <summary>
    /// Converts this DTO to a FinancialData entity
    /// </summary>
    /// <returns>A new FinancialData entity</returns>
    public FinancialData ToEntity()
    {
        return new FinancialData
        {
            Id = this.Id,
            DocumentId = this.DocumentId,
            AccountName = this.AccountName,
            AccountCode = this.AccountCode,
            Period = this.Period,
            Amount = this.Amount,
            Currency = this.Currency,
            Category = this.Category,
            SubCategory = this.SubCategory,
            DateRecorded = this.DateRecorded
        };
    }

    /// <summary>
    /// Creates a DTO from a FinancialData entity
    /// </summary>
    /// <param name="entity">The entity to convert</param>
    /// <returns>A new FinancialDataDto</returns>
    public static FinancialDataDto FromEntity(FinancialData entity)
    {
        return new FinancialDataDto
        {
            Id = entity.Id,
            DocumentId = entity.DocumentId,
            AccountName = entity.AccountName,
            AccountCode = entity.AccountCode,
            Period = entity.Period,
            Amount = entity.Amount,
            Currency = entity.Currency,
            Category = entity.Category,
            SubCategory = entity.SubCategory,
            DateRecorded = entity.DateRecorded,
            DocumentFileName = entity.Document?.FileName ?? string.Empty
        };
    }
}
