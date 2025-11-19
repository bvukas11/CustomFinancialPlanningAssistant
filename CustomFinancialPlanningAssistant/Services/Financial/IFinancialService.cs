using CustomFinancialPlanningAssistant.Core.DTOs;
using CustomFinancialPlanningAssistant.Core.Entities;
using CustomFinancialPlanningAssistant.Core.Enums;

namespace CustomFinancialPlanningAssistant.Services.Financial;

/// <summary>
/// Interface for financial analysis and calculations service
/// </summary>
public interface IFinancialService
{
    // Summary & Overview Methods
    
    /// <summary>
    /// Generates a comprehensive financial summary for a document
    /// </summary>
    /// <param name="documentId">Document identifier</param>
    /// <returns>Financial summary with key metrics</returns>
    Task<FinancialSummaryDto> GetFinancialSummaryAsync(int documentId);

    /// <summary>
    /// Generates a financial summary for a specific period across all documents
    /// </summary>
    /// <param name="period">Period identifier</param>
    /// <returns>Financial summary for the period</returns>
    Task<FinancialSummaryDto> GetFinancialSummaryByPeriodAsync(string period);

    /// <summary>
    /// Gets category-wise summary of amounts
    /// </summary>
    /// <param name="documentId">Document identifier</param>
    /// <returns>Dictionary of category totals</returns>
    Task<Dictionary<string, decimal>> GetCategorySummaryAsync(int documentId);

    /// <summary>
    /// Gets period-wise summary of amounts
    /// </summary>
    /// <param name="documentId">Document identifier</param>
    /// <returns>Dictionary of period totals</returns>
    Task<Dictionary<string, decimal>> GetPeriodSummaryAsync(int documentId);

    // Ratio Analysis Methods
    
    /// <summary>
    /// Calculates all financial ratios for a document
    /// </summary>
    /// <param name="documentId">Document identifier</param>
    /// <returns>Dictionary of calculated ratios</returns>
    Task<Dictionary<string, decimal>> CalculateFinancialRatiosAsync(int documentId);

    /// <summary>
    /// Calculates profitability ratios (Gross Margin, Net Margin, ROA, ROE)
    /// </summary>
    /// <param name="documentId">Document identifier</param>
    /// <returns>Dictionary of profitability ratios</returns>
    Task<Dictionary<string, decimal>> CalculateProfitabilityRatiosAsync(int documentId);

    /// <summary>
    /// Calculates liquidity ratios (Current Ratio, Debt-to-Equity, etc.)
    /// </summary>
    /// <param name="documentId">Document identifier</param>
    /// <returns>Dictionary of liquidity ratios</returns>
    Task<Dictionary<string, decimal>> CalculateLiquidityRatiosAsync(int documentId);

    /// <summary>
    /// Calculates efficiency ratios (Asset Turnover, Operating Expense Ratio)
    /// </summary>
    /// <param name="documentId">Document identifier</param>
    /// <returns>Dictionary of efficiency ratios</returns>
    Task<Dictionary<string, decimal>> CalculateEfficiencyRatiosAsync(int documentId);

    // Trend Analysis Methods
    
    /// <summary>
    /// Analyzes trends across multiple documents
    /// </summary>
    /// <param name="documentIds">List of document identifiers</param>
    /// <returns>Trend analysis results</returns>
    Task<TrendAnalysisDto> AnalyzeTrendsAsync(List<int> documentIds);

    /// <summary>
    /// Analyzes trends for specified periods
    /// </summary>
    /// <param name="periods">List of period identifiers</param>
    /// <returns>Trend analysis results</returns>
    Task<TrendAnalysisDto> AnalyzeTrendsByPeriodAsync(List<string> periods);

    /// <summary>
    /// Gets trend data for a specific category across periods
    /// </summary>
    /// <param name="category">Category name</param>
    /// <param name="periods">List of periods</param>
    /// <returns>List of trend data points</returns>
    Task<List<TrendDataPoint>> GetTrendDataAsync(string category, List<string> periods);

    /// <summary>
    /// Calculates growth rate between two periods
    /// </summary>
    /// <param name="category">Category name</param>
    /// <param name="startPeriod">Starting period</param>
    /// <param name="endPeriod">Ending period</param>
    /// <returns>Growth rate percentage</returns>
    Task<decimal> CalculateGrowthRateAsync(string category, string startPeriod, string endPeriod);

    // Comparison Methods
    
    /// <summary>
    /// Compares financial data between two periods
    /// </summary>
    /// <param name="period1">First period</param>
    /// <param name="period2">Second period</param>
    /// <returns>Comparison results</returns>
    Task<ComparisonResultDto> ComparePeriodsAsync(string period1, string period2);

    /// <summary>
    /// Compares two financial documents
    /// </summary>
    /// <param name="documentId1">First document ID</param>
    /// <param name="documentId2">Second document ID</param>
    /// <returns>Comparison results</returns>
    Task<ComparisonResultDto> CompareDocumentsAsync(int documentId1, int documentId2);

    /// <summary>
    /// Performs variance analysis between two documents
    /// </summary>
    /// <param name="documentId1">First document ID</param>
    /// <param name="documentId2">Second document ID</param>
    /// <returns>Dictionary of variances by category</returns>
    Task<Dictionary<string, decimal>> GetVarianceAnalysisAsync(int documentId1, int documentId2);

    // Forecasting Methods
    
    /// <summary>
    /// Generates forecast for a category
    /// </summary>
    /// <param name="category">Category to forecast</param>
    /// <param name="periodsAhead">Number of periods to forecast</param>
    /// <returns>Forecast results</returns>
    Task<ForecastResultDto> GenerateForecastAsync(string category, int periodsAhead);

    /// <summary>
    /// Generates simple forecast from historical values
    /// </summary>
    /// <param name="historicalValues">List of historical values</param>
    /// <param name="periodsAhead">Number of periods to forecast</param>
    /// <returns>Forecast results</returns>
    Task<ForecastResultDto> GenerateSimpleForecastAsync(List<decimal> historicalValues, int periodsAhead);

    /// <summary>
    /// Calculates moving average of values
    /// </summary>
    /// <param name="values">List of values</param>
    /// <param name="period">Moving average period</param>
    /// <returns>List of moving average values</returns>
    Task<List<decimal>> CalculateMovingAverageAsync(List<decimal> values, int period);

    // AI-Enhanced Analysis Methods
    
    /// <summary>
    /// Generates AI-powered insights for a document
    /// </summary>
    /// <param name="documentId">Document identifier</param>
    /// <param name="analysisType">Type of analysis to perform</param>
    /// <returns>AI analysis response</returns>
    Task<AnalysisResponseDto> GenerateAIInsightsAsync(int documentId, AnalysisType analysisType);

    /// <summary>
    /// Generates custom AI analysis based on a question
    /// </summary>
    /// <param name="documentId">Document identifier</param>
    /// <param name="question">Custom question</param>
    /// <returns>AI analysis response</returns>
    Task<AnalysisResponseDto> GenerateCustomAnalysisAsync(int documentId, string question);

    /// <summary>
    /// Generates a narrative summary of financial position
    /// </summary>
    /// <param name="documentId">Document identifier</param>
    /// <returns>Financial narrative text</returns>
    Task<string> GenerateFinancialNarrativeAsync(int documentId);

    // Anomaly Detection Methods
    
    /// <summary>
    /// Detects anomalies in a document's financial data
    /// </summary>
    /// <param name="documentId">Document identifier</param>
    /// <returns>List of detected anomalies</returns>
    Task<List<AnomalyDto>> DetectAnomaliesAsync(int documentId);

    /// <summary>
    /// Detects outliers in a list of financial data
    /// </summary>
    /// <param name="data">Financial data to analyze</param>
    /// <returns>List of detected anomalies</returns>
    Task<List<AnomalyDto>> DetectOutliersAsync(List<FinancialData> data);

    /// <summary>
    /// Checks if a value is anomalous for its category and period
    /// </summary>
    /// <param name="value">Value to check</param>
    /// <param name="category">Category name</param>
    /// <param name="period">Period identifier</param>
    /// <returns>True if anomalous</returns>
    Task<bool> IsAnomalousValueAsync(decimal value, string category, string period);

    // ===== TREND ANALYSIS METHODS (Phase 10) =====
    
    /// <summary>
    /// Gets revenue trend data for the specified number of months
    /// </summary>
    Task<TrendChartDataDto> GetRevenueTrendAsync(int documentId, int months = 12);
    
    /// <summary>
    /// Gets expense trend data for the specified number of months
    /// </summary>
    Task<TrendChartDataDto> GetExpenseTrendAsync(int documentId, int months = 12);
    
    /// <summary>
    /// Gets net income trend data for the specified number of months
    /// </summary>
    Task<TrendChartDataDto> GetNetIncomeTrendAsync(int documentId, int months = 12);
    
    /// <summary>
    /// Gets expense breakdown by category
    /// </summary>
    Task<Dictionary<string, decimal>> GetCategoryBreakdownAsync(int documentId);
    
    /// <summary>
    /// Gets period-over-period comparisons
    /// </summary>
    Task<List<PeriodComparisonDto>> GetPeriodComparisonsAsync(int documentId);
}
