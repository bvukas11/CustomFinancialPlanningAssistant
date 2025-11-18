using CustomFinancialPlanningAssistant.Core.Entities;

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
}
