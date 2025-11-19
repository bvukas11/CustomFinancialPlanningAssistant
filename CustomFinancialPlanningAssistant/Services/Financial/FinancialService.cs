using System.Diagnostics;
using Microsoft.Extensions.Logging;
using CustomFinancialPlanningAssistant.Core.DTOs;
using CustomFinancialPlanningAssistant.Core.Entities;
using CustomFinancialPlanningAssistant.Core.Enums;
using CustomFinancialPlanningAssistant.Infrastructure.Repositories;
using CustomFinancialPlanningAssistant.Services.AI;

namespace CustomFinancialPlanningAssistant.Services.Financial;

public class FinancialService : IFinancialService
{
    private readonly IFinancialDocumentRepository _documentRepo;
    private readonly IFinancialDataRepository _dataRepo;
    private readonly IAIAnalysisRepository _analysisRepo;
    private readonly ILlamaService _aiService;
    private readonly ILogger<FinancialService> _logger;

    public FinancialService(
        IFinancialDocumentRepository documentRepo,
        IFinancialDataRepository dataRepo,
        IAIAnalysisRepository analysisRepo,
        ILlamaService aiService,
        ILogger<FinancialService> logger)
    {
        _documentRepo = documentRepo ?? throw new ArgumentNullException(nameof(documentRepo));
        _dataRepo = dataRepo ?? throw new ArgumentNullException(nameof(dataRepo));
        _analysisRepo = analysisRepo ?? throw new ArgumentNullException(nameof(analysisRepo));
        _aiService = aiService ?? throw new ArgumentNullException(nameof(aiService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    #region Summary & Overview Methods

    public async Task<FinancialSummaryDto> GetFinancialSummaryAsync(int documentId)
    {
        try
        {
            _logger.LogInformation("Generating financial summary for document: {DocumentId}", documentId);

            var document = await _documentRepo.GetWithDataAsync(documentId);
            if (document == null)
            {
                throw new ArgumentException($"Document {documentId} not found");
            }

            var data = document.FinancialDataRecords.ToList();

            if (!data.Any())
            {
                throw new InvalidOperationException("No financial data found for document");
            }

            var summary = new FinancialSummaryDto
            {
                DocumentId = documentId,
                DocumentName = document.FileName,
                Period = data.FirstOrDefault()?.Period ?? "Unknown",
                GeneratedDate = DateTime.UtcNow
            };

            summary.TotalRevenue = data
                .Where(d => d.Category.Equals("Revenue", StringComparison.OrdinalIgnoreCase))
                .Sum(d => d.Amount);

            summary.TotalExpenses = data
                .Where(d => d.Category.Equals("Expense", StringComparison.OrdinalIgnoreCase))
                .Sum(d => d.Amount);

            summary.TotalAssets = data
                .Where(d => d.Category.Equals("Asset", StringComparison.OrdinalIgnoreCase))
                .Sum(d => d.Amount);

            summary.TotalLiabilities = data
                .Where(d => d.Category.Equals("Liability", StringComparison.OrdinalIgnoreCase))
                .Sum(d => d.Amount);

            summary.Equity = data
                .Where(d => d.Category.Equals("Equity", StringComparison.OrdinalIgnoreCase))
                .Sum(d => d.Amount);

            summary.NetIncome = summary.TotalRevenue - summary.TotalExpenses;
            summary.GrossProfit = summary.TotalRevenue;
            summary.OperatingIncome = summary.NetIncome;

            summary.CategoryBreakdown = data
                .GroupBy(d => d.Category)
                .ToDictionary(g => g.Key, g => g.Sum(d => d.Amount));

            summary.KeyHighlights = GenerateKeyHighlights(summary);

            _logger.LogInformation("Financial summary generated successfully for document {DocumentId}", documentId);
            return summary;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating financial summary for document {DocumentId}", documentId);
            throw;
        }
    }

    public async Task<FinancialSummaryDto> GetFinancialSummaryByPeriodAsync(string period)
    {
        try
        {
            _logger.LogInformation("Generating financial summary for period: {Period}", period);

            var data = await _dataRepo.GetByPeriodAsync(period);

            if (!data.Any())
            {
                throw new InvalidOperationException($"No financial data found for period {period}");
            }

            var summary = new FinancialSummaryDto
            {
                DocumentName = $"Period Summary: {period}",
                Period = period,
                GeneratedDate = DateTime.UtcNow
            };

            summary.TotalRevenue = data
                .Where(d => d.Category.Equals("Revenue", StringComparison.OrdinalIgnoreCase))
                .Sum(d => d.Amount);

            summary.TotalExpenses = data
                .Where(d => d.Category.Equals("Expense", StringComparison.OrdinalIgnoreCase))
                .Sum(d => d.Amount);

            summary.TotalAssets = data
                .Where(d => d.Category.Equals("Asset", StringComparison.OrdinalIgnoreCase))
                .Sum(d => d.Amount);

            summary.TotalLiabilities = data
                .Where(d => d.Category.Equals("Liability", StringComparison.OrdinalIgnoreCase))
                .Sum(d => d.Amount);

            summary.Equity = data
                .Where(d => d.Category.Equals("Equity", StringComparison.OrdinalIgnoreCase))
                .Sum(d => d.Amount);

            summary.NetIncome = summary.TotalRevenue - summary.TotalExpenses;
            summary.GrossProfit = summary.TotalRevenue;
            summary.OperatingIncome = summary.NetIncome;

            summary.CategoryBreakdown = data
                .GroupBy(d => d.Category)
                .ToDictionary(g => g.Key, g => g.Sum(d => d.Amount));

            summary.KeyHighlights = GenerateKeyHighlights(summary);

            return summary;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating financial summary for period {Period}", period);
            throw;
        }
    }

    public async Task<Dictionary<string, decimal>> GetCategorySummaryAsync(int documentId)
    {
        var document = await _documentRepo.GetWithDataAsync(documentId);
        if (document == null)
        {
            throw new ArgumentException($"Document {documentId} not found");
        }

        return document.FinancialDataRecords
            .GroupBy(d => d.Category)
            .ToDictionary(g => g.Key, g => g.Sum(d => d.Amount));
    }

    public async Task<Dictionary<string, decimal>> GetPeriodSummaryAsync(int documentId)
    {
        var document = await _documentRepo.GetWithDataAsync(documentId);
        if (document == null)
        {
            throw new ArgumentException($"Document {documentId} not found");
        }

        return document.FinancialDataRecords
            .GroupBy(d => d.Period)
            .ToDictionary(g => g.Key, g => g.Sum(d => d.Amount));
    }

    private List<string> GenerateKeyHighlights(FinancialSummaryDto summary)
    {
        var highlights = new List<string>();

        if (summary.TotalRevenue > 0)
        {
            highlights.Add($"Total Revenue: {summary.TotalRevenue:C}");
        }

        if (summary.NetIncome > 0)
        {
            highlights.Add($"Profitable period with Net Income: {summary.NetIncome:C}");
        }
        else if (summary.NetIncome < 0)
        {
            highlights.Add($"Loss of {Math.Abs(summary.NetIncome):C} recorded");
        }

        if (summary.TotalRevenue > 0)
        {
            var profitMargin = (summary.NetIncome / summary.TotalRevenue) * 100;
            highlights.Add($"Profit Margin: {profitMargin:F2}%");
        }

        if (summary.TotalRevenue > 0 && summary.TotalExpenses > 0)
        {
            var expenseRatio = (summary.TotalExpenses / summary.TotalRevenue) * 100;
            highlights.Add($"Expense Ratio: {expenseRatio:F2}%");
        }

        if (summary.TotalAssets > 0)
        {
            highlights.Add($"Total Assets: {summary.TotalAssets:C}");
        }

        if (summary.TotalLiabilities > 0 && summary.Equity > 0)
        {
            var debtToEquity = summary.TotalLiabilities / summary.Equity;
            highlights.Add($"Debt-to-Equity Ratio: {debtToEquity:F2}");
        }

        return highlights;
    }

    #endregion

    #region Ratio Analysis Methods

    public async Task<Dictionary<string, decimal>> CalculateFinancialRatiosAsync(int documentId)
    {
        try
        {
            _logger.LogInformation("Calculating financial ratios for document: {DocumentId}", documentId);

            var ratios = new Dictionary<string, decimal>();

            var profitability = await CalculateProfitabilityRatiosAsync(documentId);
            var liquidity = await CalculateLiquidityRatiosAsync(documentId);
            var efficiency = await CalculateEfficiencyRatiosAsync(documentId);

            foreach (var ratio in profitability)
                ratios[ratio.Key] = ratio.Value;

            foreach (var ratio in liquidity)
                ratios[ratio.Key] = ratio.Value;

            foreach (var ratio in efficiency)
                ratios[ratio.Key] = ratio.Value;

            return ratios;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating financial ratios for document {DocumentId}", documentId);
            throw;
        }
    }

    public async Task<Dictionary<string, decimal>> CalculateProfitabilityRatiosAsync(int documentId)
    {
        var ratios = new Dictionary<string, decimal>();
        var summary = await GetFinancialSummaryAsync(documentId);

        if (summary.TotalRevenue > 0)
        {
            ratios["GrossProfitMargin"] = (summary.GrossProfit / summary.TotalRevenue) * 100;
            ratios["NetProfitMargin"] = (summary.NetIncome / summary.TotalRevenue) * 100;
            ratios["OperatingProfitMargin"] = (summary.OperatingIncome / summary.TotalRevenue) * 100;
        }

        if (summary.TotalAssets > 0)
        {
            ratios["ReturnOnAssets"] = (summary.NetIncome / summary.TotalAssets) * 100;
        }

        if (summary.Equity > 0)
        {
            ratios["ReturnOnEquity"] = (summary.NetIncome / summary.Equity) * 100;
        }

        return ratios;
    }

    public async Task<Dictionary<string, decimal>> CalculateLiquidityRatiosAsync(int documentId)
    {
        var ratios = new Dictionary<string, decimal>();
        var summary = await GetFinancialSummaryAsync(documentId);

        if (summary.TotalLiabilities > 0)
        {
            ratios["CurrentRatio"] = summary.TotalAssets / summary.TotalLiabilities;
        }

        if (summary.Equity > 0)
        {
            ratios["DebtToEquity"] = summary.TotalLiabilities / summary.Equity;
        }

        if (summary.TotalAssets > 0)
        {
            ratios["DebtToAssets"] = (summary.TotalLiabilities / summary.TotalAssets) * 100;
            ratios["EquityRatio"] = (summary.Equity / summary.TotalAssets) * 100;
        }

        return ratios;
    }

    public async Task<Dictionary<string, decimal>> CalculateEfficiencyRatiosAsync(int documentId)
    {
        var ratios = new Dictionary<string, decimal>();
        var summary = await GetFinancialSummaryAsync(documentId);

        if (summary.TotalAssets > 0)
        {
            ratios["AssetTurnover"] = summary.TotalRevenue / summary.TotalAssets;
        }

        if (summary.TotalRevenue > 0)
        {
            ratios["OperatingExpenseRatio"] = (summary.TotalExpenses / summary.TotalRevenue) * 100;
        }

        return ratios;
    }

    #endregion

    #region Trend Analysis Methods

    public async Task<TrendAnalysisDto> AnalyzeTrendsAsync(List<int> documentIds)
{
    var periods = new List<string>();
    foreach (var docId in documentIds)
    {
        var doc = await _documentRepo.GetWithDataAsync(docId);
        if (doc?.FinancialDataRecords.Any() == true)
        {
            var period = doc.FinancialDataRecords.First().Period;
            if (!periods.Contains(period))
                periods.Add(period);
        }
    }

    return await AnalyzeTrendsByPeriodAsync(periods);
}

public async Task<TrendAnalysisDto> AnalyzeTrendsByPeriodAsync(List<string> periods)
{
    try
    {
        _logger.LogInformation("Analyzing trends for {Count} periods", periods.Count);

        var allData = new List<FinancialData>();
        foreach (var period in periods.OrderBy(p => p))
        {
            var periodData = await _dataRepo.GetByPeriodAsync(period);
            allData.AddRange(periodData);
        }

        if (!allData.Any())
        {
            throw new InvalidOperationException("No data found for specified periods");
        }

        var periodTotals = allData
            .GroupBy(d => d.Period)
            .OrderBy(g => g.Key)
            .Select(g => new TrendDataPoint
            {
                Period = g.Key,
                Value = g.Sum(d => d.Amount)
            })
            .ToList();

        for (int i = 1; i < periodTotals.Count; i++)
        {
            var previous = periodTotals[i - 1].Value;
            var current = periodTotals[i].Value;

            if (previous != 0)
            {
                periodTotals[i].PercentageChange = ((current - previous) / previous) * 100;
            }
        }

        var trendDirection = DetermineTrendDirection(periodTotals);
        var growthRates = periodTotals
            .Where(p => p.PercentageChange.HasValue)
            .Select(p => p.PercentageChange.Value)
            .ToList();

        var avgGrowthRate = growthRates.Any() ? growthRates.Average() : 0;

        var result = new TrendAnalysisDto
        {
            Category = "Overall",
            DataPoints = periodTotals,
            TrendDirection = trendDirection,
            AverageGrowthRate = avgGrowthRate,
            StartPeriod = periods.First(),
            EndPeriod = periods.Last(),
            TotalChange = periodTotals.Last().Value - periodTotals.First().Value,
            PercentageChange = periodTotals.First().Value != 0
                ? ((periodTotals.Last().Value - periodTotals.First().Value) / periodTotals.First().Value) * 100
                : 0
        };

        result.Insights = GenerateTrendInsights(result);

        return result;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error analyzing trends");
        throw;
    }
}

public async Task<List<TrendDataPoint>> GetTrendDataAsync(string category, List<string> periods)
{
    var dataPoints = new List<TrendDataPoint>();

    foreach (var period in periods.OrderBy(p => p))
    {
        var periodData = await _dataRepo.GetByPeriodAsync(period);
        var categoryData = periodData.Where(d => d.Category.Equals(category, StringComparison.OrdinalIgnoreCase));

        var total = categoryData.Sum(d => d.Amount);

        dataPoints.Add(new TrendDataPoint
        {
            Period = period,
            Value = total
        });
    }

    for (int i = 1; i < dataPoints.Count; i++)
    {
        var previous = dataPoints[i - 1].Value;
        var current = dataPoints[i].Value;

        if (previous != 0)
        {
            dataPoints[i].PercentageChange = ((current - previous) / previous) * 100;
        }
    }

    return dataPoints;
}

public async Task<decimal> CalculateGrowthRateAsync(string category, string startPeriod, string endPeriod)
{
    var startData = await _dataRepo.GetByPeriodAsync(startPeriod);
    var endData = await _dataRepo.GetByPeriodAsync(endPeriod);

    var startValue = startData
        .Where(d => d.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
        .Sum(d => d.Amount);

    var endValue = endData
        .Where(d => d.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
        .Sum(d => d.Amount);

    if (startValue == 0)
        return 0;

    return ((endValue - startValue) / startValue) * 100;
}

private string DetermineTrendDirection(List<TrendDataPoint> dataPoints)
{
    if (dataPoints.Count < 2)
        return "Insufficient Data";

    var changes = dataPoints
        .Where(p => p.PercentageChange.HasValue)
        .Select(p => p.PercentageChange.Value)
        .ToList();

    if (!changes.Any())
        return "Stable";

    var avgChange = changes.Average();

    if (avgChange > 5)
        return "Increasing";
    else if (avgChange < -5)
        return "Decreasing";
    else
        return "Stable";
}

private List<string> GenerateTrendInsights(TrendAnalysisDto trend)
{
    var insights = new List<string>();

    insights.Add($"Overall trend is {trend.TrendDirection.ToLower()} with average growth rate of {trend.AverageGrowthRate:F2}%");

    if (trend.PercentageChange > 0)
    {
        insights.Add($"Total increase of {trend.PercentageChange:F2}% from {trend.StartPeriod} to {trend.EndPeriod}");
    }
    else if (trend.PercentageChange < 0)
    {
        insights.Add($"Total decrease of {Math.Abs(trend.PercentageChange):F2}% from {trend.StartPeriod} to {trend.EndPeriod}");
    }

    if (trend.DataPoints.Any(p => p.PercentageChange.HasValue))
    {
        var volatility = trend.DataPoints
            .Where(p => p.PercentageChange.HasValue)
            .Select(p => Math.Abs(p.PercentageChange.Value))
            .Average();

        if (volatility > 20)
        {
            insights.Add($"High volatility detected with average absolute change of {volatility:F2}%");
        }
    }

    return insights;
}

#endregion

#region Comparison Methods

public async Task<ComparisonResultDto> ComparePeriodsAsync(string period1, string period2)
{
    try
    {
        _logger.LogInformation("Comparing periods: {Period1} vs {Period2}", period1, period2);

        var data1 = await _dataRepo.GetByPeriodAsync(period1);
        var data2 = await _dataRepo.GetByPeriodAsync(period2);

        if (!data1.Any() || !data2.Any())
        {
            throw new InvalidOperationException("Insufficient data for comparison");
        }

        var result = new ComparisonResultDto
        {
            Period1 = period1,
            Period2 = period2,
            ComparisonDate = DateTime.UtcNow
        };

        var categories = data1.Select(d => d.Category)
            .Union(data2.Select(d => d.Category))
            .Distinct();

        foreach (var category in categories)
        {
            var value1 = data1.Where(d => d.Category == category).Sum(d => d.Amount);
            var value2 = data2.Where(d => d.Category == category).Sum(d => d.Amount);

            var variance = value2 - value1;
            var percentageChange = value1 != 0 ? (variance / value1) * 100 : 0;

            var metric = new ComparisonMetric
            {
                Category = category,
                Value1 = value1,
                Value2 = value2,
                Variance = variance,
                PercentageChange = percentageChange,
                ChangeType = variance > 0 ? "Increase" : variance < 0 ? "Decrease" : "NoChange"
            };

            result.Metrics[category] = metric;

            if (Math.Abs(percentageChange) > 10)
            {
                result.SignificantChanges.Add($"{category}: {percentageChange:F2}% change ({metric.ChangeType})");
            }
        }

        var totalChange1 = data1.Sum(d => d.Amount);
        var totalChange2 = data2.Sum(d => d.Amount);

        if (totalChange2 > totalChange1)
            result.OverallTrend = "Growth";
        else if (totalChange2 < totalChange1)
            result.OverallTrend = "Decline";
        else
            result.OverallTrend = "Stable";

        return result;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error comparing periods");
        throw;
    }
}

public async Task<ComparisonResultDto> CompareDocumentsAsync(int documentId1, int documentId2)
{
    var doc1 = await _documentRepo.GetWithDataAsync(documentId1);
    var doc2 = await _documentRepo.GetWithDataAsync(documentId2);

    if (doc1 == null || doc2 == null)
    {
        throw new ArgumentException("One or both documents not found");
    }

    var period1 = doc1.FinancialDataRecords.FirstOrDefault()?.Period ?? "Document 1";
    var period2 = doc2.FinancialDataRecords.FirstOrDefault()?.Period ?? "Document 2";

    return await ComparePeriodsAsync(period1, period2);
}

public async Task<Dictionary<string, decimal>> GetVarianceAnalysisAsync(int documentId1, int documentId2)
{
    var comparison = await CompareDocumentsAsync(documentId1, documentId2);

    return comparison.Metrics.ToDictionary(
        m => m.Key,
        m => m.Value.Variance
    );
}

#endregion

// TO BE CONTINUED IN NEXT FILE...


// PART 3 OF FINANCIALSERVICE - FORECASTING, AI, AND ANOMALY DETECTION
// Copy these into FinancialService.cs after Part 2

#region Forecasting Methods

public async Task<ForecastResultDto> GenerateForecastAsync(string category, int periodsAhead)
{
    try
    {
        _logger.LogInformation("Generating forecast for {Category}, {PeriodsAhead} periods ahead", category, periodsAhead);

        var allData = await _dataRepo.GetAllAsync();
        var categoryData = allData
            .Where(d => d.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
            .GroupBy(d => d.Period)
            .OrderBy(g => g.Key)
            .Select(g => g.Sum(d => d.Amount))
            .ToList();

        if (categoryData.Count < 3)
        {
            throw new InvalidOperationException("Insufficient historical data for forecasting (need at least 3 periods)");
        }

        var forecasted = ForecastLinear(categoryData, periodsAhead);

        var result = new ForecastResultDto
        {
            Category = category,
            Method = "Linear Regression",
            ConfidenceLevel = 70,
            GeneratedDate = DateTime.UtcNow
        };

        var lastPeriod = allData.Max(d => d.Period);
        for (int i = 1; i <= periodsAhead; i++)
        {
            var forecastedValue = forecasted[i - 1];
            var margin = forecastedValue * 0.1m;

            result.ForecastedValues.Add(new ForecastDataPoint
            {
                Period = $"{lastPeriod}+{i}",
                ForecastedValue = forecastedValue,
                LowerBound = forecastedValue - margin,
                UpperBound = forecastedValue + margin
            });
        }

        result.Assumptions.Add("Based on linear trend analysis of historical data");
        result.Assumptions.Add("Assumes consistent market conditions");

        result.RiskFactors.Add("Market volatility could impact actual values");
        result.RiskFactors.Add("External economic factors not considered");

        return result;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error generating forecast");
        throw;
    }
}

public async Task<ForecastResultDto> GenerateSimpleForecastAsync(List<decimal> historicalValues, int periodsAhead)
{
    var forecasted = ForecastLinear(historicalValues, periodsAhead);

    var result = new ForecastResultDto
    {
        Category = "General",
        Method = "Simple Linear Forecast",
        ConfidenceLevel = 65,
        GeneratedDate = DateTime.UtcNow
    };

    for (int i = 1; i <= periodsAhead; i++)
    {
        var value = forecasted[i - 1];
        var margin = value * 0.15m;

        result.ForecastedValues.Add(new ForecastDataPoint
        {
            Period = $"Period {i}",
            ForecastedValue = value,
            LowerBound = value - margin,
            UpperBound = value + margin
        });
    }

    return result;
}

public async Task<List<decimal>> CalculateMovingAverageAsync(List<decimal> values, int period)
{
    var movingAverages = new List<decimal>();

    if (values.Count < period)
    {
        return movingAverages;
    }

    for (int i = period - 1; i < values.Count; i++)
    {
        var sum = values.Skip(i - period + 1).Take(period).Sum();
        movingAverages.Add(sum / period);
    }

    return movingAverages;
}

private List<decimal> ForecastLinear(List<decimal> historicalValues, int periodsAhead)
{
    var n = historicalValues.Count;

    var sumX = 0m;
    var sumY = 0m;
    var sumXY = 0m;
    var sumX2 = 0m;

    for (int i = 0; i < n; i++)
    {
        var x = i + 1;
        var y = historicalValues[i];

        sumX += x;
        sumY += y;
        sumXY += x * y;
        sumX2 += x * x;
    }

    var slope = (n * sumXY - sumX * sumY) / (n * sumX2 - sumX * sumX);
    var intercept = (sumY - slope * sumX) / n;

    var forecasts = new List<decimal>();
    for (int i = 1; i <= periodsAhead; i++)
    {
        var x = n + i;
        var forecast = slope * x + intercept;
        forecasts.Add(Math.Max(0, forecast));
    }

    return forecasts;
}

#endregion

#region AI-Enhanced Analysis Methods

public async Task<AnalysisResponseDto> GenerateAIInsightsAsync(int documentId, AnalysisType analysisType)
{
    try
    {
        _logger.LogInformation("Generating AI insights for document {DocumentId}, type: {AnalysisType}", documentId, analysisType);

        var stopwatch = Stopwatch.StartNew();

        var document = await _documentRepo.GetWithDataAsync(documentId);
        if (document == null)
        {
            throw new ArgumentException($"Document {documentId} not found");
        }

        var data = document.FinancialDataRecords.ToList();

        string aiResponse = analysisType switch
        {
            AnalysisType.Summary => await _aiService.GenerateSummaryAsync(data),
            AnalysisType.TrendAnalysis => await _aiService.AnalyzeTrendsAsync(data, data.FirstOrDefault()?.Period ?? ""),
            AnalysisType.AnomalyDetection => await _aiService.DetectAnomaliesAsync(data),
            _ => await _aiService.GenerateSummaryAsync(data)
        };

        stopwatch.Stop();

        var response = new AnalysisResponseDto
        {
            DocumentId = documentId,
            AnalysisType = analysisType,
            DetailedAnalysis = aiResponse,
            ExecutionTime = (int)stopwatch.ElapsedMilliseconds,
            GeneratedDate = DateTime.UtcNow,
            ModelUsed = "Llama 3.2"
        };

        response.KeyFindings = AIResponseParser.ExtractKeyFindings(aiResponse);
        response.Recommendations = AIResponseParser.ExtractRecommendations(aiResponse);
        response.Summary = aiResponse.Length > 500
            ? aiResponse.Substring(0, 500) + "..."
            : aiResponse;

        // Save analysis to database
        var analysis = new AIAnalysis
        {
            DocumentId = documentId,
            AnalysisType = analysisType.ToString(),
            Prompt = $"Generated {analysisType} analysis",
            Response = aiResponse,
            ModelUsed = "Llama 3.2",
            ExecutionTime = (int)stopwatch.ElapsedMilliseconds,
            CreatedDate = DateTime.UtcNow
        };

        await _analysisRepo.AddAsync(analysis);
        await _analysisRepo.SaveChangesAsync();

        return response;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error generating AI insights");
        throw;
    }
}

public async Task<AnalysisResponseDto> GenerateCustomAnalysisAsync(int documentId, string question)
{
    try
    {
        _logger.LogInformation("Generating custom AI analysis for document {DocumentId}", documentId);

        var stopwatch = Stopwatch.StartNew();

        var document = await _documentRepo.GetWithDataAsync(documentId);
        if (document == null)
        {
            throw new ArgumentException($"Document {documentId} not found");
        }

        var data = document.FinancialDataRecords.ToList();

        var aiResponse = await _aiService.CustomAnalysisAsync(question, data);

        stopwatch.Stop();

        var response = new AnalysisResponseDto
        {
            DocumentId = documentId,
            AnalysisType = AnalysisType.Custom,
            DetailedAnalysis = aiResponse,
            Summary = question,
            ExecutionTime = (int)stopwatch.ElapsedMilliseconds,
            GeneratedDate = DateTime.UtcNow,
            ModelUsed = "Llama 3.2"
        };

        response.KeyFindings = AIResponseParser.ExtractKeyFindings(aiResponse);
        response.Recommendations = AIResponseParser.ExtractRecommendations(aiResponse);

        return response;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error generating custom AI analysis");
        throw;
    }
}

public async Task<string> GenerateFinancialNarrativeAsync(int documentId)
{
    var summary = await GetFinancialSummaryAsync(documentId);
    var ratios = await CalculateFinancialRatiosAsync(documentId);

    var prompt = $@"Generate a professional financial narrative based on the following data:

Summary:
- Total Revenue: {summary.TotalRevenue:C}
- Total Expenses: {summary.TotalExpenses:C}
- Net Income: {summary.NetIncome:C}
- Total Assets: {summary.TotalAssets:C}
- Total Liabilities: {summary.TotalLiabilities:C}

Key Ratios:
{string.Join("\n", ratios.Select(r => $"- {r.Key}: {r.Value:F2}"))}

Provide a concise 2-3 paragraph narrative explaining the financial position.";

    return await _aiService.GenerateResponseAsync(prompt);
}

#endregion

#region Anomaly Detection Methods

public async Task<List<AnomalyDto>> DetectAnomaliesAsync(int documentId)
{
    try
    {
        _logger.LogInformation("Detecting anomalies in document {DocumentId}", documentId);

        var document = await _documentRepo.GetWithDataAsync(documentId);
        if (document == null)
        {
            throw new ArgumentException($"Document {documentId} not found");
        }

        var data = document.FinancialDataRecords.ToList();

        return await DetectOutliersAsync(data);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error detecting anomalies");
        throw;
    }
}

public async Task<List<AnomalyDto>> DetectOutliersAsync(List<FinancialData> data)
{
    var anomalies = new List<AnomalyDto>();

    if (!data.Any())
        return anomalies;

    var categoryGroups = data.GroupBy(d => d.Category);

    foreach (var group in categoryGroups)
    {
        var values = group.Select(d => d.Amount).ToList();

        if (values.Count < 3)
            continue;

        var mean = values.Average();
        var stdDev = CalculateStandardDeviation(values.Select(v => (decimal)v).ToList());

        foreach (var record in group)
        {
            var deviation = Math.Abs((double)record.Amount - (double)mean);
            var deviationInStdDev = stdDev > 0 ? deviation / stdDev : 0;

            if (deviationInStdDev > 3)
            {
                var severity = deviationInStdDev > 5 ? "High" : deviationInStdDev > 4 ? "Medium" : "Low";

                anomalies.Add(new AnomalyDto
                {
                    RecordId = record.Id,
                    AccountName = record.AccountName,
                    Period = record.Period,
                    Value = record.Amount,
                    ExpectedValue = (decimal)mean,
                    Deviation = (decimal)deviation,
                    DeviationPercentage = mean != 0 ? (decimal)(deviation / (double)mean * 100) : 0,
                    Severity = severity,
                    Reason = $"Value is {deviationInStdDev:F2} standard deviations from category mean",
                    DetectedDate = DateTime.UtcNow
                });
            }
        }
    }

    return anomalies;
}

public async Task<bool> IsAnomalousValueAsync(decimal value, string category, string period)
{
    var data = await _dataRepo.GetByCategoryAsync(category);
    var periodData = data.Where(d => d.Period == period).ToList();

    if (!periodData.Any())
        return false;

    var values = periodData.Select(d => d.Amount).ToList();
    var mean = values.Average();
    var stdDev = CalculateStandardDeviation(values);

    var deviation = Math.Abs((double)value - (double)mean);
    var deviationInStdDev = stdDev > 0 ? deviation / stdDev : 0;

    return deviationInStdDev > 3;
}

private double CalculateStandardDeviation(List<decimal> values)
{
    if (!values.Any())
        return 0;

    var avg = values.Average();
    var sumOfSquaresOfDifferences = values
        .Select(val => (double)(val - avg) * (double)(val - avg))
        .Sum();

    return Math.Sqrt(sumOfSquaresOfDifferences / values.Count);
}

#endregion

// ===== TREND ANALYSIS METHODS (Phase 10) =====

public async Task<TrendChartDataDto> GetRevenueTrendAsync(int documentId, int months = 12)
{
    // Get the selected document to find its period
    var selectedDocument = await _documentRepo.GetWithDataAsync(documentId);
    if (selectedDocument == null) throw new ArgumentException($"Document {documentId} not found");
    
    // Get the period of the selected document (e.g., "2023-06")
    var selectedPeriod = selectedDocument.FinancialDataRecords.FirstOrDefault()?.Period;
    
    // Get ALL documents' data to show trends across time
    var allDocuments = await _documentRepo.GetAllAsync();
    var allData = new List<FinancialData>();
    
    foreach (var doc in allDocuments)
    {
        var docWithData = await _documentRepo.GetWithDataAsync(doc.Id);
        if (docWithData?.FinancialDataRecords != null)
        {
            allData.AddRange(docWithData.FinancialDataRecords);
        }
    }
    
    // Filter to show only periods UP TO the selected document's period
    var revenueData = allData
        .Where(d => d.Category == "Revenue")
        .Where(d => string.IsNullOrEmpty(selectedPeriod) || string.Compare(d.Period, selectedPeriod) <= 0) // Only periods <= selected
        .GroupBy(d => d.Period)
        .OrderBy(g => g.Key)
        .TakeLast(months) // Take last N months up to selected period
        .Select(g => new ChartDataPointDto
        {
            Label = g.Key,
            Value = g.Sum(d => d.Amount),
            Category = "Revenue",
            Date = DateTime.TryParse(g.Key + "-01", out var date) ? date : DateTime.Now,
            Color = "#4CAF50" // Green
        })
        .ToList();
    
    if (!revenueData.Any())
    {
        return new TrendChartDataDto
        {
            Title = "Revenue Trend",
            DataPoints = new List<ChartDataPointDto>()
        };
    }
    
    return new TrendChartDataDto
    {
        DataPoints = revenueData,
        Title = "Revenue Trend",
        GrowthRate = CalculateGrowthRate(revenueData),
        Average = revenueData.Average(d => d.Value),
        Minimum = revenueData.Min(d => d.Value),
        Maximum = revenueData.Max(d => d.Value)
    };
}

public async Task<TrendChartDataDto> GetExpenseTrendAsync(int documentId, int months = 12)
{
    // Get the selected document to find its period
    var selectedDocument = await _documentRepo.GetWithDataAsync(documentId);
    if (selectedDocument == null) throw new ArgumentException($"Document {documentId} not found");
    
    var selectedPeriod = selectedDocument.FinancialDataRecords.FirstOrDefault()?.Period;
    
    // Get ALL documents' data to show trends across time
    var allDocuments = await _documentRepo.GetAllAsync();
    var allData = new List<FinancialData>();
    
    foreach (var doc in allDocuments)
    {
        var docWithData = await _documentRepo.GetWithDataAsync(doc.Id);
        if (docWithData?.FinancialDataRecords != null)
        {
            allData.AddRange(docWithData.FinancialDataRecords);
        }
    }
    
    // Filter to show only periods UP TO the selected document's period
    var expenseData = allData
        .Where(d => d.Category == "Expense")
        .Where(d => string.IsNullOrEmpty(selectedPeriod) || string.Compare(d.Period, selectedPeriod) <= 0)
        .GroupBy(d => d.Period)
        .OrderBy(g => g.Key)
        .TakeLast(months) // Take last N months up to selected period
        .Select(g => new ChartDataPointDto
        {
            Label = g.Key,
            Value = g.Sum(d => d.Amount),
            Category = "Expense",
            Date = DateTime.TryParse(g.Key + "-01", out var date) ? date : DateTime.Now,
            Color = "#F44336" // Red
        })
        .ToList();
    
    if (!expenseData.Any())
    {
        return new TrendChartDataDto
        {
            Title = "Expense Trend",
            DataPoints = new List<ChartDataPointDto>()
        };
    }
    
    return new TrendChartDataDto
    {
        DataPoints = expenseData,
        Title = "Expense Trend",
        GrowthRate = CalculateGrowthRate(expenseData),
        Average = expenseData.Average(d => d.Value),
        Minimum = expenseData.Min(d => d.Value),
        Maximum = expenseData.Max(d => d.Value)
    };
}

public async Task<TrendChartDataDto> GetNetIncomeTrendAsync(int documentId, int months = 12)
{
    // Get the selected document to find its period
    var selectedDocument = await _documentRepo.GetWithDataAsync(documentId);
    if (selectedDocument == null) throw new ArgumentException($"Document {documentId} not found");
    
    var selectedPeriod = selectedDocument.FinancialDataRecords.FirstOrDefault()?.Period;
    
    // Get ALL documents' data to show trends across time
    var allDocuments = await _documentRepo.GetAllAsync();
    var allData = new List<FinancialData>();
    
    foreach (var doc in allDocuments)
    {
        var docWithData = await _documentRepo.GetWithDataAsync(doc.Id);
        if (docWithData?.FinancialDataRecords != null)
        {
            allData.AddRange(docWithData.FinancialDataRecords);
        }
    }
    
    // Filter to show only periods UP TO the selected document's period
    var periodData = allData
        .Where(d => string.IsNullOrEmpty(selectedPeriod) || string.Compare(d.Period, selectedPeriod) <= 0)
        .GroupBy(d => d.Period)
        .OrderBy(g => g.Key)
        .TakeLast(months) // Take last N months up to selected period
        .Select(g => new
        {
            Period = g.Key,
            Revenue = g.Where(d => d.Category == "Revenue").Sum(d => d.Amount),
            Expenses = g.Where(d => d.Category == "Expense").Sum(d => d.Amount)
        })
        .Select(p => new ChartDataPointDto
        {
            Label = p.Period,
            Value = p.Revenue - p.Expenses,
            Category = "Net Income",
            Date = DateTime.TryParse(p.Period + "-01", out var date) ? date : DateTime.Now,
            Color = p.Revenue - p.Expenses > 0 ? "#4CAF50" : "#F44336"
        })
        .ToList();
    
    if (!periodData.Any())
    {
        return new TrendChartDataDto
        {
            Title = "Net Income Trend",
            DataPoints = new List<ChartDataPointDto>()
        };
    }
    
    return new TrendChartDataDto
    {
        DataPoints = periodData,
        Title = "Net Income Trend",
        GrowthRate = CalculateGrowthRate(periodData),
        Average = periodData.Average(d => d.Value),
        Minimum = periodData.Min(d => d.Value),
        Maximum = periodData.Max(d => d.Value)
    };
}

public async Task<Dictionary<string, decimal>> GetCategoryBreakdownAsync(int documentId)
{
    var document = await _documentRepo.GetWithDataAsync(documentId);
    if (document == null) throw new ArgumentException($"Document {documentId} not found");
    
    return document.FinancialDataRecords
        .Where(d => d.Category == "Expense" && !string.IsNullOrEmpty(d.SubCategory))
        .GroupBy(d => d.SubCategory)
        .ToDictionary(
            g => g.Key,
            g => g.Sum(d => d.Amount)
        );
}

public async Task<List<PeriodComparisonDto>> GetPeriodComparisonsAsync(int documentId)
{
    // Get the selected document to find its period
    var selectedDocument = await _documentRepo.GetWithDataAsync(documentId);
    if (selectedDocument == null) throw new ArgumentException($"Document {documentId} not found");
    
    var selectedPeriod = selectedDocument.FinancialDataRecords.FirstOrDefault()?.Period;
    
    // Get ALL documents' data to show trends across time
    var allDocuments = await _documentRepo.GetAllAsync();
    var allData = new List<FinancialData>();
    
    foreach (var doc in allDocuments)
    {
        var docWithData = await _documentRepo.GetWithDataAsync(doc.Id);
        if (docWithData?.FinancialDataRecords != null)
        {
            allData.AddRange(docWithData.FinancialDataRecords);
        }
    }
    
    // Filter to show only periods UP TO the selected document's period
    var periodTotals = allData
        .Where(d => string.IsNullOrEmpty(selectedPeriod) || string.Compare(d.Period, selectedPeriod) <= 0)
        .GroupBy(d => d.Period)
        .OrderBy(g => g.Key)
        .Select(g => new
        {
            Period = g.Key,
            Revenue = g.Where(d => d.Category == "Revenue").Sum(d => d.Amount),
            Expenses = g.Where(d => d.Category == "Expense").Sum(d => d.Amount)
        })
        .ToList();
    
    var comparisons = new List<PeriodComparisonDto>();
    
    for (int i = 1; i < periodTotals.Count; i++)
    {
        var current = periodTotals[i];
        var previous = periodTotals[i - 1];
        
        var netIncomeCurrent = current.Revenue - current.Expenses;
        var netIncomePrevious = previous.Revenue - previous.Expenses;
        var change = netIncomeCurrent - netIncomePrevious;
        var changePercent = netIncomePrevious != 0 ? (change / netIncomePrevious) * 100 : 0;
        
        comparisons.Add(new PeriodComparisonDto
        {
            CurrentPeriod = current.Period,
            PreviousPeriod = previous.Period,
            CurrentValue = netIncomeCurrent,
            PreviousValue = netIncomePrevious,
            Change = change,
            ChangePercentage = changePercent,
            IsImprovement = change > 0
        });
    }
    
    return comparisons;
}

private decimal CalculateGrowthRate(List<ChartDataPointDto> data)
{
    if (data.Count < 2) return 0;
    
    var first = data.First().Value;
    var last = data.Last().Value;
    
    if (first == 0) return 0;
    
    return ((last - first) / first) * 100;
}
}
