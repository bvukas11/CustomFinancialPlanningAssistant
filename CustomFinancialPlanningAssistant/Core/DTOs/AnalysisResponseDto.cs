using System.Text.Json;
using CustomFinancialPlanningAssistant.Core.Enums;

namespace CustomFinancialPlanningAssistant.Core.DTOs;

/// <summary>
/// Data Transfer Object for AI analysis results
/// </summary>
public class AnalysisResponseDto
{
    /// <summary>
    /// ID of the analysis record
    /// </summary>
    public int AnalysisId { get; set; }

    /// <summary>
    /// ID of the analyzed document
    /// </summary>
    public int DocumentId { get; set; }

    /// <summary>
    /// Type of analysis performed
    /// </summary>
    public AnalysisType AnalysisType { get; set; }

    /// <summary>
    /// Brief summary of the analysis
    /// </summary>
    public string Summary { get; set; } = string.Empty;

    /// <summary>
    /// Detailed analysis text
    /// </summary>
    public string DetailedAnalysis { get; set; } = string.Empty;

    /// <summary>
    /// Key findings from the analysis
    /// </summary>
    public List<string> KeyFindings { get; set; }

    /// <summary>
    /// Actionable recommendations
    /// </summary>
    public List<string> Recommendations { get; set; }

    /// <summary>
    /// Chart data for visualization (JSON-serializable)
    /// </summary>
    public Dictionary<string, object>? ChartData { get; set; }

    /// <summary>
    /// Time taken to generate the analysis in milliseconds
    /// </summary>
    public int ExecutionTime { get; set; }

    /// <summary>
    /// Date and time when the analysis was generated
    /// </summary>
    public DateTime GeneratedDate { get; set; }

    /// <summary>
    /// AI model used for the analysis
    /// </summary>
    public string ModelUsed { get; set; } = string.Empty;

    /// <summary>
    /// Initializes a new instance with empty collections
    /// </summary>
    public AnalysisResponseDto()
    {
        KeyFindings = new List<string>();
        Recommendations = new List<string>();
        GeneratedDate = DateTime.UtcNow;
    }

    /// <summary>
    /// Serializes the response to JSON
    /// </summary>
    /// <returns>JSON string representation</returns>
    public string ToJson()
    {
        return JsonSerializer.Serialize(this, new JsonSerializerOptions
        {
            WriteIndented = true
        });
    }
}
