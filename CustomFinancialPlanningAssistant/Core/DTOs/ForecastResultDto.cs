namespace CustomFinancialPlanningAssistant.Core.DTOs;

/// <summary>
/// Data Transfer Object for forecast results
/// </summary>
public class ForecastResultDto
{
    /// <summary>
    /// Category being forecasted
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// List of forecasted values
    /// </summary>
    public List<ForecastDataPoint> ForecastedValues { get; set; } = new();

    /// <summary>
    /// Forecasting method used (Linear, MovingAverage, AIBased)
    /// </summary>
    public string Method { get; set; } = string.Empty;

    /// <summary>
    /// Confidence level of the forecast (0-100)
    /// </summary>
    public decimal ConfidenceLevel { get; set; }

    /// <summary>
    /// Assumptions made in the forecast
    /// </summary>
    public List<string> Assumptions { get; set; } = new();

    /// <summary>
    /// Risk factors that could impact the forecast
    /// </summary>
    public List<string> RiskFactors { get; set; } = new();

    /// <summary>
    /// Date when this forecast was generated
    /// </summary>
    public DateTime GeneratedDate { get; set; }
}

/// <summary>
/// Represents a single forecasted data point
/// </summary>
public class ForecastDataPoint
{
    /// <summary>
    /// Period identifier for the forecast
    /// </summary>
    public string Period { get; set; } = string.Empty;

    /// <summary>
    /// Forecasted value
    /// </summary>
    public decimal ForecastedValue { get; set; }

    /// <summary>
    /// Lower bound of the confidence interval
    /// </summary>
    public decimal LowerBound { get; set; }

    /// <summary>
    /// Upper bound of the confidence interval
    /// </summary>
    public decimal UpperBound { get; set; }
}
