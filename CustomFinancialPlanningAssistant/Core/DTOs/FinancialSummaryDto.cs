namespace CustomFinancialPlanningAssistant.Core.DTOs;

/// <summary>
/// Data Transfer Object for financial summary information
/// </summary>
public class FinancialSummaryDto
{
    /// <summary>
    /// Document identifier
    /// </summary>
    public int DocumentId { get; set; }

    /// <summary>
    /// Name of the document
    /// </summary>
    public string DocumentName { get; set; } = string.Empty;

    /// <summary>
    /// Financial period (e.g., "2024-Q1")
    /// </summary>
    public string Period { get; set; } = string.Empty;

    /// <summary>
    /// Total revenue for the period
    /// </summary>
    public decimal TotalRevenue { get; set; }

    /// <summary>
    /// Total expenses for the period
    /// </summary>
    public decimal TotalExpenses { get; set; }

    /// <summary>
    /// Net income (Revenue - Expenses)
    /// </summary>
    public decimal NetIncome { get; set; }

    /// <summary>
    /// Gross profit
    /// </summary>
    public decimal GrossProfit { get; set; }

    /// <summary>
    /// Operating income
    /// </summary>
    public decimal OperatingIncome { get; set; }

    /// <summary>
    /// Total assets
    /// </summary>
    public decimal TotalAssets { get; set; }

    /// <summary>
    /// Total liabilities
    /// </summary>
    public decimal TotalLiabilities { get; set; }

    /// <summary>
    /// Total equity
    /// </summary>
    public decimal Equity { get; set; }

    /// <summary>
    /// Breakdown of amounts by category
    /// </summary>
    public Dictionary<string, decimal> CategoryBreakdown { get; set; } = new();

    /// <summary>
    /// Key financial highlights
    /// </summary>
    public List<string> KeyHighlights { get; set; } = new();

    /// <summary>
    /// Date when this summary was generated
    /// </summary>
    public DateTime GeneratedDate { get; set; }
}
