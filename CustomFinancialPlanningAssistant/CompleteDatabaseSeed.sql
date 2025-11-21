-- =============================================
-- Financial Analysis Assistant - Complete Seed Data Script
-- Includes: Financial Documents, Data Records, AI Analyses, Reports, and Industry Benchmarks
-- Version: 3.0 - Complete seeding with Phase 12 Industry Benchmarking
-- =============================================

USE FinancialAnalysisDB;
GO

-- =============================================
-- STEP 0: Clean up existing data (OPTIONAL - uncomment if needed)
-- =============================================

/*
DELETE FROM Reports;
DELETE FROM AIAnalyses;
DELETE FROM FinancialDataRecords;
DELETE FROM FinancialDocuments;
DELETE FROM IndustryBenchmarks;
PRINT 'Existing data cleared';
*/

-- Disable constraints for faster insertion
ALTER TABLE FinancialDataRecords NOCHECK CONSTRAINT ALL;
ALTER TABLE AIAnalyses NOCHECK CONSTRAINT ALL;
GO

-- =============================================
-- STEP 1: Seed Financial Documents (24 documents = 2 years of monthly data)
-- =============================================

DECLARE @DocumentCounter INT = 1;
DECLARE @TotalDocuments INT = 120; -- 2 years of monthly reports
DECLARE @StartDate DATE = '2013-01-01';

PRINT 'Creating Financial Documents...';

WHILE @DocumentCounter <= @TotalDocuments
BEGIN
    DECLARE @UploadDate DATETIME = DATEADD(MONTH, (@DocumentCounter - 1), @StartDate);
    DECLARE @Period VARCHAR(20) = FORMAT(@UploadDate, 'yyyy-MM');
    DECLARE @FileType VARCHAR(20) = CASE
        WHEN @DocumentCounter % 3 = 0 THEN 'Excel'
        WHEN @DocumentCounter % 3 = 1 THEN 'CSV'
        ELSE 'PDF'
    END;

    INSERT INTO FinancialDocuments (FileName, FileType, UploadDate, FileSize, FilePath, Status, CreatedBy)
    VALUES (
        'Financial_Report_' + @Period + '.xlsx',
        @FileType,
        @UploadDate,
        CAST((RAND(CHECKSUM(NEWID())) * 5000000 + 500000) AS BIGINT), -- Random size between 500KB and 5.5MB
        '/uploads/' + CAST(YEAR(@UploadDate) AS VARCHAR) + '/' + FORMAT(@UploadDate, 'MM') + '/report.xlsx',
        CASE
            WHEN @DocumentCounter = @TotalDocuments THEN 'Processing' -- Latest is still processing
            WHEN @DocumentCounter % 12 = 0 THEN 'Uploaded' -- Some just uploaded
            ELSE 'Analyzed'
        END,
        'system@financialassistant.com'
    );

    SET @DocumentCounter = @DocumentCounter + 1;
END;

PRINT CAST(@TotalDocuments AS VARCHAR) + ' documents created';
GO

-- =============================================
-- STEP 2: Create Account Definitions with Growth Patterns
-- =============================================

CREATE TABLE #AccountDefinitions (
    AccountName VARCHAR(200),
    AccountCode VARCHAR(50),
    Category VARCHAR(50),
    SubCategory VARCHAR(100),
    BaseAmount DECIMAL(18,2),
    MonthlyGrowthRate DECIMAL(5,4), -- Percentage growth per month
    Variance DECIMAL(18,2), -- Random variance
    Seasonality VARCHAR(20) -- Seasonal pattern
);

-- Insert account definitions with realistic growth patterns
INSERT INTO #AccountDefinitions VALUES
-- ==================== REVENUE ACCOUNTS ====================
('Product Sales Revenue', '4000', 'Revenue', 'Product Sales', 450000, 0.0250, 75000, 'Seasonal'), -- 2.5% monthly growth
('Service Revenue', '4100', 'Revenue', 'Services', 280000, 0.0180, 50000, 'Stable'),
('Consulting Revenue', '4200', 'Revenue', 'Professional Services', 180000, 0.0150, 35000, 'Stable'),
('Subscription Revenue', '4300', 'Revenue', 'Recurring', 120000, 0.0350, 20000, 'Growth'), -- 3.5% growth (subscriptions)
('Licensing Revenue', '4400', 'Revenue', 'IP', 85000, 0.0100, 15000, 'Stable'),
('Interest Income', '4500', 'Revenue', 'Financial', 5000, 0.0050, 2000, 'Stable'),

-- ==================== COST OF GOODS SOLD ====================
('Raw Materials', '5000', 'Expense', 'COGS', 135000, 0.0200, 25000, 'Seasonal'),
('Direct Labor', '5100', 'Expense', 'COGS', 110000, 0.0150, 15000, 'Stable'),
('Manufacturing Overhead', '5200', 'Expense', 'COGS', 68000, 0.0120, 12000, 'Stable'),
('Shipping and Handling', '5300', 'Expense', 'COGS', 42000, 0.0180, 8000, 'Seasonal'),

-- ==================== OPERATING EXPENSES ====================
('Executive Salaries', '6000', 'Expense', 'Personnel', 185000, 0.0080, 5000, 'Stable'),
('Staff Salaries', '6010', 'Expense', 'Personnel', 145000, 0.0120, 8000, 'Stable'),
('Employee Benefits', '6100', 'Expense', 'Personnel', 68000, 0.0100, 5000, 'Stable'),
('Payroll Taxes', '6110', 'Expense', 'Personnel', 42000, 0.0100, 3000, 'Stable'),
('Office Rent', '6200', 'Expense', 'Facilities', 45000, 0.0000, 0, 'Fixed'), -- Fixed cost
('Utilities', '6300', 'Expense', 'Facilities', 12500, 0.0050, 2500, 'Seasonal'),
('Office Supplies', '6400', 'Expense', 'Operations', 8500, 0.0080, 2000, 'Stable'),
('Maintenance and Repairs', '6410', 'Expense', 'Facilities', 6500, 0.0100, 2500, 'Variable'),
('Software Licenses', '6500', 'Expense', 'Technology', 28000, 0.0200, 5000, 'Growth'),
('Cloud Services', '6510', 'Expense', 'Technology', 18000, 0.0300, 4000, 'Growth'),
('Hardware and Equipment', '6600', 'Expense', 'Technology', 15000, 0.0150, 5000, 'Variable'),
('Marketing Campaigns', '6700', 'Expense', 'Marketing', 65000, 0.0220, 15000, 'Seasonal'),
('Digital Advertising', '6710', 'Expense', 'Marketing', 38000, 0.0280, 10000, 'Growth'),
('Professional Fees', '6800', 'Expense', 'Professional Services', 32000, 0.0080, 8000, 'Variable'),
('Legal Fees', '6810', 'Expense', 'Professional Services', 18000, 0.0050, 6000, 'Variable'),
('Insurance Premiums', '6900', 'Expense', 'Risk Management', 22000, 0.0030, 2000, 'Fixed'),
('Travel Expenses', '7000', 'Expense', 'Operations', 16000, 0.0150, 6000, 'Variable'),
('Training and Development', '7100', 'Expense', 'Personnel', 12000, 0.0180, 4000, 'Variable'),
('Telecommunications', '7200', 'Expense', 'Communications', 9500, 0.0080, 1500, 'Stable'),
('Bank Fees', '7300', 'Expense', 'Financial', 4500, 0.0050, 1000, 'Stable'),
('Depreciation Expense', '7400', 'Expense', 'Non-Cash', 28000, 0.0000, 0, 'Fixed'),
('Bad Debt Expense', '7500', 'Expense', 'Financial', 8000, 0.0100, 3000, 'Variable'),

-- ==================== ASSETS ====================
('Cash and Cash Equivalents', '1000', 'Asset', 'Current Assets', 650000, 0.0180, 120000, 'Variable'),
('Accounts Receivable', '1100', 'Asset', 'Current Assets', 380000, 0.0200, 75000, 'Variable'),
('Inventory', '1200', 'Asset', 'Current Assets', 245000, 0.0150, 50000, 'Seasonal'),
('Prepaid Expenses', '1300', 'Asset', 'Current Assets', 42000, 0.0080, 8000, 'Stable'),
('Short-term Investments', '1400', 'Asset', 'Current Assets', 185000, 0.0120, 35000, 'Stable'),
('Property and Equipment', '1500', 'Asset', 'Fixed Assets', 1250000, 0.0050, 50000, 'Growth'),
('Accumulated Depreciation', '1510', 'Asset', 'Fixed Assets', -385000, -0.0023, 5000, 'Fixed'), -- Grows negatively
('Intangible Assets', '1600', 'Asset', 'Intangible', 280000, 0.0080, 20000, 'Stable'),
('Goodwill', '1700', 'Asset', 'Intangible', 450000, 0.0000, 0, 'Fixed'),

-- ==================== LIABILITIES ====================
('Accounts Payable', '2000', 'Liability', 'Current Liabilities', 185000, 0.0150, 35000, 'Variable'),
('Accrued Expenses', '2100', 'Liability', 'Current Liabilities', 92000, 0.0120, 18000, 'Variable'),
('Short-term Notes Payable', '2200', 'Liability', 'Current Liabilities', 125000, -0.0080, 15000, 'Declining'), -- Paying down
('Current Portion LT Debt', '2250', 'Liability', 'Current Liabilities', 85000, 0.0000, 0, 'Fixed'),
('Deferred Revenue', '2300', 'Liability', 'Current Liabilities', 68000, 0.0250, 12000, 'Growth'),
('Long-term Debt', '2500', 'Liability', 'Long-term Liabilities', 650000, -0.0040, 20000, 'Declining'),
('Deferred Tax Liabilities', '2600', 'Liability', 'Long-term Liabilities', 145000, 0.0050, 10000, 'Stable'),

-- ==================== EQUITY ====================
('Common Stock', '3000', 'Equity', 'Share Capital', 1500000, 0.0000, 0, 'Fixed'),
('Additional Paid-in Capital', '3100', 'Equity', 'Share Capital', 450000, 0.0000, 0, 'Fixed'),
('Retained Earnings', '3200', 'Equity', 'Accumulated Earnings', 725000, 0.0150, 80000, 'Growth'),
('Treasury Stock', '3300', 'Equity', 'Share Capital', -120000, 0.0000, 0, 'Fixed');

PRINT 'Account definitions created';
GO

-- =============================================
-- STEP 3: Generate Financial Data with Growth Trends
-- =============================================

DECLARE @DocId INT;
DECLARE @Period VARCHAR(20);
DECLARE @MonthIndex INT;

DECLARE doc_cursor CURSOR FOR
    SELECT Id, FORMAT(UploadDate, 'yyyy-MM') AS Period,
           DATEDIFF(MONTH, '2023-01-01', UploadDate) AS MonthIndex
    FROM FinancialDocuments
    ORDER BY UploadDate;

OPEN doc_cursor;
FETCH NEXT FROM doc_cursor INTO @DocId, @Period, @MonthIndex;

PRINT 'Generating financial data records...';

WHILE @@FETCH_STATUS = 0
BEGIN
    -- Calculate seasonal factor (higher in Q4, lower in Q1)
    DECLARE @Month INT = CAST(RIGHT(@Period, 2) AS INT);
    DECLARE @SeasonalFactor DECIMAL(5,3) = CASE
        WHEN @Month IN (11, 12) THEN 1.15  -- 15% boost in Nov/Dec
        WHEN @Month IN (1, 2) THEN 0.90    -- 10% reduction in Jan/Feb
        WHEN @Month IN (6, 7) THEN 1.08    -- 8% boost in summer
        ELSE 1.00
    END;

    -- Insert financial records for this period
    INSERT INTO FinancialDataRecords (DocumentId, AccountName, AccountCode, Period, Amount, Currency, Category, SubCategory, DateRecorded)
    SELECT
        @DocId,
        AccountName,
        AccountCode,
        @Period,
        ROUND(
            -- Base amount with growth applied
            (BaseAmount * POWER(1 + MonthlyGrowthRate, @MonthIndex))
            -- Apply seasonal factor
            * (CASE WHEN Seasonality = 'Seasonal' THEN @SeasonalFactor ELSE 1.0 END)
            -- Add random variance
            + (RAND(CHECKSUM(NEWID())) * Variance * 2 - Variance),
        2),
        'USD',
        Category,
        SubCategory,
        DATEADD(DAY, CAST((RAND(CHECKSUM(NEWID())) * 28) AS INT), @Period + '-01')
    FROM #AccountDefinitions
    WHERE
        -- Only include accounts that make sense for this period
        (Category != 'Equity' OR @MonthIndex % 3 = 0); -- Equity less frequent

    IF @DocId % 5 = 0
        PRINT 'Processed document ' + CAST(@DocId AS VARCHAR) + ' for period ' + @Period;

    FETCH NEXT FROM doc_cursor INTO @DocId, @Period, @MonthIndex;
END;

CLOSE doc_cursor;
DEALLOCATE doc_cursor;

-- Cleanup
DROP TABLE #AccountDefinitions;

PRINT 'Financial data generation complete!';
GO

-- =============================================
-- STEP 4: Generate AI Analyses (Realistic distribution)
-- =============================================

PRINT 'Generating AI analyses...';

DECLARE @AnalysisCounter INT = 1;
DECLARE @TotalAnalyses INT = 500; -- Reduced to more realistic number
DECLARE @DocCount INT = (SELECT COUNT(*) FROM FinancialDocuments);

WHILE @AnalysisCounter <= @TotalAnalyses
BEGIN
    DECLARE @RandomDocId INT = CAST((RAND(CHECKSUM(NEWID())) * @DocCount + 1) AS INT);

    -- Realistic analysis type distribution
    DECLARE @RandomType VARCHAR(50) = CASE CAST((RAND(CHECKSUM(NEWID())) * 100) AS INT) % 7
        WHEN 0 THEN 'Summary'
        WHEN 1 THEN 'TrendAnalysis'
        WHEN 2 THEN 'AnomalyDetection'
        WHEN 3 THEN 'RatioAnalysis'
        WHEN 4 THEN 'Comparison'
        WHEN 5 THEN 'Forecasting'
        ELSE 'Custom'
    END;

    DECLARE @ExecutionTime INT = CAST((RAND(CHECKSUM(NEWID())) * 4500 + 800) AS INT); -- 800-5300ms

    INSERT INTO AIAnalyses (DocumentId, AnalysisType, Prompt, Response, ModelUsed, ExecutionTime, CreatedDate, Rating)
    VALUES (
        @RandomDocId,
        @RandomType,
        'Analyze the financial data for ' + @RandomType + ' insights',
        CASE @RandomType
            WHEN 'Summary' THEN 'Financial Summary: Strong revenue growth of ' + CAST(CAST((RAND(CHECKSUM(NEWID())) * 30 + 10) AS INT) AS VARCHAR) + '% observed. Operating margins healthy at ' + CAST(CAST((RAND(CHECKSUM(NEWID())) * 15 + 15) AS INT) AS VARCHAR) + '%. Key strengths include diversified revenue streams.'
            WHEN 'TrendAnalysis' THEN 'Trend Analysis: Revenue showing consistent upward trend with ' + CAST(CAST((RAND(CHECKSUM(NEWID())) * 5 + 10) AS INT) AS VARCHAR) + '% quarterly growth. Expenses well-controlled with stable margins.'
            WHEN 'AnomalyDetection' THEN 'Anomaly Detection: Identified ' + CAST(CAST((RAND(CHECKSUM(NEWID())) * 5 + 1) AS INT) AS VARCHAR) + ' unusual patterns. Investigation recommended for accounts with >3? deviation.'
            WHEN 'RatioAnalysis' THEN 'Ratio Analysis: Current Ratio: ' + CAST(CAST((RAND(CHECKSUM(NEWID())) * 2 + 1.5) AS DECIMAL(4,2)) AS VARCHAR) + ', Debt-to-Equity: ' + CAST(CAST((RAND(CHECKSUM(NEWID())) * 0.5 + 0.4) AS DECIMAL(4,2)) AS VARCHAR) + ', Profit Margin: ' + CAST(CAST((RAND(CHECKSUM(NEWID())) * 10 + 15) AS INT) AS VARCHAR) + '%'
            WHEN 'Comparison' THEN 'Period Comparison: Current period shows ' + CAST(CAST((RAND(CHECKSUM(NEWID())) * 20 + 5) AS INT) AS VARCHAR) + '% improvement vs prior period with controlled expense growth.'
            WHEN 'Forecasting' THEN 'Forecast: Based on 12-month trends, revenue projected to reach $' + CAST(CAST((RAND(CHECKSUM(NEWID())) * 500000 + 1500000) AS INT) AS VARCHAR) + ' in next quarter with 85% confidence.'
            ELSE 'Custom Analysis: Comprehensive financial analysis completed with actionable insights. ' + CAST(@AnalysisCounter AS VARCHAR) + ' key recommendations provided.'
        END,
        CASE
            WHEN @AnalysisCounter % 3 = 0 THEN 'llama3.2'
            WHEN @AnalysisCounter % 3 = 1 THEN 'qwen2.5:8b'
            ELSE 'llama3.2-vision'
        END,
        @ExecutionTime,
        DATEADD(MINUTE, -CAST((RAND(CHECKSUM(NEWID())) * 50000) AS INT), GETDATE()),
        CASE
            WHEN @AnalysisCounter % 7 = 0 THEN NULL  -- 14% unrated
            ELSE CAST((RAND(CHECKSUM(NEWID())) * 2 + 3.5) AS INT)  -- Rating 3-5
        END
    );

    IF @AnalysisCounter % 100 = 0
        PRINT 'Generated ' + CAST(@AnalysisCounter AS VARCHAR) + ' analyses';

    SET @AnalysisCounter = @AnalysisCounter + 1;
END;

PRINT 'AI analyses generation complete!';
GO

-- =============================================
-- STEP 5: Generate Reports (Realistic distribution)
-- =============================================

PRINT 'Generating reports...';

DECLARE @ReportCounter INT = 1;
DECLARE @TotalReports INT = 150;

WHILE @ReportCounter <= @TotalReports
BEGIN
    DECLARE @ReportType VARCHAR(50) = CASE CAST((RAND(CHECKSUM(NEWID())) * 100) AS INT) % 6
        WHEN 0 THEN 'Summary'
        WHEN 1 THEN 'Detailed'
        WHEN 2 THEN 'Comparison'
        WHEN 3 THEN 'Trend'
        WHEN 4 THEN 'RatioAnalysis'
        ELSE 'Custom'
    END;

    DECLARE @ReportDocId INT = CAST((RAND(CHECKSUM(NEWID())) * 24 + 1) AS INT);

    INSERT INTO Reports (Title, Description, ReportType, GeneratedDate, Content, Parameters)
    VALUES (
        @ReportType + ' Financial Report #' + CAST(@ReportCounter AS VARCHAR),
        'Comprehensive ' + @ReportType + ' financial report for period analysis',
        @ReportType,
        DATEADD(HOUR, -CAST((RAND(CHECKSUM(NEWID())) * 2000) AS INT), GETDATE()),
        '{"reportType":"' + @ReportType + '","summary":"Generated successfully","metrics":{"recordsAnalyzed":' + CAST(CAST((RAND(CHECKSUM(NEWID())) * 5000 + 1000) AS INT) AS VARCHAR) + '}}',
        '{"documentId":' + CAST(@ReportDocId AS VARCHAR) + ',"period":"2023-' + RIGHT('0' + CAST(CAST((RAND(CHECKSUM(NEWID())) * 12 + 1) AS INT) AS VARCHAR), 2) + '","format":"Excel"}'
    );

    IF @ReportCounter % 50 = 0
        PRINT 'Generated ' + CAST(@ReportCounter AS VARCHAR) + ' reports';

    SET @ReportCounter = @ReportCounter + 1;
END;

PRINT 'Reports generation complete!';
GO

-- =============================================
-- STEP 6: Seed Industry Benchmark Data (Phase 12)
-- =============================================

PRINT 'Seeding industry benchmark data...';

-- Technology Industry Benchmarks
INSERT INTO IndustryBenchmarks (Industry, MetricName, AverageValue, MedianValue, Percentile25, Percentile75, LastUpdated, DataSource, SampleSize, Notes) VALUES
(0, 'GrossMargin', 65.0, 68.0, 55.0, 75.0, GETUTCDATE(), 'S&P Global Market Intelligence', 250, 'Software and technology services companies'),
(0, 'OperatingMargin', 15.0, 18.0, 5.0, 25.0, GETUTCDATE(), 'S&P Global Market Intelligence', 250, 'EBITDA margins for tech sector'),
(0, 'NetMargin', 12.0, 15.0, 2.0, 22.0, GETUTCDATE(), 'S&P Global Market Intelligence', 250, 'Net profit margins vary significantly by sub-sector'),
(0, 'CurrentRatio', 2.8, 2.5, 1.8, 3.5, GETUTCDATE(), 'Industry Financial Reports', 200, 'Strong liquidity position typical in tech'),
(0, 'DebtToEquity', 0.3, 0.2, 0.0, 0.6, GETUTCDATE(), 'S&P Global Market Intelligence', 250, 'Conservative leverage in technology sector'),
(0, 'ReturnOnAssets', 8.5, 9.0, 4.0, 14.0, GETUTCDATE(), 'Industry Financial Reports', 200, 'Asset efficiency varies by business model'),
(0, 'ReturnOnEquity', 15.0, 16.0, 8.0, 22.0, GETUTCDATE(), 'Industry Financial Reports', 200, 'ROE varies by business model'),

-- Healthcare Industry Benchmarks
(1, 'GrossMargin', 58.0, 60.0, 45.0, 70.0, GETUTCDATE(), 'Healthcare Financial Management Association', 180, 'Hospitals and healthcare providers'),
(1, 'OperatingMargin', 4.5, 5.0, 1.0, 8.0, GETUTCDATE(), 'Healthcare Financial Management Association', 180, 'Tight margins in healthcare sector'),
(1, 'CurrentRatio', 1.9, 1.8, 1.2, 2.5, GETUTCDATE(), 'Industry Financial Reports', 150, 'Adequate liquidity for operations'),
(1, 'DebtToEquity', 0.8, 0.7, 0.3, 1.2, GETUTCDATE(), 'Healthcare Financial Management Association', 180, 'Moderate leverage in healthcare'),

-- Finance Industry Benchmarks
(2, 'NetMargin', 18.0, 20.0, 12.0, 25.0, GETUTCDATE(), 'Federal Reserve Bank Data', 300, 'Strong profitability in banking sector'),
(2, 'ReturnOnEquity', 12.5, 13.0, 8.0, 18.0, GETUTCDATE(), 'Federal Reserve Bank Data', 300, 'ROE varies by bank size and business model'),
(2, 'CurrentRatio', 1.2, 1.1, 0.8, 1.5, GETUTCDATE(), 'Federal Reserve Bank Data', 300, 'Liquidity requirements for banks'),
(2, 'DebtToEquity', 8.0, 7.5, 5.0, 12.0, GETUTCDATE(), 'Federal Reserve Bank Data', 300, 'High leverage typical in banking'),

-- Manufacturing Industry Benchmarks
(3, 'GrossMargin', 35.0, 38.0, 25.0, 45.0, GETUTCDATE(), 'National Association of Manufacturers', 220, 'Manufacturing sector margins'),
(3, 'AssetTurnover', 1.2, 1.1, 0.8, 1.6, GETUTCDATE(), 'Industry Financial Reports', 180, 'Asset utilization in manufacturing'),
(3, 'CurrentRatio', 1.8, 1.7, 1.3, 2.2, GETUTCDATE(), 'Industry Financial Reports', 180, 'Working capital management'),
(3, 'DebtToEquity', 0.7, 0.6, 0.3, 1.1, GETUTCDATE(), 'National Association of Manufacturers', 220, 'Capital structure in manufacturing'),

-- Retail Industry Benchmarks
(4, 'GrossMargin', 22.0, 25.0, 15.0, 30.0, GETUTCDATE(), 'National Retail Federation', 160, 'Retail sector gross margins'),
(4, 'InventoryTurnover', 4.8, 5.0, 3.2, 6.5, GETUTCDATE(), 'National Retail Federation', 160, 'Inventory management efficiency'),
(4, 'CurrentRatio', 1.5, 1.4, 1.0, 2.0, GETUTCDATE(), 'Industry Financial Reports', 140, 'Working capital in retail'),
(4, 'AssetTurnover', 2.1, 2.0, 1.5, 2.8, GETUTCDATE(), 'National Retail Federation', 160, 'Asset efficiency in retail'),

-- Energy Industry Benchmarks
(5, 'OperatingMargin', 8.5, 9.0, 3.0, 15.0, GETUTCDATE(), 'Energy Information Administration', 140, 'Energy sector profitability varies by commodity prices'),
(5, 'DebtToEquity', 0.8, 0.7, 0.3, 1.4, GETUTCDATE(), 'Energy Information Administration', 140, 'Capital intensive industry with higher leverage'),
(5, 'CurrentRatio', 1.3, 1.2, 0.9, 1.8, GETUTCDATE(), 'Industry Financial Reports', 120, 'Liquidity management in energy'),
(5, 'ReturnOnAssets', 4.5, 5.0, 2.0, 8.0, GETUTCDATE(), 'Energy Information Administration', 140, 'Asset returns vary by energy type'),

-- Real Estate Industry Benchmarks
(6, 'OperatingMargin', 25.0, 28.0, 15.0, 35.0, GETUTCDATE(), 'National Association of Realtors', 190, 'Real estate operating margins'),
(6, 'DebtToEquity', 1.8, 1.6, 1.0, 2.5, GETUTCDATE(), 'National Association of Realtors', 190, 'High leverage typical in real estate'),
(6, 'CurrentRatio', 1.1, 1.0, 0.7, 1.5, GETUTCDATE(), 'Industry Financial Reports', 160, 'Liquidity in real estate'),
(6, 'AssetTurnover', 0.15, 0.12, 0.08, 0.22, GETUTCDATE(), 'National Association of Realtors', 190, 'Asset turnover in real estate'),

-- Transportation Industry Benchmarks
(7, 'OperatingMargin', 6.5, 7.0, 2.0, 12.0, GETUTCDATE(), 'Transportation Industry Analysis', 130, 'Transportation sector margins'),
(7, 'CurrentRatio', 1.4, 1.3, 0.9, 1.9, GETUTCDATE(), 'Industry Financial Reports', 110, 'Working capital in transportation'),
(7, 'DebtToEquity', 1.2, 1.1, 0.6, 1.8, GETUTCDATE(), 'Transportation Industry Analysis', 130, 'Capital structure in transportation'),
(7, 'AssetTurnover', 0.8, 0.7, 0.5, 1.1, GETUTCDATE(), 'Transportation Industry Analysis', 130, 'Asset utilization'),

-- Professional Services Industry Benchmarks
(8, 'GrossMargin', 45.0, 48.0, 35.0, 55.0, GETUTCDATE(), 'Professional Services Industry Report', 170, 'Professional services margins'),
(8, 'OperatingMargin', 12.0, 14.0, 5.0, 20.0, GETUTCDATE(), 'Professional Services Industry Report', 170, 'Operating margins in professional services'),
(8, 'CurrentRatio', 2.2, 2.0, 1.5, 2.8, GETUTCDATE(), 'Industry Financial Reports', 140, 'Strong liquidity in professional services'),
(8, 'DebtToEquity', 0.4, 0.3, 0.1, 0.8, GETUTCDATE(), 'Professional Services Industry Report', 170, 'Conservative leverage'),

-- Agriculture Industry Benchmarks
(9, 'GrossMargin', 18.0, 20.0, 12.0, 25.0, GETUTCDATE(), 'USDA Agricultural Statistics', 120, 'Agriculture sector margins'),
(9, 'OperatingMargin', 8.0, 9.0, 3.0, 14.0, GETUTCDATE(), 'USDA Agricultural Statistics', 120, 'Operating margins in agriculture'),
(9, 'CurrentRatio', 1.6, 1.5, 1.1, 2.1, GETUTCDATE(), 'Industry Financial Reports', 100, 'Liquidity in agriculture'),
(9, 'DebtToEquity', 0.9, 0.8, 0.4, 1.4, GETUTCDATE(), 'USDA Agricultural Statistics', 120, 'Leverage in agricultural businesses');

PRINT 'Industry benchmark data seeded successfully!';
GO

-- =============================================
-- STEP 7: Verify Data and Show Statistics
-- =============================================

PRINT '';
PRINT '=============================================';
PRINT '   DATABASE SEEDING COMPLETE!';
PRINT '=============================================';
PRINT '';
PRINT 'RECORD COUNTS:';
PRINT '-------------';
PRINT 'Financial Documents:    ' + CAST((SELECT COUNT(*) FROM FinancialDocuments) AS VARCHAR);
PRINT 'Financial Data Records: ' + CAST((SELECT COUNT(*) FROM FinancialDataRecords) AS VARCHAR);
PRINT 'AI Analyses:            ' + CAST((SELECT COUNT(*) FROM AIAnalyses) AS VARCHAR);
PRINT 'Reports:                ' + CAST((SELECT COUNT(*) FROM Reports) AS VARCHAR);
PRINT 'Industry Benchmarks:    ' + CAST((SELECT COUNT(*) FROM IndustryBenchmarks) AS VARCHAR);
PRINT '';

PRINT 'DATA QUALITY CHECKS:';
PRINT '-------------------';

-- Check period distribution
SELECT
    'Period Distribution' AS CheckType,
    COUNT(DISTINCT Period) AS DistinctPeriods,
    MIN(Period) AS EarliestPeriod,
    MAX(Period) AS LatestPeriod
FROM FinancialDataRecords;

-- Check category totals
SELECT
    Category,
    COUNT(*) AS RecordCount,
    FORMAT(SUM(Amount), 'C', 'en-US') AS TotalAmount,
    FORMAT(AVG(Amount), 'C', 'en-US') AS AvgAmount
FROM FinancialDataRecords
GROUP BY Category
ORDER BY Category;

PRINT '';
PRINT 'INDUSTRY BENCHMARK VERIFICATION:';
PRINT '--------------------------------';

SELECT
    Industry,
    COUNT(*) AS BenchmarkCount,
    STRING_AGG(MetricName, ', ') AS Metrics
FROM IndustryBenchmarks
GROUP BY Industry
ORDER BY Industry;

SELECT
    'Total Benchmarks' AS Metric,
    COUNT(*) AS Count
FROM IndustryBenchmarks;

SELECT
    MetricName,
    COUNT(*) AS IndustryCount,
    FORMAT(AVG(AverageValue), 'F2') AS AvgValue,
    FORMAT(MIN(AverageValue), 'F2') AS MinValue,
    FORMAT(MAX(AverageValue), 'F2') AS MaxValue
FROM IndustryBenchmarks
GROUP BY MetricName
ORDER BY MetricName;

PRINT '';
PRINT 'SAMPLE DATA:';
PRINT '-----------';

-- Show latest document
SELECT TOP 1
    'Latest Document:' AS Info,
    FileName,
    FileType,
    Status,
    FORMAT(UploadDate, 'yyyy-MM-dd') AS UploadDate
FROM FinancialDocuments
ORDER BY UploadDate DESC;

-- Show revenue trend
SELECT TOP 12
    Period,
    FORMAT(SUM(Amount), 'C0', 'en-US') AS TotalRevenue
FROM FinancialDataRecords
WHERE Category = 'Revenue'
GROUP BY Period
ORDER BY Period DESC;

PRINT '';
PRINT '=============================================';
PRINT '  Seeding completed successfully!';
PRINT '  You can now use the AI Insights page to see';
PRINT '  Industry Benchmarking, Investment Advice,';
PRINT '  and Cash Flow Optimization features!';
PRINT '  The Trends page shows beautiful charts';
PRINT '  with 24 months of data!'; 
PRINT '=============================================';

GO