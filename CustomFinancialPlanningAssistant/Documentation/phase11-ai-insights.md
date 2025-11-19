# Financial Analysis Assistant - PHASE 11: AI Insights Enhancement

## Overview
This phase transforms the AI Insights page into a comprehensive, interactive AI-powered financial advisory system with automated analysis, recommendations, risk assessment, and custom Q&A capabilities.

**Estimated Time:** 60-90 minutes  
**Prerequisites:** Phase 1-10 completed  
**Dependencies:** Ollama with Llama 3.2, existing AI services  
**Why AI Insights:** Provide intelligent, actionable financial insights that help users make informed decisions

---

## Why We're Enhancing AI Insights

### Business Value:
- ? **Automated Financial Advisory** - AI provides expert-level financial guidance
- ? **Risk Detection** - Identifies financial risks and warning signs
- ? **Personalized Recommendations** - Tailored advice based on user's data
- ? **24/7 Availability** - Always-on financial advisor
- ? **Data-Driven Decisions** - Insights backed by actual financial data

### Technical Benefits:
- ? **Ollama Integration** - Already connected and working
- ? **Existing Services** - FinancialService + LlamaService ready
- ? **Prompt Templates** - Already have comprehensive prompts
- ? **Real Financial Data** - Access to all user's financial records

---

## Phase 11 Goals

### What We'll Build:

1. **Enhanced AI Insights Page:**
   - Document selector with context
   - Multiple analysis types (tabs)
   - Real-time streaming responses
   - Response history
   - Export insights

2. **Analysis Templates:**
   - ?? **Financial Health Check** - Overall assessment
   - ?? **Risk Analysis** - Identify financial risks
   - ?? **Optimization Suggestions** - Improve financial performance
   - ?? **Growth Strategy** - Revenue/profit recommendations
   - ?? **Anomaly Report** - Flag unusual transactions
   - ?? **Forecast Analysis** - Future predictions
   - ?? **Cost Reduction** - Expense optimization
   - ?? **Cash Flow Analysis** - Liquidity assessment

3. **Interactive Features:**
   - Custom question input
   - Follow-up questions
   - Context-aware responses
   - Save insights to database
   - Share/export functionality

4. **Visualization:**
   - Health score gauge (0-100)
   - Risk level indicator
   - Key metrics cards
   - Trend charts
   - Recommendation cards

---

## Step 11.1: Enhanced DTOs

### File: Core/DTOs/AIInsightDto.cs

```csharp
namespace CustomFinancialPlanningAssistant.Core.DTOs;

/// <summary>
/// Comprehensive AI insight response
/// </summary>
public class AIInsightDto
{
    public int Id { get; set; }
    public int DocumentId { get; set; }
    public string DocumentName { get; set; } = string.Empty;
    public string AnalysisType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string DetailedAnalysis { get; set; } = string.Empty;
    public List<string> KeyFindings { get; set; } = new();
    public List<string> Recommendations { get; set; } = new();
    public List<string> RiskFactors { get; set; } = new();
    public List<string> Opportunities { get; set; } = new();
    public int HealthScore { get; set; } // 0-100
    public string RiskLevel { get; set; } = "Medium"; // Low, Medium, High
    public DateTime GeneratedDate { get; set; }
    public int ExecutionTime { get; set; }
    public string ModelUsed { get; set; } = "Llama 3.2";
}

/// <summary>
/// Request for AI analysis
/// </summary>
public class AIInsightRequestDto
{
    public int DocumentId { get; set; }
    public string AnalysisType { get; set; } = string.Empty;
    public string CustomQuestion { get; set; } = string.Empty;
    public bool IncludeHistoricalData { get; set; }
    public bool IncludeComparisons { get; set; }
    public bool IncludeForecasts { get; set; }
}

/// <summary>
/// Financial health assessment
/// </summary>
public class FinancialHealthDto
{
    public int OverallScore { get; set; } // 0-100
    public int ProfitabilityScore { get; set; }
    public int LiquidityScore { get; set; }
    public int EfficiencyScore { get; set; }
    public int StabilityScore { get; set; }
    public string OverallRating { get; set; } = string.Empty; // Excellent, Good, Fair, Poor
    public List<string> Strengths { get; set; } = new();
    public List<string> Weaknesses { get; set; } = new();
    public List<string> Priorities { get; set; } = new();
}

/// <summary>
/// Risk assessment result
/// </summary>
public class RiskAssessmentDto
{
    public string RiskLevel { get; set; } = "Medium"; // Low, Medium, High, Critical
    public int RiskScore { get; set; } // 0-100 (higher = more risk)
    public List<RiskItemDto> Risks { get; set; } = new();
    public List<string> MitigationStrategies { get; set; } = new();
}

/// <summary>
/// Individual risk item
/// </summary>
public class RiskItemDto
{
    public string Category { get; set; } = string.Empty; // Liquidity, Profitability, etc.
    public string Description { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty; // Low, Medium, High, Critical
    public string Impact { get; set; } = string.Empty;
    public List<string> Recommendations { get; set; } = new();
}
```

---

## Step 11.2: Enhanced AI Service Methods

### File: Services/AI/ILlamaService.cs

**Add new methods:**
```csharp
// Enhanced Analysis
Task<AIInsightDto> GenerateComprehensiveInsightsAsync(int documentId, string analysisType);
Task<FinancialHealthDto> AssessFinancialHealthAsync(int documentId);
Task<RiskAssessmentDto> AssessRisksAsync(int documentId);
Task<List<string>> GenerateOptimizationSuggestionsAsync(int documentId);
Task<List<string>> GenerateGrowthStrategiesAsync(int documentId);
Task<string> AnswerCustomQuestionAsync(int documentId, string question);

// Context-aware Q&A
Task<string> AnswerWithContextAsync(int documentId, string question, List<string> conversationHistory);
```

### File: Services/AI/LlamaService.cs

**Implement comprehensive analysis:**
```csharp
public async Task<AIInsightDto> GenerateComprehensiveInsightsAsync(int documentId, string analysisType)
{
    var stopwatch = Stopwatch.StartNew();
    
    // Get financial data
    var document = await _documentRepo.GetWithDataAsync(documentId);
    var summary = await _financialService.GetFinancialSummaryAsync(documentId);
    var ratios = await _financialService.CalculateFinancialRatiosAsync(documentId);
    
    // Build comprehensive prompt
    var prompt = BuildComprehensivePrompt(summary, ratios, analysisType);
    
    // Get AI response
    var response = await GenerateResponseAsync(prompt);
    
    stopwatch.Stop();
    
    // Parse response into structured format
    return new AIInsightDto
    {
        DocumentId = documentId,
        DocumentName = document.FileName,
        AnalysisType = analysisType,
        Title = $"{analysisType} Analysis",
        Summary = ExtractSummary(response),
        DetailedAnalysis = response,
        KeyFindings = AIResponseParser.ExtractKeyFindings(response),
        Recommendations = AIResponseParser.ExtractRecommendations(response),
        RiskFactors = ExtractRiskFactors(response),
        Opportunities = ExtractOpportunities(response),
        HealthScore = CalculateHealthScore(summary, ratios),
        RiskLevel = DetermineRiskLevel(summary, ratios),
        GeneratedDate = DateTime.UtcNow,
        ExecutionTime = (int)stopwatch.ElapsedMilliseconds,
        ModelUsed = _configuration.DefaultTextModel
    };
}

public async Task<FinancialHealthDto> AssessFinancialHealthAsync(int documentId)
{
    var summary = await _financialService.GetFinancialSummaryAsync(documentId);
    var ratios = await _financialService.CalculateFinancialRatiosAsync(documentId);
    
    var prompt = $@"As a financial health expert, assess the overall financial health based on:

**Financial Summary:**
- Total Revenue: {summary.TotalRevenue:C}
- Total Expenses: {summary.TotalExpenses:C}
- Net Income: {summary.NetIncome:C}
- Total Assets: {summary.TotalAssets:C}
- Total Liabilities: {summary.TotalLiabilities:C}

**Key Ratios:**
{string.Join("\n", ratios.Select(r => $"- {r.Key}: {r.Value:F2}"))}

Provide:
1. Overall health score (0-100)
2. Profitability score (0-100)
3. Liquidity score (0-100)
4. Efficiency score (0-100)
5. Top 3 strengths
6. Top 3 weaknesses
7. Top 3 priority actions

Format: Score: [number] | Strengths: [list] | Weaknesses: [list] | Priorities: [list]";

    var response = await GenerateResponseAsync(prompt);
    
    return ParseFinancialHealth(response, summary, ratios);
}

public async Task<RiskAssessmentDto> AssessRisksAsync(int documentId)
{
    var summary = await _financialService.GetFinancialSummaryAsync(documentId);
    var ratios = await _financialService.CalculateFinancialRatiosAsync(documentId);
    var anomalies = await _financialService.DetectAnomaliesAsync(documentId);
    
    var prompt = PromptTemplates.GetRiskAssessmentPrompt(summary, ratios, anomalies);
    var response = await GenerateResponseAsync(prompt);
    
    return ParseRiskAssessment(response);
}

public async Task<List<string>> GenerateOptimizationSuggestionsAsync(int documentId)
{
    var summary = await _financialService.GetFinancialSummaryAsync(documentId);
    
    var prompt = $@"Based on this financial data, provide 5-7 specific, actionable optimization suggestions:

**Current State:**
- Revenue: {summary.TotalRevenue:C}
- Expenses: {summary.TotalExpenses:C}
- Net Income: {summary.NetIncome:C}
- Profit Margin: {(summary.NetIncome / summary.TotalRevenue * 100):F2}%

Focus on:
1. Expense reduction opportunities
2. Revenue enhancement strategies
3. Efficiency improvements
4. Working capital optimization
5. Cost structure optimization

Format each suggestion as a single actionable sentence.";

    var response = await GenerateResponseAsync(prompt);
    return AIResponseParser.ExtractBulletPoints(response);
}

public async Task<string> AnswerWithContextAsync(
    int documentId, 
    string question, 
    List<string> conversationHistory)
{
    var document = await _documentRepo.GetWithDataAsync(documentId);
    var summary = await _financialService.GetFinancialSummaryAsync(documentId);
    
    var contextBuilder = new StringBuilder();
    contextBuilder.AppendLine("Previous conversation:");
    foreach (var msg in conversationHistory.TakeLast(5))
    {
        contextBuilder.AppendLine(msg);
    }
    
    var prompt = $@"You are a financial advisor. Answer the following question based on the financial data and conversation history.

{contextBuilder}

**Current Financial Summary:**
- Revenue: {summary.TotalRevenue:C}
- Expenses: {summary.TotalExpenses:C}
- Net Income: {summary.NetIncome:C}

**Question:** {question}

Provide a clear, concise answer with specific numbers where applicable.";

    return await GenerateResponseAsync(prompt);
}

// Helper methods
private string BuildComprehensivePrompt(
    FinancialSummaryDto summary, 
    Dictionary<string, decimal> ratios, 
    string analysisType)
{
    return analysisType switch
    {
        "HealthCheck" => PromptTemplates.GetFinancialHealthPrompt(summary, ratios),
        "RiskAnalysis" => PromptTemplates.GetRiskAnalysisPrompt(summary, ratios),
        "Optimization" => PromptTemplates.GetOptimizationPrompt(summary),
        "Growth" => PromptTemplates.GetGrowthStrategyPrompt(summary, ratios),
        _ => PromptTemplates.GetFinancialSummaryPrompt(summary.DocumentName)
    };
}

private int CalculateHealthScore(FinancialSummaryDto summary, Dictionary<string, decimal> ratios)
{
    int score = 50; // Start at middle
    
    // Profitability (30 points)
    if (summary.NetIncome > 0)
        score += 15;
    if (ratios.ContainsKey("NetProfitMargin") && ratios["NetProfitMargin"] > 10)
        score += 15;
    
    // Liquidity (30 points)
    if (ratios.ContainsKey("CurrentRatio") && ratios["CurrentRatio"] > 1.5m)
        score += 15;
    if (summary.TotalAssets > summary.TotalLiabilities)
        score += 15;
    
    // Efficiency (20 points)
    if (ratios.ContainsKey("AssetTurnover") && ratios["AssetTurnover"] > 1)
        score += 10;
    if (ratios.ContainsKey("OperatingExpenseRatio") && ratios["OperatingExpenseRatio"] < 80)
        score += 10;
    
    return Math.Clamp(score, 0, 100);
}

private string DetermineRiskLevel(FinancialSummaryDto summary, Dictionary<string, decimal> ratios)
{
    int riskScore = 0;
    
    // Negative income = high risk
    if (summary.NetIncome < 0) riskScore += 30;
    
    // High debt to equity
    if (ratios.ContainsKey("DebtToEquity") && ratios["DebtToEquity"] > 2) riskScore += 20;
    
    // Low liquidity
    if (ratios.ContainsKey("CurrentRatio") && ratios["CurrentRatio"] < 1) riskScore += 25;
    
    // High expense ratio
    if (ratios.ContainsKey("OperatingExpenseRatio") && ratios["OperatingExpenseRatio"] > 90) riskScore += 25;
    
    return riskScore switch
    {
        > 60 => "Critical",
        > 40 => "High",
        > 20 => "Medium",
        _ => "Low"
    };
}
```

---

## Step 11.3: Enhanced Prompt Templates

### File: Services/AI/PromptTemplates.cs

**Add specialized prompts:**
```csharp
public static string GetFinancialHealthPrompt(FinancialSummaryDto summary, Dictionary<string, decimal> ratios)
{
    return $@"You are a financial health expert. Provide a comprehensive health assessment.

**Financial Metrics:**
Revenue: {summary.TotalRevenue:C}
Expenses: {summary.TotalExpenses:C}
Net Income: {summary.NetIncome:C}
Assets: {summary.TotalAssets:C}
Liabilities: {summary.TotalLiabilities:C}

**Ratios:**
{string.Join("\n", ratios.Select(r => $"{r.Key}: {r.Value:F2}"))}

Provide:
1. **Overall Health Score (0-100):** [number]
2. **Assessment:** [1-2 sentences]
3. **Top 3 Strengths:** 
   - [strength 1]
   - [strength 2]
   - [strength 3]
4. **Top 3 Weaknesses:**
   - [weakness 1]
   - [weakness 2]
   - [weakness 3]
5. **Priority Actions:**
   - [action 1]
   - [action 2]
   - [action 3]

Be specific with numbers and percentages.";
}

public static string GetRiskAnalysisPrompt(FinancialSummaryDto summary, Dictionary<string, decimal> ratios)
{
    return $@"You are a risk assessment expert. Identify and analyze financial risks.

**Financial Data:**
{FormatFinancialSummary(summary)}

**Ratios:**
{string.Join("\n", ratios.Select(r => $"{r.Key}: {r.Value:F2}"))}

Identify:
1. **Risk Level:** [Low/Medium/High/Critical]
2. **Key Risks:**
   - **Liquidity Risk:** [analysis]
   - **Profitability Risk:** [analysis]
   - **Solvency Risk:** [analysis]
   - **Operational Risk:** [analysis]
3. **Risk Mitigation Strategies:**
   - [strategy 1]
   - [strategy 2]
   - [strategy 3]
4. **Warning Signs:**
   - [sign 1]
   - [sign 2]

Be specific and actionable.";
}

public static string GetOptimizationPrompt(FinancialSummaryDto summary)
{
    return $@"You are an optimization expert. Suggest improvements to financial performance.

**Current Performance:**
- Revenue: {summary.TotalRevenue:C}
- Expenses: {summary.TotalExpenses:C}
- Net Income: {summary.NetIncome:C}
- Margin: {(summary.NetIncome / summary.TotalRevenue * 100):F2}%

Provide 7-10 specific optimization suggestions covering:
1. Revenue growth opportunities
2. Cost reduction strategies
3. Efficiency improvements
4. Working capital optimization
5. Process improvements

Format each as: **[Area]:** [Specific actionable suggestion]";
}

public static string GetGrowthStrategyPrompt(FinancialSummaryDto summary, Dictionary<string, decimal> ratios)
{
    return $@"You are a growth strategy consultant. Develop strategies to grow revenue and profitability.

**Current State:**
- Revenue: {summary.TotalRevenue:C}
- Net Income: {summary.NetIncome:C}
- Profit Margin: {(summary.NetIncome / summary.TotalRevenue * 100):F2}%

**Ratios:**
{string.Join("\n", ratios.Select(r => $"{r.Key}: {r.Value:F2}"))}

Provide:
1. **Growth Potential:** [analysis]
2. **Revenue Growth Strategies:** (5 strategies)
3. **Profitability Enhancement:** (3 strategies)
4. **Market Opportunities:** (3 opportunities)
5. **Investment Priorities:** (3 priorities)

Be specific and data-driven.";
}
```

---

## Step 11.4: Enhanced AI Insights Page

### File: Components/Pages/AIInsights.razor

**Create comprehensive AI insights interface:**

```razor
@page "/ai-insights"
@inject IFinancialService FinancialService
@inject ILlamaService AIService
@inject IFinancialDocumentRepository DocumentRepo
@inject ISnackbar Snackbar
@rendermode InteractiveServer

<PageTitle>AI Insights - Financial Analysis Assistant</PageTitle>

<MudText Typo="Typo.h4" GutterBottom="true">
    <MudIcon Icon="@Icons.Material.Filled.Psychology" Class="mr-2" />
    AI-Powered Financial Insights
</MudText>

<!-- Document & Analysis Selector -->
<MudCard Elevation="2" Class="mb-4">
    <MudCardContent>
        <MudGrid>
            <MudItem xs="12" md="8">
                <MudSelect @bind-Value="_selectedDocumentId" 
                           Label="Select Document" 
                           Variant="Variant.Outlined"
                           Disabled="_isGenerating">
                    @foreach (var doc in _documents)
                    {
                        <MudSelectItem Value="@doc.Id">@doc.FileName</MudSelectItem>
                    }
                </MudSelect>
            </MudItem>
            <MudItem xs="12" md="4">
                <MudButton Variant="Variant.Filled" 
                           Color="Color.Primary" 
                           FullWidth="true"
                           StartIcon="@Icons.Material.Filled.AutoAwesome"
                           OnClick="GenerateInsights"
                           Disabled="_isGenerating || _selectedDocumentId == 0">
                    Generate AI Insights
                </MudButton>
            </MudItem>
        </MudGrid>
    </MudCardContent>
</MudCard>

@if (_isGenerating)
{
    <MudCard Elevation="2" Class="mb-4">
        <MudCardContent>
            <MudProgressLinear Color="Color.Primary" Indeterminate="true" />
            <MudText Align="Align.Center" Class="mt-2">
                <MudIcon Icon="@Icons.Material.Filled.AutoFixHigh" Class="mr-2" />
                AI is analyzing your financial data... (@_analysisType)
            </MudText>
        </MudCardContent>
    </MudCard>
}

<!-- Analysis Tabs -->
@if (_currentInsight != null)
{
    <MudTabs Elevation="2" Rounded="true" ApplyEffectsToContainer="true" PanelClass="pa-6">
        <!-- Overview Tab -->
        <MudTabPanel Text="Overview" Icon="@Icons.Material.Filled.Dashboard">
            <MudGrid>
                <!-- Health Score -->
                <MudItem xs="12" md="4">
                    <MudCard Elevation="0">
                        <MudCardContent>
                            <MudText Typo="Typo.h6" Align="Align.Center">Financial Health Score</MudText>
                            <MudProgressCircular Value="_currentInsight.HealthScore" 
                                                 Size="Size.Large" 
                                                 Color="@GetHealthScoreColor(_currentInsight.HealthScore)"
                                                 Class="mt-4">
                                <MudText Typo="Typo.h4">@_currentInsight.HealthScore</MudText>
                            </MudProgressCircular>
                            <MudText Align="Align.Center" Class="mt-2">
                                @GetHealthRating(_currentInsight.HealthScore)
                            </MudText>
                        </MudCardContent>
                    </MudCard>
                </MudItem>
                
                <!-- Risk Level -->
                <MudItem xs="12" md="4">
                    <MudCard Elevation="0">
                        <MudCardContent>
                            <MudText Typo="Typo.h6" Align="Align.Center">Risk Level</MudText>
                            <MudIcon Icon="@GetRiskIcon(_currentInsight.RiskLevel)" 
                                     Color="@GetRiskColor(_currentInsight.RiskLevel)"
                                     Size="Size.Large"
                                     Class="mt-4" />
                            <MudText Typo="Typo.h5" Align="Align.Center" Class="mt-2">
                                @_currentInsight.RiskLevel
                            </MudText>
                        </MudCardContent>
                    </MudCard>
                </MudItem>
                
                <!-- Generation Info -->
                <MudItem xs="12" md="4">
                    <MudCard Elevation="0">
                        <MudCardContent>
                            <MudText Typo="Typo.h6">Analysis Details</MudText>
                            <MudText Typo="Typo.body2" Class="mt-2">
                                <strong>Type:</strong> @_currentInsight.AnalysisType
                            </MudText>
                            <MudText Typo="Typo.body2">
                                <strong>Generated:</strong> @_currentInsight.GeneratedDate.ToString("g")
                            </MudText>
                            <MudText Typo="Typo.body2">
                                <strong>Time:</strong> @_currentInsight.ExecutionTime ms
                            </MudText>
                            <MudText Typo="Typo.body2">
                                <strong>Model:</strong> @_currentInsight.ModelUsed
                            </MudText>
                        </MudCardContent>
                    </MudCard>
                </MudItem>
            </MudGrid>
            
            <!-- Summary -->
            <MudCard Elevation="0" Class="mt-4">
                <MudCardHeader>
                    <MudText Typo="Typo.h6">Executive Summary</MudText>
                </MudCardHeader>
                <MudCardContent>
                    <MudText Typo="Typo.body1">@_currentInsight.Summary</MudText>
                </MudCardContent>
            </MudCard>
        </MudTabPanel>
        
        <!-- Key Findings Tab -->
        <MudTabPanel Text="Key Findings" Icon="@Icons.Material.Filled.Lightbulb">
            <MudGrid>
                @foreach (var finding in _currentInsight.KeyFindings)
                {
                    <MudItem xs="12" md="6">
                        <MudAlert Severity="Severity.Info" Variant="Variant.Outlined">
                            @finding
                        </MudAlert>
                    </MudItem>
                }
            </MudGrid>
        </MudTabPanel>
        
        <!-- Recommendations Tab -->
        <MudTabPanel Text="Recommendations" Icon="@Icons.Material.Filled.Recommend">
            <MudList>
                @foreach (var (recommendation, index) in _currentInsight.Recommendations.Select((r, i) => (r, i)))
                {
                    <MudListItem Icon="@Icons.Material.Filled.CheckCircle" IconColor="Color.Success">
                        <MudText Typo="Typo.body1"><strong>@(index + 1).</strong> @recommendation</MudText>
                    </MudListItem>
                    <MudDivider />
                }
            </MudList>
        </MudTabPanel>
        
        <!-- Risks Tab -->
        <MudTabPanel Text="Risk Factors" Icon="@Icons.Material.Filled.Warning">
            @if (_currentInsight.RiskFactors.Any())
            {
                <MudGrid>
                    @foreach (var risk in _currentInsight.RiskFactors)
                    {
                        <MudItem xs="12">
                            <MudAlert Severity="Severity.Warning">
                                <MudText Typo="Typo.body1">@risk</MudText>
                            </MudAlert>
                        </MudItem>
                    }
                </MudGrid>
            }
            else
            {
                <MudAlert Severity="Severity.Success">
                    No significant risk factors detected.
                </MudAlert>
            }
        </MudTabPanel>
        
        <!-- Opportunities Tab -->
        <MudTabPanel Text="Opportunities" Icon="@Icons.Material.Filled.TrendingUp">
            @if (_currentInsight.Opportunities.Any())
            {
                <MudList>
                    @foreach (var opportunity in _currentInsight.Opportunities)
                    {
                        <MudListItem Icon="@Icons.Material.Filled.Stars" IconColor="Color.Primary">
                            <MudText Typo="Typo.body1">@opportunity</MudText>
                        </MudListItem>
                        <MudDivider />
                    }
                </MudList>
            }
            else
            {
                <MudAlert Severity="Severity.Info">
                    No specific opportunities identified at this time.
                </MudAlert>
            }
        </MudTabPanel>
        
        <!-- Full Analysis Tab -->
        <MudTabPanel Text="Full Analysis" Icon="@Icons.Material.Filled.Article">
            <MudPaper Elevation="0" Class="pa-4">
                <MudText Typo="Typo.body1" Style="white-space: pre-wrap;">
                    @_currentInsight.DetailedAnalysis
                </MudText>
            </MudPaper>
        </MudTabPanel>
        
        <!-- Custom Q&A Tab -->
        <MudTabPanel Text="Ask AI" Icon="@Icons.Material.Filled.QuestionAnswer">
            <MudCard Elevation="0">
                <MudCardContent>
                    <MudTextField @bind-Value="_customQuestion" 
                                  Label="Ask a question about your financial data" 
                                  Variant="Variant.Outlined"
                                  Lines="3"
                                  FullWidth="true"
                                  Disabled="_isAnswering" />
                    <MudButton Variant="Variant.Filled" 
                               Color="Color.Primary" 
                               Class="mt-2"
                               StartIcon="@Icons.Material.Filled.Send"
                               OnClick="AskCustomQuestion"
                               Disabled="_isAnswering || string.IsNullOrWhiteSpace(_customQuestion)">
                        Ask AI
                    </MudButton>
                </MudCardContent>
            </MudCard>
            
            @if (_isAnswering)
            {
                <MudProgressLinear Color="Color.Primary" Indeterminate="true" Class="mt-4" />
            }
            
            @if (_qaHistory.Any())
            {
                <MudTimeline Class="mt-4">
                    @foreach (var qa in _qaHistory)
                    {
                        <MudTimelineItem Color="Color.Primary">
                            <MudCard Elevation="0">
                                <MudCardContent>
                                    <MudText Typo="Typo.body1" Color="Color.Primary">
                                        <strong>Q:</strong> @qa.Question
                                    </MudText>
                                    <MudText Typo="Typo.body1" Class="mt-2">
                                        <strong>A:</strong> @qa.Answer
                                    </MudText>
                                </MudCardContent>
                            </MudCard>
                        </MudTimelineItem>
                    }
                </MudTimeline>
            }
        </MudTabPanel>
    </MudTabs>
    
    <!-- Action Buttons -->
    <MudCard Elevation="2" Class="mt-4">
        <MudCardActions>
            <MudButton Variant="Variant.Outlined" 
                       Color="Color.Primary"
                       StartIcon="@Icons.Material.Filled.Refresh"
                       OnClick="() => GenerateInsightsOfType(_analysisType)">
                Regenerate
            </MudButton>
            <MudButton Variant="Variant.Outlined" 
                       Color="Color.Success"
                       StartIcon="@Icons.Material.Filled.Download"
                       OnClick="ExportInsights">
                Export Report
            </MudButton>
            <MudSpacer />
            <MudSelect @bind-Value="_analysisType" 
                       Label="Analysis Type" 
                       Variant="Variant.Outlined"
                       Immediate="true"
                       OnClose="() => GenerateInsightsOfType(_analysisType)">
                <MudSelectItem Value="@("HealthCheck")">Financial Health Check</MudSelectItem>
                <MudSelectItem Value="@("RiskAnalysis")">Risk Assessment</MudSelectItem>
                <MudSelectItem Value="@("Optimization")">Optimization Suggestions</MudSelectItem>
                <MudSelectItem Value="@("Growth")">Growth Strategy</MudSelectItem>
                <MudSelectItem Value="@("CashFlow")">Cash Flow Analysis</MudSelectItem>
                <MudSelectItem Value="@("Forecast")">Forecast Analysis</MudSelectItem>
            </MudSelect>
        </MudCardActions>
    </MudCard>
}

@code {
    private List<FinancialDocument> _documents = new();
    private int _selectedDocumentId;
    private bool _isGenerating;
    private bool _isAnswering;
    private string _analysisType = "HealthCheck";
    private AIInsightDto? _currentInsight;
    private string _customQuestion = string.Empty;
    private List<(string Question, string Answer)> _qaHistory = new();
    
    protected override async Task OnInitializedAsync()
    {
        await LoadDocuments();
    }
    
    private async Task LoadDocuments()
    {
        try
        {
            _documents = (await DocumentRepo.GetAllAsync()).ToList();
            if (_documents.Any())
            {
                _selectedDocumentId = _documents.First().Id;
            }
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error loading documents: {ex.Message}", Severity.Error);
        }
    }
    
    private async Task GenerateInsights()
    {
        await GenerateInsightsOfType(_analysisType);
    }
    
    private async Task GenerateInsightsOfType(string analysisType)
    {
        if (_selectedDocumentId == 0) return;
        
        try
        {
            _isGenerating = true;
            _analysisType = analysisType;
            
            _currentInsight = await AIService.GenerateComprehensiveInsightsAsync(_selectedDocumentId, analysisType);
            
            Snackbar.Add($"AI analysis complete! ({_currentInsight.ExecutionTime}ms)", Severity.Success);
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error generating insights: {ex.Message}", Severity.Error);
        }
        finally
        {
            _isGenerating = false;
        }
    }
    
    private async Task AskCustomQuestion()
    {
        if (string.IsNullOrWhiteSpace(_customQuestion) || _selectedDocumentId == 0) return;
        
        try
        {
            _isAnswering = true;
            
            var answer = await AIService.AnswerCustomQuestionAsync(_selectedDocumentId, _customQuestion);
            
            _qaHistory.Add((_customQuestion, answer));
            _customQuestion = string.Empty;
            
            Snackbar.Add("Question answered!", Severity.Success);
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error: {ex.Message}", Severity.Error);
        }
        finally
        {
            _isAnswering = false;
        }
    }
    
    private async Task ExportInsights()
    {
        // TODO: Implement export functionality
        Snackbar.Add("Export feature coming soon!", Severity.Info);
    }
    
    // Helper methods for UI
    private Color GetHealthScoreColor(int score)
    {
        return score switch
        {
            >= 80 => Color.Success,
            >= 60 => Color.Info,
            >= 40 => Color.Warning,
            _ => Color.Error
        };
    }
    
    private string GetHealthRating(int score)
    {
        return score switch
        {
            >= 80 => "Excellent",
            >= 60 => "Good",
            >= 40 => "Fair",
            _ => "Needs Attention"
        };
    }
    
    private Color GetRiskColor(string riskLevel)
    {
        return riskLevel switch
        {
            "Low" => Color.Success,
            "Medium" => Color.Warning,
            "High" => Color.Error,
            "Critical" => Color.Error,
            _ => Color.Default
        };
    }
    
    private string GetRiskIcon(string riskLevel)
    {
        return riskLevel switch
        {
            "Low" => Icons.Material.Filled.CheckCircle,
            "Medium" => Icons.Material.Filled.Warning,
            "High" => Icons.Material.Filled.Error,
            "Critical" => Icons.Material.Filled.Dangerous,
            _ => Icons.Material.Filled.Help
        };
    }
}
```

---

## Step 11.5: Implementation Checklist

### ? Implementation Steps:

1. **Create DTOs** (15 minutes)
   - AIInsightDto
   - FinancialHealthDto
   - RiskAssessmentDto
   - RiskItemDto

2. **Enhance AI Service** (30 minutes)
   - Add methods to ILlamaService
   - Implement in LlamaService
   - Add prompt templates

3. **Build UI** (30 minutes)
   - Update AIInsights.razor
   - Add tabs and visualizations
   - Implement Q&A feature

4. **Test & Polish** (15 minutes)
   - Test all analysis types
   - Verify Ollama integration
   - Check UI responsiveness

---

## Step 11.6: Success Criteria

### ? Feature Complete When:

1. ? **Analysis Types Work**
   - Health check generates score
   - Risk analysis identifies risks
   - Optimization suggests improvements
   - Growth strategies provided

2. ? **UI Professional**
   - Health score gauge displays
   - Risk level indicator works
   - Tabs navigate smoothly
   - Custom Q&A functional

3. ? **AI Integration Solid**
   - Responses comprehensive
   - Context-aware answers
   - Fast response times (<30s)
   - Error handling robust

4. ? **User Experience Smooth**
   - Loading indicators clear
   - Progress visible
   - Results easy to read
   - Export works (future)

---

## Phase 11 Summary

### What We'll Build:
? Comprehensive AI insights dashboard  
? Multiple analysis types  
? Financial health scoring  
? Risk assessment system  
? Custom Q&A interface  
? Beautiful visualizations  

### Time Investment:
**Planned:** 60-90 minutes  
**Actual:** (to be recorded)

---

**Ready to start building Phase 11: AI Insights!** ???

Let me know when you're ready to begin implementation!
