using System.Text;
using CustomFinancialPlanningAssistant.Core.Entities;

namespace CustomFinancialPlanningAssistant.Services.AI;

/// <summary>
/// Provides specialized prompt templates for financial analysis with AI models
/// </summary>
public static class PromptTemplates
{
    /// <summary>
    /// Generates a prompt for financial summary analysis
    /// </summary>
    /// <param name="data">Financial data to analyze</param>
    /// <returns>Formatted prompt for AI analysis</returns>
    public static string GetFinancialSummaryPrompt(List<FinancialData> data)
    {
        var formattedData = FormatFinancialDataForPrompt(data);
        
        return $@"You are an expert financial analyst. Analyze the following financial data and provide a comprehensive summary.

{formattedData}

Please provide your analysis in the following structured format:

1. **Summary Overview**
   - Total Revenue
   - Total Expenses
   - Net Income/Loss
   - Overall financial health assessment

2. **Key Observations**
   - Notable trends
   - Significant items
   - Areas of concern

3. **Recommendations**
   - Actionable insights
   - Suggested improvements
   - Risk mitigation strategies

Provide clear, professional analysis with specific numbers and percentages.";
    }

    /// <summary>
    /// Generates a prompt for trend analysis over time
    /// </summary>
    /// <param name="data">Financial data to analyze</param>
    /// <param name="period">Time period for analysis</param>
    /// <returns>Formatted prompt for trend analysis</returns>
    public static string GetTrendAnalysisPrompt(List<FinancialData> data, string period)
    {
        var formattedData = FormatFinancialDataForPrompt(data);
        
        return $@"You are a financial analyst specializing in trend analysis. Analyze the following financial data for the period: {period}

{formattedData}

Please provide your trend analysis including:

1. **Period-over-Period Comparison**
   - Compare current period to previous periods if data available
   - Calculate percentage changes
   - Identify growth or decline patterns

2. **Trend Identification**
   - Revenue trends (growing, stable, declining)
   - Expense trends and their drivers
   - Profitability trends

3. **Pattern Recognition**
   - Seasonal patterns
   - Cyclical trends
   - Emerging trends

4. **Visualization Recommendations**
   - Suggest appropriate chart types
   - Key metrics to visualize

5. **Forecasting Insights**
   - Short-term trend projections
   - Factors that may impact future trends

Provide specific numbers, percentages, and timeframes in your analysis.";
    }

    /// <summary>
    /// Generates a prompt for anomaly detection
    /// </summary>
    /// <param name="data">Financial data to analyze</param>
    /// <returns>Formatted prompt for anomaly detection</returns>
    public static string GetAnomalyDetectionPrompt(List<FinancialData> data)
    {
        var formattedData = FormatFinancialDataForPrompt(data);
        var stats = CalculateBasicStatistics(data);
        
        return $@"You are a financial auditor specializing in anomaly detection. Analyze the following financial data for unusual patterns or outliers.

{formattedData}

Statistical Context:
- Average transaction amount: {stats.Average:C2}
- Median amount: {stats.Median:C2}
- Total transactions: {stats.Count}

Please identify and analyze:

1. **Outliers and Unusual Amounts**
   - Transactions significantly above or below normal
   - Severity rating (Low/Medium/High)
   - Potential explanations

2. **Pattern Anomalies**
   - Unexpected categorizations
   - Missing expected transactions
   - Duplicate or redundant entries

3. **Timing Anomalies**
   - Transactions at unusual times/dates
   - Period inconsistencies

4. **Red Flags**
   - Suspicious patterns
   - Compliance concerns
   - Fraud indicators

5. **Investigation Recommendations**
   - Priority items requiring review
   - Suggested next steps
   - Required documentation

Rate each anomaly by severity and provide specific transaction details.";
    }

    /// <summary>
    /// Generates a prompt for financial ratio analysis
    /// </summary>
    /// <param name="ratios">Calculated financial ratios</param>
    /// <returns>Formatted prompt for ratio analysis</returns>
    public static string GetRatioAnalysisPrompt(Dictionary<string, decimal> ratios)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Calculated Financial Ratios:");
        foreach (var ratio in ratios)
        {
            sb.AppendLine($"  {ratio.Key}: {ratio.Value:N2}");
        }
        
        return $@"You are a financial analyst specializing in ratio analysis. Evaluate the following financial ratios:

{sb}

Please provide comprehensive analysis:

1. **Ratio Interpretation**
   - Explain what each ratio indicates
   - Assess whether values are healthy/concerning
   - Context for each metric

2. **Industry Benchmarking**
   - Compare to typical industry standards
   - Identify strengths and weaknesses
   - Competitive position assessment

3. **Liquidity Analysis**
   - Current ratio and quick ratio evaluation
   - Working capital assessment
   - Short-term financial health

4. **Profitability Analysis**
   - Profit margin evaluation
   - Return on investment assessment
   - Efficiency metrics

5. **Overall Financial Health**
   - Composite health score
   - Key strengths
   - Areas requiring attention

6. **Strategic Recommendations**
   - Actions to improve weak ratios
   - Strategies to leverage strengths
   - Risk mitigation plans

Provide specific, actionable insights with clear explanations.";
    }

    /// <summary>
    /// Generates a prompt for comparing two time periods
    /// </summary>
    /// <param name="currentPeriod">Current period financial data</param>
    /// <param name="previousPeriod">Previous period financial data</param>
    /// <returns>Formatted prompt for period comparison</returns>
    public static string GetComparisonPrompt(List<FinancialData> currentPeriod, List<FinancialData> previousPeriod)
    {
        var currentData = FormatFinancialDataForPrompt(currentPeriod);
        var previousData = FormatFinancialDataForPrompt(previousPeriod);
        
        return $@"You are a financial analyst performing period-over-period comparison. Analyze the following data:

**CURRENT PERIOD:**
{currentData}

**PREVIOUS PERIOD:**
{previousData}

Please provide detailed comparison:

1. **Revenue Analysis**
   - Period-over-period change (amount and %)
   - Revenue drivers
   - Growth/decline explanation

2. **Expense Analysis**
   - Cost changes by category
   - Expense trends
   - Cost efficiency improvements or concerns

3. **Variance Analysis**
   - Significant variances explained
   - Budget vs actual (if applicable)
   - Unexpected changes

4. **Profitability Changes**
   - Net income change
   - Margin improvements/deterioration
   - Contributing factors

5. **Strategic Insights**
   - What's working well
   - What needs attention
   - Recommended actions

Highlight the most significant changes with specific numbers and percentages.";
    }

    /// <summary>
    /// Generates a prompt for cash flow analysis
    /// </summary>
    /// <param name="data">Financial data focused on cash accounts</param>
    /// <returns>Formatted prompt for cash flow analysis</returns>
    public static string GetCashFlowAnalysisPrompt(List<FinancialData> data)
    {
        var formattedData = FormatFinancialDataForPrompt(data);
        
        return $@"You are a financial analyst specializing in cash flow management. Analyze the following financial data with focus on cash and liquidity:

{formattedData}

Please provide comprehensive cash flow analysis:

1. **Cash Flow Categories**
   - Operating activities
   - Investing activities
   - Financing activities

2. **Liquidity Assessment**
   - Current cash position
   - Cash burn rate (if applicable)
   - Runway calculation
   - Liquidity ratios

3. **Working Capital Analysis**
   - Working capital position
   - Changes in working capital
   - Efficiency of working capital usage

4. **Cash Flow Trends**
   - Cash generation patterns
   - Seasonal variations
   - Sustainability of cash flow

5. **Concerns and Red Flags**
   - Cash flow problems
   - Liquidity risks
   - Warning signs

6. **Recommendations**
   - Cash management improvements
   - Working capital optimization
   - Liquidity enhancement strategies

Focus on actionable insights for cash management and liquidity improvement.";
    }

    /// <summary>
    /// Generates a prompt for financial forecasting
    /// </summary>
    /// <param name="historicalData">Historical financial data</param>
    /// <param name="periodsAhead">Number of periods to forecast</param>
    /// <returns>Formatted prompt for forecasting</returns>
    public static string GetForecastingPrompt(List<FinancialData> historicalData, int periodsAhead)
    {
        var formattedData = FormatFinancialDataForPrompt(historicalData);
        
        return $@"You are a financial forecasting specialist. Based on the following historical data, provide projections for the next {periodsAhead} period(s):

{formattedData}

Please provide detailed forecast:

1. **Trend-Based Projections**
   - Revenue forecast with confidence levels
   - Expense projections by category
   - Expected net income

2. **Methodology**
   - Forecasting approach used
   - Key assumptions made
   - Statistical basis for projections

3. **Confidence Levels**
   - Best case scenario
   - Most likely scenario
   - Worst case scenario
   - Confidence intervals

4. **Growth Assumptions**
   - Expected growth rates
   - Market conditions considered
   - Seasonal adjustments

5. **Risk Factors**
   - Internal risks
   - External risks
   - Sensitivity to assumptions

6. **Recommendations**
   - Planning suggestions
   - Risk mitigation strategies
   - Contingency plans

Provide specific numbers with ranges and probabilities where applicable.";
    }

    /// <summary>
    /// Generates a custom analysis prompt based on user question
    /// </summary>
    /// <param name="userQuestion">User's specific question</param>
    /// <param name="data">Relevant financial data</param>
    /// <returns>Formatted prompt combining question and data</returns>
    public static string GetCustomAnalysisPrompt(string userQuestion, List<FinancialData> data)
    {
        var formattedData = FormatFinancialDataForPrompt(data);
        
        return $@"You are an expert financial analyst. Please answer the following question using the provided financial data:

**Question:**
{userQuestion}

**Financial Data:**
{formattedData}

Please provide:
1. Direct answer to the question
2. Supporting evidence from the data
3. Relevant analysis and context
4. Specific numbers and calculations
5. Any caveats or limitations

Be specific, data-driven, and actionable in your response.";
    }

    /// <summary>
    /// Formats financial data into readable text for AI prompts
    /// </summary>
    /// <param name="data">Financial data to format</param>
    /// <returns>Formatted string representation</returns>
    private static string FormatFinancialDataForPrompt(List<FinancialData> data)
    {
        if (data == null || !data.Any())
        {
            return "No financial data available.";
        }

        var sb = new StringBuilder();
        sb.AppendLine("```");
        sb.AppendLine($"{"Account",-40} | {"Category",-15} | {"Period",-12} | {"Amount",15}");
        sb.AppendLine(new string('-', 90));

        foreach (var item in data.OrderBy(d => d.Category).ThenBy(d => d.Period))
        {
            var accountName = item.AccountName.Length > 38 
                ? item.AccountName.Substring(0, 35) + "..." 
                : item.AccountName;
            
            sb.AppendLine($"{accountName,-40} | {item.Category,-15} | {item.Period,-12} | {item.Amount,15:C2}");
        }

        // Add summary by category
        sb.AppendLine(new string('-', 90));
        sb.AppendLine("\nSummary by Category:");
        var categoryTotals = data
            .GroupBy(d => d.Category)
            .Select(g => new { Category = g.Key, Total = g.Sum(x => x.Amount) })
            .OrderByDescending(x => Math.Abs(x.Total));

        foreach (var category in categoryTotals)
        {
            sb.AppendLine($"  {category.Category,-30}: {category.Total,15:C2}");
        }
        
        sb.AppendLine("```");
        return sb.ToString();
    }

    /// <summary>
    /// Calculates basic statistics for anomaly detection context
    /// </summary>
    private static (decimal Average, decimal Median, int Count) CalculateBasicStatistics(List<FinancialData> data)
    {
        if (data == null || !data.Any())
        {
            return (0, 0, 0);
        }

        var amounts = data.Select(d => d.Amount).OrderBy(x => x).ToList();
        var average = amounts.Average();
        var median = amounts.Count % 2 == 0
            ? (amounts[amounts.Count / 2 - 1] + amounts[amounts.Count / 2]) / 2
            : amounts[amounts.Count / 2];

        return (average, median, data.Count);
    }
}
