namespace CustomFinancialPlanningAssistant.Core.Enums;

/// <summary>
/// Represents the type of AI analysis to perform on financial data
/// </summary>
public enum AnalysisType
{
    /// <summary>
    /// Generate a summary of financial data
    /// </summary>
    Summary = 0,

    /// <summary>
    /// Analyze trends over time
    /// </summary>
    TrendAnalysis = 1,

    /// <summary>
    /// Detect anomalies and outliers
    /// </summary>
    AnomalyDetection = 2,

    /// <summary>
    /// Compare different time periods or datasets
    /// </summary>
    Comparison = 3,

    /// <summary>
    /// Forecast future financial performance
    /// </summary>
    Forecasting = 4,

    /// <summary>
    /// Calculate and analyze financial ratios
    /// </summary>
    RatioAnalysis = 5,

    /// <summary>
    /// Custom analysis based on user-defined criteria
    /// </summary>
    Custom = 6
}
