using CustomFinancialPlanningAssistant.Core.Enums;

namespace CustomFinancialPlanningAssistant.Core.Entities;

/// <summary>
/// Industry benchmark data for comparative analysis
/// Contains statistical benchmarks for different financial metrics by industry
/// </summary>
public class IndustryBenchmark
{
    /// <summary>
    /// Unique identifier for the benchmark record
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The industry this benchmark applies to
    /// </summary>
    public IndustryType Industry { get; set; }

    /// <summary>
    /// Name of the financial metric (e.g., "GrossMargin", "OperatingMargin", "CurrentRatio")
    /// </summary>
    public string MetricName { get; set; } = string.Empty;

    /// <summary>
    /// Industry average value for this metric
    /// </summary>
    public decimal AverageValue { get; set; }

    /// <summary>
    /// Industry median value for this metric
    /// </summary>
    public decimal MedianValue { get; set; }

    /// <summary>
    /// 25th percentile value (lower quartile)
    /// </summary>
    public decimal Percentile25 { get; set; }

    /// <summary>
    /// 75th percentile value (upper quartile)
    /// </summary>
    public decimal Percentile75 { get; set; }

    /// <summary>
    /// When this benchmark data was last updated
    /// </summary>
    public DateTime LastUpdated { get; set; }

    /// <summary>
    /// Source of the benchmark data (e.g., "S&P Global", "Industry Reports", "Internal Research")
    /// </summary>
    public string DataSource { get; set; } = "Industry Standard";

    /// <summary>
    /// Sample size used to calculate these benchmarks
    /// </summary>
    public int SampleSize { get; set; }

    /// <summary>
    /// Additional notes about this benchmark
    /// </summary>
    public string Notes { get; set; } = string.Empty;
}