using System.Text;
using System.Text.RegularExpressions;

namespace CustomFinancialPlanningAssistant.Services.AI;

/// <summary>
/// Utility class for parsing and formatting AI responses
/// </summary>
public static class AIResponseParser
{
    /// <summary>
    /// Extracts structured sections from AI response based on numbered headers or markdown
    /// </summary>
    /// <param name="response">The AI response text</param>
    /// <returns>Dictionary with section names as keys and content as values</returns>
    public static Dictionary<string, string> ParseSections(string response)
    {
        var sections = new Dictionary<string, string>();
        
        if (string.IsNullOrWhiteSpace(response))
        {
            return sections;
        }

        // Pattern for numbered sections: 1., 2., etc. or **Section Name**
        var sectionPattern = @"(?:^|\n)(?:#+\s+|\*\*|__)?(\d+\.?\s+[^\n:*_]+|\*\*[^*]+\*\*|__[^_]+__)(?:\*\*|__)?:?\s*\n((?:(?!\n(?:#+\s+|\*\*|__|\ d+\.)).)*";
        
        var matches = Regex.Matches(response, sectionPattern, RegexOptions.Multiline | RegexOptions.Singleline);

        foreach (Match match in matches)
        {
            if (match.Success && match.Groups.Count >= 3)
            {
                var title = CleanSectionTitle(match.Groups[1].Value);
                var content = match.Groups[2].Value.Trim();
                
                if (!string.IsNullOrWhiteSpace(title))
                {
                    sections[title] = content;
                }
            }
        }

        // If no sections found, try alternative parsing
        if (!sections.Any())
        {
            var lines = response.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            var currentSection = "Main Content";
            var currentContent = new StringBuilder();

            foreach (var line in lines)
            {
                if (IsLikelySectionHeader(line))
                {
                    if (currentContent.Length > 0)
                    {
                        sections[currentSection] = currentContent.ToString().Trim();
                        currentContent.Clear();
                    }
                    currentSection = CleanSectionTitle(line);
                }
                else
                {
                    currentContent.AppendLine(line);
                }
            }

            if (currentContent.Length > 0)
            {
                sections[currentSection] = currentContent.ToString().Trim();
            }
        }

        return sections;
    }

    /// <summary>
    /// Extracts key findings from the response as a list
    /// </summary>
    /// <param name="response">The AI response text</param>
    /// <returns>List of key findings</returns>
    public static List<string> ExtractKeyFindings(string response)
    {
        var findings = new List<string>();
        
        if (string.IsNullOrWhiteSpace(response))
        {
            return findings;
        }

        // Look for bullet points: -, *, •
        var bulletPattern = @"(?:^|\n)\s*[-*•]\s+(.+)(?:\n|$)";
        var matches = Regex.Matches(response, bulletPattern, RegexOptions.Multiline);

        foreach (Match match in matches)
        {
            if (match.Success && match.Groups.Count >= 2)
            {
                var finding = match.Groups[1].Value.Trim();
                if (!string.IsNullOrWhiteSpace(finding))
                {
                    findings.Add(finding);
                }
            }
        }

        // Look for numbered lists: 1., 2., etc.
        if (!findings.Any())
        {
            var numberedPattern = @"(?:^|\n)\s*\d+\.\s+(.+)(?:\n|$)";
            matches = Regex.Matches(response, numberedPattern, RegexOptions.Multiline);

            foreach (Match match in matches)
            {
                if (match.Success && match.Groups.Count >= 2)
                {
                    var finding = match.Groups[1].Value.Trim();
                    if (!string.IsNullOrWhiteSpace(finding))
                    {
                        findings.Add(finding);
                    }
                }
            }
        }

        return findings;
    }

    /// <summary>
    /// Extracts recommendations from the response
    /// </summary>
    /// <param name="response">The AI response text</param>
    /// <returns>List of recommendations</returns>
    public static List<string> ExtractRecommendations(string response)
    {
        var recommendations = new List<string>();
        
        if (string.IsNullOrWhiteSpace(response))
        {
            return recommendations;
        }

        // Look for recommendation section
        var recSectionPattern = @"(?i)(?:recommendation|suggest|action|next step)[s]?:?\s*\n((?:(?!\n\n).)+)";
        var match = Regex.Match(response, recSectionPattern, RegexOptions.Singleline);

        if (match.Success && match.Groups.Count >= 2)
        {
            var recSection = match.Groups[1].Value;
            
            // Extract individual recommendations from the section
            var bulletPattern = @"[-*•]\s+(.+)(?:\n|$)";
            var recMatches = Regex.Matches(recSection, bulletPattern);

            foreach (Match recMatch in recMatches)
            {
                if (recMatch.Success && recMatch.Groups.Count >= 2)
                {
                    var rec = recMatch.Groups[1].Value.Trim();
                    if (!string.IsNullOrWhiteSpace(rec))
                    {
                        recommendations.Add(rec);
                    }
                }
            }
        }

        // If no specific recommendations section, look for action-oriented statements
        if (!recommendations.Any())
        {
            var actionPattern = @"(?i)(?:should|must|need to|recommended to|consider|suggest)\s+([^.!?\n]+[.!?])";
            var matches = Regex.Matches(response, actionPattern);

            foreach (Match actionMatch in matches.Take(5)) // Limit to avoid noise
            {
                if (actionMatch.Success && actionMatch.Groups.Count >= 2)
                {
                    var action = actionMatch.Groups[1].Value.Trim();
                    if (!string.IsNullOrWhiteSpace(action))
                    {
                        recommendations.Add(action);
                    }
                }
            }
        }

        return recommendations;
    }

    /// <summary>
    /// Cleans and formats the response for display
    /// </summary>
    /// <param name="response">The AI response text</param>
    /// <returns>Formatted response</returns>
    public static string FormatForDisplay(string response)
    {
        if (string.IsNullOrWhiteSpace(response))
        {
            return string.Empty;
        }

        var formatted = response;

        // Remove excessive whitespace
        formatted = Regex.Replace(formatted, @"\n{3,}", "\n\n");
        formatted = Regex.Replace(formatted, @" {2,}", " ");

        // Ensure consistent line endings
        formatted = formatted.Replace("\r\n", "\n");

        // Add spacing after section headers
        formatted = Regex.Replace(formatted, @"(#{1,6}\s+.+)\n(?!\n)", "$1\n\n");

        // Trim
        formatted = formatted.Trim();

        return formatted;
    }

    /// <summary>
    /// Extracts numerical data (amounts, percentages, ratios) from the response
    /// </summary>
    /// <param name="response">The AI response text</param>
    /// <returns>Dictionary with labels and their numeric values</returns>
    public static Dictionary<string, decimal> ExtractNumericData(string response)
    {
        var numericData = new Dictionary<string, decimal>();
        
        if (string.IsNullOrWhiteSpace(response))
        {
            return numericData;
        }

        // Pattern for amounts with labels: "Revenue: $1,234.56" or "Total expenses: 5000"
        var amountPattern = @"([A-Za-z\s]+):\s*\$?\s*([\d,]+\.?\d*)\s*(?:USD|%)?";
        var matches = Regex.Matches(response, amountPattern);

        foreach (Match match in matches)
        {
            if (match.Success && match.Groups.Count >= 3)
            {
                var label = match.Groups[1].Value.Trim();
                var valueStr = match.Groups[2].Value.Replace(",", "");
                
                if (decimal.TryParse(valueStr, out var value))
                {
                    // Use first occurrence if duplicate labels
                    if (!numericData.ContainsKey(label))
                    {
                        numericData[label] = value;
                    }
                }
            }
        }

        return numericData;
    }

    /// <summary>
    /// Extracts percentage values from the response
    /// </summary>
    /// <param name="response">The AI response text</param>
    /// <returns>Dictionary with labels and their percentage values</returns>
    public static Dictionary<string, double> ExtractPercentages(string response)
    {
        var percentages = new Dictionary<string, double>();
        
        if (string.IsNullOrWhiteSpace(response))
        {
            return percentages;
        }

        // Pattern for percentages: "growth: 15.5%" or "increase of 20%"
        var percentPattern = @"([A-Za-z\s]+)(?:\s+of\s+|\s*:\s*)?([\d.]+)\s*%";
        var matches = Regex.Matches(response, percentPattern);

        foreach (Match match in matches)
        {
            if (match.Success && match.Groups.Count >= 3)
            {
                var label = match.Groups[1].Value.Trim();
                var valueStr = match.Groups[2].Value;
                
                if (double.TryParse(valueStr, out var value))
                {
                    if (!percentages.ContainsKey(label))
                    {
                        percentages[label] = value;
                    }
                }
            }
        }

        return percentages;
    }

    // ========== Private Helper Methods ==========

    private static string CleanSectionTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return string.Empty;
        }

        // Remove markdown formatting
        title = Regex.Replace(title, @"[#*_]+", "");
        
        // Remove numbering
        title = Regex.Replace(title, @"^\d+\.?\s*", "");
        
        // Remove colons
        title = title.Replace(":", "");
        
        return title.Trim();
    }

    private static bool IsLikelySectionHeader(string line)
    {
        if (string.IsNullOrWhiteSpace(line))
        {
            return false;
        }

        // Check for markdown headers
        if (line.TrimStart().StartsWith("#"))
        {
            return true;
        }

        // Check for bold markers
        if (line.Contains("**") || line.Contains("__"))
        {
            return true;
        }

        // Check for numbered sections
        if (Regex.IsMatch(line, @"^\s*\d+\.?\s+[A-Z]"))
        {
            return true;
        }

        // Check if line is short and ends with colon
        if (line.Length < 100 && line.TrimEnd().EndsWith(":"))
        {
            return true;
        }

        return false;
    }
}
