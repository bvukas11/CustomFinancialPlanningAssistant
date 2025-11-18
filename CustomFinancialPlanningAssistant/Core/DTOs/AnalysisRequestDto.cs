using System.ComponentModel.DataAnnotations;
using CustomFinancialPlanningAssistant.Core.Enums;

namespace CustomFinancialPlanningAssistant.Core.DTOs;

/// <summary>
/// Data Transfer Object for requesting AI analysis
/// </summary>
public class AnalysisRequestDto
{
    /// <summary>
    /// ID of the document to analyze
    /// </summary>
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "DocumentId must be greater than 0")]
    public int DocumentId { get; set; }

    /// <summary>
    /// Type of analysis to perform
    /// </summary>
    [Required]
    public AnalysisType AnalysisType { get; set; }

    /// <summary>
    /// Optional custom prompt for the AI
    /// </summary>
    public string? CustomPrompt { get; set; }

    /// <summary>
    /// Whether to include charts in the analysis
    /// </summary>
    public bool IncludeCharts { get; set; } = false;

    /// <summary>
    /// Start date for filtering data (optional)
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// End date for filtering data (optional)
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Period to compare against (for comparison analysis)
    /// </summary>
    public string? ComparisonPeriod { get; set; }

    /// <summary>
    /// Parameterless constructor
    /// </summary>
    public AnalysisRequestDto()
    {
    }

    /// <summary>
    /// Constructor with required parameters
    /// </summary>
    /// <param name="documentId">Document ID to analyze</param>
    /// <param name="analysisType">Type of analysis</param>
    public AnalysisRequestDto(int documentId, AnalysisType analysisType)
    {
        DocumentId = documentId;
        AnalysisType = analysisType;
    }

    /// <summary>
    /// Validates that EndDate is after StartDate if both are provided
    /// </summary>
    public bool IsValid(out string errorMessage)
    {
        if (StartDate.HasValue && EndDate.HasValue && EndDate.Value <= StartDate.Value)
        {
            errorMessage = "EndDate must be after StartDate";
            return false;
        }

        errorMessage = string.Empty;
        return true;
    }
}
