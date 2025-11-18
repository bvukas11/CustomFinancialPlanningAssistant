namespace CustomFinancialPlanningAssistant.Core.DTOs;

/// <summary>
/// Data Transfer Object for comparison results between periods or documents
/// </summary>
public class ComparisonResultDto
{
    /// <summary>
    /// First period being compared
    /// </summary>
    public string Period1 { get; set; } = string.Empty;

    /// <summary>
    /// Second period being compared
    /// </summary>
    public string Period2 { get; set; } = string.Empty;

    /// <summary>
    /// Dictionary of metrics comparing the two periods
    /// </summary>
    public Dictionary<string, ComparisonMetric> Metrics { get; set; } = new();

    /// <summary>
    /// List of significant changes identified
    /// </summary>
    public List<string> SignificantChanges { get; set; } = new();

    /// <summary>
    /// Overall trend (Growth, Decline, Stable)
    /// </summary>
    public string OverallTrend { get; set; } = string.Empty;

    /// <summary>
    /// Date when this comparison was performed
    /// </summary>
    public DateTime ComparisonDate { get; set; }
}

/// <summary>
/// Represents a single metric comparison between two periods
/// </summary>
public class ComparisonMetric
{
    /// <summary>
    /// Category or metric name
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Value in first period
    /// </summary>
    public decimal Value1 { get; set; }

    /// <summary>
    /// Value in second period
    /// </summary>
    public decimal Value2 { get; set; }

    /// <summary>
    /// Absolute variance between periods
    /// </summary>
    public decimal Variance { get; set; }

    /// <summary>
    /// Percentage change between periods
    /// </summary>
    public decimal PercentageChange { get; set; }

    /// <summary>
    /// Type of change (Increase, Decrease, NoChange)
    /// </summary>
    public string ChangeType { get; set; } = string.Empty;
}
