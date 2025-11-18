# Sample Test Files Guide

## ?? Available Test Files

### 1. SampleFinancialData.csv
**Location:** `CustomFinancialPlanningAssistant\Documentation\SampleFinancialData.csv`

**Contents:** 20 sample financial records including:
- Revenue accounts (Sales, Services, Consulting)
- Expense accounts (Salaries, Rent, Marketing, etc.)
- Asset accounts (Cash, Receivables, Equipment)
- Liability accounts (Payables, Loans)
- Equity accounts (Owner's Equity)

**Format:**
```csv
Account Name,Account Code,Period,Amount,Currency,Category,Sub Category,Date
Sales Revenue,4000,2024-Q1,150000,USD,Revenue,Product Sales,2024-03-31
...
```

**How to Use:**
1. Navigate to `/upload-test` in your application
2. Click "Choose File" and select `SampleFinancialData.csv`
3. Click "Upload & Process"
4. Should import 20 records successfully

---

## ?? Creating Your Own Excel Test File

Since we can't directly create .xlsx files in code, here's how to create one manually:

### Option 1: Convert CSV to Excel (Easiest)
1. Open the `SampleFinancialData.csv` file in Excel
2. Click File ? Save As
3. Choose "Excel Workbook (*.xlsx)" as the format
4. Save as `SampleFinancialData.xlsx`

### Option 2: Create from Scratch in Excel

**Step 1: Create Headers (Row 1)**
| A | B | C | D | E | F | G | H |
|---|---|---|---|---|---|---|---|
| Account Name | Account Code | Period | Amount | Currency | Category | Sub Category | Date |

**Step 2: Add Sample Data (Starting Row 2)**
| Account Name | Account Code | Period | Amount | Currency | Category | Sub Category | Date |
|---|---|---|---|---|---|---|---|
| Sales Revenue | 4000 | 2024-Q1 | 150000 | USD | Revenue | Product Sales | 2024-03-31 |
| Service Revenue | 4100 | 2024-Q1 | 80000 | USD | Revenue | Services | 2024-03-31 |
| Salaries and Wages | 5000 | 2024-Q1 | 95000 | USD | Expense | Personnel | 2024-03-31 |
| Office Rent | 5200 | 2024-Q1 | 12000 | USD | Expense | Facilities | 2024-03-31 |
| Marketing Expenses | 5300 | 2024-Q1 | 25000 | USD | Expense | Marketing | 2024-03-31 |

**Step 3: Save**
- File ? Save As
- Choose "Excel Workbook (*.xlsx)"
- Save in Documentation folder

---

## ?? Sample Data Breakdown

### Revenue Accounts (Total: $275,000)
- Sales Revenue: $150,000
- Service Revenue: $80,000
- Consulting Revenue: $45,000

### Expense Accounts (Total: $197,000)
- Personnel (Salaries + Benefits): $113,000
- Facilities (Rent + Utilities): $15,500
- Marketing (Marketing + Advertising): $40,000
- Operations: $2,800
- Technology: $8,500
- Professional Services: $12,000
- Travel: $6,200

### Asset Accounts (Total: $183,000)
- Cash on Hand: $75,000
- Accounts Receivable: $42,000
- Inventory: $38,000
- Equipment: $28,000

### Liability Accounts (Total: $78,000)
- Accounts Payable: $28,000
- Short-term Loan: $50,000

### Equity Accounts (Total: $250,000)
- Owner's Equity: $250,000

### Financial Summary
- **Total Revenue:** $275,000
- **Total Expenses:** $197,000
- **Net Income:** $78,000
- **Profit Margin:** 28.4%

---

## ? Testing Checklist

### CSV Upload Test
- [ ] Navigate to `/upload-test`
- [ ] Upload `SampleFinancialData.csv`
- [ ] Verify "Upload Successful" message
- [ ] Check "Records Imported: 20"
- [ ] Note the Document ID returned

### Excel Upload Test (After Creating .xlsx)
- [ ] Create Excel file from CSV
- [ ] Upload the .xlsx file
- [ ] Verify successful import
- [ ] Compare import time (should be similar to CSV)

### Database Verification
- [ ] Open SQL Server Management Studio
- [ ] Connect to: `BVUKAS5080\MSSQL2025`
- [ ] Database: `FinancialAnalysisDB`
- [ ] Check `FinancialDocuments` table for new record
- [ ] Check `FinancialData` table for 20 new records

**SQL Query to Verify:**
```sql
-- Check latest document
SELECT TOP 1 * FROM FinancialDocuments 
ORDER BY UploadDate DESC

-- Check imported data (replace X with your Document ID)
SELECT * FROM FinancialData 
WHERE DocumentId = X
ORDER BY AccountName

-- Summary by Category
SELECT 
    Category,
    COUNT(*) as RecordCount,
    SUM(Amount) as TotalAmount
FROM FinancialData
WHERE DocumentId = X
GROUP BY Category
ORDER BY Category
```

---

## ?? Troubleshooting

### Issue: "No financial data found"
**Cause:** Headers not recognized or file format incorrect
**Solution:**
- Verify headers exactly match: Account Name, Amount, Category, Period
- Check that data starts in row 2 (row 1 is headers)
- Ensure no blank rows between header and data

### Issue: "File validation failed"
**Cause:** File too large or wrong format
**Solution:**
- Check file size < 50MB
- Verify extension is .xlsx, .xls, or .csv
- Try opening file first to ensure it's not corrupted

### Issue: Some rows skipped
**Cause:** Missing required fields
**Solution:**
- Every row must have: Account Name, Amount, Category, Period
- Optional fields: Account Code, Currency, Sub Category, Date
- Check rows that were skipped in the logs

---

## ?? Custom Test Data

Want to create your own test data? Use this template:

### Minimum Required Columns
```csv
Account Name,Period,Amount,Category
Test Account 1,2024-Q1,10000,Revenue
Test Account 2,2024-Q1,5000,Expense
```

### Full Column Set (Recommended)
```csv
Account Name,Account Code,Period,Amount,Currency,Category,Sub Category,Date
Custom Revenue,4000,2024-Q1,25000,USD,Revenue,Sales,2024-03-31
Custom Expense,5000,2024-Q1,15000,USD,Expense,Operations,2024-03-31
```

---

## ?? Expected Results

After uploading `SampleFinancialData.csv`:

**Upload Result:**
- ? Success: true
- ?? Document ID: (auto-generated number)
- ?? Records Imported: 20
- ? Processing Time: 500-2000ms (depending on system)

**Database Records:**
```
FinancialDocuments: 1 new record
- FileName: SampleFinancialData.csv
- FileType: CSV
- Status: Analyzed
- FileSize: ~1.5KB

FinancialData: 20 new records
- Linked to Document ID
- All have valid Account Names, Amounts, Categories, Periods
```

---

## ?? Additional Resources

- **Phase 4 Documentation:** `Phase4-CompletionReport.md`
- **Quick Reference:** `DocumentProcessor-QuickReference.md`
- **Testing Guide:** This document

---

**Last Updated:** 2025-01-17  
**Sample Data Version:** 1.0  
**Records in Sample:** 20
