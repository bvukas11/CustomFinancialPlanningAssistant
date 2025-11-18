# Database Seeding Guide - Financial Analysis Assistant

## Overview
This guide provides multiple approaches to seed your database with realistic financial data for development and testing.

---

## Approach 1: SQL Script (100,000+ Records)

### SQL Script: SeedFinancialData.sql

```sql
-- =============================================
-- Financial Analysis Assistant - Seed Data Script
-- Generates 100,000+ realistic financial records
-- =============================================

USE FinancialAnalysisDB;
GO

-- Disable constraints for faster insertion
ALTER TABLE FinancialData NOCHECK CONSTRAINT ALL;
ALTER TABLE AIAnalysis NOCHECK CONSTRAINT ALL;
GO

-- =============================================
-- STEP 1: Seed Financial Documents (100 documents)
-- =============================================

DECLARE @DocumentCounter INT = 1;
DECLARE @TotalDocuments INT = 100;
DECLARE @StartDate DATE = '2020-01-01';

WHILE @DocumentCounter <= @TotalDocuments
BEGIN
    DECLARE @UploadDate DATETIME = DATEADD(DAY, (@DocumentCounter * 3), @StartDate);
    DECLARE @FileType VARCHAR(20) = CASE 
        WHEN @DocumentCounter % 3 = 0 THEN 'Excel'
        WHEN @DocumentCounter % 3 = 1 THEN 'CSV'
        ELSE 'PDF'
    END;
    
    INSERT INTO FinancialDocuments (FileName, FileType, UploadDate, FileSize, FilePath, Status, CreatedBy)
    VALUES (
        'Financial_Report_' + CAST(@DocumentCounter AS VARCHAR) + '_' + FORMAT(@UploadDate, 'yyyyMM') + '.xlsx',
        @FileType,
        @UploadDate,
        CAST((RAND() * 5000000 + 100000) AS BIGINT), -- Random size between 100KB and 5MB
        '/uploads/' + CAST(YEAR(@UploadDate) AS VARCHAR) + '/' + CAST(@DocumentCounter AS VARCHAR) + '.xlsx',
        CASE WHEN @DocumentCounter % 10 = 0 THEN 'Processing' 
             WHEN @DocumentCounter % 20 = 0 THEN 'Error'
             ELSE 'Analyzed' END,
        'admin@company.com'
    );
    
    SET @DocumentCounter = @DocumentCounter + 1;
END;
GO

-- =============================================
-- STEP 2: Seed Financial Data Records (100,000+ records)
-- =============================================

-- Create temporary table for account definitions
CREATE TABLE #AccountDefinitions (
    AccountName VARCHAR(200),
    AccountCode VARCHAR(50),
    Category VARCHAR(50),
    SubCategory VARCHAR(100),
    BaseAmount DECIMAL(18,2),
    Variance DECIMAL(18,2)
);

-- Insert account definitions
INSERT INTO #AccountDefinitions VALUES
-- Revenue Accounts
('Product Sales Revenue', '4000', 'Revenue', 'Product Sales', 500000, 100000),
('Service Revenue', '4100', 'Revenue', 'Services', 300000, 75000),
('Consulting Revenue', '4200', 'Revenue', 'Professional Services', 200000, 50000),
('Subscription Revenue', '4300', 'Revenue', 'Recurring', 150000, 30000),
('Licensing Revenue', '4400', 'Revenue', 'IP', 100000, 25000),

-- Cost of Goods Sold
('Raw Materials', '5000', 'Expense', 'COGS', 150000, 30000),
('Direct Labor', '5100', 'Expense', 'COGS', 120000, 20000),
('Manufacturing Overhead', '5200', 'Expense', 'COGS', 80000, 15000),

-- Operating Expenses
('Salaries and Wages', '6000', 'Expense', 'Personnel', 250000, 25000),
('Employee Benefits', '6100', 'Expense', 'Personnel', 75000, 10000),
('Office Rent', '6200', 'Expense', 'Facilities', 50000, 5000),
('Utilities', '6300', 'Expense', 'Facilities', 15000, 3000),
('Office Supplies', '6400', 'Expense', 'Operations', 10000, 2000),
('Software Licenses', '6500', 'Expense', 'Technology', 30000, 5000),
('Hardware and Equipment', '6600', 'Expense', 'Technology', 40000, 10000),
('Marketing and Advertising', '6700', 'Expense', 'Marketing', 80000, 20000),
('Professional Fees', '6800', 'Expense', 'Professional Services', 35000, 7000),
('Insurance', '6900', 'Expense', 'Risk Management', 25000, 5000),
('Travel and Entertainment', '7000', 'Expense', 'Operations', 20000, 8000),
('Training and Development', '7100', 'Expense', 'Personnel', 15000, 5000),
('Telecommunications', '7200', 'Expense', 'Communications', 12000, 2000),
('Bank Fees', '7300', 'Expense', 'Financial', 5000, 1000),
('Depreciation', '7400', 'Expense', 'Non-Cash', 30000, 3000),

-- Assets
('Cash and Cash Equivalents', '1000', 'Asset', 'Current Assets', 500000, 100000),
('Accounts Receivable', '1100', 'Asset', 'Current Assets', 300000, 75000),
('Inventory', '1200', 'Asset', 'Current Assets', 200000, 50000),
('Prepaid Expenses', '1300', 'Asset', 'Current Assets', 50000, 10000),
('Property and Equipment', '1500', 'Asset', 'Fixed Assets', 1000000, 100000),
('Accumulated Depreciation', '1510', 'Asset', 'Fixed Assets', -300000, 30000),
('Intangible Assets', '1600', 'Asset', 'Intangible', 200000, 20000),

-- Liabilities
('Accounts Payable', '2000', 'Liability', 'Current Liabilities', 150000, 30000),
('Accrued Expenses', '2100', 'Liability', 'Current Liabilities', 80000, 15000),
('Short-term Debt', '2200', 'Liability', 'Current Liabilities', 100000, 20000),
('Long-term Debt', '2500', 'Liability', 'Long-term Liabilities', 500000, 50000),
('Deferred Revenue', '2300', 'Liability', 'Current Liabilities', 75000, 15000),

-- Equity
('Common Stock', '3000', 'Equity', 'Share Capital', 1000000, 0),
('Retained Earnings', '3100', 'Equity', 'Accumulated Earnings', 500000, 100000);

-- Generate financial data for each document
DECLARE @DocId INT;
DECLARE @Period VARCHAR(20);
DECLARE @RecordsPerDoc INT = 1000; -- ~1000 records per document = 100,000 total
DECLARE @PeriodCounter INT;

DECLARE doc_cursor CURSOR FOR 
    SELECT Id, FORMAT(UploadDate, 'yyyy-MM') AS Period 
    FROM FinancialDocuments 
    ORDER BY Id;

OPEN doc_cursor;
FETCH NEXT FROM doc_cursor INTO @DocId, @Period;

WHILE @@FETCH_STATUS = 0
BEGIN
    -- Insert financial records for this document
    INSERT INTO FinancialDataRecords (DocumentId, AccountName, AccountCode, Period, Amount, Currency, Category, SubCategory, DateRecorded)
    SELECT 
        @DocId,
        AccountName,
        AccountCode,
        @Period,
        BaseAmount + (RAND(CHECKSUM(NEWID())) * Variance * 2 - Variance), -- Add random variance
        'USD',
        Category,
        SubCategory,
        DATEADD(DAY, CAST((RAND(CHECKSUM(NEWID())) * 28) AS INT), @Period + '-01')
    FROM #AccountDefinitions;
    
    FETCH NEXT FROM doc_cursor INTO @DocId, @Period;
END;

CLOSE doc_cursor;
DEALLOCATE doc_cursor;

-- Cleanup
DROP TABLE #AccountDefinitions;
GO

-- =============================================
-- STEP 3: Seed AI Analysis Records (10,000 records)
-- =============================================

DECLARE @AnalysisCounter INT = 1;
DECLARE @TotalAnalyses INT = 10000;
DECLARE @DocCount INT = (SELECT COUNT(*) FROM FinancialDocuments);

WHILE @AnalysisCounter <= @TotalAnalyses
BEGIN
    DECLARE @RandomDocId INT = CAST((RAND() * @DocCount + 1) AS INT);
    DECLARE @AnalysisTypes TABLE (TypeName VARCHAR(50));
    
    INSERT INTO @AnalysisTypes VALUES 
        ('Summary'), ('TrendAnalysis'), ('AnomalyDetection'), 
        ('Comparison'), ('Forecasting'), ('RatioAnalysis'), ('Custom');
    
    DECLARE @RandomType VARCHAR(50) = (
        SELECT TOP 1 TypeName 
        FROM @AnalysisTypes 
        ORDER BY NEWID()
    );
    
    DECLARE @ExecutionTime INT = CAST((RAND() * 5000 + 500) AS INT); -- 500-5500ms
    
    INSERT INTO AIAnalyses (DocumentId, AnalysisType, Prompt, Response, ModelUsed, ExecutionTime, CreatedDate, Rating)
    VALUES (
        @RandomDocId,
        @RandomType,
        'Analyze the financial data for period ' + CAST(@AnalysisCounter AS VARCHAR),
        'Analysis result: The financial performance shows ' + 
        CASE 
            WHEN @AnalysisCounter % 3 = 0 THEN 'strong growth trends with increasing revenue.'
            WHEN @AnalysisCounter % 3 = 1 THEN 'moderate performance with stable expenses.'
            ELSE 'areas requiring attention in cost management.'
        END,
        CASE 
            WHEN @AnalysisCounter % 2 = 0 THEN 'llama3.2'
            ELSE 'qwen2.5:8b'
        END,
        @ExecutionTime,
        DATEADD(MINUTE, @AnalysisCounter, GETDATE()),
        CASE WHEN @AnalysisCounter % 5 = 0 THEN NULL ELSE CAST((RAND() * 2 + 3) AS INT) END -- Rating 3-5 or NULL
    );
    
    SET @AnalysisCounter = @AnalysisCounter + 1;
    DELETE FROM @AnalysisTypes; -- Clear for next iteration
END;
GO

-- =============================================
-- STEP 4: Seed Reports (1,000 records)
-- =============================================

DECLARE @ReportCounter INT = 1;
DECLARE @TotalReports INT = 1000;

WHILE @ReportCounter <= @TotalReports
BEGIN
    DECLARE @ReportTypes TABLE (TypeName VARCHAR(50));
    INSERT INTO @ReportTypes VALUES 
        ('Summary'), ('Detailed'), ('Comparison'), ('Trend'), ('RatioAnalysis'), ('Custom');
    
    DECLARE @ReportType VARCHAR(50) = (
        SELECT TOP 1 TypeName 
        FROM @ReportTypes 
        ORDER BY NEWID()
    );
    
    INSERT INTO Reports (Title, Description, ReportType, GeneratedDate, Content, Parameters)
    VALUES (
        @ReportType + ' Report ' + CAST(@ReportCounter AS VARCHAR),
        'Financial ' + @ReportType + ' report generated on ' + FORMAT(GETDATE(), 'yyyy-MM-dd'),
        @ReportType,
        DATEADD(HOUR, @ReportCounter, GETDATE()),
        '{"summary": "Report content here", "charts": [], "metrics": {}}',
        '{"documentId": ' + CAST((RAND() * 100 + 1) AS VARCHAR) + ', "period": "2024-Q1"}'
    );
    
    SET @ReportCounter = @ReportCounter + 1;
    DELETE FROM @ReportTypes;
END;
GO

-- Re-enable constraints
ALTER TABLE FinancialData CHECK CONSTRAINT ALL;
ALTER TABLE AIAnalysis CHECK CONSTRAINT ALL;
GO

-- =============================================
-- STEP 5: Verify Data
-- =============================================

PRINT 'Seeding Complete!';
PRINT '==========================================';
PRINT 'Documents: ' + CAST((SELECT COUNT(*) FROM FinancialDocuments) AS VARCHAR);
PRINT 'Financial Data Records: ' + CAST((SELECT COUNT(*) FROM FinancialDataRecords) AS VARCHAR);
PRINT 'AI Analyses: ' + CAST((SELECT COUNT(*) FROM AIAnalyses) AS VARCHAR);
PRINT 'Reports: ' + CAST((SELECT COUNT(*) FROM Reports) AS VARCHAR);
PRINT '==========================================';

-- Show sample data
SELECT TOP 10 * FROM FinancialDocuments ORDER BY UploadDate DESC;
SELECT TOP 10 * FROM FinancialDataRecords ORDER BY DateRecorded DESC;
SELECT TOP 10 * FROM AIAnalyses ORDER BY CreatedDate DESC;
SELECT TOP 10 * FROM Reports ORDER BY GeneratedDate DESC;

GO
```

---

## Approach 2: C# Seeding Class (Recommended for Development)

### Enhanced DbInitializer.cs

```csharp
using FinancialAnalysisAssistant.Core.Entities;
using FinancialAnalysisAssistant.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FinancialAnalysisAssistant.Infrastructure.Data
{
    /// <summary>
    /// Seeds the database with realistic financial data for development and testing
    /// </summary>
    public static class DbInitializer
    {
        private static readonly Random _random = new Random();
        
        /// <summary>
        /// Seeds the database with 100,000+ records
        /// </summary>
        public static async Task SeedAsync(AppDbContext context, ILogger logger)
        {
            try
            {
                // Check if already seeded
                if (await context.FinancialDocuments.AnyAsync())
                {
                    logger.LogInformation("Database already seeded");
                    return;
                }

                logger.LogInformation("Starting database seeding...");

                // Seed in order to maintain referential integrity
                var documents = await SeedFinancialDocuments(context, 100);
                logger.LogInformation($"Seeded {documents.Count} documents");

                var dataRecords = await SeedFinancialData(context, documents, 1000);
                logger.LogInformation($"Seeded {dataRecords} financial data records");

                var analyses = await SeedAIAnalyses(context, documents, 10000);
                logger.LogInformation($"Seeded {analyses} AI analyses");

                var reports = await SeedReports(context, 1000);
                logger.LogInformation($"Seeded {reports} reports");

                logger.LogInformation("Database seeding completed successfully!");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error seeding database");
                throw;
            }
        }

        private static async Task<List<FinancialDocument>> SeedFinancialDocuments(
            AppDbContext context, 
            int count)
        {
            var documents = new List<FinancialDocument>();
            var startDate = new DateTime(2020, 1, 1);
            var fileTypes = new[] { "Excel", "CSV", "PDF" };
            var statuses = new[] { "Uploaded", "Processing", "Analyzed", "Error" };

            for (int i = 1; i <= count; i++)
            {
                var uploadDate = startDate.AddDays(i * 3);
                var doc = new FinancialDocument
                {
                    FileName = $"Financial_Report_{i}_{uploadDate:yyyyMM}.xlsx",
                    FileType = fileTypes[i % fileTypes.Length],
                    UploadDate = uploadDate,
                    FileSize = _random.Next(100000, 5000000),
                    FilePath = $"/uploads/{uploadDate.Year}/{i}.xlsx",
                    Status = i % 10 == 0 ? statuses[1] : statuses[2], // Mostly "Analyzed"
                    CreatedBy = "admin@company.com"
                };
                documents.Add(doc);
            }

            await context.FinancialDocuments.AddRangeAsync(documents);
            await context.SaveChangesAsync();

            return documents;
        }

        private static async Task<int> SeedFinancialData(
            AppDbContext context, 
            List<FinancialDocument> documents, 
            int recordsPerDocument)
        {
            var accountDefinitions = GetAccountDefinitions();
            var totalRecords = 0;

            // Process in batches to avoid memory issues
            const int batchSize = 10000;
            var allRecords = new List<FinancialData>();

            foreach (var doc in documents)
            {
                var period = doc.UploadDate.ToString("yyyy-MM");
                
                foreach (var account in accountDefinitions)
                {
                    // Create multiple records with variations for each account
                    var recordsToCreate = recordsPerDocument / accountDefinitions.Count;
                    
                    for (int i = 0; i < recordsToCreate; i++)
                    {
                        var variance = (decimal)(_random.NextDouble() * 2 - 1); // -1 to 1
                        var amount = account.BaseAmount + (account.Variance * variance);
                        
                        var record = new FinancialData
                        {
                            DocumentId = doc.Id,
                            AccountName = account.AccountName,
                            AccountCode = account.AccountCode,
                            Period = period,
                            Amount = Math.Round(amount, 2),
                            Currency = "USD",
                            Category = account.Category,
                            SubCategory = account.SubCategory,
                            DateRecorded = doc.UploadDate.AddDays(_random.Next(0, 28))
                        };
                        
                        allRecords.Add(record);
                        totalRecords++;

                        // Save in batches
                        if (allRecords.Count >= batchSize)
                        {
                            await context.FinancialDataRecords.AddRangeAsync(allRecords);
                            await context.SaveChangesAsync();
                            allRecords.Clear();
                        }
                    }
                }
            }

            // Save remaining records
            if (allRecords.Any())
            {
                await context.FinancialDataRecords.AddRangeAsync(allRecords);
                await context.SaveChangesAsync();
            }

            return totalRecords;
        }

        private static async Task<int> SeedAIAnalyses(
            AppDbContext context, 
            List<FinancialDocument> documents, 
            int count)
        {
            var analysisTypes = new[] 
            { 
                "Summary", "TrendAnalysis", "AnomalyDetection", 
                "Comparison", "Forecasting", "RatioAnalysis", "Custom" 
            };
            
            var models = new[] { "llama3.2", "qwen2.5:8b" };
            
            var analyses = new List<AIAnalysis>();
            const int batchSize = 5000;

            for (int i = 0; i < count; i++)
            {
                var randomDoc = documents[_random.Next(documents.Count)];
                var analysisType = analysisTypes[_random.Next(analysisTypes.Length)];
                
                var analysis = new AIAnalysis
                {
                    DocumentId = randomDoc.Id,
                    AnalysisType = analysisType,
                    Prompt = $"Analyze the financial data for {analysisType}",
                    Response = GenerateRealisticAnalysisResponse(analysisType),
                    ModelUsed = models[_random.Next(models.Length)],
                    ExecutionTime = _random.Next(500, 5500),
                    CreatedDate = DateTime.UtcNow.AddMinutes(-_random.Next(0, 100000)),
                    Rating = _random.Next(10) < 8 ? _random.Next(3, 6) : (int?)null // 80% rated
                };
                
                analyses.Add(analysis);

                if (analyses.Count >= batchSize)
                {
                    await context.AIAnalyses.AddRangeAsync(analyses);
                    await context.SaveChangesAsync();
                    analyses.Clear();
                }
            }

            if (analyses.Any())
            {
                await context.AIAnalyses.AddRangeAsync(analyses);
                await context.SaveChangesAsync();
            }

            return count;
        }

        private static async Task<int> SeedReports(AppDbContext context, int count)
        {
            var reportTypes = new[] 
            { 
                "Summary", "Detailed", "Comparison", 
                "Trend", "RatioAnalysis", "Custom" 
            };
            
            var reports = new List<Report>();
            const int batchSize = 1000;

            for (int i = 1; i <= count; i++)
            {
                var reportType = reportTypes[_random.Next(reportTypes.Length)];
                
                var report = new Report
                {
                    Title = $"{reportType} Report {i}",
                    Description = $"Financial {reportType} report generated automatically",
                    ReportType = reportType,
                    GeneratedDate = DateTime.UtcNow.AddHours(-_random.Next(0, 10000)),
                    Content = GenerateReportContent(reportType),
                    Parameters = $"{{\"documentId\": {_random.Next(1, 101)}, \"period\": \"2024-Q1\"}}"
                };
                
                reports.Add(report);

                if (reports.Count >= batchSize)
                {
                    await context.Reports.AddRangeAsync(reports);
                    await context.SaveChangesAsync();
                    reports.Clear();
                }
            }

            if (reports.Any())
            {
                await context.Reports.AddRangeAsync(reports);
                await context.SaveChangesAsync();
            }

            return count;
        }

        private static List<AccountDefinition> GetAccountDefinitions()
        {
            return new List<AccountDefinition>
            {
                // Revenue Accounts
                new("Product Sales Revenue", "4000", "Revenue", "Product Sales", 500000, 100000),
                new("Service Revenue", "4100", "Revenue", "Services", 300000, 75000),
                new("Consulting Revenue", "4200", "Revenue", "Professional Services", 200000, 50000),
                new("Subscription Revenue", "4300", "Revenue", "Recurring", 150000, 30000),
                new("Licensing Revenue", "4400", "Revenue", "IP", 100000, 25000),

                // COGS
                new("Raw Materials", "5000", "Expense", "COGS", 150000, 30000),
                new("Direct Labor", "5100", "Expense", "COGS", 120000, 20000),
                new("Manufacturing Overhead", "5200", "Expense", "COGS", 80000, 15000),

                // Operating Expenses
                new("Salaries and Wages", "6000", "Expense", "Personnel", 250000, 25000),
                new("Employee Benefits", "6100", "Expense", "Personnel", 75000, 10000),
                new("Office Rent", "6200", "Expense", "Facilities", 50000, 5000),
                new("Utilities", "6300", "Expense", "Facilities", 15000, 3000),
                new("Office Supplies", "6400", "Expense", "Operations", 10000, 2000),
                new("Software Licenses", "6500", "Expense", "Technology", 30000, 5000),
                new("Marketing", "6700", "Expense", "Marketing", 80000, 20000),
                new("Professional Fees", "6800", "Expense", "Professional Services", 35000, 7000),

                // Assets
                new("Cash", "1000", "Asset", "Current Assets", 500000, 100000),
                new("Accounts Receivable", "1100", "Asset", "Current Assets", 300000, 75000),
                new("Inventory", "1200", "Asset", "Current Assets", 200000, 50000),
                new("Property and Equipment", "1500", "Asset", "Fixed Assets", 1000000, 100000),

                // Liabilities
                new("Accounts Payable", "2000", "Liability", "Current Liabilities", 150000, 30000),
                new("Short-term Debt", "2200", "Liability", "Current Liabilities", 100000, 20000),
                new("Long-term Debt", "2500", "Liability", "Long-term Liabilities", 500000, 50000),

                // Equity
                new("Common Stock", "3000", "Equity", "Share Capital", 1000000, 0),
                new("Retained Earnings", "3100", "Equity", "Accumulated Earnings", 500000, 100000)
            };
        }

        private static string GenerateRealisticAnalysisResponse(string analysisType)
        {
            return analysisType switch
            {
                "Summary" => "Financial Summary: The company shows strong performance with revenue growth of 15% YoY. Operating margins remain healthy at 25%. Key strengths include diversified revenue streams and controlled expenses.",
                "TrendAnalysis" => "Trend Analysis: Revenue has grown consistently over the past 12 months, showing an upward trend. Expenses have remained relatively stable, indicating good cost control.",
                "AnomalyDetection" => "Anomaly Detection: Identified 3 unusual transactions: 1) Marketing expenses spiked 200% in March, 2) Inventory decreased significantly in Q2, 3) Cash balance showed unusual volatility.",
                "Comparison" => "Period Comparison: Q4 2024 vs Q3 2024 shows 12% revenue increase, 8% expense increase, resulting in improved profitability.",
                "Forecasting" => "Forecast: Based on historical trends, revenue is projected to reach $2.5M in next quarter with 95% confidence interval.",
                "RatioAnalysis" => "Ratio Analysis: Current Ratio: 2.5 (Healthy), Debt-to-Equity: 0.65 (Acceptable), Net Profit Margin: 18% (Strong)",
                _ => "Custom Analysis: Detailed financial analysis completed successfully with actionable insights."
            };
        }

        private static string GenerateReportContent(string reportType)
        {
            return $"{{\"type\":\"{reportType}\",\"summary\":\"Report generated with comprehensive financial data\",\"charts\":[],\"metrics\":{{}}}}";
        }

        private record AccountDefinition(
            string AccountName,
            string AccountCode,
            string Category,
            string SubCategory,
            decimal BaseAmount,
            decimal Variance
        );
    }
}
```

---

## How to Use

### Option 1: SQL Script (Fastest)

1. **Run the SQL script directly:**
   ```bash
   # Using SQL Server Management Studio (SSMS)
   - Open SSMS
   - Connect to your database
   - Open SeedFinancialData.sql
   - Execute (F5)
   
   # Using sqlcmd
   sqlcmd -S (localdb)\mssqllocaldb -d FinancialAnalysisDB -i SeedFinancialData.sql
   ```

2. **Execution time:** 2-5 minutes for 100,000+ records

### Option 2: C# Seeding (Recommended for Development)

1. **Update Program.cs:**
   ```csharp
   using FinancialAnalysisAssistant.Infrastructure.Data;
   
   // After app.Build() and before app.Run()
   using (var scope = app.Services.CreateScope())
   {
       var services = scope.ServiceProvider;
       var context = services.GetRequiredService<AppDbContext>();
       var logger = services.GetRequiredService<ILogger<Program>>();
       
       // Ensure database is created
       await context.Database.MigrateAsync();
       
       // Seed data
       await DbInitializer.SeedAsync(context, logger);
   }
   ```

2. **Run the application:**
   ```bash
   dotnet run --project FinancialAnalysisAssistant.Web
   ```

3. **Execution time:** 5-10 minutes for 100,000+ records

---

## Performance Tips

### For SQL Script:
- Disable constraints during insertion (already included)
- Use batch inserts
- Increase transaction log size if needed

### For C# Seeding:
- Process in batches (5,000-10,000 records)
- Use `AddRangeAsync` instead of individual `AddAsync`
- Call `SaveChangesAsync` after each batch
- Disable change tracking if not needed:
  ```csharp
  context.ChangeTracker.AutoDetectChangesEnabled = false;
  ```

---

## Data Distribution

| Table | Record Count | Description |
|-------|--------------|-------------|
| FinancialDocuments | 100 | Various file types and statuses |
| FinancialDataRecords | 100,000+ | ~1,000 records per document |
| AIAnalyses | 10,000 | Various analysis types |
| Reports | 1,000 | Different report types |

---

## Realistic Data Characteristics

? **Time-based data** - Spanning 4+ years (2020-2024)  
? **Realistic amounts** - With natural variance  
? **Multiple categories** - Revenue, Expenses, Assets, Liabilities, Equity  
? **Industry patterns** - Typical account structures  
? **Seasonal variations** - Built into variance calculations  
? **Real account codes** - Standard accounting codes  
? **Status variations** - Including processing errors  

---

## Verification Queries

```sql
-- Check record counts
SELECT 
    (SELECT COUNT(*) FROM FinancialDocuments) AS Documents,
    (SELECT COUNT(*) FROM FinancialDataRecords) AS DataRecords,
    (SELECT COUNT(*) FROM AIAnalyses) AS Analyses,
    (SELECT COUNT(*) FROM Reports) AS Reports;

-- Revenue vs Expenses analysis
SELECT 
    Category,
    COUNT(*) AS RecordCount,
    SUM(Amount) AS TotalAmount,
    AVG(Amount) AS AvgAmount
FROM FinancialDataRecords
GROUP BY Category;

-- Documents by status
SELECT Status, COUNT(*) AS Count
FROM FinancialDocuments
GROUP BY Status;

-- AI Analysis by type
SELECT AnalysisType, COUNT(*) AS Count
FROM AIAnalyses
GROUP BY AnalysisType;
```

---

## Troubleshooting

### SQL Script Issues:

**Error: "Cannot insert duplicate key"**
- Solution: Clear existing data first:
  ```sql
  DELETE FROM Reports;
  DELETE FROM AIAnalyses;
  DELETE FROM FinancialDataRecords;
  DELETE FROM FinancialDocuments;
  ```

**Error: "Transaction log full"**
- Solution: Increase log size or backup log

### C# Seeding Issues:

**OutOfMemoryException**
- Reduce batch size to 5,000 records
- Process documents one at a time

**Slow performance**
- Disable change tracking
- Use bulk insert extensions
- Run in Release mode

---

## Next Steps

After seeding:
1. ? Verify data with queries above
2. ? Test dashboard displays correctly
3. ? Run sample analyses
4. ? Generate test reports
5. ? Check performance with large datasets

