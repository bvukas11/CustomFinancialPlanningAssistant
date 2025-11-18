using CustomFinancialPlanningAssistant.Core.DTOs;
using CustomFinancialPlanningAssistant.Core.Entities;
using CustomFinancialPlanningAssistant.Core.Enums;
using CustomFinancialPlanningAssistant.Services.Financial;
using Microsoft.Extensions.Logging;

namespace CustomFinancialPlanningAssistant.Services.Reports;

/// <summary>
/// Main report service that coordinates PDF and Excel report generation
/// </summary>
public class ReportService : IReportService
{
    private readonly ExcelReportService _excelService;
    private readonly IFinancialService _financialService;
    private readonly ILogger<ReportService> _logger;
    
    public ReportService(
        ExcelReportService excelService,
        IFinancialService financialService,
        ILogger<ReportService> logger)
    {
        _excelService = excelService;
        _financialService = financialService;
        _logger = logger;
    }
    
    public Task<byte[]> GeneratePdfReportAsync(int documentId, ReportType reportType)
    {
        // PDF generation temporarily disabled - use Excel instead
        throw new NotImplementedException("PDF generation is temporarily disabled. Please use Excel reports.");
    }
    
    public Task<byte[]> GenerateCustomPdfReportAsync(int documentId, ReportTemplate template)
    {
        throw new NotImplementedException("PDF generation is temporarily disabled.");
    }
    
    public Task<byte[]> GenerateComparisonPdfAsync(int documentId1, int documentId2)
    {
        throw new NotImplementedException("PDF generation is temporarily disabled.");
    }
    
    public async Task<byte[]> GenerateExcelReportAsync(int documentId, ReportType reportType)
    {
        var options = CreateOptionsForReportType(documentId, reportType);
        return await _excelService.GenerateExcelReportAsync(documentId, options);
    }
    
    public Task<byte[]> GenerateExcelSummaryAsync(List<int> documentIds)
    {
        throw new NotImplementedException("Multi-document summary not yet implemented.");
    }
    
    public async Task<byte[]> ExportFinancialDataAsync(int documentId)
    {
        return await _excelService.ExportFinancialDataAsync(documentId);
    }
    
    public Task<Report> SaveReportAsync(int documentId, string title, string content, ReportType reportType)
    {
        // Report persistence not yet implemented
        var report = new Report
        {
            Title = title,
            ReportType = reportType.ToString(),
            Content = content,
            GeneratedDate = DateTime.UtcNow,
            Parameters = System.Text.Json.JsonSerializer.Serialize(new { DocumentId = documentId })
        };
        
        return Task.FromResult(report);
    }
    
    public Task<Report?> GetReportByIdAsync(int reportId)
    {
        throw new NotImplementedException("Report persistence not yet implemented.");
    }
    
    public Task<List<Report>> GetReportsByDocumentIdAsync(int documentId)
    {
        return Task.FromResult(new List<Report>());
    }
    
    public Task<List<Report>> GetAllReportsAsync()
    {
        return Task.FromResult(new List<Report>());
    }
    
    public Task DeleteReportAsync(int reportId)
    {
        throw new NotImplementedException("Report persistence not yet implemented.");
    }
    
    public Task<ReportTemplate> CreateTemplateAsync(string name, string content)
    {
        throw new NotImplementedException("Report templates not yet implemented.");
    }
    
    public Task<List<ReportTemplate>> GetTemplatesAsync()
    {
        return Task.FromResult(new List<ReportTemplate>());
    }
    
    public Task<ReportTemplate?> GetTemplateByIdAsync(int templateId)
    {
        throw new NotImplementedException("Report templates not yet implemented.");
    }
    
    private ReportGenerationOptions CreateOptionsForReportType(int documentId, ReportType reportType)
    {
        return reportType switch
        {
            ReportType.Summary => new ReportGenerationOptions
            {
                DocumentId = documentId,
                ReportType = reportType,
                Title = "Financial Summary Report",
                IncludeSummary = true,
                IncludeDetailedData = false,
                IncludeRatios = false,
                IncludeCharts = false,
                IncludeAIInsights = false
            },
            ReportType.Detailed => new ReportGenerationOptions
            {
                DocumentId = documentId,
                ReportType = reportType,
                Title = "Detailed Financial Report",
                IncludeSummary = true,
                IncludeDetailedData = true,
                IncludeRatios = true,
                IncludeCharts = false,
                IncludeAIInsights = false
            },
            ReportType.RatioAnalysis => new ReportGenerationOptions
            {
                DocumentId = documentId,
                ReportType = reportType,
                Title = "Financial Ratio Analysis",
                IncludeSummary = true,
                IncludeDetailedData = false,
                IncludeRatios = true,
                IncludeCharts = false,
                IncludeAIInsights = false
            },
            _ => new ReportGenerationOptions
            {
                DocumentId = documentId,
                ReportType = reportType,
                Title = "Financial Report",
                IncludeSummary = true,
                IncludeDetailedData = true,
                IncludeRatios = true,
                IncludeCharts = false,
                IncludeAIInsights = false
            }
        };
    }
}
