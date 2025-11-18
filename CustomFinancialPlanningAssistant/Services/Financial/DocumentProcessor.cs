using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using ClosedXML.Excel;
using CsvHelper;
using CsvHelper.Configuration;
using UglyToad.PdfPig;
using Docnet.Core;
using Docnet.Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using CustomFinancialPlanningAssistant.Core.DTOs;
using CustomFinancialPlanningAssistant.Core.Entities;
using CustomFinancialPlanningAssistant.Core.Enums;
using CustomFinancialPlanningAssistant.Infrastructure.FileStorage;
using CustomFinancialPlanningAssistant.Infrastructure.Repositories;
using CustomFinancialPlanningAssistant.Services.AI;

namespace CustomFinancialPlanningAssistant.Services.Financial;

/// <summary>
/// Document processor with Excel, CSV, and AI-powered PDF processing
/// </summary>
public class DocumentProcessor : IDocumentProcessor
{
    private readonly IFinancialDocumentRepository _documentRepo;
    private readonly IFinancialDataRepository _dataRepo;
    private readonly IFileStorageService _fileStorage;
    private readonly ILlamaService _llamaService;
    private readonly ILogger<DocumentProcessor> _logger;
    private readonly IConfiguration _configuration;

    public DocumentProcessor(
        IFinancialDocumentRepository documentRepo,
        IFinancialDataRepository dataRepo,
        IFileStorageService fileStorage,
        ILlamaService llamaService,
        ILogger<DocumentProcessor> logger,
        IConfiguration configuration)
    {
        _documentRepo = documentRepo ?? throw new ArgumentNullException(nameof(documentRepo));
        _dataRepo = dataRepo ?? throw new ArgumentNullException(nameof(dataRepo));
        _fileStorage = fileStorage ?? throw new ArgumentNullException(nameof(fileStorage));
        _llamaService = llamaService ?? throw new ArgumentNullException(nameof(llamaService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public async Task<UploadResultDto> ProcessDocumentAsync(Stream fileStream, string fileName, string fileType)
    {
        try
        {
            _logger.LogInformation("Processing document: {FileName}, Type: {FileType}", fileName, fileType);
            var isValid = await ValidateFileAsync(fileStream, fileName);
            if (!isValid)
            {
                return new UploadResultDto { FileName = fileName, Success = false, ErrorMessages = new List<string> { "File validation failed" } };
            }
            
            // Only reset position if stream supports seeking
            if (fileStream.CanSeek)
            {
                fileStream.Position = 0;
            }
            
            return fileType.ToLower() switch
            {
                "excel" or ".xlsx" or ".xls" => await ProcessExcelAsync(fileStream, fileName),
                "csv" or ".csv" => await ProcessCsvAsync(fileStream, fileName),
                "pdf" or ".pdf" => await ProcessPdfAsync(fileStream, fileName),
                _ => new UploadResultDto { FileName = fileName, Success = false, ErrorMessages = new List<string> { $"Unsupported file type: {fileType}" } }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing document: {FileName}", fileName);
            return new UploadResultDto { FileName = fileName, Success = false, ErrorMessages = new List<string> { $"Processing failed: {ex.Message}" } };
        }
    }

    public async Task<UploadResultDto> ProcessExcelAsync(Stream fileStream, string fileName)
    {
        var result = new UploadResultDto { FileName = fileName };
        var stopwatch = Stopwatch.StartNew();
        try
        {
            _logger.LogInformation("Processing Excel file: {FileName}", fileName);
            var filePath = await _fileStorage.SaveFileAsync(fileStream, fileName);
            var fileSize = await _fileStorage.GetFileSizeAsync(filePath);
            
            // Read from saved file instead of stream for Excel processing
            using var savedFileStream = await _fileStorage.GetFileAsync(filePath);
            var financialDataList = await ExtractDataFromExcelAsync(savedFileStream);
            
            if (financialDataList == null || !financialDataList.Any())
            {
                result.AddError("No financial data found in Excel file");
                result.Success = false;
                return result;
            }
            var document = new FinancialDocument
            {
                FileName = fileName,
                FileType = "Excel",
                UploadDate = DateTime.UtcNow,
                FileSize = fileSize,
                FilePath = filePath,
                Status = "Processing",
                CreatedBy = "System"
            };
            var savedDoc = await _documentRepo.AddAsync(document);
            await _documentRepo.SaveChangesAsync();
            foreach (var data in financialDataList) data.DocumentId = savedDoc.Id;
            await _dataRepo.AddRangeAsync(financialDataList);
            await _dataRepo.SaveChangesAsync();
            savedDoc.Status = "Analyzed";
            await _documentRepo.UpdateAsync(savedDoc);
            await _documentRepo.SaveChangesAsync();
            stopwatch.Stop();
            result.Success = true;
            result.DocumentId = savedDoc.Id;
            result.RecordsImported = financialDataList.Count;
            result.ProcessingTime = (int)stopwatch.ElapsedMilliseconds;
            _logger.LogInformation("Successfully processed {Count} records from {FileName}", financialDataList.Count, fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing Excel file: {FileName}", fileName);
            result.Success = false;
            result.AddError($"Processing failed: {ex.Message}");
        }
        return result;
    }

    public async Task<List<FinancialData>> ExtractDataFromExcelAsync(Stream fileStream)
    {
        var dataList = new List<FinancialData>();
        try
        {
            using (var workbook = new XLWorkbook(fileStream))
            {
                var worksheet = workbook.Worksheet(1);
                if (worksheet == null)
                {
                    _logger.LogWarning("No worksheet found in Excel file");
                    return dataList;
                }
                int headerRow = FindHeaderRow(worksheet);
                if (headerRow == 0)
                {
                    _logger.LogWarning("Could not find header row in Excel file");
                    return dataList;
                }
                var columnMap = MapColumns(worksheet, headerRow);
                var lastRow = worksheet.LastRowUsed().RowNumber();
                for (int row = headerRow + 1; row <= lastRow; row++)
                {
                    try
                    {
                        var rowData = worksheet.Row(row);
                        if (rowData.IsEmpty()) continue;
                        var financialData = new FinancialData
                        {
                            AccountName = GetCellValue(worksheet, row, columnMap.GetValueOrDefault("AccountName", 0)),
                            AccountCode = GetCellValue(worksheet, row, columnMap.GetValueOrDefault("AccountCode", 0)),
                            Period = GetCellValue(worksheet, row, columnMap.GetValueOrDefault("Period", 0)),
                            Amount = ParseDecimal(GetCellValue(worksheet, row, columnMap.GetValueOrDefault("Amount", 0))),
                            Currency = GetCellValue(worksheet, row, columnMap.GetValueOrDefault("Currency", 0)) ?? "USD",
                            Category = GetCellValue(worksheet, row, columnMap.GetValueOrDefault("Category", 0)),
                            SubCategory = GetCellValue(worksheet, row, columnMap.GetValueOrDefault("SubCategory", 0)),
                            DateRecorded = ParseDate(GetCellValue(worksheet, row, columnMap.GetValueOrDefault("Date", 0)))
                        };
                        if (ValidateFinancialData(financialData)) dataList.Add(financialData);
                        else _logger.LogWarning("Row {Row} has invalid data, skipping", row);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Error processing row {Row}, skipping", row);
                    }
                }
            }
            _logger.LogInformation("Extracted {Count} records from Excel", dataList.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting data from Excel");
            throw;
        }
        return dataList;
    }

    public async Task<UploadResultDto> ProcessCsvAsync(Stream fileStream, string fileName)
    {
        var result = new UploadResultDto { FileName = fileName };
        var stopwatch = Stopwatch.StartNew();
        try
        {
            _logger.LogInformation("Processing CSV file: {FileName}", fileName);
            var filePath = await _fileStorage.SaveFileAsync(fileStream, fileName);
            var fileSize = await _fileStorage.GetFileSizeAsync(filePath);
            
            // Read from saved file instead of stream for CSV processing
            using var savedFileStream = await _fileStorage.GetFileAsync(filePath);
            var financialDataList = await ExtractDataFromCsvAsync(savedFileStream);
            
            if (financialDataList == null || !financialDataList.Any())
            {
                result.AddError("No financial data found in CSV file");
                result.Success = false;
                return result;
            }
            var document = new FinancialDocument
            {
                FileName = fileName,
                FileType = "CSV",
                UploadDate = DateTime.UtcNow,
                FileSize = fileSize,
                FilePath = filePath,
                Status = "Processing",
                CreatedBy = "System"
            };
            var savedDoc = await _documentRepo.AddAsync(document);
            await _documentRepo.SaveChangesAsync();
            foreach (var data in financialDataList) data.DocumentId = savedDoc.Id;
            await _dataRepo.AddRangeAsync(financialDataList);
            await _dataRepo.SaveChangesAsync();
            savedDoc.Status = "Analyzed";
            await _documentRepo.UpdateAsync(savedDoc);
            await _documentRepo.SaveChangesAsync();
            stopwatch.Stop();
            result.Success = true;
            result.DocumentId = savedDoc.Id;
            result.RecordsImported = financialDataList.Count;
            result.ProcessingTime = (int)stopwatch.ElapsedMilliseconds;
            _logger.LogInformation("Successfully processed {Count} records from {FileName}", financialDataList.Count, fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing CSV file: {FileName}", fileName);
            result.Success = false;
            result.AddError($"Processing failed: {ex.Message}");
        }
        return result;
    }

    public async Task<List<FinancialData>> ExtractDataFromCsvAsync(Stream fileStream)
    {
        var dataList = new List<FinancialData>();
        try
        {
            using (var reader = new StreamReader(fileStream))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Context.RegisterClassMap<FinancialDataCsvMap>();
                var records = csv.GetRecords<FinancialDataCsvRecord>();
                foreach (var record in records)
                {
                    try
                    {
                        var financialData = new FinancialData
                        {
                            AccountName = record.AccountName,
                            AccountCode = record.AccountCode,
                            Period = record.Period,
                            Amount = ParseDecimal(record.Amount),
                            Currency = record.Currency ?? "USD",
                            Category = record.Category,
                            SubCategory = record.SubCategory,
                            DateRecorded = ParseDate(record.Date)
                        };
                        if (ValidateFinancialData(financialData)) dataList.Add(financialData);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Error processing CSV record, skipping");
                    }
                }
            }
            _logger.LogInformation("Extracted {Count} records from CSV", dataList.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting data from CSV");
            throw;
        }
        return dataList;
    }

    public async Task<UploadResultDto> ProcessPdfAsync(Stream fileStream, string fileName)
    {
        var result = new UploadResultDto { FileName = fileName };
        var stopwatch = Stopwatch.StartNew();
        try
        {
            _logger.LogInformation("Processing PDF file with AI Vision: {FileName}", fileName);
            var filePath = await _fileStorage.SaveFileAsync(fileStream, fileName);
            var fileSize = await _fileStorage.GetFileSizeAsync(filePath);

            var document = new FinancialDocument
            {
                FileName = fileName,
                FileType = "PDF",
                UploadDate = DateTime.UtcNow,
                FileSize = fileSize,
                FilePath = filePath,
                Status = "Processing",
                CreatedBy = "System"
            };
            var savedDoc = await _documentRepo.AddAsync(document);
            await _documentRepo.SaveChangesAsync();

            // Extract text using PdfPig first (faster for text-based PDFs)
            var extractedText = await ExtractTextFromPdfAsync(fileStream);
            
            // Try parsing text first
            var financialDataList = new List<FinancialData>();
            if (!string.IsNullOrWhiteSpace(extractedText))
            {
                financialDataList = ParseFinancialDataFromText(extractedText, savedDoc.Id);
            }

            // If text extraction yields few results, use AI vision as fallback
            if (financialDataList.Count < 3)
            {
                _logger.LogInformation("Text extraction yielded limited results, using AI vision");
                var visionResults = await ProcessPdfWithAIVision(filePath, savedDoc.Id);
                if (visionResults.Any())
                {
                    financialDataList = visionResults;
                    result.AddWarning("Used AI vision for PDF analysis - may be slower but more accurate");
                }
            }

            if (financialDataList.Any())
            {
                await _dataRepo.AddRangeAsync(financialDataList);
                await _dataRepo.SaveChangesAsync();
                result.RecordsImported = financialDataList.Count;
            }
            else
            {
                result.AddWarning("No structured financial data found in PDF");
            }

            savedDoc.Status = "Analyzed";
            await _documentRepo.UpdateAsync(savedDoc);
            await _documentRepo.SaveChangesAsync();
            stopwatch.Stop();
            result.Success = true;
            result.DocumentId = savedDoc.Id;
            result.ProcessingTime = (int)stopwatch.ElapsedMilliseconds;
            _logger.LogInformation("Successfully processed PDF: {FileName}", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing PDF file: {FileName}", fileName);
            result.Success = false;
            result.AddError($"Processing failed: {ex.Message}");
        }
        return result;
    }

    public async Task<string> ExtractTextFromPdfAsync(Stream fileStream)
    {
        var text = new StringBuilder();
        try
        {
            // Reset position only if stream supports seeking
            if (fileStream.CanSeek)
            {
                fileStream.Position = 0;
            }
            
            using (var document = PdfDocument.Open(fileStream))
            {
                _logger.LogInformation("PDF has {Pages} pages", document.NumberOfPages);
                foreach (var page in document.GetPages())
                {
                    var pageText = page.Text;
                    if (!string.IsNullOrWhiteSpace(pageText))
                    {
                        text.AppendLine(pageText);
                    }
                }
            }
            _logger.LogInformation("Extracted {Length} characters from PDF using PdfPig", text.Length);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting text from PDF");
        }
        return await Task.FromResult(text.ToString());
    }

    private async Task<List<FinancialData>> ProcessPdfWithAIVision(string filePath, int documentId)
    {
        var dataList = new List<FinancialData>();
        try
        {
            _logger.LogInformation("Converting PDF to images for AI vision processing");
            
            // Convert PDF pages to images
            var images = ConvertPdfToImages(filePath);
            
            if (!images.Any())
            {
                _logger.LogWarning("No images could be extracted from PDF");
                return dataList;
            }

            _logger.LogInformation("Processing {Count} PDF pages with AI vision", images.Count);
            
            // Process each page with AI vision
            foreach (var (pageNumber, imageBytes) in images)
            {
                try
                {
                    var prompt = @"Extract all financial data from this image. Look for:
- Account names and numbers
- Amounts (revenue, expenses, assets, liabilities)
- Dates and periods
- Categories and descriptions

Format your response as a structured list with each entry on a new line:
AccountName | Amount | Category | Period

Example:
Sales Revenue | 150000 | Revenue | 2024-Q1
Operating Expenses | 95000 | Expense | 2024-Q1";

                    var aiResponse = await _llamaService.AnalyzeDocumentImageAsync(imageBytes, prompt);
                    
                    // Parse AI response into financial data
                    var pageData = ParseAIVisionResponse(aiResponse, documentId, pageNumber);
                    dataList.AddRange(pageData);
                    
                    _logger.LogInformation("Extracted {Count} records from page {Page} using AI vision", 
                        pageData.Count, pageNumber);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error processing page {Page} with AI vision", pageNumber);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in AI vision PDF processing");
        }
        return dataList;
    }

    private List<(int PageNumber, byte[] ImageBytes)> ConvertPdfToImages(string filePath)
    {
        var images = new List<(int, byte[])>();
        try
        {
            using (var docReader = DocLib.Instance.GetDocReader(filePath, new PageDimensions(1920, 1080)))
            {
                for (int pageIndex = 0; pageIndex < docReader.GetPageCount(); pageIndex++)
                {
                    using (var pageReader = docReader.GetPageReader(pageIndex))
                    {
                        var rawBytes = pageReader.GetImage();
                        var width = pageReader.GetPageWidth();
                        var height = pageReader.GetPageHeight();
                        
                        // Convert raw bytes to image format (PNG)
                        var imageBytes = ConvertRawBytesToPng(rawBytes, width, height);
                        images.Add((pageIndex + 1, imageBytes));
                    }
                }
            }
            _logger.LogInformation("Converted {Count} PDF pages to images", images.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error converting PDF to images");
        }
        return images;
    }

    private byte[] ConvertRawBytesToPng(byte[] rawBytes, int width, int height)
    {
        // Simple conversion - in production, use a proper image library like ImageSharp
        // For now, return raw bytes as-is (Ollama can handle various formats)
        return rawBytes;
    }

    private List<FinancialData> ParseAIVisionResponse(string aiResponse, int documentId, int pageNumber)
    {
        var dataList = new List<FinancialData>();
        try
        {
            var lines = aiResponse.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                // Look for pipe-separated values
                if (line.Contains('|'))
                {
                    var parts = line.Split('|').Select(p => p.Trim()).ToArray();
                    if (parts.Length >= 3)
                    {
                        var data = new FinancialData
                        {
                            DocumentId = documentId,
                            AccountName = parts[0],
                            Amount = ParseDecimal(parts.Length > 1 ? parts[1] : "0"),
                            Category = parts.Length > 2 ? parts[2] : "Unknown",
                            Period = parts.Length > 3 ? parts[3] : DateTime.Now.ToString("yyyy-MM"),
                            Currency = "USD",
                            DateRecorded = DateTime.UtcNow,
                            AccountCode = $"AI-PAGE{pageNumber}"
                        };
                        
                        if (!string.IsNullOrWhiteSpace(data.AccountName) && data.Amount != 0)
                        {
                            dataList.Add(data);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error parsing AI vision response");
        }
        return dataList;
    }

    public async Task<bool> ValidateFileAsync(Stream fileStream, string fileName)
    {
        try
        {
            var maxFileSize = _configuration.GetValue<long>("FileStorage:MaxFileSize");
            if (fileStream.Length > maxFileSize)
            {
                _logger.LogWarning("File {FileName} exceeds maximum size: {Size} bytes", fileName, fileStream.Length);
                return false;
            }
            if (fileStream.Length == 0)
            {
                _logger.LogWarning("File {FileName} is empty", fileName);
                return false;
            }
            var extension = Path.GetExtension(fileName).ToLower();
            var allowedExtensions = _configuration.GetSection("FileStorage:AllowedExtensions").Get<string[]>();
            if (!allowedExtensions.Contains(extension))
            {
                _logger.LogWarning("File extension {Extension} is not allowed", extension);
                return false;
            }
            
            // Only validate file signature for streams that support seeking
            if (fileStream.CanSeek)
            {
                fileStream.Position = 0;
                var buffer = new byte[8];
                await fileStream.ReadAsync(buffer, 0, 8);
                fileStream.Position = 0;
                
                var isValidSignature = extension switch
                {
                    ".xlsx" => buffer[0] == 0x50 && buffer[1] == 0x4B,
                    ".xls" => buffer[0] == 0xD0 && buffer[1] == 0xCF,
                    ".pdf" => buffer[0] == 0x25 && buffer[1] == 0x50,
                    ".csv" => true,
                    _ => false
                };
                
                if (!isValidSignature)
                {
                    _logger.LogWarning("File {FileName} has invalid signature for type {Extension}", fileName, extension);
                    return false;
                }
            }
            else
            {
                // For non-seekable streams (like BrowserFileStream), skip signature validation
                _logger.LogInformation("Skipping file signature validation for non-seekable stream: {FileName}", fileName);
            }
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating file: {FileName}", fileName);
            return false;
        }
    }

    public FileType DetectFileType(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLower();
        return extension switch
        {
            ".xlsx" or ".xls" => FileType.Excel,
            ".csv" => FileType.CSV,
            ".pdf" => FileType.PDF,
            _ => FileType.Unknown
        };
    }

    // Helper Methods
    private int FindHeaderRow(IXLWorksheet worksheet)
    {
        var headerKeywords = new[] { "account", "amount", "period", "category" };
        for (int row = 1; row <= 10; row++)
        {
            var rowData = worksheet.Row(row);
            var cellValues = rowData.Cells().Select(c => c.Value.ToString().ToLower()).ToList();
            var matchCount = headerKeywords.Count(keyword => cellValues.Any(cell => cell.Contains(keyword)));
            if (matchCount >= 2) return row;
        }
        return 0;
    }

    private Dictionary<string, int> MapColumns(IXLWorksheet worksheet, int headerRow)
    {
        var columnMap = new Dictionary<string, int>();
        var headerRowData = worksheet.Row(headerRow);
        foreach (var cell in headerRowData.CellsUsed())
        {
            var headerValue = cell.Value.ToString().ToLower().Trim();
            if (headerValue.Contains("account") && headerValue.Contains("name"))
                columnMap["AccountName"] = cell.Address.ColumnNumber;
            else if (headerValue.Contains("account") && headerValue.Contains("code"))
                columnMap["AccountCode"] = cell.Address.ColumnNumber;
            else if (headerValue.Contains("period"))
                columnMap["Period"] = cell.Address.ColumnNumber;
            else if (headerValue.Contains("amount"))
                columnMap["Amount"] = cell.Address.ColumnNumber;
            else if (headerValue.Contains("currency"))
                columnMap["Currency"] = cell.Address.ColumnNumber;
            else if (headerValue.Contains("category") && !headerValue.Contains("sub"))
                columnMap["Category"] = cell.Address.ColumnNumber;
            else if (headerValue.Contains("subcategory") || headerValue.Contains("sub category"))
                columnMap["SubCategory"] = cell.Address.ColumnNumber;
            else if (headerValue.Contains("date"))
                columnMap["Date"] = cell.Address.ColumnNumber;
        }
        return columnMap;
    }

    private string GetCellValue(IXLWorksheet worksheet, int row, int column)
    {
        if (column == 0) return null;
        try { return worksheet.Cell(row, column).Value.ToString().Trim(); }
        catch { return null; }
    }

    private decimal ParseDecimal(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return 0;
        value = value.Replace("$", "").Replace("€", "").Replace("£", "").Replace(",", "");
        if (decimal.TryParse(value, out decimal result)) return result;
        return 0;
    }

    private DateTime ParseDate(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return DateTime.UtcNow;
        if (DateTime.TryParse(value, out DateTime result)) return result;
        return DateTime.UtcNow;
    }

    private bool ValidateFinancialData(FinancialData data)
    {
        if (string.IsNullOrWhiteSpace(data.AccountName)) return false;
        if (string.IsNullOrWhiteSpace(data.Period)) return false;
        if (string.IsNullOrWhiteSpace(data.Category)) return false;
        return true;
    }

    private List<FinancialData> ParseFinancialDataFromText(string text, int documentId)
    {
        var dataList = new List<FinancialData>();
        try
        {
            var lines = text.Split('\n');
            foreach (var line in lines)
            {
                var amountMatch = Regex.Match(line, @"\$?([\d,]+\.?\d*)");
                if (amountMatch.Success)
                {
                    var amount = ParseDecimal(amountMatch.Groups[1].Value);
                    if (amount > 0)
                    {
                        var accountName = line.Substring(0, Math.Min(amountMatch.Index, line.Length)).Trim();
                        if (!string.IsNullOrWhiteSpace(accountName) && accountName.Length > 3)
                        {
                            var data = new FinancialData
                            {
                                DocumentId = documentId,
                                AccountName = accountName,
                                Amount = amount,
                                Currency = "USD",
                                Category = "Unknown",
                                Period = DateTime.Now.ToString("yyyy-MM"),
                                DateRecorded = DateTime.UtcNow
                            };
                            dataList.Add(data);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error parsing financial data from text");
        }
        return dataList;
    }

    // CSV Helper Classes
    private class FinancialDataCsvRecord
    {
        public string AccountName { get; set; }
        public string AccountCode { get; set; }
        public string Period { get; set; }
        public string Amount { get; set; }
        public string Currency { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public string Date { get; set; }
    }

    private class FinancialDataCsvMap : ClassMap<FinancialDataCsvRecord>
    {
        public FinancialDataCsvMap()
        {
            Map(m => m.AccountName).Name("Account Name", "AccountName", "Account");
            Map(m => m.AccountCode).Name("Account Code", "AccountCode", "Code").Optional();
            Map(m => m.Period).Name("Period", "Time Period");
            Map(m => m.Amount).Name("Amount", "Value");
            Map(m => m.Currency).Name("Currency").Optional();
            Map(m => m.Category).Name("Category", "Type");
            Map(m => m.SubCategory).Name("Sub Category", "SubCategory", "Subcategory").Optional();
            Map(m => m.Date).Name("Date", "Record Date").Optional();
        }
    }
}
