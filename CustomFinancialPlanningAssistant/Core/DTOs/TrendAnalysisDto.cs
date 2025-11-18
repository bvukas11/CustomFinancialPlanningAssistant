namespace CustomFinancialPlanningAssistant.Core.DTOs;

/// <summary>
/// Data Transfer Object for trend analysis results
/// </summary>
public class TrendAnalysisDto
{
    /// <summary>
    /// Category being analyzed
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// List of data points showing the trend
    /// </summary>
    public List<TrendDataPoint> DataPoints { get; set; } = new();

    /// <summary>
    /// Overall trend direction (Increasing, Decreasing, Stable)
    /// </summary>
    public string TrendDirection { get; set; } = string.Empty;

    /// <summary>
    /// Average growth rate across all periods
    /// </summary>
    public decimal AverageGrowthRate { get; set; }

    /// <summary>
    /// Total change in value from start to end
    /// </summary>
    public decimal TotalChange { get; set; }

    /// <summary>
    /// Percentage change from start to end
    /// </summary>
    public decimal PercentageChange { get; set; }

    /// <summary>
    /// Starting period
    /// </summary>
    public string StartPeriod { get; set; } = string.Empty;

    /// <summary>
    /// Ending period
    /// </summary>
    public string EndPeriod { get; set; } = string.Empty;

    /// <summary>
    /// Insights generated from the trend analysis
    /// </summary>
    public List<string> Insights { get; set; } = new();
}

/// <summary>
/// Represents a single data point in a trend
/// </summary>
public class TrendDataPoint
{
    /// <summary>
    /// Period identifier
    /// </summary>
    public string Period { get; set; } = string.Empty;

    /// <summary>
    /// Value for this period
    /// </summary>
    public decimal Value { get; set; }

    /// <summary>
    /// Percentage change from previous period
    /// </summary>
    public decimal? PercentageChange { get; set; }

    /// <summary>
    /// Indicates if this is an anomalous data point
    /// </summary>
    public bool IsAnomaly { get; set; }
}
