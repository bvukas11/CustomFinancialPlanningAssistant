# Document Processor - Quick Reference Guide

## Overview
The DocumentProcessor handles Excel, CSV, and PDF files with intelligent AI-powered extraction.

---

## Quick Start

### 1. Basic Usage

```csharp
// Inject the service
private readonly IDocumentProcessor _documentProcessor;

public MyService(IDocumentProcessor documentProcessor)
{
    _documentProcessor = documentProcessor;
}

// Process any document
using var fileStream = File.OpenRead("document.xlsx");
var result = await _documentProcessor.ProcessDocumentAsync(
    fileStream, 
    "document.xlsx", 
    "excel"
);

if (result.Success)
{
    Console.WriteLine($"? Imported {result.RecordsImported} records");
    Console.WriteLine($"?? Document ID: {result.DocumentId}");
    Console.WriteLine($"? Time: {result.ProcessingTime}ms");
}
else
{
    foreach (var error in result.ErrorMessages)
    {
        Console.WriteLine($"? {error}");
    }
}
```

---

## File Format Support

### Excel Files (.xlsx, .xls)

**Best For:** Structured financial data with clear headers

**Features:**
- Automatic header detection (first 10 rows)
- Flexible column name matching
- Multiple worksheet support (uses first sheet)
- Currency symbol removal
- Data validation

**Sample Excel Structure:**
| Account Name | Account Code | Period | Amount | Currency | Category | Sub Category | Date |
|---|---|---|---|---|---|---|---|
| Sales Revenue | 4000 | 2024-Q1 | 150000 | USD | Revenue | Product Sales | 2024-03-31 |
| Operating Expenses | 5000 | 2024-Q1 | 95000 | USD | Expense | Operations | 2024-03-31 |

**Supported Column Names (case-insensitive):**
- **Account Name:** "Account Name", "AccountName", "Account"
- **Account Code:** "Account Code", "AccountCode", "Code"
- **Period:** "Period", "Time Period"
- **Amount:** "Amount", "Value"
- **Currency:** "Currency"
- **Category:** "Category", "Type"
- **Sub Category:** "Sub Category", "SubCategory", "Subcategory"
- **Date:** "Date", "Record Date"

---

### CSV Files (.csv)

**Best For:** Simple exports from accounting software

**Features:**
- UTF-8 encoding support
- Flexible delimiter detection
- Optional field handling
- Culture-invariant parsing

**Sample CSV Structure:**
```csv
Account Name,Account Code,Period,Amount,Currency,Category,Sub Category,Date
Sales Revenue,4000,2024-Q1,150000,USD,Revenue,Product Sales,2024-03-31
Operating Expenses,5000,2024-Q1,95000,USD,Expense,Operations,2024-03-31
```

---

### PDF Files (.pdf) ?? AI-Powered

**Best For:** Invoices, statements, scanned documents

**Features:**
- **Hybrid Processing:**
  - Text extraction (fast) for computer-generated PDFs
  - AI vision (accurate) for scanned PDFs
- **Automatic fallback** to AI vision if text extraction yields < 3 records
- **Page-by-page processing** with llama3.2-vision

**How It Works:**
1. Attempts text extraction first (PdfPig)
2. If insufficient results, converts to images (Docnet.Core)
3. Processes each page with AI vision (llama3.2-vision)
4. Parses structured responses

**AI Vision Prompt:**
```
Extract all financial data from this image. Look for:
- Account names and numbers
- Amounts (revenue, expenses, assets, liabilities)
- Dates and periods
- Categories and descriptions

Format your response as: AccountName | Amount | Category | Period
```

---

## Code Examples

### Example 1: Process Excel File

```csharp
public async Task<UploadResultDto> UploadExcelFile(IFormFile file)
{
    using var stream = file.OpenReadStream();
    
    var result = await _documentProcessor.ProcessExcelAsync(
        stream,
        file.FileName
    );
    
    return result;
}
```

### Example 2: Process CSV File

```csharp
public async Task<UploadResultDto> UploadCsvFile(Stream fileStream, string fileName)
{
    return await _documentProcessor.ProcessCsvAsync(fileStream, fileName);
}
```

### Example 3: Process PDF with AI Vision

```csharp
public async Task<UploadResultDto> UploadPdfFile(Stream fileStream, string fileName)
{
    // Automatically uses AI vision if text extraction insufficient
    var result = await _documentProcessor.ProcessPdfAsync(fileStream, fileName);
    
    if (result.Warnings.Any())
    {
        // Check if AI vision was used
        foreach (var warning in result.Warnings)
        {
            Console.WriteLine($"?? {warning}");
        }
    }
    
    return result;
}
```

### Example 4: Automatic File Type Detection

```csharp
public async Task<UploadResultDto> UploadAnyFile(Stream fileStream, string fileName)
{
    // Automatically detects file type from extension
    var fileType = _documentProcessor.DetectFileType(fileName);
    
    return await _documentProcessor.ProcessDocumentAsync(
        fileStream,
        fileName,
        fileType.ToString()
    );
}
```

### Example 5: Extract Data Without Saving

```csharp
// Just extract data, don't save to database
public async Task<List<FinancialData>> PreviewExcelData(Stream fileStream)
{
    return await _documentProcessor.ExtractDataFromExcelAsync(fileStream);
}

public async Task<List<FinancialData>> PreviewCsvData(Stream fileStream)
{
    return await _documentProcessor.ExtractDataFromCsvAsync(fileStream);
}
```

---

## Result Object (UploadResultDto)

```csharp
public class UploadResultDto
{
    public bool Success { get; set; }
    public int? DocumentId { get; set; }
    public string FileName { get; set; }
    public int RecordsImported { get; set; }
    public List<string> ErrorMessages { get; set; }
    public List<string> Warnings { get; set; }
    public int ProcessingTime { get; set; }  // milliseconds
    
    // Helper properties
    public bool HasErrors => ErrorMessages.Any();
    public bool HasWarnings => Warnings.Any();
    
    // Helper methods
    public void AddError(string message);
    public void AddWarning(string message);
}
```

---

## Error Handling

### Common Errors and Solutions

| Error | Cause | Solution |
|-------|-------|----------|
| "File validation failed" | Invalid file or size > 50MB | Check file integrity and size |
| "No financial data found" | Empty file or wrong format | Verify file contains data |
| "Could not find header row" | No headers in first 10 rows | Add/move headers to top |
| "Unsupported file type" | Wrong extension | Use .xlsx, .xls, .csv, or .pdf |

### Validation Checks

```csharp
// File validation includes:
1. ? Size check (max 50MB configurable)
2. ? Extension whitelist (.xlsx, .xls, .csv, .pdf)
3. ? Magic number validation (file signature)
4. ? Empty file check
5. ? Path sanitization
```

---

## Configuration

### appsettings.json

```json
{
  "FileStorage": {
    "BasePath": "FileStorage",
    "MaxFileSize": 52428800,  // 50 MB in bytes
    "AllowedExtensions": [".xlsx", ".xls", ".csv", ".pdf"],
    "UploadFolder": "uploads",
    "ProcessedFolder": "processed",
    "ErrorFolder": "errors"
  }
}
```

### Customizing Settings

```csharp
// Change max file size (in bytes)
"MaxFileSize": 104857600  // 100 MB

// Add allowed extensions
"AllowedExtensions": [".xlsx", ".xls", ".csv", ".pdf", ".txt"]

// Change storage folders
"UploadFolder": "incoming"
"ProcessedFolder": "completed"
```

---

## Performance Tips

### Excel Files
- ? Process up to 10,000 rows efficiently
- ? Avoid extremely large files (split if > 50MB)
- ? Use first worksheet only (performance)

### CSV Files
- ? Fastest format to process
- ? UTF-8 encoding recommended
- ? Remove extra blank lines

### PDF Files
- ? **Text-based PDFs:** 3-8 seconds
- ?? **Scanned PDFs:** 15-45 seconds (AI vision)
- ?? **Tip:** Convert scanned PDFs to text-based if possible

---

## AI Vision Details

### When AI Vision Activates

PDF processing uses a hybrid approach:

1. **Always tries text extraction first** (PdfPig) - Fast
2. **Fallback to AI vision if:**
   - Text extraction yields < 3 records
   - PDF is scanned/image-based
   - Tables are complex

### AI Vision Process

```
1. Convert PDF pages to images (1920x1080)
   ?
2. For each page:
   - Send to llama3.2-vision model
   - Use structured prompt
   - Parse response
   ?
3. Combine results from all pages
   ?
4. Save to database
```

### Expected Processing Times

| PDF Type | Pages | Time | Method |
|----------|-------|------|--------|
| Text-based | 5 | 5s | Text extraction |
| Scanned | 5 | 25s | AI vision |
| Mixed | 5 | 15s | Hybrid |

---

## Sample Test Files

### Creating Test Excel File

```csharp
using ClosedXML.Excel;

var workbook = new XLWorkbook();
var worksheet = workbook.Worksheets.Add("Financial Data");

// Add headers
worksheet.Cell("A1").Value = "Account Name";
worksheet.Cell("B1").Value = "Account Code";
worksheet.Cell("C1").Value = "Period";
worksheet.Cell("D1").Value = "Amount";
worksheet.Cell("E1").Value = "Currency";
worksheet.Cell("F1").Value = "Category";

// Add data
worksheet.Cell("A2").Value = "Sales Revenue";
worksheet.Cell("B2").Value = "4000";
worksheet.Cell("C2").Value = "2024-Q1";
worksheet.Cell("D2").Value = 150000;
worksheet.Cell("E2").Value = "USD";
worksheet.Cell("F2").Value = "Revenue";

workbook.SaveAs("test-financial-data.xlsx");
```

### Creating Test CSV File

```csharp
using CsvHelper;
using System.Globalization;

var records = new[]
{
    new { AccountName = "Sales Revenue", AccountCode = "4000", 
          Period = "2024-Q1", Amount = 150000, Currency = "USD", Category = "Revenue" },
    new { AccountName = "Operating Expenses", AccountCode = "5000", 
          Period = "2024-Q1", Amount = 95000, Currency = "USD", Category = "Expense" }
};

using var writer = new StreamWriter("test-financial-data.csv");
using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
csv.WriteRecords(records);
```

---

## Blazor Component Example

```razor
@page "/upload"
@inject IDocumentProcessor DocumentProcessor

<h3>Upload Financial Document</h3>

<InputFile OnChange="HandleFileUpload" accept=".xlsx,.xls,.csv,.pdf" />

@if (isUploading)
{
    <p>Processing...</p>
}

@if (result != null)
{
    @if (result.Success)
    {
        <div class="alert alert-success">
            ? Imported @result.RecordsImported records in @result.ProcessingTime ms
        </div>
    }
    else
    {
        <div class="alert alert-danger">
            @foreach (var error in result.ErrorMessages)
            {
                <p>? @error</p>
            }
        </div>
    }
}

@code {
    private bool isUploading;
    private UploadResultDto result;

    private async Task HandleFileUpload(InputFileChangeEventArgs e)
    {
        isUploading = true;
        var file = e.File;

        using var stream = file.OpenReadStream(maxAllowedSize: 52428800); // 50MB
        
        result = await DocumentProcessor.ProcessDocumentAsync(
            stream,
            file.Name,
            Path.GetExtension(file.Name)
        );

        isUploading = false;
    }
}
```

---

## FAQ

**Q: What's the maximum file size?**  
A: 50MB by default, configurable in appsettings.json

**Q: How long does PDF processing take?**  
A: 3-8s for text-based, 15-45s for scanned PDFs

**Q: Can I process multiple files at once?**  
A: Yes, call ProcessDocumentAsync for each file

**Q: What if my Excel has multiple sheets?**  
A: Currently uses only the first sheet

**Q: Does it work with password-protected files?**  
A: No, files must be unprotected

**Q: What languages does AI vision support?**  
A: Primarily English, but llama3.2-vision supports multiple languages

**Q: Can I customize the AI prompt?**  
A: Yes, modify the prompt in `ProcessPdfWithAIVision` method

---

## Troubleshooting

### Excel Issues

**Problem:** Headers not detected
- **Solution:** Move headers to first 10 rows
- **Check:** Headers contain keywords: "account", "amount", "period", "category"

**Problem:** Data not extracted
- **Solution:** Ensure data is in same sheet as headers
- **Check:** Cells are not empty or merged

### CSV Issues

**Problem:** Wrong delimiter
- **Solution:** Use comma (,) as delimiter
- **Check:** File encoding is UTF-8

**Problem:** Extra columns
- **Solution:** Optional columns are handled automatically
- **Check:** Required columns present: Account Name, Amount, Category, Period

### PDF Issues

**Problem:** Slow processing
- **Solution:** Normal for scanned PDFs (AI vision)
- **Check:** Consider preprocessing (convert to text-based)

**Problem:** No data extracted
- **Solution:** PDF may be image-only without searchable text
- **Check:** AI vision should activate automatically

---

## Support

For issues or questions:
1. Check logs in application output
2. Review UploadResultDto.ErrorMessages
3. Verify file format matches examples
4. Ensure Ollama is running (for PDF AI vision)

---

**Last Updated:** 2025-01-16  
**Version:** 1.0  
**Status:** Production Ready ?
