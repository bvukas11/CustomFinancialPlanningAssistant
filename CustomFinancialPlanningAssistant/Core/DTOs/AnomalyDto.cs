namespace CustomFinancialPlanningAssistant.Core.DTOs;

/// <summary>
/// Data Transfer Object for detected anomalies in financial data
/// </summary>
public class AnomalyDto
{
    /// <summary>
    /// ID of the financial data record
    /// </summary>
    public int RecordId { get; set; }

    /// <summary>
    /// Account name where anomaly was detected
    /// </summary>
    public string AccountName { get; set; } = string.Empty;

    /// <summary>
    /// Period when the anomaly occurred
    /// </summary>
    public string Period { get; set; } = string.Empty;

    /// <summary>
    /// Actual value recorded
    /// </summary>
    public decimal Value { get; set; }

    /// <summary>
    /// Expected value based on historical data
    /// </summary>
    public decimal ExpectedValue { get; set; }

    /// <summary>
    /// Absolute deviation from expected value
    /// </summary>
    public decimal Deviation { get; set; }

    /// <summary>
    /// Percentage deviation from expected value
    /// </summary>
    public decimal DeviationPercentage { get; set; }

    /// <summary>
    /// Severity of the anomaly (Low, Medium, High)
    /// </summary>
    public string Severity { get; set; } = string.Empty;

    /// <summary>
    /// Reason why this was flagged as an anomaly
    /// </summary>
    public string Reason { get; set; } = string.Empty;

    /// <summary>
    /// Date when the anomaly was detected
    /// </summary>
    public DateTime DetectedDate { get; set; }
}
