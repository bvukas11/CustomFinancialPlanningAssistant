namespace CustomFinancialPlanningAssistant.Core.DTOs;

/// <summary>
/// DTO for investment recommendation results
/// </summary>
public class InvestmentRecommendationDto
{
    /// <summary>
    /// ID of the document being analyzed
    /// </summary>
    public int DocumentId { get; set; }

    /// <summary>
    /// Investor's risk tolerance level
    /// </summary>
    public string RiskTolerance { get; set; } = string.Empty;

    /// <summary>
    /// Overall investment recommendation (Buy/Hold/Sell)
    /// </summary>
    public string Recommendation { get; set; } = string.Empty;

    /// <summary>
    /// Confidence level in the recommendation
    /// </summary>
    public string ConfidenceLevel { get; set; } = string.Empty;

    /// <summary>
    /// Key factors influencing the recommendation
    /// </summary>
    public List<string> KeyFactors { get; set; } = new();

    /// <summary>
    /// Recommended investment time horizon
    /// </summary>
    public string TimeHorizon { get; set; } = string.Empty;

    /// <summary>
    /// Expected returns description
    /// </summary>
    public string ExpectedReturns { get; set; } = string.Empty;

    /// <summary>
    /// Key investment risks
    /// </summary>
    public List<string> Risks { get; set; } = new();

    /// <summary>
    /// Alternative investment options
    /// </summary>
    public List<string> Alternatives { get; set; } = new();

    /// <summary>
    /// When this analysis was performed
    /// </summary>
    public DateTime AnalysisDate { get; set; }
}