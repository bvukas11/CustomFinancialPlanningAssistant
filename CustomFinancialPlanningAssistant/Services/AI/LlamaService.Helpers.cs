using CustomFinancialPlanningAssistant.Core.Entities;
using CustomFinancialPlanningAssistant.Core.DTOs;

namespace CustomFinancialPlanningAssistant.Services.AI;

/// <summary>
/// LlamaService partial class - Private Helper Methods
/// </summary>
public partial class LlamaService
{
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
        var profitMargin = summary.Revenue != 0 ? (summary.NetIncome / summary.Revenue * 100) : 0;
        var debtToEquity = ratios.GetValueOrDefault("DebtToEquity", 0);
        var currentRatio = ratios.GetValueOrDefault("CurrentRatio", 0);
        var expenseRatio = summary.Revenue != 0 ? (summary.Expenses / summary.Revenue * 100) : 0;

        return analysisType switch
        {
            "HealthCheck" => $@"You are a financial analyst. Use ONLY the calculations below. Do NOT interpret or add context.

FINANCIAL DATA (USE THESE EXACT NUMBERS):
Revenue: {summary.Revenue:C}
Expenses: {summary.Expenses:C}
Net Income: {summary.NetIncome:C}
Assets: {summary.Assets:C}
Liabilities: {summary.Liabilities:C}
Equity: {summary.Equity:C}

CALCULATED RATIOS (USE THESE EXACT NUMBERS):
Profit Margin: {profitMargin:F2}%
Expense Ratio: {expenseRatio:F2}%
Current Ratio: {currentRatio:F2}
Debt/Equity: {debtToEquity:F2}

ANALYSIS RULES:
- Use ONLY the numbers above
- Do NOT calculate additional metrics
- Do NOT add interpretation beyond the numbers
- Reference specific $ amounts and % values

FORMAT (MUST MATCH EXACTLY):

OVERALL RATING: {(profitMargin > 15 ? "Good" : profitMargin > 5 ? "Fair" : "Poor")}

STRENGTHS (EXACTLY 3):
1. Profit margin of {profitMargin:F2}% {(profitMargin > 10 ? "indicates strong profitability" : "shows profitability")}
2. Current ratio of {currentRatio:F2} {(currentRatio > 1.5m ? "demonstrates strong liquidity" : "indicates adequate liquidity")}
3. {(summary.NetIncome > 0 ? $"Positive net income of {summary.NetIncome:C}" : $"Revenue of {summary.Revenue:C}")} supports operations

CONCERNS (EXACTLY 3):
1. {(debtToEquity > 1 ? $"Debt-to-equity ratio of {debtToEquity:F2} indicates leverage risk" : $"Equity of {summary.Equity:C} requires monitoring")}
2. {(expenseRatio > 70 ? $"Expense ratio of {expenseRatio:F2}% is high relative to revenue" : $"Expenses of {summary.Expenses:C} represent {expenseRatio:F2}% of revenue")}
3. {(summary.Liabilities > summary.Assets * 0.5m ? $"Liabilities of {summary.Liabilities:C} are {(summary.Liabilities / summary.Assets * 100):F1}% of assets" : $"Asset base of {summary.Assets:C} requires strategic deployment")}

PRIORITY ACTIONS (EXACTLY 3):
1. {(expenseRatio > 70 ? "Reduce operating expenses to improve margins" : "Monitor expense growth relative to revenue")}
2. {(currentRatio < 1.5m ? "Improve working capital management" : "Maintain current liquidity levels")}
3. {(profitMargin < 10 ? "Increase revenue through market expansion or pricing optimization" : "Focus on operational efficiency to maintain profitability")}",
            "RiskAnalysis" => $@"You are a risk expert. Analyze ONLY these specific metrics. Do NOT add interpretation.

FINANCIAL METRICS (USE THESE EXACT NUMBERS):
Net Income: {summary.NetIncome:C}
Revenue: {summary.Revenue:C}
Expenses: {summary.Expenses:C}
Debt/Equity Ratio: {debtToEquity:F2}
Current Ratio: {currentRatio:F2}
Profit Margin: {profitMargin:F2}%
Expense Ratio: {expenseRatio:F2}%

RISK SCORING:
- Net Income < 0: High Risk
- Debt/Equity > 2: High Risk
- Current Ratio < 1: Medium Risk
- Expense Ratio > 80%: Medium Risk

FORMAT (MUST MATCH EXACTLY):

RISK LEVEL: {(summary.NetIncome < 0 || debtToEquity > 2 ? "High" : currentRatio < 1 || expenseRatio > 80 ? "Medium" : "Low")}

RISK FACTORS (EXACTLY 5):
1. {(summary.NetIncome < 0 ? $"Negative net income of {summary.NetIncome:C}" : $"Net income of {summary.NetIncome:C}")} - Severity: {(summary.NetIncome < 0 ? "High" : "Low")}
2. {(debtToEquity > 2 ? $"High debt-to-equity ratio of {debtToEquity:F2}" : $"Debt-to-equity ratio of {debtToEquity:F2}")} - Severity: {(debtToEquity > 2 ? "High" : debtToEquity > 1 ? "Medium" : "Low")}
3. {(currentRatio < 1 ? $"Low current ratio of {currentRatio:F2} indicates liquidity constraints" : $"Current ratio of {currentRatio:F2}")} - Severity: {(currentRatio < 1 ? "High" : currentRatio < 1.5m ? "Medium" : "Low")}
4. {(expenseRatio > 80 ? $"High expense ratio of {expenseRatio:F2}% reduces profitability" : $"Expense ratio of {expenseRatio:F2}%")} - Severity: {(expenseRatio > 80 ? "Medium" : "Low")}
5. {(summary.Liabilities > summary.Assets * 0.6m ? $"Liabilities at {(summary.Liabilities / summary.Assets * 100):F1}% of assets" : $"Liabilities of {summary.Liabilities:C}")} - Severity: {(summary.Liabilities > summary.Assets * 0.6m ? "Medium" : "Low")}

MITIGATION STRATEGIES (EXACTLY 3):
1. {(summary.NetIncome < 0 ? "Implement cost reduction program to achieve profitability" : "Monitor profit margins and maintain cost controls")}
2. {(debtToEquity > 1.5m ? "Reduce debt levels or increase equity to improve leverage ratio" : "Maintain balanced capital structure")}
3. {(currentRatio < 1.5m ? "Improve cash flow management and working capital" : "Continue prudent financial management")}",

            "Optimization" => $@"You are an optimization consultant. Analyze ONLY these metrics. Provide specific % targets.

CURRENT METRICS (USE THESE EXACT NUMBERS):
Revenue: {summary.Revenue:C}
Expenses: {summary.Expenses:C}
Net Income: {summary.NetIncome:C}
Profit Margin: {profitMargin:F2}%
Expense Ratio: {expenseRatio:F2}%

FORMAT (MUST MATCH EXACTLY):

OPTIMIZATION OPPORTUNITIES (EXACTLY 5):
1. Reduce operating expenses by 5-10% to save ${(summary.Expenses * 0.05m):N0} to ${(summary.Expenses * 0.10m):N0}
2. Increase revenue by 10-15% through market expansion to reach ${(summary.Revenue * 1.10m):N0} to ${(summary.Revenue * 1.15m):N0}
3. Improve profit margin from {profitMargin:F2}% to {(profitMargin * 1.2m):F2}% through efficiency gains
4. Reduce expense ratio from {expenseRatio:F2}% to {(expenseRatio * 0.90m):F2}% to improve profitability
5. Target net income increase of 20-30% to ${(summary.NetIncome * 1.20m):N0} to ${(summary.NetIncome * 1.30m):N0}",

            "Growth" => $@"You are a growth strategist. Base strategies ONLY on these metrics. Provide specific targets.

CURRENT METRICS (USE THESE EXACT NUMBERS):
Revenue: {summary.Revenue:C}
Net Income: {summary.NetIncome:C}
Profit Margin: {profitMargin:F2}%
Assets: {summary.Assets:C}

FORMAT (MUST MATCH EXACTLY):

GROWTH STRATEGIES (EXACTLY 5):
1. Expand into new markets to increase revenue by 15-20% to ${(summary.Revenue * 1.15m):N0}-${(summary.Revenue * 1.20m):N0}
2. Launch new products or services to add ${(summary.Revenue * 0.10m):N0} in additional revenue
3. Increase profit margin from {profitMargin:F2}% to {(profitMargin + 3):F2}% through operational improvements
4. Leverage asset base of {summary.Assets:C} to generate 10-15% ROA improvement
5. Target net income growth of 25% to ${(summary.NetIncome * 1.25m):N0} within 12 months",

            _ => $@"You are a financial analyst. Use ONLY these numbers. Do NOT interpret.

FINANCIAL DATA (USE THESE EXACT NUMBERS):
Revenue: {summary.Revenue:C}
Expenses: {summary.Expenses:C}
Net Income: {summary.NetIncome:C}
Assets: {summary.Assets:C}
Liabilities: {summary.Liabilities:C}
Profit Margin: {profitMargin:F2}%
Expense Ratio: {expenseRatio:F2}%

FORMAT (MUST MATCH EXACTLY):

KEY OBSERVATIONS (EXACTLY 3):
1. Revenue of {summary.Revenue:C} with profit margin of {profitMargin:F2}%
2. Expenses of {summary.Expenses:C} represent {expenseRatio:F2}% of revenue
3. Net income of {summary.NetIncome:C} {(summary.NetIncome > 0 ? "indicates profitability" : "requires attention")}

RECOMMENDATIONS (EXACTLY 3):
1. {(expenseRatio > 75 ? "Reduce expense ratio to improve profitability" : "Monitor expense growth")}
2. {(profitMargin < 15 ? "Increase profit margins through revenue growth or cost reduction" : "Maintain profitability")}
3. {(currentRatio < 1.5m ? "Improve liquidity and working capital management" : "Continue current financial management")}
"
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
        var risks = new List<string>();
        
        if (string.IsNullOrWhiteSpace(response))
        {
            return new List<string> { "No risk analysis available" };
        }
        
        // Try to find the RISK FACTORS section using multiple patterns
        var lines = response.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        bool inRiskSection = false;
        
        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i].Trim();
            
            // Look for the start of RISK FACTORS or CONCERNS section
            if (line.Contains("RISK FACTORS", StringComparison.OrdinalIgnoreCase) ||
                line.Contains("CONCERNS", StringComparison.OrdinalIgnoreCase))
            {
                inRiskSection = true;
                continue;
            }
            
            // Stop at next major section (but not sub-sections like "Severity:")
            if (inRiskSection && 
                (line.StartsWith("MITIGATION", StringComparison.OrdinalIgnoreCase) ||
                 line.StartsWith("PRIORITY", StringComparison.OrdinalIgnoreCase) ||
                 line.StartsWith("VALIDATION", StringComparison.OrdinalIgnoreCase) ||
                 line.StartsWith("STRENGTHS", StringComparison.OrdinalIgnoreCase) ||
                 line.StartsWith("KEY OBSERVATIONS", StringComparison.OrdinalIgnoreCase) ||
                 line.StartsWith("RECOMMENDATIONS", StringComparison.OrdinalIgnoreCase)))
            {
                break;
            }
            
            // Extract numbered items in the section (1., 2., 3., etc.)
            if (inRiskSection && System.Text.RegularExpressions.Regex.IsMatch(line, @"^\d+\.\s"))
            {
                // Remove the number prefix and any leading/trailing whitespace
                var risk = System.Text.RegularExpressions.Regex.Replace(line, @"^\d+\.\s*", "").Trim();
                
                // Only add if it's actual content (not empty and not a validation message)
                if (!string.IsNullOrEmpty(risk) && 
                    !risk.Contains("MUST", StringComparison.OrdinalIgnoreCase) &&
                    !risk.Contains("exactly", StringComparison.OrdinalIgnoreCase) &&
                    !risk.Contains("items", StringComparison.OrdinalIgnoreCase))
                {
                    risks.Add(risk);
                }
            }
        }
        
        // If structured extraction found nothing, try alternative patterns
        if (risks.Count == 0)
        {
            // Look for any numbered lists that might contain risks/concerns
            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i].Trim();
                if (System.Text.RegularExpressions.Regex.IsMatch(line, @"^\d+\.\s") &&
                    (line.ToLower().Contains("risk") || 
                     line.ToLower().Contains("concern") ||
                     line.ToLower().Contains("debt") ||
                     line.ToLower().Contains("equity") ||
                     line.ToLower().Contains("loss") ||
                     line.ToLower().Contains("negative")))
                {
                    var risk = System.Text.RegularExpressions.Regex.Replace(line, @"^\d+\.\s*", "").Trim();
                    if (!string.IsNullOrEmpty(risk))
                    {
                        risks.Add(risk);
                    }
                }
            }
        }
        
        // Last resort: if still nothing, provide meaningful default based on actual analysis
        if (risks.Count == 0)
        {
            _logger?.LogWarning("No risk factors extracted from AI response. Response length: {Length}", response.Length);
            return new List<string> { "Unable to extract structured risk analysis from AI response" };
        }
        
        return risks.Distinct().Take(5).ToList();
    }

    private List<string> ExtractOpportunities(string response)
    {
        var opportunities = new List<string>();
        
        if (string.IsNullOrWhiteSpace(response))
        {
            return new List<string> { "No opportunity analysis available" };
        }
        
        // Try to find numbered opportunity items in specific sections
        var lines = response.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        bool inOpportunitySection = false;
        
        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i].Trim();
            
            // Look for opportunities/growth/optimization sections
            if (line.Contains("OPPORTUNITIES", StringComparison.OrdinalIgnoreCase) ||
                line.Contains("GROWTH STRATEGIES", StringComparison.OrdinalIgnoreCase) ||
                line.Contains("OPTIMIZATION", StringComparison.OrdinalIgnoreCase) ||
                line.Contains("STRENGTHS", StringComparison.OrdinalIgnoreCase))
            {
                inOpportunitySection = true;
                continue;
            }
            
            // Stop at next major section
            if (inOpportunitySection && 
                (line.StartsWith("VALIDATION", StringComparison.OrdinalIgnoreCase) ||
                 line.StartsWith("RISK", StringComparison.OrdinalIgnoreCase) ||
                 line.StartsWith("CONCERNS", StringComparison.OrdinalIgnoreCase) ||
                 line.StartsWith("PRIORITY", StringComparison.OrdinalIgnoreCase)))
            {
                break;
            }
            
            // Extract numbered items (1., 2., 3., etc.)
            if (inOpportunitySection && System.Text.RegularExpressions.Regex.IsMatch(line, @"^\d+\.\s"))
            {
                var opportunity = System.Text.RegularExpressions.Regex.Replace(line, @"^\d+\.\s*", "").Trim();
                
                // Only add if it's actual content
                if (!string.IsNullOrEmpty(opportunity) &&
                    !opportunity.Contains("MUST", StringComparison.OrdinalIgnoreCase) &&
                    !opportunity.Contains("exactly", StringComparison.OrdinalIgnoreCase) &&
                    !opportunity.Contains("items", StringComparison.OrdinalIgnoreCase))
                {
                    opportunities.Add(opportunity);
                }
            }
        }
        
        // Alternative extraction if structured parsing failed
        if (opportunities.Count == 0)
        {
            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i].Trim();
                if (System.Text.RegularExpressions.Regex.IsMatch(line, @"^\d+\.\s") &&
                    (line.ToLower().Contains("opportunity") || 
                     line.ToLower().Contains("potential") ||
                     line.ToLower().Contains("growth") ||
                     line.ToLower().Contains("increase") ||
                     line.ToLower().Contains("improve") ||
                     line.ToLower().Contains("expand")))
                {
                    var opportunity = System.Text.RegularExpressions.Regex.Replace(line, @"^\d+\.\s*", "").Trim();
                    if (!string.IsNullOrEmpty(opportunity))
                    {
                        opportunities.Add(opportunity);
                    }
                }
            }
        }
        
        // Last resort
        if (opportunities.Count == 0)
        {
            _logger?.LogWarning("No opportunities extracted from AI response. Response length: {Length}", response.Length);
            return new List<string> { "Unable to extract structured opportunity analysis from AI response" };
        }
        
        return opportunities.Distinct().Take(5).ToList();
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

    // ========== Private Helper Methods for Phase 12 ==========

    private Dictionary<string, decimal> CalculateKeyMetrics(List<FinancialData> data)
    {
        var metrics = new Dictionary<string, decimal>();

        // Revenue metrics
        var revenue = data.Where(d => d.Category == "Revenue").Sum(d => d.Amount);
        metrics["Revenue"] = revenue;

        // Expense metrics
        var expenses = data.Where(d => d.Category == "Expense").Sum(d => d.Amount);
        metrics["Expenses"] = expenses;

        // Profit metrics
        var netIncome = revenue - expenses;
        metrics["NetIncome"] = netIncome;

        // Asset metrics
        var assets = data.Where(d => d.Category == "Asset").Sum(d => d.Amount);
        metrics["Assets"] = assets;

        // Liability metrics
        var liabilities = data.Where(d => d.Category == "Liability").Sum(d => d.Amount);
        metrics["Liabilities"] = liabilities;

        // Equity metrics
        var equity = data.Where(d => d.Category == "Equity").Sum(d => d.Amount);
        metrics["Equity"] = equity;

        // Ratio calculations
        if (revenue > 0)
        {
            metrics["GrossMargin"] = revenue > 0 ? ((revenue - expenses) / revenue) * 100 : 0;
            metrics["OperatingMargin"] = netIncome > 0 ? (netIncome / revenue) * 100 : 0;
            metrics["NetMargin"] = netIncome > 0 ? (netIncome / revenue) * 100 : 0;
        }

        if (liabilities > 0)
        {
            metrics["CurrentRatio"] = assets / liabilities;
            metrics["QuickRatio"] = (assets - liabilities) / liabilities; // Simplified
        }

        if (equity > 0)
        {
            metrics["DebtToEquity"] = liabilities / equity;
            metrics["ReturnOnEquity"] = netIncome > 0 ? (netIncome / equity) * 100 : 0;
        }

        if (assets > 0)
        {
            metrics["ReturnOnAssets"] = netIncome > 0 ? (netIncome / assets) * 100 : 0;
        }

        return metrics;
    }

    private string GetPerformanceRating(decimal variancePercentage)
    {
        return variancePercentage switch
        {
            > 25 => "Well Above Average",
            > 10 => "Above Average",
            >= -10 => "At Industry Average",
            >= -25 => "Below Average",
            _ => "Well Below Average"
        };
    }

    private decimal CalculatePercentileRanking(decimal companyValue, IndustryBenchmarkDto benchmark)
    {
        // Simplified percentile calculation based on standard deviations from mean
        var mean = benchmark.IndustryAverage;
        var stdDev = Math.Abs(benchmark.IndustryMedian - benchmark.IndustryAverage) * 0.5m; // Rough approximation

        if (stdDev == 0) return 50; // Default to median

        var zScore = (companyValue - mean) / stdDev;

        // Convert z-score to percentile (simplified)
        var percentile = 50 + (zScore * 34.13m); // Rough approximation
        return Math.Clamp(percentile, 0, 100);
    }

    private string GenerateMetricRecommendation(string metricName, string performanceRating, decimal variance)
    {
        return performanceRating switch
        {
            "Well Above Average" => $"Excellent performance in {metricName}. Maintain current strategies.",
            "Above Average" => $"Strong performance in {metricName}. Continue current approach with minor optimizations.",
            "At Industry Average" => $"Average performance in {metricName}. Focus on targeted improvements.",
            "Below Average" => $"{metricName} needs attention. Implement industry best practices.",
            "Well Below Average" => $"Critical improvement needed in {metricName}. Immediate action required.",
            _ => $"Review {metricName} performance and develop improvement plan."
        };
    }

    private CompetitivePositioningDto CalculateCompetitivePositioning(List<IndustryBenchmarkDto> benchmarks)
    {
        var aboveAverageCount = benchmarks.Count(b => b.PerformanceRating.Contains("Above"));
        var belowAverageCount = benchmarks.Count(b => b.PerformanceRating.Contains("Below"));
        var totalMetrics = benchmarks.Count;

        var competitiveScore = (aboveAverageCount * 100 + (totalMetrics - belowAverageCount - aboveAverageCount) * 50) / totalMetrics;

        string overallPosition = competitiveScore switch
        {
            >= 80 => "Leader",
            >= 60 => "Above Average",
            >= 40 => "Average",
            >= 20 => "Below Average",
            _ => "Laggard"
        };

        return new CompetitivePositioningDto
        {
            OverallPosition = overallPosition,
            CompetitiveScore = competitiveScore,
            StrengthsCount = aboveAverageCount,
            WeaknessesCount = belowAverageCount,
            CompetitiveAdvantages = benchmarks
                .Where(b => b.PerformanceRating.Contains("Above"))
                .Select(b => $"{b.MetricName}: {b.VariancePercentage:F1}% above industry average")
                .ToList(),
            CompetitiveDisadvantages = benchmarks
                .Where(b => b.PerformanceRating.Contains("Below"))
                .Select(b => $"{b.MetricName}: {Math.Abs(b.VariancePercentage):F1}% below industry average")
                .ToList(),
            MarketPositionSummary = $"Company ranks as {overallPosition} with {aboveAverageCount} strengths and {belowAverageCount} areas for improvement out of {totalMetrics} key metrics."
        };
    }

    // ========== Response Parsing Helpers for Phase 12 ==========

    private List<string> ExtractIndustryTrends(string response)
    {
        return response.Split('\n')
            .Where(line => line.ToLower().Contains("trend") ||
                          line.ToLower().Contains("industry") ||
                          line.ToLower().Contains("market"))
            .Take(5)
            .ToList();
    }

    private string ExtractExecutiveSummary(string response)
    {
        var lines = response.Split('\n').Take(3);
        return string.Join(" ", lines).Trim();
    }

    private string ExtractInvestmentRating(string response)
    {
        var lines = response.Split('\n')
            .Where(line => line.ToLower().Contains("buy") ||
                          line.ToLower().Contains("hold") ||
                          line.ToLower().Contains("sell"))
            .FirstOrDefault();
        return lines ?? "Hold";
    }

    private string ExtractConfidenceLevel(string response)
    {
        if (response.ToLower().Contains("high confidence")) return "High";
        if (response.ToLower().Contains("moderate confidence")) return "Moderate";
        if (response.ToLower().Contains("low confidence")) return "Low";
        return "Moderate";
    }

    private string ExtractTimeHorizon(string response)
    {
        if (response.ToLower().Contains("long-term")) return "Long-term (3-5 years)";
        if (response.ToLower().Contains("medium-term")) return "Medium-term (1-3 years)";
        if (response.ToLower().Contains("short-term")) return "Short-term (6-12 months)";
        return "Medium-term (1-3 years)";
    }

    private string ExtractExpectedReturns(string response)
    {
        var returnLines = response.Split('\n')
            .Where(line => line.ToLower().Contains("return") ||
                          line.ToLower().Contains("growth") ||
                          line.ToLower().Contains("yield"))
            .FirstOrDefault();
        return returnLines ?? "Moderate growth expected";
    }

    private List<string> ExtractAlternatives(string response)
    {
        return response.Split('\n')
            .Where(line => line.ToLower().Contains("alternative") ||
                          line.ToLower().Contains("option"))
            .Take(3)
            .ToList();
    }

    private List<string> ExtractImmediateActions(string response)
    {
        return response.Split('\n')
            .Where(line => line.ToLower().Contains("immediate") ||
                          line.ToLower().Contains("next 30") ||
                          line.ToLower().Contains("week"))
            .Take(5)
            .ToList();
    }

    private List<string> ExtractShortTermImprovements(string response)
    {
        return response.Split('\n')
            .Where(line => line.ToLower().Contains("short-term") ||
                          line.ToLower().Contains("3-6 months") ||
                          line.ToLower().Contains("quarter"))
            .Take(5)
            .ToList();
    }

    private List<string> ExtractLongTermStrategies(string response)
    {
        return response.Split('\n')
            .Where(line => line.ToLower().Contains("long-term") ||
                          line.ToLower().Contains("6-12 months") ||
                          line.ToLower().Contains("year"))
            .Take(5)
            .ToList();
    }

    private List<string> ExtractWorkingCapitalOptimizations(string response)
    {
        return response.Split('\n')
            .Where(line => line.ToLower().Contains("working capital") ||
                          line.ToLower().Contains("receivable") ||
                          line.ToLower().Contains("payable") ||
                          line.ToLower().Contains("inventory"))
            .Take(5)
            .ToList();
    }

    private List<string> ExtractCashGenerationStrategies(string response)
    {
        return response.Split('\n')
            .Where(line => line.ToLower().Contains("generation") ||
                          line.ToLower().Contains("revenue") ||
                          line.ToLower().Contains("sales"))
            .Take(5)
            .ToList();
    }

    private List<string> ExtractRiskMitigations(string response)
    {
        return response.Split('\n')
            .Where(line => line.ToLower().Contains("risk") ||
                          line.ToLower().Contains("mitigation") ||
                          line.ToLower().Contains("contingency"))
            .Take(5)
            .ToList();
    }

    private List<string> ExtractImplementationRoadmap(string response)
    {
        return response.Split('\n')
            .Where(line => line.ToLower().Contains("roadmap") ||
                          line.ToLower().Contains("timeline") ||
                          line.ToLower().Contains("milestone"))
            .Take(5)
            .ToList();
    }

    private List<string> ExtractSuccessMetrics(string response)
    {
        return response.Split('\n')
            .Where(line => line.ToLower().Contains("metric") ||
                          line.ToLower().Contains("measure") ||
                          line.ToLower().Contains("kpi"))
            .Take(5)
            .ToList();
    }

    private Dictionary<string, decimal> CalculateCashFlowMetrics(List<FinancialData> data)
    {
        var metrics = new Dictionary<string, decimal>();

        // Get basic financial summary
        var revenue = data.Where(d => d.Category == "Revenue").Sum(d => d.Amount);
        var expenses = data.Where(d => d.Category == "Expense").Sum(d => d.Amount);
        var assets = data.Where(d => d.Category == "Asset").Sum(d => d.Amount);
        var liabilities = data.Where(d => d.Category == "Liability").Sum(d => d.Amount);

        // Calculate operating cash flow (simplified)
        var operatingCash = revenue - expenses;
        metrics["OperatingCashFlow"] = operatingCash;

        // Calculate cash position from cash accounts
        var cashPosition = data.Where(d =>
            d.AccountName.Contains("Cash") ||
            d.AccountName.Contains("Cash Equivalents")).Sum(d => d.Amount);
        metrics["CashPosition"] = cashPosition > 0 ? cashPosition : assets * 0.1m; // Estimate if no cash account

        // Calculate burn rate (negative operating cash flow indicates burning cash)
        if (operatingCash < 0)
        {
            metrics["BurnRate"] = Math.Abs(operatingCash) / 12; // Monthly burn rate
            metrics["RunwayMonths"] = cashPosition > 0 ? cashPosition / metrics["BurnRate"] : 6; // Default 6 months if no cash
        }
        else
        {
            metrics["BurnRate"] = 0;
            metrics["RunwayMonths"] = 999; // No burn, unlimited runway
        }

        // Calculate working capital
        var currentAssets = data.Where(d =>
            d.Category == "Asset" &&
            !d.AccountName.Contains("Property") &&
            !d.AccountName.Contains("Equipment")).Sum(d => d.Amount);
        var currentLiabilities = data.Where(d =>
            d.Category == "Liability" &&
            !d.AccountName.Contains("Long-term")).Sum(d => d.Amount);
        metrics["WorkingCapital"] = currentAssets - currentLiabilities;

        // Additional metrics
        metrics["InvestingCashFlow"] = -assets * 0.05m; // Estimate based on asset base
        metrics["FinancingCashFlow"] = liabilities * 0.02m; // Estimate based on liability base
        metrics["NetCashFlow"] = operatingCash + metrics["InvestingCashFlow"] + metrics["FinancingCashFlow"];

        return metrics;
    }
}
