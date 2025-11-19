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
/// </summary>
public class LlamaService : ILlamaService
{
    private readonly OllamaApiClient _ollamaClient;
    private readonly AIModelConfiguration _config;
    private readonly ILogger<LlamaService> _logger;
    private readonly AsyncRetryPolicy<string> _retryPolicy;
    
    // Phase 11: Additional dependencies for comprehensive insights
    private readonly IFinancialDocumentRepository _documentRepo;

    /// <summary>
    /// Initializes a new instance of the LlamaService class
    /// </summary>
    public LlamaService(
        IOptions<AIModelConfiguration> config,
        ILogger<LlamaService> logger,
        IFinancialDocumentRepository documentRepo)
    {
        _config = config?.Value ?? throw new ArgumentNullException(nameof(config));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _documentRepo = documentRepo ?? throw new ArgumentNullException(nameof(documentRepo));

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
            _logger.LogInformation("Checking if Ollama service is available");
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

    // ========== PHASE 11: Enhanced AI Insights Implementation ==========

    public async Task<AIInsightDto> GenerateComprehensiveInsightsAsync(
        int documentId,
        string analysisType,
        CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            _logger.LogInformation(
                "Generating comprehensive AI insights for document {DocumentId}, type: {AnalysisType}",
                documentId,
                analysisType);

            // Get document and financial data
            var document = await _documentRepo.GetWithDataAsync(documentId);
            if (document == null)
            {
                throw new ArgumentException($"Document {documentId} not found");
            }

            var data = document.FinancialDataRecords.ToList();
            if (!data.Any())
            {
                throw new InvalidOperationException("No financial data found for analysis");
            }

            // Calculate basic summary metrics
            var summary = CalculateQuickSummary(data);
            var ratios = CalculateBasicRatios(summary);

            // Build specialized prompt based on analysis type
            var prompt = BuildAnalysisPrompt(summary, ratios, analysisType);

            // Get AI response
            var aiResponse = await GenerateResponseAsync(prompt, cancellationToken);

            stopwatch.Stop();

            // Parse and structure the response
            return new AIInsightDto
            {
                DocumentId = documentId,
                DocumentName = document.FileName,
                AnalysisType = analysisType,
                Title = $"{analysisType} Analysis",
                Summary = ExtractSummaryFromResponse(aiResponse),
                DetailedAnalysis = aiResponse,
                KeyFindings = AIResponseParser.ExtractKeyFindings(aiResponse),
                Recommendations = AIResponseParser.ExtractRecommendations(aiResponse),
                RiskFactors = ExtractRiskFactors(aiResponse),
                Opportunities = ExtractOpportunities(aiResponse),
                HealthScore = CalculateHealthScore(summary, ratios),
                RiskLevel = DetermineRiskLevel(summary, ratios),
                GeneratedDate = DateTime.UtcNow,
                ExecutionTime = (int)stopwatch.ElapsedMilliseconds,
                ModelUsed = _config.DefaultTextModel
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating comprehensive AI insights");
            throw;
        }
    }

    public async Task<FinancialHealthDto> AssessFinancialHealthAsync(
        int documentId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Assessing financial health for document {DocumentId}", documentId);

            var document = await _documentRepo.GetWithDataAsync(documentId);
            if (document == null)
            {
                throw new ArgumentException($"Document {documentId} not found");
            }

            var data = document.FinancialDataRecords.ToList();
            var summary = CalculateQuickSummary(data);
            var ratios = CalculateBasicRatios(summary);

            // Build health assessment prompt
            var prompt = $@"As a financial health expert, assess the overall financial health based on:

**Financial Summary:**
- Total Revenue: {summary.Revenue:C}
- Total Expenses: {summary.Expenses:C}
- Net Income: {summary.NetIncome:C}
- Total Assets: {summary.Assets:C}
- Total Liabilities: {summary.Liabilities:C}
- Equity: {summary.Equity:C}

**Key Ratios:**
{string.Join("\n", ratios.Select(r => $"- {r.Key}: {r.Value:F2}"))}

Provide:
1. Overall health assessment (Excellent/Good/Fair/Poor)
2. Top 3 financial strengths
3. Top 3 areas of concern
4. Top 3 priority actions

Be specific with numbers and percentages.";

            var response = await GenerateResponseAsync(prompt, cancellationToken);

            return ParseFinancialHealth(response, summary, ratios);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assessing financial health");
            throw;
        }
    }

    public async Task<RiskAssessmentDto> AssessRisksAsync(
        int documentId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Assessing risks for document {DocumentId}", documentId);

            var document = await _documentRepo.GetWithDataAsync(documentId);
            if (document == null)
            {
                throw new ArgumentException($"Document {documentId} not found");
            }

            var data = document.FinancialDataRecords.ToList();
            var summary = CalculateQuickSummary(data);
            var ratios = CalculateBasicRatios(summary);

            var prompt = $@"You are a risk assessment expert. Identify and analyze financial risks based on:

**Financial Data:**
- Net Income: {summary.NetIncome:C}
- Debt to Equity: {(summary.Equity != 0 ? (summary.Liabilities / summary.Equity) : 0):F2}
- Current Ratio: {(summary.Liabilities != 0 ? (summary.Assets / summary.Liabilities) : 0):F2}
- Profit Margin: {(summary.Revenue != 0 ? (summary.NetIncome / summary.Revenue * 100) : 0):F2}%";

            var response = await GenerateResponseAsync(prompt, cancellationToken);

            return ParseRiskAssessment(response, summary, ratios);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assessing risks");
            throw;
        }
    }

    public async Task<List<string>> GenerateOptimizationSuggestionsAsync(
        int documentId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Generating optimization suggestions for document {DocumentId}", documentId);

            var document = await _documentRepo.GetWithDataAsync(documentId);
            if (document == null)
            {
                throw new ArgumentException($"Document {documentId} not found");
            }

            var data = document.FinancialDataRecords.ToList();
            var summary = CalculateQuickSummary(data);

            var prompt = $@"Based on this financial data, provide 5-7 specific, actionable optimization suggestions:

**Current State:**
- Revenue: {summary.Revenue:C}
- Expenses: {summary.Expenses:C}
- Net Income: {summary.NetIncome:C}
- Profit Margin: {(summary.Revenue != 0 ? (summary.NetIncome / summary.Revenue * 100) : 0):F2}%";

            var response = await GenerateResponseAsync(prompt, cancellationToken);
            return AIResponseParser.ExtractKeyFindings(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating optimization suggestions");
            throw;
        }
    }

    public async Task<List<string>> GenerateGrowthStrategiesAsync(
        int documentId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Generating growth strategies for document {DocumentId}", documentId);

            var document = await _documentRepo.GetWithDataAsync(documentId);
            if (document == null)
            {
                throw new ArgumentException($"Document {documentId} not found");
            }

            var data = document.FinancialDataRecords.ToList();
            var summary = CalculateQuickSummary(data);

            var prompt = $@"Develop 5-7 growth strategies to grow revenue and profitability:

**Current State:**
- Revenue: {summary.Revenue:C}
- Net Income: {summary.NetIncome:C}
- Profit Margin: {(summary.Revenue != 0 ? (summary.NetIncome / summary.Revenue * 100) : 0):F2}%";

            var response = await GenerateResponseAsync(prompt, cancellationToken);
            return AIResponseParser.ExtractKeyFindings(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating growth strategies");
            throw;
        }
    }

    public async Task<string> AnswerCustomQuestionAsync(
        int documentId,
        string question,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Answering custom question for document {DocumentId}", documentId);

            var document = await _documentRepo.GetWithDataAsync(documentId);
            if (document == null)
            {
                throw new ArgumentException($"Document {documentId} not found");
            }

            var data = document.FinancialDataRecords.ToList();
            var summary = CalculateQuickSummary(data);

            var prompt = $@"You are a financial advisor. Answer the following question based on the financial data:

**Question:** {question}

**Financial Summary:**
- Revenue: {summary.Revenue:C}
- Expenses: {summary.Expenses:C}
- Net Income: {summary.NetIncome:C}
- Assets: {summary.Assets:C}
- Liabilities: {summary.Liabilities:C}

Provide a clear, concise answer with specific numbers where applicable.";

            return await GenerateResponseAsync(prompt, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error answering custom question");
            throw;
        }
    }

    public async Task<string> AnswerWithContextAsync(
        int documentId,
        string question,
        List<string> conversationHistory,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var document = await _documentRepo.GetWithDataAsync(documentId);
            if (document == null)
            {
                throw new ArgumentException($"Document {documentId} not found");
            }

            var data = document.FinancialDataRecords.ToList();
            var summary = CalculateQuickSummary(data);

            var contextBuilder = new StringBuilder();
            contextBuilder.AppendLine("Previous conversation:");
            foreach (var msg in conversationHistory.TakeLast(5))
            {
                contextBuilder.AppendLine(msg);
            }

            var prompt = $@"You are a financial advisor. Answer the following question considering the conversation history.

{contextBuilder}

**Current Financial Summary:**
- Revenue: {summary.Revenue:C}
- Expenses: {summary.Expenses:C}
- Net Income: {summary.NetIncome:C}

**Question:** {question}

Provide a clear, concise answer with specific numbers where applicable.";

            return await GenerateResponseAsync(prompt, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error answering with context");
            throw;
        }
    }

    // ========== Private Helper Methods for Phase 11 ==========

    private (decimal Revenue, decimal Expenses, decimal NetIncome, decimal Assets, decimal Liabilities, decimal Equity) CalculateQuickSummary(List<FinancialData> data)
    {
        var revenue = data.Where(d => d.Category == "Revenue").Sum(d => d.Amount);
        var expenses = data.Where(d => d.Category == "Expense").Sum(d => d.Amount);
        var assets = data.Where(d => d.Category == "Asset").Sum(d => d.Amount);
        var liabilities = data.Where(d => d.Category == "Liability").Sum(d => d.Amount);
        var equity = data.Where(d => d.Category == "Equity").Sum(d => d.Amount);
        var netIncome = revenue - expenses;

        return (revenue, expenses, netIncome, assets, liabilities, equity);
    }

    private Dictionary<string, decimal> CalculateBasicRatios(
        (decimal Revenue, decimal Expenses, decimal NetIncome, decimal Assets, decimal Liabilities, decimal Equity) summary)
    {
        var ratios = new Dictionary<string, decimal>();

        if (summary.Revenue > 0)
        {
            ratios["ProfitMargin"] = (summary.NetIncome / summary.Revenue) * 100;
        }

        if (summary.Liabilities > 0 && summary.Assets > 0)
        {
            ratios["CurrentRatio"] = summary.Assets / summary.Liabilities;
        }

        if (summary.Equity > 0 && summary.Liabilities > 0)
        {
            ratios["DebtToEquity"] = summary.Liabilities / summary.Equity;
        }

        return ratios;
    }

    private string BuildAnalysisPrompt(
        (decimal Revenue, decimal Expenses, decimal NetIncome, decimal Assets, decimal Liabilities, decimal Equity) summary,
        Dictionary<string, decimal> ratios,
        string analysisType)
    {
        return analysisType switch
        {
            "HealthCheck" => $"Assess overall financial health. Revenue: {summary.Revenue:C}, Net Income: {summary.NetIncome:C}, Assets: {summary.Assets:C}",
            "RiskAnalysis" => $"Identify financial risks. Debt/Equity: {ratios.GetValueOrDefault("DebtToEquity", 0):F2}, Profit Margin: {ratios.GetValueOrDefault("ProfitMargin", 0):F2}",
            "Optimization" => $"Suggest optimization strategies. Expenses: {summary.Expenses:C}, Revenue: {summary.Revenue:C}",
            "Growth" => $"Develop growth strategies. Current Revenue: {summary.Revenue:C}, Net Income: {summary.NetIncome:C}",
            _ => $"Analyze financial data. Revenue: {summary.Revenue:C}, Net Income: {summary.NetIncome:C}"
        };
    }

    private int CalculateHealthScore(
        (decimal Revenue, decimal Expenses, decimal NetIncome, decimal Assets, decimal Liabilities, decimal Equity) summary,
        Dictionary<string, decimal> ratios)
    {
        int score = 50; // Start at middle

        // Profitability (30 points)
        if (summary.NetIncome > 0) score += 15;
        if (ratios.GetValueOrDefault("ProfitMargin", 0) > 10) score += 15;

        // Liquidity (30 points)
        if (ratios.GetValueOrDefault("CurrentRatio", 0) > 1.5m) score += 15;
        if (summary.Assets > summary.Liabilities) score += 15;

        // Efficiency (20 points)
        if (summary.Revenue > 0 && summary.Expenses > 0)
        {
            var expenseRatio = (summary.Expenses / summary.Revenue) * 100;
            if (expenseRatio < 80) score += 20;
        }

        return Math.Clamp(score, 0, 100);
    }

    private string DetermineRiskLevel(
        (decimal Revenue, decimal Expenses, decimal NetIncome, decimal Assets, decimal Liabilities, decimal Equity) summary,
        Dictionary<string, decimal> ratios)
    {
        int riskScore = 0;

        if (summary.NetIncome < 0) riskScore += 30;
        if (ratios.GetValueOrDefault("DebtToEquity", 0) > 2) riskScore += 20;
        if (ratios.GetValueOrDefault("CurrentRatio", 0) < 1) riskScore += 25;
        if (summary.Revenue > 0 && (summary.Expenses / summary.Revenue) > 0.9m) riskScore += 25;

        return riskScore switch
        {
            > 60 => "Critical",
            > 40 => "High",
            > 20 => "Medium",
            _ => "Low"
        };
    }

    private string ExtractSummaryFromResponse(string response)
    {
        var lines = response.Split('\n').Take(5);
        return string.Join(" ", lines).Trim();
    }

    private List<string> ExtractRiskFactors(string response)
    {
        // Simple extraction - look for lines containing "risk" or "concern"
        return response.Split('\n')
            .Where(line => line.ToLower().Contains("risk") || line.ToLower().Contains("concern"))
            .Take(5)
            .ToList();
    }

    private List<string> ExtractOpportunities(string response)
    {
        // Simple extraction - look for lines containing "opportunity" or "potential"
        return response.Split('\n')
            .Where(line => line.ToLower().Contains("opportunity") || line.ToLower().Contains("potential"))
            .Take(5)
            .ToList();
    }

    private FinancialHealthDto ParseFinancialHealth(
        string response,
        (decimal Revenue, decimal Expenses, decimal NetIncome, decimal Assets, decimal Liabilities, decimal Equity) summary,
        Dictionary<string, decimal> ratios)
    {
        var healthScore = CalculateHealthScore(summary, ratios);
        
        return new FinancialHealthDto
        {
            OverallScore = healthScore,
            ProfitabilityScore = (int)Math.Clamp(ratios.GetValueOrDefault("ProfitMargin", 0) * 5, 0, 100),
            LiquidityScore = (int)Math.Clamp(ratios.GetValueOrDefault("CurrentRatio", 0) * 50, 0, 100),
            EfficiencyScore = 75, // Placeholder
            StabilityScore = summary.NetIncome > 0 ? 80 : 40,
            OverallRating = healthScore switch
            {
                >= 80 => "Excellent",
                >= 60 => "Good",
                >= 40 => "Fair",
                _ => "Poor"
            },
            Strengths = AIResponseParser.ExtractKeyFindings(response).Take(3).ToList(),
            Weaknesses = ExtractRiskFactors(response).Take(3).ToList(),
            Priorities = AIResponseParser.ExtractRecommendations(response).Take(3).ToList()
        };
    }

    private RiskAssessmentDto ParseRiskAssessment(
        string response,
        (decimal Revenue, decimal Expenses, decimal NetIncome, decimal Assets, decimal Liabilities, decimal Equity) summary,
        Dictionary<string, decimal> ratios)
    {
        var riskLevel = DetermineRiskLevel(summary, ratios);
        
        return new RiskAssessmentDto
        {
            RiskLevel = riskLevel,
            RiskScore = riskLevel switch
            {
                "Critical" => 80,
                "High" => 60,
                "Medium" => 40,
                _ => 20
            },
            Risks = new List<RiskItemDto>
            {
                new RiskItemDto
                {
                    Category = "Financial",
                    Description = $"Net Income: {summary.NetIncome:C}",
                    Severity = summary.NetIncome < 0 ? "High" : "Low",
                    Impact = "Affects profitability",
                    Recommendations = new List<string> { "Review expenses", "Increase revenue" }
                }
            },
            MitigationStrategies = AIResponseParser.ExtractRecommendations(response)
        };
    }
}
