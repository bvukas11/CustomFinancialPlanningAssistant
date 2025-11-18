namespace CustomFinancialPlanningAssistant.Core.Enums;

/// <summary>
/// Types of financial reports that can be generated
/// </summary>
public enum ReportType
{
    /// <summary>
    /// Summary report with key financial metrics
    /// </summary>
    Summary = 0,
    
    /// <summary>
    /// Detailed report with comprehensive data
    /// </summary>
    Detailed = 1,
    
    /// <summary>
    /// Comparison report between multiple documents
    /// </summary>
    Comparison = 2,
    
    /// <summary>
    /// Trend analysis report
    /// </summary>
    Trend = 3,
    
    /// <summary>
    /// Financial ratio analysis report
    /// </summary>
    RatioAnalysis = 4,
    
    /// <summary>
    /// Custom report using templates
    /// </summary>
    Custom = 5
}
