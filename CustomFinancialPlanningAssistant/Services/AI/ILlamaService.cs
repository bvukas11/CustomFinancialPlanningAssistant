using CustomFinancialPlanningAssistant.Core.Entities;
using CustomFinancialPlanningAssistant.Core.DTOs;

namespace CustomFinancialPlanningAssistant.Services.AI;

/// <summary>
/// Interface for AI-powered financial analysis using Llama models via Ollama
/// </summary>
public interface ILlamaService
{
    // ========== Core AI Operations ==========

    /// <summary>
    /// Generates an AI response for the given prompt using the default model
    /// </summary>
    /// <param name="prompt">The prompt to send to the AI</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>AI-generated response text</returns>
    Task<string> GenerateResponseAsync(string prompt, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates an AI response for the given prompt using a specific model
    /// </summary>
    /// <param name="prompt">The prompt to send to the AI</param>
    /// <param name="model">The specific model to use</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>AI-generated response text</returns>
    Task<string> GenerateResponseAsync(string prompt, string model, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if the Ollama service is available and running
    /// </summary>
    /// <returns>True if service is available, false otherwise</returns>
    Task<bool> IsServiceAvailableAsync();

    /// <summary>
    /// Checks if a specific model is available locally
    /// </summary>
    /// <param name="modelName">Name of the model to check</param>
    /// <returns>True if model is available, false otherwise</returns>
    Task<bool> IsModelAvailableAsync(string modelName);

    /// <summary>
    /// Gets a list of all available models
    /// </summary>
    /// <returns>List of model names</returns>
    Task<List<string>> GetAvailableModelsAsync();

    // ========== Financial Analysis Operations ==========

    /// <summary>
    /// Generates a comprehensive financial summary
    /// </summary>
    /// <param name="data">Financial data to analyze</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>AI-generated summary analysis</returns>
    Task<string> GenerateSummaryAsync(List<FinancialData> data, CancellationToken cancellationToken = default);

    /// <summary>
    /// Analyzes financial trends over time
    /// </summary>
    /// <param name="data">Financial data to analyze</param>
    /// <param name="period">Period identifier for analysis</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>AI-generated trend analysis</returns>
    Task<string> AnalyzeTrendsAsync(List<FinancialData> data, string period, CancellationToken cancellationToken = default);

    /// <summary>
    /// Detects anomalies and outliers in financial data
    /// </summary>
    /// <param name="data">Financial data to analyze</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>AI-generated anomaly detection report</returns>
    Task<string> DetectAnomaliesAsync(List<FinancialData> data, CancellationToken cancellationToken = default);

    /// <summary>
    /// Analyzes financial ratios and their implications
    /// </summary>
    /// <param name="ratios">Dictionary of ratio names and values</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>AI-generated ratio analysis</returns>
    Task<string> AnalyzeRatiosAsync(Dictionary<string, decimal> ratios, CancellationToken cancellationToken = default);

    /// <summary>
    /// Compares financial performance between two periods
    /// </summary>
    /// <param name="current">Current period data</param>
    /// <param name="previous">Previous period data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>AI-generated comparison analysis</returns>
    Task<string> ComparePeriodsAsync(List<FinancialData> current, List<FinancialData> previous, CancellationToken cancellationToken = default);

    /// <summary>
    /// Analyzes cash flow and liquidity
    /// </summary>
    /// <param name="data">Financial data with focus on cash accounts</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>AI-generated cash flow analysis</returns>
    Task<string> AnalyzeCashFlowAsync(List<FinancialData> data, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates financial forecasts based on historical data
    /// </summary>
    /// <param name="historicalData">Historical financial data</param>
    /// <param name="periodsAhead">Number of periods to forecast</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>AI-generated forecast</returns>
    Task<string> GenerateForecastAsync(List<FinancialData> historicalData, int periodsAhead, CancellationToken cancellationToken = default);

    /// <summary>
    /// Performs custom analysis based on user's specific question
    /// </summary>
    /// <param name="question">User's question about the data</param>
    /// <param name="data">Relevant financial data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>AI-generated answer to the question</returns>
    Task<string> CustomAnalysisAsync(string question, List<FinancialData> data, CancellationToken cancellationToken = default);

    // ========== Vision/Document Analysis Operations ==========

    /// <summary>
    /// Analyzes a document image using vision model
    /// </summary>
    /// <param name="imageData">Image data as byte array</param>
    /// <param name="question">Question about the image content</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>AI-generated analysis of the image</returns>
    Task<string> AnalyzeDocumentImageAsync(byte[] imageData, string question, CancellationToken cancellationToken = default);

    /// <summary>
    /// Extracts structured data from a document image
    /// </summary>
    /// <param name="imageData">Image data as byte array</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Extracted data as text</returns>
    Task<string> ExtractDataFromImageAsync(byte[] imageData, CancellationToken cancellationToken = default);

    // ========== PHASE 11: Enhanced AI Insights ==========

    /// <summary>
    /// Generates comprehensive AI insights with health scoring and risk assessment
    /// </summary>
    /// <param name="documentId">Document to analyze</param>
    /// <param name="analysisType">Type of analysis (HealthCheck, RiskAnalysis, Optimization, Growth)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Comprehensive AI insights</returns>
    Task<AIInsightDto> GenerateComprehensiveInsightsAsync(int documentId, string analysisType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Assesses financial health with scoring breakdown
    /// </summary>
    /// <param name="documentId">Document to analyze</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Financial health assessment</returns>
    Task<FinancialHealthDto> AssessFinancialHealthAsync(int documentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Performs comprehensive risk assessment
    /// </summary>
    /// <param name="documentId">Document to analyze</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Risk assessment with mitigation strategies</returns>
    Task<RiskAssessmentDto> AssessRisksAsync(int documentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates optimization suggestions for financial improvement
    /// </summary>
    /// <param name="documentId">Document to analyze</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of optimization suggestions</returns>
    Task<List<string>> GenerateOptimizationSuggestionsAsync(int documentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates growth strategies for revenue and profitability
    /// </summary>
    /// <param name="documentId">Document to analyze</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of growth strategies</returns>
    Task<List<string>> GenerateGrowthStrategiesAsync(int documentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Answers a custom question about the financial data
    /// </summary>
    /// <param name="documentId">Document to query</param>
    /// <param name="question">User's question</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>AI-generated answer</returns>
    Task<string> AnswerCustomQuestionAsync(int documentId, string question, CancellationToken cancellationToken = default);

    /// <summary>
    /// Answers question with conversation history context
    /// </summary>
    /// <param name="documentId">Document to query</param>
    /// <param name="question">Current question</param>
    /// <param name="conversationHistory">Previous Q&A pairs</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Context-aware answer</returns>
    Task<string> AnswerWithContextAsync(int documentId, string question, List<string> conversationHistory, CancellationToken cancellationToken = default);

    // ========== PHASE 12: Industry Benchmarking & Competitive Analysis ==========

    /// <summary>
    /// Performs comprehensive industry benchmarking and competitive analysis
    /// </summary>
    /// <param name="documentId">Document to analyze</param>
    /// <param name="industry">Industry for benchmarking</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Complete competitive analysis with benchmarks and insights</returns>
    Task<CompetitiveAnalysisDto> PerformIndustryBenchmarkingAsync(int documentId, Core.Enums.IndustryType industry, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates personalized investment recommendations
    /// </summary>
    /// <param name="documentId">Document to analyze</param>
    /// <param name="riskTolerance">Investor's risk tolerance level</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Investment recommendation with rationale</returns>
    Task<InvestmentRecommendationDto> GenerateInvestmentAdviceAsync(int documentId, string riskTolerance, CancellationToken cancellationToken = default);

    /// <summary>
    /// Optimizes cash flow management with actionable strategies
    /// </summary>
    /// <param name="documentId">Document to analyze</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Cash flow optimization plan with timeline</returns>
    Task<CashFlowOptimizationDto> OptimizeCashFlowAsync(int documentId, CancellationToken cancellationToken = default);
}
