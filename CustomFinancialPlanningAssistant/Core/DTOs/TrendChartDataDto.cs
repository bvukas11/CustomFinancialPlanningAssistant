namespace CustomFinancialPlanningAssistant.Core.DTOs;

/// <summary>
/// Represents a single data point in a trend chart
/// </summary>
public class ChartDataPointDto
{
    public string Label { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public string Category { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Color { get; set; } = "#1976D2";
}

/// <summary>
/// Represents trend chart data with statistics
/// </summary>
public class TrendChartDataDto
{
    public List<ChartDataPointDto> DataPoints { get; set; } = new();
    public string Title { get; set; } = string.Empty;
    public string YAxisLabel { get; set; } = "Amount ($)";
    public string XAxisLabel { get; set; } = "Period";
    public decimal GrowthRate { get; set; }
    public decimal Average { get; set; }
    public decimal Minimum { get; set; }
    public decimal Maximum { get; set; }
}

/// <summary>
/// Represents a period-over-period comparison
/// </summary>
public class PeriodComparisonDto
{
    public string CurrentPeriod { get; set; } = string.Empty;
    public string PreviousPeriod { get; set; } = string.Empty;
    public decimal CurrentValue { get; set; }
    public decimal PreviousValue { get; set; }
    public decimal Change { get; set; }
    public decimal ChangePercentage { get; set; }
    public bool IsImprovement { get; set; }
}
