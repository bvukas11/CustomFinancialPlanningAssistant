using ClosedXML.Excel;
using CustomFinancialPlanningAssistant.Core.DTOs;
using CustomFinancialPlanningAssistant.Core.Entities;
using CustomFinancialPlanningAssistant.Infrastructure.Repositories;
using CustomFinancialPlanningAssistant.Services.Financial;
using Microsoft.Extensions.Logging;

namespace CustomFinancialPlanningAssistant.Services.Reports;

/// <summary>
/// Service for generating Excel reports
/// </summary>
public class ExcelReportService
{
    private readonly IFinancialService _financialService;
    private readonly IFinancialDocumentRepository _documentRepo;
    private readonly ILogger<ExcelReportService> _logger;
    
    public ExcelReportService(
        IFinancialService financialService,
        IFinancialDocumentRepository documentRepo,
        ILogger<ExcelReportService> logger)
    {
        _financialService = financialService;
        _documentRepo = documentRepo;
        _logger = logger;
    }
    
    public async Task<byte[]> GenerateExcelReportAsync(int documentId, ReportGenerationOptions options)
    {
        try
        {
            _logger.LogInformation($"Generating Excel report for document {documentId}");
            
            using var workbook = new XLWorkbook();
            
            // Get data
            var summary = await _financialService.GetFinancialSummaryAsync(documentId);
            var document = await _documentRepo.GetWithDataAsync(documentId);
            
            // Add Summary Sheet
            if (options.IncludeSummary)
            {
                AddSummarySheet(workbook, summary, options);
            }
            
            // Add Ratios Sheet
            if (options.IncludeRatios)
            {
                var ratios = await _financialService.CalculateFinancialRatiosAsync(documentId);
                AddRatiosSheet(workbook, ratios);
            }
            
            // Add Detailed Data Sheet
            if (options.IncludeDetailedData)
            {
                AddDetailedDataSheet(workbook, document.FinancialDataRecords.ToList());
            }
            
            // Save to memory stream
            using var memoryStream = new MemoryStream();
            workbook.SaveAs(memoryStream);
            
            _logger.LogInformation("Excel report generated successfully");
            return memoryStream.ToArray();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating Excel report");
            throw;
        }
    }
    
    private void AddSummarySheet(XLWorkbook workbook, FinancialSummaryDto summary, ReportGenerationOptions options)
    {
        var worksheet = workbook.Worksheets.Add("Summary");
        
        // Title
        worksheet.Cell(1, 1).Value = options.Title ?? "Financial Summary Report";
        worksheet.Cell(1, 1).Style.Font.Bold = true;
        worksheet.Cell(1, 1).Style.Font.FontSize = 16;
        worksheet.Range(1, 1, 1, 2).Merge();
        
        // Report date
        worksheet.Cell(2, 1).Value = $"Report Date: {DateTime.Now:yyyy-MM-dd}";
        worksheet.Range(2, 1, 2, 2).Merge();
        
        // Headers
        int row = 4;
        worksheet.Cell(row, 1).Value = "Metric";
        worksheet.Cell(row, 2).Value = "Amount";
        worksheet.Range(row, 1, row, 2).Style.Font.Bold = true;
        worksheet.Range(row, 1, row, 2).Style.Fill.BackgroundColor = XLColor.LightGray;
        
        // Data
        row++;
        AddExcelRow(worksheet, row++, "Total Revenue", summary.TotalRevenue, XLColor.LightGreen);
        AddExcelRow(worksheet, row++, "Total Expenses", summary.TotalExpenses, XLColor.LightPink);
        AddExcelRow(worksheet, row++, "Net Income", summary.NetIncome, 
            summary.NetIncome > 0 ? XLColor.LightGreen : XLColor.LightPink);
        row++;
        AddExcelRow(worksheet, row++, "Total Assets", summary.TotalAssets);
        AddExcelRow(worksheet, row++, "Total Liabilities", summary.TotalLiabilities);
        AddExcelRow(worksheet, row++, "Equity", summary.Equity);
        
        // Key Highlights
        if (summary.KeyHighlights.Any())
        {
            row += 2;
            worksheet.Cell(row, 1).Value = "Key Highlights";
            worksheet.Cell(row, 1).Style.Font.Bold = true;
            worksheet.Range(row, 1, row, 2).Merge();
            
            row++;
            foreach (var highlight in summary.KeyHighlights)
            {
                worksheet.Cell(row, 1).Value = "• " + highlight;
                worksheet.Range(row, 1, row, 2).Merge();
                row++;
            }
        }
        
        // Auto-fit columns
        worksheet.Columns().AdjustToContents();
    }
    
    private void AddRatiosSheet(XLWorkbook workbook, Dictionary<string, decimal> ratios)
    {
        var worksheet = workbook.Worksheets.Add("Financial Ratios");
        
        // Title
        worksheet.Cell(1, 1).Value = "Financial Ratios Analysis";
        worksheet.Cell(1, 1).Style.Font.Bold = true;
        worksheet.Cell(1, 1).Style.Font.FontSize = 14;
        worksheet.Range(1, 1, 1, 2).Merge();
        
        // Headers
        int row = 3;
        worksheet.Cell(row, 1).Value = "Ratio Name";
        worksheet.Cell(row, 2).Value = "Value";
        worksheet.Range(row, 1, row, 2).Style.Font.Bold = true;
        worksheet.Range(row, 1, row, 2).Style.Fill.BackgroundColor = XLColor.LightGray;
        
        // Data
        row++;
        foreach (var ratio in ratios)
        {
            worksheet.Cell(row, 1).Value = FormatRatioName(ratio.Key);
            worksheet.Cell(row, 2).Value = ratio.Value;
            worksheet.Cell(row, 2).Style.NumberFormat.Format = "#,##0.00";
            row++;
        }
        
        worksheet.Columns().AdjustToContents();
    }
    
    private void AddDetailedDataSheet(XLWorkbook workbook, List<FinancialData> data)
    {
        var worksheet = workbook.Worksheets.Add("Detailed Data");
        
        // Title
        worksheet.Cell(1, 1).Value = "Detailed Financial Data";
        worksheet.Cell(1, 1).Style.Font.Bold = true;
        worksheet.Cell(1, 1).Style.Font.FontSize = 14;
        
        // Headers
        int row = 3;
        string[] headers = { "Account Name", "Account Code", "Category", "Sub Category", "Period", "Amount", "Currency", "Date" };
        for (int col = 1; col <= headers.Length; col++)
        {
            worksheet.Cell(row, col).Value = headers[col - 1];
            worksheet.Cell(row, col).Style.Font.Bold = true;
            worksheet.Cell(row, col).Style.Fill.BackgroundColor = XLColor.LightGray;
        }
        
        // Data
        row++;
        foreach (var item in data.OrderBy(d => d.Category).ThenBy(d => d.AccountName))
        {
            worksheet.Cell(row, 1).Value = item.AccountName;
            worksheet.Cell(row, 2).Value = item.AccountCode;
            worksheet.Cell(row, 3).Value = item.Category;
            worksheet.Cell(row, 4).Value = item.SubCategory;
            worksheet.Cell(row, 5).Value = item.Period;
            worksheet.Cell(row, 6).Value = item.Amount;
            worksheet.Cell(row, 6).Style.NumberFormat.Format = "$#,##0.00";
            worksheet.Cell(row, 7).Value = item.Currency;
            worksheet.Cell(row, 8).Value = item.DateRecorded.ToString("yyyy-MM-dd");
            
            row++;
        }
        
        // Add filters
        var dataRange = worksheet.Range(3, 1, row - 1, headers.Length);
        dataRange.SetAutoFilter();
        
        worksheet.Columns().AdjustToContents();
    }
    
    public async Task<byte[]> ExportFinancialDataAsync(int documentId)
    {
        _logger.LogInformation($"Exporting financial data for document {documentId}");
        
        var document = await _documentRepo.GetWithDataAsync(documentId);
        if (document == null)
        {
            throw new ArgumentException($"Document {documentId} not found");
        }
        
        using var workbook = new XLWorkbook();
        AddDetailedDataSheet(workbook, document.FinancialDataRecords.ToList());
        
        using var memoryStream = new MemoryStream();
        workbook.SaveAs(memoryStream);
        
        return memoryStream.ToArray();
    }
    
    // Helper methods
    private void AddExcelRow(IXLWorksheet worksheet, int row, string label, decimal value, XLColor color = default)
    {
        worksheet.Cell(row, 1).Value = label;
        worksheet.Cell(row, 2).Value = value;
        worksheet.Cell(row, 2).Style.NumberFormat.Format = "$#,##0.00";
        
        if (color != default(XLColor))
        {
            worksheet.Range(row, 1, row, 2).Style.Fill.BackgroundColor = color;
        }
    }
    
    private string FormatRatioName(string ratioKey)
    {
        return System.Text.RegularExpressions.Regex.Replace(ratioKey, "([A-Z])", " $1").Trim();
    }
}
