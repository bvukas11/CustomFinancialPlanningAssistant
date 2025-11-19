namespace CustomFinancialPlanningAssistant.Core.DTOs;

/// <summary>
/// DTO for cash flow optimization analysis results
/// </summary>
public class CashFlowOptimizationDto
{
    /// <summary>
    /// ID of the document being analyzed
    /// </summary>
    public int DocumentId { get; set; }

    /// <summary>
    /// Current cash position
    /// </summary>
    public decimal CurrentCashPosition { get; set; }

    /// <summary>
    /// Monthly cash burn rate
    /// </summary>
    public decimal MonthlyBurnRate { get; set; }

    /// <summary>
    /// Number of months of runway remaining
    /// </summary>
    public decimal RunwayMonths { get; set; }

    /// <summary>
    /// Immediate actions to take (next 30 days)
    /// </summary>
    public List<string> ImmediateActions { get; set; } = new();

    /// <summary>
    /// Short-term improvements (3-6 months)
    /// </summary>
    public List<string> ShortTermImprovements { get; set; } = new();

    /// <summary>
    /// Long-term optimization strategies (6-12 months)
    /// </summary>
    public List<string> LongTermStrategies { get; set; } = new();

    /// <summary>
    /// Working capital optimization recommendations
    /// </summary>
    public List<string> WorkingCapitalOptimizations { get; set; } = new();

    /// <summary>
    /// Cash generation strategies
    /// </summary>
    public List<string> CashGenerationStrategies { get; set; } = new();

    /// <summary>
    /// Risk mitigation strategies
    /// </summary>
    public List<string> RiskMitigations { get; set; } = new();

    /// <summary>
    /// Implementation roadmap with timeline
    /// </summary>
    public List<string> ImplementationRoadmap { get; set; } = new();

    /// <summary>
    /// Success metrics to track
    /// </summary>
    public List<string> SuccessMetrics { get; set; } = new();

    /// <summary>
    /// When this analysis was performed
    /// </summary>
    public DateTime AnalysisDate { get; set; }
}