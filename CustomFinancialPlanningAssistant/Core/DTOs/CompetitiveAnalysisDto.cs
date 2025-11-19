using CustomFinancialPlanningAssistant.Core.Enums;

namespace CustomFinancialPlanningAssistant.Core.DTOs;

/// <summary>
/// DTO for comprehensive competitive analysis results
/// </summary>
public class CompetitiveAnalysisDto
{
    /// <summary>
    /// ID of the document being analyzed
    /// </summary>
    public int DocumentId { get; set; }

    /// <summary>
    /// Name of the document being analyzed
    /// </summary>
    public string DocumentName { get; set; } = string.Empty;

    /// <summary>
    /// Industry selected for benchmarking
    /// </summary>
    public IndustryType Industry { get; set; }

    /// <summary>
    /// Detailed benchmark comparisons for each metric
    /// </summary>
    public List<IndustryBenchmarkDto> Benchmarks { get; set; } = new();

    /// <summary>
    /// Overall competitive positioning assessment
    /// </summary>
    public CompetitivePositioningDto Positioning { get; set; } = new();

    /// <summary>
    /// Key insights from the competitive analysis
    /// </summary>
    public List<string> KeyInsights { get; set; } = new();

    /// <summary>
    /// Strategic recommendations based on the analysis
    /// </summary>
    public List<string> Recommendations { get; set; } = new();

    /// <summary>
    /// Industry-specific trends and observations
    /// </summary>
    public List<string> IndustryTrends { get; set; } = new();

    /// <summary>
    /// When this analysis was performed
    /// </summary>
    public DateTime AnalysisDate { get; set; }

    /// <summary>
    /// Execution time for the analysis in milliseconds
    /// </summary>
    public long ExecutionTimeMs { get; set; }

    /// <summary>
    /// AI model used for the analysis
    /// </summary>
    public string ModelUsed { get; set; } = string.Empty;

    /// <summary>
    /// Summary of the competitive analysis
    /// </summary>
    public string ExecutiveSummary { get; set; } = string.Empty;
}