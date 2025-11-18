using CustomFinancialPlanningAssistant.Core.Entities;
using CustomFinancialPlanningAssistant.Core.DTOs;
using CustomFinancialPlanningAssistant.Core.Enums;

namespace CustomFinancialPlanningAssistant.Services.Reports;

/// <summary>
/// Service interface for report generation and management
/// </summary>
public interface IReportService
{
    /// <summary>
    /// Generates a PDF report for the specified document
    /// </summary>
    Task<byte[]> GeneratePdfReportAsync(int documentId, ReportType reportType);
    
    /// <summary>
    /// Generates a custom PDF report using a template
    /// </summary>
    Task<byte[]> GenerateCustomPdfReportAsync(int documentId, ReportTemplate template);
    
    /// <summary>
    /// Generates a comparison PDF report between two documents
    /// </summary>
    Task<byte[]> GenerateComparisonPdfAsync(int documentId1, int documentId2);
    
    /// <summary>
    /// Generates an Excel report for the specified document
    /// </summary>
    Task<byte[]> GenerateExcelReportAsync(int documentId, ReportType reportType);
    
    /// <summary>
    /// Generates an Excel summary for multiple documents
    /// </summary>
    Task<byte[]> GenerateExcelSummaryAsync(List<int> documentIds);
    
    /// <summary>
    /// Exports all financial data for a document to Excel
    /// </summary>
    Task<byte[]> ExportFinancialDataAsync(int documentId);
    
    /// <summary>
    /// Saves a generated report to the database
    /// </summary>
    Task<Report> SaveReportAsync(int documentId, string title, string content, ReportType reportType);
    
    /// <summary>
    /// Gets a report by ID
    /// </summary>
    Task<Report?> GetReportByIdAsync(int reportId);
    
    /// <summary>
    /// Gets all reports for a specific document
    /// </summary>
    Task<List<Report>> GetReportsByDocumentIdAsync(int documentId);
    
    /// <summary>
    /// Gets all reports
    /// </summary>
    Task<List<Report>> GetAllReportsAsync();
    
    /// <summary>
    /// Deletes a report
    /// </summary>
    Task DeleteReportAsync(int reportId);
    
    /// <summary>
    /// Creates a new report template
    /// </summary>
    Task<ReportTemplate> CreateTemplateAsync(string name, string content);
    
    /// <summary>
    /// Gets all available report templates
    /// </summary>
    Task<List<ReportTemplate>> GetTemplatesAsync();
    
    /// <summary>
    /// Gets a specific report template by ID
    /// </summary>
    Task<ReportTemplate?> GetTemplateByIdAsync(int templateId);
}
