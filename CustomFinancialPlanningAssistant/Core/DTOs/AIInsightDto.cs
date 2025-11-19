namespace CustomFinancialPlanningAssistant.Core.DTOs;

/// <summary>
/// Comprehensive AI insight response with all analysis components
/// </summary>
public class AIInsightDto
{
    public int Id { get; set; }
    public int DocumentId { get; set; }
    public string DocumentName { get; set; } = string.Empty;
    public string AnalysisType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string DetailedAnalysis { get; set; } = string.Empty;
    public List<string> KeyFindings { get; set; } = new();
    public List<string> Recommendations { get; set; } = new();
    public List<string> RiskFactors { get; set; } = new();
    public List<string> Opportunities { get; set; } = new();
    public int HealthScore { get; set; } // 0-100
    public string RiskLevel { get; set; } = "Medium"; // Low, Medium, High, Critical
    public DateTime GeneratedDate { get; set; }
    public int ExecutionTime { get; set; }
    public string ModelUsed { get; set; } = "Llama 3.2";
}

/// <summary>
/// Request for AI analysis with options
/// </summary>
public class AIInsightRequestDto
{
    public int DocumentId { get; set; }
    public string AnalysisType { get; set; } = string.Empty;
    public string CustomQuestion { get; set; } = string.Empty;
    public bool IncludeHistoricalData { get; set; }
    public bool IncludeComparisons { get; set; }
    public bool IncludeForecasts { get; set; }
}

/// <summary>
/// Financial health assessment with breakdown scores
/// </summary>
public class FinancialHealthDto
{
    public int OverallScore { get; set; } // 0-100
    public int ProfitabilityScore { get; set; }
    public int LiquidityScore { get; set; }
    public int EfficiencyScore { get; set; }
    public int StabilityScore { get; set; }
    public string OverallRating { get; set; } = string.Empty; // Excellent, Good, Fair, Poor
    public List<string> Strengths { get; set; } = new();
    public List<string> Weaknesses { get; set; } = new();
    public List<string> Priorities { get; set; } = new();
}

/// <summary>
/// Comprehensive risk assessment result
/// </summary>
public class RiskAssessmentDto
{
    public string RiskLevel { get; set; } = "Medium"; // Low, Medium, High, Critical
    public int RiskScore { get; set; } // 0-100 (higher = more risk)
    public List<RiskItemDto> Risks { get; set; } = new();
    public List<string> MitigationStrategies { get; set; } = new();
}

/// <summary>
/// Individual risk item with details
/// </summary>
public class RiskItemDto
{
    public string Category { get; set; } = string.Empty; // Liquidity, Profitability, Solvency, etc.
    public string Description { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty; // Low, Medium, High, Critical
    public string Impact { get; set; } = string.Empty;
    public List<string> Recommendations { get; set; } = new();
}
