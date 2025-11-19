using CustomFinancialPlanningAssistant.Core.Enums;

namespace CustomFinancialPlanningAssistant.Core.DTOs;

/// <summary>
/// DTO for industry benchmark comparison results
/// </summary>
public class IndustryBenchmarkDto
{
    /// <summary>
    /// Name of the financial metric being compared
    /// </summary>
    public string MetricName { get; set; } = string.Empty;

    /// <summary>
    /// The company's actual value for this metric
    /// </summary>
    public decimal CompanyValue { get; set; }

    /// <summary>
    /// Industry average value for comparison
    /// </summary>
    public decimal IndustryAverage { get; set; }

    /// <summary>
    /// Industry median value for comparison
    /// </summary>
    public decimal IndustryMedian { get; set; }

    /// <summary>
    /// Performance rating relative to industry (Above Average, Below Average, At Industry Average)
    /// </summary>
    public string PerformanceRating { get; set; } = string.Empty;

    /// <summary>
    /// Percentile ranking (0-100) where the company falls in the industry distribution
    /// </summary>
    public decimal PercentileRanking { get; set; }

    /// <summary>
    /// Variance from industry average (positive = above average, negative = below average)
    /// </summary>
    public decimal VarianceFromAverage { get; set; }

    /// <summary>
    /// Variance as a percentage from industry average
    /// </summary>
    public decimal VariancePercentage { get; set; }

    /// <summary>
    /// Brief explanation of what this metric indicates
    /// </summary>
    public string MetricDescription { get; set; } = string.Empty;

    /// <summary>
    /// Recommended action based on the performance
    /// </summary>
    public string Recommendation { get; set; } = string.Empty;
}