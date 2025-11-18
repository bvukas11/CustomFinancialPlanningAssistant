using CustomFinancialPlanningAssistant.Core.Enums;

namespace CustomFinancialPlanningAssistant.Core.DTOs;

/// <summary>
/// Template for generating custom reports
/// </summary>
public class ReportTemplate
{
    public int Id { get; set; }
    
    /// <summary>
    /// Template name
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Template description
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// JSON string containing template section definitions
    /// </summary>
    public string TemplateSections { get; set; } = string.Empty;
    
    /// <summary>
    /// Include financial summary section
    /// </summary>
    public bool IncludeSummary { get; set; }
    
    /// <summary>
    /// Include financial ratios section
    /// </summary>
    public bool IncludeRatios { get; set; }
    
    /// <summary>
    /// Include trend analysis section
    /// </summary>
    public bool IncludeTrends { get; set; }
    
    /// <summary>
    /// Include charts and visualizations
    /// </summary>
    public bool IncludeCharts { get; set; }
    
    /// <summary>
    /// Include AI-generated insights
    /// </summary>
    public bool IncludeAIInsights { get; set; }
    
    /// <summary>
    /// When the template was created
    /// </summary>
    public DateTime CreatedDate { get; set; }
    
    /// <summary>
    /// When the template was last modified
    /// </summary>
    public DateTime? LastModified { get; set; }
}

/// <summary>
/// Options for generating a financial report
/// </summary>
public class ReportGenerationOptions
{
    /// <summary>
    /// Document ID to generate report for
    /// </summary>
    public int DocumentId { get; set; }
    
    /// <summary>
    /// Type of report to generate
    /// </summary>
    public ReportType ReportType { get; set; }
    
    /// <summary>
    /// Include summary section
    /// </summary>
    public bool IncludeSummary { get; set; } = true;
    
    /// <summary>
    /// Include detailed data section
    /// </summary>
    public bool IncludeDetailedData { get; set; } = true;
    
    /// <summary>
    /// Include financial ratios section
    /// </summary>
    public bool IncludeRatios { get; set; } = true;
    
    /// <summary>
    /// Include charts and visualizations
    /// </summary>
    public bool IncludeCharts { get; set; } = true;
    
    /// <summary>
    /// Include AI-generated insights
    /// </summary>
    public bool IncludeAIInsights { get; set; } = false;
    
    /// <summary>
    /// Report title
    /// </summary>
    public string Title { get; set; } = "Financial Report";
    
    /// <summary>
    /// Report subtitle
    /// </summary>
    public string Subtitle { get; set; } = string.Empty;
    
    /// <summary>
    /// Company name to display on report
    /// </summary>
    public string CompanyName { get; set; } = string.Empty;
    
    /// <summary>
    /// Report date (defaults to current date)
    /// </summary>
    public DateTime? ReportDate { get; set; }
}
