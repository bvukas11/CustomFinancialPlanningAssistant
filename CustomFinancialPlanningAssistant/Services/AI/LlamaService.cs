using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OllamaSharp;
using OllamaSharp.Models;
using Polly;
using Polly.Retry;
using CustomFinancialPlanningAssistant.Core.Entities;
using CustomFinancialPlanningAssistant.Core.DTOs;
using CustomFinancialPlanningAssistant.Infrastructure.Repositories;
using System.Diagnostics;
using System.Text;

namespace CustomFinancialPlanningAssistant.Services.AI;

/// <summary>
/// Implementation of AI service using Ollama for local Llama model inference
/// Core AI operations and infrastructure
/// </summary>
public partial class LlamaService : ILlamaService
{
    private readonly OllamaApiClient _ollamaClient;
    private readonly AIModelConfiguration _config;
    private readonly ILogger<LlamaService> _logger;
    private readonly AsyncRetryPolicy<string> _retryPolicy;
    
    // Phase 11: Additional dependencies for comprehensive insights
    private readonly IFinancialDocumentRepository _documentRepo;
    private readonly IIndustryBenchmarkRepository _industryBenchmarkRepo;

    /// <summary>
    /// Initializes a new instance of the LlamaService class
    /// </summary>
    public LlamaService(
        IOptions<AIModelConfiguration> config,
        ILogger<LlamaService> logger,
        IFinancialDocumentRepository documentRepo,
        IIndustryBenchmarkRepository industryBenchmarkRepo)
    {
        _config = config?.Value ?? throw new ArgumentNullException(nameof(config));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _documentRepo = documentRepo ?? throw new ArgumentNullException(nameof(documentRepo));
        _industryBenchmarkRepo = industryBenchmarkRepo ?? throw new ArgumentNullException(nameof(industryBenchmarkRepo));

        // Initialize Ollama client
        _ollamaClient = new OllamaApiClient(_config.OllamaBaseUrl);

        // Configure retry policy with exponential backoff
        _retryPolicy = Policy<string>
            .Handle<HttpRequestException>()
            .Or<TimeoutException>()
            .Or<TaskCanceledException>()
            .WaitAndRetryAsync(
                _config.MaxRetries,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt) * _config.RetryDelaySeconds),
                onRetry: (outcome, timespan, retryCount, context) =>
                {
                    _logger.LogWarning(
                        "Retry {RetryCount} after {Delay}s delay due to: {Error}",
                        retryCount,
                        timespan.TotalSeconds,
                        outcome.Exception?.Message ?? "Unknown error");
                });

        _logger.LogInformation("LlamaService initialized with Ollama at {BaseUrl}", _config.OllamaBaseUrl);
    }

    // ========== Core AI Operations ==========

    public async Task<bool> IsServiceAvailableAsync()
    {
        try
        {
            _logger.LogInformation("Checking if Olloma service is available");
            var models = await _ollamaClient.ListLocalModels();
            _logger.LogInformation("Ollama service is available with {Count} models", models.Count());
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ollama service is not available");
            return false;
        }
    }

    public async Task<bool> IsModelAvailableAsync(string modelName)
    {
        try
        {
            _logger.LogInformation("Checking availability of model: {ModelName}", modelName);
            var models = await _ollamaClient.ListLocalModels();
            var isAvailable = models.Any(m => m.Name.Contains(modelName, StringComparison.OrdinalIgnoreCase));
            
            _logger.LogInformation(
                "Model {ModelName} is {Status}",
                modelName,
                isAvailable ? "available" : "not available");
            
            return isAvailable;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking model availability for {ModelName}", modelName);
            return false;
        }
    }

    public async Task<List<string>> GetAvailableModelsAsync()
    {
        try
        {
            _logger.LogInformation("Retrieving list of available models");
            var models = await _ollamaClient.ListLocalModels();
            var modelNames = models.Select(m => m.Name).ToList();
            
            _logger.LogInformation("Found {Count} available models", modelNames.Count);
            return modelNames;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving available models");
            return new List<string>();
        }
    }

    public Task<string> GenerateResponseAsync(string prompt, CancellationToken cancellationToken = default)
    {
        return GenerateResponseAsync(prompt, _config.DefaultTextModel, cancellationToken);
    }

    public async Task<string> GenerateResponseAsync(
        string prompt,
        string model,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(prompt))
        {
            throw new ArgumentException("Prompt cannot be null or empty", nameof(prompt));
        }

        try
        {
            _logger.LogInformation(
                "Generating AI response using model: {Model}, Prompt length: {Length}",
                model,
                prompt.Length);

            var startTime = DateTime.UtcNow;

            var response = await _retryPolicy.ExecuteAsync(async () =>
            {
                using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                cts.CancelAfter(_config.Timeout);

                var request = new GenerateRequest
                {
                    Model = model,
                    Prompt = prompt,
                    Stream = false
                };

                var responseBuilder = new System.Text.StringBuilder();
                
                // Generate returns IAsyncEnumerable, we need to iterate it
                await foreach (var streamResponse in _ollamaClient.Generate(request, cts.Token))
                {
                    if (streamResponse?.Response != null)
                    {
                        responseBuilder.Append(streamResponse.Response);
                    }
                }

                var fullResponse = responseBuilder.ToString();
                if (string.IsNullOrWhiteSpace(fullResponse))
                {
                    throw new InvalidOperationException("Empty response from AI model");
                }

                return fullResponse;
            });

            var duration = DateTime.UtcNow - startTime;
            _logger.LogInformation(
                "AI response generated successfully in {Duration}ms. Response length: {Length}",
                duration.TotalMilliseconds,
                response.Length);

            return response;
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("AI request was cancelled or timed out");
            throw new TimeoutException($"AI request timed out after {_config.TimeoutSeconds} seconds");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating AI response with model {Model}", model);
            throw new InvalidOperationException(
                "Failed to generate AI response. Please check if Ollama is running and the model is available.",
                ex);
        }
    }

    // ========== Financial Analysis Operations ==========

    public async Task<string> GenerateSummaryAsync(
        List<FinancialData> data,
        CancellationToken cancellationToken = default)
    {
        ValidateFinancialData(data, nameof(data));
        
        _logger.LogInformation("Generating financial summary for {Count} records", data.Count);
        var prompt = PromptTemplates.GetFinancialSummaryPrompt(data);
        return await GenerateResponseAsync(prompt, cancellationToken);
    }

    public async Task<string> AnalyzeTrendsAsync(
        List<FinancialData> data,
        string period,
        CancellationToken cancellationToken = default)
    {
        ValidateFinancialData(data, nameof(data));
        
        _logger.LogInformation("Analyzing trends for period: {Period}", period);
        var prompt = PromptTemplates.GetTrendAnalysisPrompt(data, period);
        return await GenerateResponseAsync(prompt, cancellationToken);
    }

    public async Task<string> DetectAnomaliesAsync(
        List<FinancialData> data,
        CancellationToken cancellationToken = default)
    {
        ValidateFinancialData(data, nameof(data));
        
        _logger.LogInformation("Detecting anomalies in {Count} records", data.Count);
        var prompt = PromptTemplates.GetAnomalyDetectionPrompt(data);
        return await GenerateResponseAsync(prompt, cancellationToken);
    }

    public async Task<string> AnalyzeRatiosAsync(
        Dictionary<string, decimal> ratios,
        CancellationToken cancellationToken = default)
    {
        if (ratios == null || !ratios.Any())
        {
            throw new ArgumentException("Ratios dictionary cannot be null or empty", nameof(ratios));
        }
        
        _logger.LogInformation("Analyzing {Count} financial ratios", ratios.Count);
        var prompt = PromptTemplates.GetRatioAnalysisPrompt(ratios);
        return await GenerateResponseAsync(prompt, cancellationToken);
    }

    public async Task<string> ComparePeriodsAsync(
        List<FinancialData> current,
        List<FinancialData> previous,
        CancellationToken cancellationToken = default)
    {
        ValidateFinancialData(current, nameof(current));
        ValidateFinancialData(previous, nameof(previous));
        
        _logger.LogInformation(
            "Comparing periods: Current={CurrentCount}, Previous={PreviousCount}",
            current.Count,
            previous.Count);
        
        var prompt = PromptTemplates.GetComparisonPrompt(current, previous);
        return await GenerateResponseAsync(prompt, cancellationToken);
    }

    public async Task<string> AnalyzeCashFlowAsync(
        List<FinancialData> data,
        CancellationToken cancellationToken = default)
    {
        ValidateFinancialData(data, nameof(data));
        
        _logger.LogInformation("Analyzing cash flow for {Count} records", data.Count);
        var prompt = PromptTemplates.GetCashFlowAnalysisPrompt(data);
        return await GenerateResponseAsync(prompt, cancellationToken);
    }

    public async Task<string> GenerateForecastAsync(
        List<FinancialData> historicalData,
        int periodsAhead,
        CancellationToken cancellationToken = default)
    {
        ValidateFinancialData(historicalData, nameof(historicalData));
        
        if (periodsAhead <= 0)
        {
            throw new ArgumentException("Periods ahead must be greater than zero", nameof(periodsAhead));
        }
        
        _logger.LogInformation(
            "Generating forecast for {Periods} periods using {Count} historical records",
            periodsAhead,
            historicalData.Count);
        
        var prompt = PromptTemplates.GetForecastingPrompt(historicalData, periodsAhead);
        return await GenerateResponseAsync(prompt, cancellationToken);
    }

    public async Task<string> CustomAnalysisAsync(
        string question,
        List<FinancialData> data,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(question))
        {
            throw new ArgumentException("Question cannot be null or empty", nameof(question));
        }
        
        ValidateFinancialData(data, nameof(data));
        
        _logger.LogInformation("Performing custom analysis: {Question}", question);
        var prompt = PromptTemplates.GetCustomAnalysisPrompt(question, data);
        return await GenerateResponseAsync(prompt, cancellationToken);
    }

    // ========== Vision/Document Analysis Operations ==========

    public async Task<string> AnalyzeDocumentImageAsync(
        byte[] imageData,
        string question,
        CancellationToken cancellationToken = default)
    {
        if (imageData == null || imageData.Length == 0)
        {
            throw new ArgumentException("Image data cannot be null or empty", nameof(imageData));
        }

        if (string.IsNullOrWhiteSpace(question))
        {
            throw new ArgumentException("Question cannot be null or empty", nameof(question));
        }

        try
        {
            _logger.LogInformation(
                "Analyzing document image with vision model. Image size: {Size} bytes",
                imageData.Length);

            var base64Image = Convert.ToBase64String(imageData);
            var prompt = $"{question}\n\nPlease analyze the provided image and extract relevant financial information.";

            // Note: Vision model usage may vary based on OllamaSharp version
            // This is a basic implementation that may need adjustment
            var response = await GenerateResponseAsync(prompt, _config.VisionModel, cancellationToken);

            _logger.LogInformation("Document image analysis completed");
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analyzing document image");
            throw new InvalidOperationException("Failed to analyze document image with vision model", ex);
        }
    }

    public async Task<string> ExtractDataFromImageAsync(
        byte[] imageData,
        CancellationToken cancellationToken = default)
    {
        var question = @"Extract all financial data from this image including:
- Account names and numbers
- Amounts and currencies
- Dates and periods
- Categories and descriptions

Format the extracted data in a structured, tabular format.";

        return await AnalyzeDocumentImageAsync(imageData, question, cancellationToken);
    }

    // ========== Comprehensive AI Insights Methods ==========

    public async Task<string> GenerateComprehensiveInsightsAsync(
        List<FinancialData> data,
        CancellationToken cancellationToken = default)
    {
        ValidateFinancialData(data, nameof(data));
        
        _logger.LogInformation("Generating comprehensive insights for {Count} records", data.Count);
        
        // Call underlying analysis methods and aggregate responses
        var summary = await GenerateSummaryAsync(data, cancellationToken);
        var trends = await AnalyzeTrendsAsync(data, "YTD", cancellationToken);
        var anomalies = await DetectAnomaliesAsync(data, cancellationToken);
        var ratios = await AnalyzeRatiosAsync(CalculateRatios(data), cancellationToken);
        
        // Combine insights into a cohesive response
        var insights = new StringBuilder();
        insights.AppendLine("=== Comprehensive Financial Insights ===");
        insights.AppendLine("SUMMARY:");
        insights.AppendLine(summary);
        insights.AppendLine();
        insights.AppendLine("TRENDS:");
        insights.AppendLine(trends);
        insights.AppendLine();
        insights.AppendLine("ANOMALIES:");
        insights.AppendLine(anomalies);
        insights.AppendLine();
        insights.AppendLine("RATIO ANALYSIS:");
        insights.AppendLine(ratios);
        
        return insights.ToString();
    }

    private Dictionary<string, decimal> CalculateRatios(List<FinancialData> data)
    {
        // Implement custom logic to calculate financial ratios from data
        // Placeholder for ratio calculation logic
        return new Dictionary<string, decimal>
        {
            { "CurrentRatio", 1.5m },
            { "QuickRatio", 1.2m },
            { "DebtToEquity", 0.3m }
        };
    }

    // ========== Private Helper Methods ==========

    private void ValidateFinancialData(List<FinancialData> data, string paramName)
    {
        if (data == null || !data.Any())
        {
            throw new ArgumentException("Financial data cannot be null or empty", paramName);
        }
    }
}
