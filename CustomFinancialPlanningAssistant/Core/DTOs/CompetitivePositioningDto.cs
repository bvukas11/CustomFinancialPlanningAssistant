namespace CustomFinancialPlanningAssistant.Core.DTOs;

/// <summary>
/// DTO for competitive positioning assessment
/// </summary>
public class CompetitivePositioningDto
{
    /// <summary>
    /// Overall competitive position (Leader, Above Average, Average, Below Average, Laggard)
    /// </summary>
    public string OverallPosition { get; set; } = string.Empty;

    /// <summary>
    /// Competitive score (0-100) based on benchmark performance
    /// </summary>
    public decimal CompetitiveScore { get; set; }

    /// <summary>
    /// Number of metrics where company performs above industry average
    /// </summary>
    public int StrengthsCount { get; set; }

    /// <summary>
    /// Number of metrics where company performs below industry average
    /// </summary>
    public int WeaknessesCount { get; set; }

    /// <summary>
    /// Key competitive advantages
    /// </summary>
    public List<string> CompetitiveAdvantages { get; set; } = new();

    /// <summary>
    /// Key competitive disadvantages
    /// </summary>
    public List<string> CompetitiveDisadvantages { get; set; } = new();

    /// <summary>
    /// Market positioning summary
    /// </summary>
    public string MarketPositionSummary { get; set; } = string.Empty;
}