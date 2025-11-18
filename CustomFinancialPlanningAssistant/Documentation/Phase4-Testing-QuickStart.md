# ?? Phase 4 Testing - Quick Start Guide

## ? What We Just Created

1. **Upload Test Page** - `/upload-test`
   - Drag & drop or select files
   - Real-time progress indicator
   - Detailed results display
   - Error handling with helpful messages

2. **Sample CSV File** - `SampleFinancialData.csv`
   - 20 financial records ready to import
   - Revenue, Expenses, Assets, Liabilities, Equity
   - Properly formatted with all required columns

3. **Navigation Link** - "Upload Test" in main menu
   - Easy access to test page

---

## ?? How to Test RIGHT NOW

### Step 1: Start the Application

```powershell
# If not already running:
cd C:\source\CustomFinancialPlanningAssistant\CustomFinancialPlanningAssistant
dotnet run

# Application will start at:
https://localhost:5001
```

### Step 2: Navigate to Upload Test

**Option A:** Click "Upload Test" in the left navigation menu

**Option B:** Go directly to: `https://localhost:5001/upload-test`

### Step 3: Upload the Sample CSV

1. **Click "Choose File"** button
2. **Navigate to:** `C:\source\CustomFinancialPlanningAssistant\CustomFinancialPlanningAssistant\Documentation\`
3. **Select:** `SampleFinancialData.csv`
4. **Click "Upload & Process"** button
5. **Wait** 2-5 seconds for processing

### Step 4: Verify Success

You should see:
- ? **"Upload Successful!"** message
- **Document ID:** (a number - save this!)
- **Records Imported:** 20
- **Processing Time:** ~500-2000 ms

---

## ?? Verify in Database

### Option 1: SQL Server Management Studio

```sql
-- Connect to: BVUKAS5080\MSSQL2025
-- Database: FinancialAnalysisDB

-- Check the document was created
SELECT TOP 1 * 
FROM FinancialDocuments 
ORDER BY UploadDate DESC;

-- Check all imported records (replace X with your Document ID from upload)
SELECT * 
FROM FinancialData 
WHERE DocumentId = X
ORDER BY AccountName;

-- Summary by category
SELECT 
    Category,
    COUNT(*) as Count,
    SUM(Amount) as Total,
    CAST(SUM(Amount) as money) as TotalFormatted
FROM FinancialData
WHERE DocumentId = X
GROUP BY Category
ORDER BY Category;
```

**Expected Results:**
- **FinancialDocuments:** 1 new row
  - FileName: SampleFinancialData.csv
  - FileType: CSV
  - Status: Analyzed
  - FileSize: ~1500 bytes

- **FinancialData:** 20 new rows
  - All linked to same DocumentId
  - 3 Revenue records ($275,000 total)
  - 10 Expense records ($197,000 total)
  - 4 Asset records ($183,000 total)
  - 2 Liability records ($78,000 total)
  - 1 Equity record ($250,000 total)

---

## ?? Additional Tests to Try

### Test 2: Create Your Own CSV

Create a simple CSV file with this content:

```csv
Account Name,Period,Amount,Category
Test Revenue,2024-Q1,50000,Revenue
Test Expense,2024-Q1,30000,Expense
```

Save as `MyTest.csv` and upload it.

**Expected:** 2 records imported

### Test 3: Create Excel File

1. Open `SampleFinancialData.csv` in Excel
2. Save As ? Excel Workbook (*.xlsx)
3. Upload the .xlsx file
4. Should work identically to CSV

### Test 4: Test Validation

Try uploading:
- ? A .txt file (should fail: "Unsupported file type")
- ? A very large file >50MB (should fail: "File too large")
- ? An empty CSV (should fail: "No data found")

---

## ?? Troubleshooting

### Issue: "No financial data found"
**Check:**
- Does your CSV have headers in row 1?
- Are the headers spelled correctly?
- Is there data in row 2 and beyond?

**Fix:** Make sure columns include at least:
- Account Name
- Amount
- Category
- Period

### Issue: "File validation failed"
**Check:**
- Is the file < 50MB?
- Is the extension .csv, .xlsx, .xls, or .pdf?
- Is the file corrupted?

**Fix:** Try re-saving the file or using the provided sample

### Issue: Page shows "Processing..." but never completes
**Check:**
- Look at the browser console (F12) for errors
- Check the application logs for exceptions
- Verify database connection is working

**Fix:** 
1. Restart the application
2. Check database connection string
3. Ensure SQL Server is running

---

## ?? What You Should See

### 1. Before Upload
- File selector
- Instructions
- Sample files information

### 2. After Selecting File
- File name displayed
- File size shown
- "Upload & Process" button enabled
- File type detected

### 3. During Processing
- Progress animation
- "Processing..." message
- Upload button disabled

### 4. After Success
- Green success box
- Document ID
- Number of records imported
- Processing time
- Database information section

### 5. If Errors Occur
- Red error box
- Specific error messages
- Troubleshooting hints

---

## ?? Success Criteria

Your test is successful if:

? Upload page loads without errors  
? You can select a file  
? File information displays correctly  
? Processing completes within 5 seconds  
? Success message appears  
? Document ID is returned  
? "Records Imported: 20" is shown  
? Database has 1 new FinancialDocument record  
? Database has 20 new FinancialData records  
? All records link to the same DocumentId  

---

## ?? Test Results Template

Document your test:

```
Date: _______________
Tester: _______________

Test 1: CSV Upload
[ ] Page loads successfully
[ ] File selector works
[ ] Upload completes
[ ] Success message shown
[ ] Document ID: _______
[ ] Records: 20
[ ] Time: _____ ms

Database Verification:
[ ] FinancialDocuments record exists
[ ] 20 FinancialData records exist
[ ] All records linked correctly

Issues Encountered:
_________________________________
_________________________________

Overall Result: [ ] PASS [ ] FAIL
```

---

## ?? Next Steps After Testing

Once CSV upload works:

1. ? **Test Excel Upload**
   - Convert CSV to .xlsx
   - Upload and verify

2. ? **Test PDF Upload** (Optional)
   - Requires AI vision model running
   - Slower but demonstrates AI capability

3. ? **Move to Phase 5**
   - Financial calculations
   - Ratio analysis
   - Trend detection

---

## ?? Pro Tips

1. **Keep the Document ID** from your first upload - you'll need it to query data

2. **Watch the logs** in the console - they show detailed processing information

3. **Check file size** - The upload shows you the size before processing

4. **Try different formats** - CSV is fastest, Excel works identically

5. **Database tip:** Use the Document ID to filter queries:
   ```sql
   SELECT * FROM FinancialData WHERE DocumentId = 1
   ```

---

## ?? Related Documentation

- **Quick Reference:** `DocumentProcessor-QuickReference.md`
- **Sample Files Guide:** `SampleTestFiles-Guide.md`
- **Phase 4 Report:** `Phase4-CompletionReport.md`

---

**Ready to test?** ??

1. Start the app: `dotnet run`
2. Go to: `https://localhost:5001/upload-test`
3. Upload: `SampleFinancialData.csv`
4. Celebrate! ??

**Test Duration:** ~5 minutes  
**Difficulty:** Easy  
**Prerequisites:** Application running, Database accessible
