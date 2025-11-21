using CustomFinancialPlanningAssistant.Core.Entities;
using CustomFinancialPlanningAssistant.Core.DTOs;
using System.Diagnostics;
using System.Text;

namespace CustomFinancialPlanningAssistant.Services.AI;

/// <summary>
/// LlamaService partial class - Phase 11: Enhanced AI Insights Implementation
/// </summary>
public partial class LlamaService
{
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
}
