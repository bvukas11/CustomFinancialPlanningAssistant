# ??? Database Setup - Complete Guide

## ? Database Schema Created Successfully

The database schema has been created using Entity Framework Core migrations.

---

## ?? Database Information

**Server:** `BVUKAS5080\MSSQL2025`  
**Database:** `FinancialAnalysisDB`  
**Connection:** Trusted Connection (Windows Authentication)  
**Status:** ? Created and Ready

---

## ??? Schema Overview

### Tables Created

#### 1. **FinancialDocuments**
Stores metadata about uploaded documents.

| Column | Type | Description |
|--------|------|-------------|
| Id | int | Primary key (auto-increment) |
| FileName | nvarchar(255) | Name of uploaded file |
| FileType | nvarchar(50) | Type (Excel, CSV, PDF) |
| UploadDate | datetime2 | When file was uploaded |
| FileSize | bigint | Size in bytes |
| FilePath | nvarchar(500) | Path to stored file |
| Status | nvarchar(50) | Processing status |
| CreatedBy | nvarchar(100) | User who uploaded |

#### 2. **FinancialData**
Stores individual financial records extracted from documents.

| Column | Type | Description |
|--------|------|-------------|
| Id | int | Primary key (auto-increment) |
| DocumentId | int | Foreign key to FinancialDocuments |
| AccountName | nvarchar(200) | Account name |
| AccountCode | nvarchar(50) | Account code/number |
| Period | nvarchar(50) | Time period (e.g., 2024-Q1) |
| Amount | decimal(18,2) | Financial amount |
| Currency | nvarchar(10) | Currency code (e.g., USD) |
| Category | nvarchar(100) | Category (Revenue, Expense, etc.) |
| SubCategory | nvarchar(100) | Sub-category |
| DateRecorded | datetime2 | Record date |

#### 3. **AIAnalyses**
Stores AI-generated financial analyses.

| Column | Type | Description |
|--------|------|-------------|
| Id | int | Primary key (auto-increment) |
| DocumentId | int | Foreign key to FinancialDocuments |
| AnalysisType | nvarchar(50) | Type of analysis |
| Prompt | nvarchar(max) | AI prompt used |
| Response | nvarchar(max) | AI response |
| ModelUsed | nvarchar(50) | AI model name |
| ExecutionTime | int | Processing time (ms) |
| CreatedDate | datetime2 | When analysis was created |

#### 4. **Reports**
Stores generated financial reports.

| Column | Type | Description |
|--------|------|-------------|
| Id | int | Primary key (auto-increment) |
| DocumentId | int | Foreign key to FinancialDocuments |
| ReportType | nvarchar(100) | Type of report |
| Title | nvarchar(200) | Report title |
| Content | nvarchar(max) | Report content (JSON/HTML) |
| GeneratedDate | datetime2 | When report was generated |
| GeneratedBy | nvarchar(100) | User who generated |

---

## ?? Migration Files Created

### Location
```
CustomFinancialPlanningAssistant\Migrations\
??? 20251118134429_InitialCreate.cs           # Migration up/down methods
??? 20251118134429_InitialCreate.Designer.cs  # Migration metadata
??? AppDbContextModelSnapshot.cs              # Current model snapshot
```

### Migration Name
**InitialCreate** - Created on 2025-01-18 at 13:44:29

---

## ?? Automatic Migration on Startup

The application is now configured to **automatically apply migrations** when it starts.

### Code Added to Program.cs
```csharp
// Apply database migrations automatically on startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        var logger = services.GetRequiredService<ILogger<Program>>();
        
        logger.LogInformation("Applying database migrations...");
        context.Database.Migrate();
        logger.LogInformation("Database migrations applied successfully");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database");
        if (builder.Environment.IsDevelopment())
        {
            throw; // Fail fast in development
        }
    }
}
```

### Benefits
? Database always up-to-date  
? No manual migration required  
? Safe for development and production  
? Automatic on every app start  

---

## ?? Connection String Configuration

### appsettings.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=BVUKAS5080\\MSSQL2025;Database=FinancialAnalysisDB;Trusted_Connection=True;MultipleActiveResultSets=True;TrustServerCertificate=True"
  }
}
```

### Key Settings
- **Server:** Local SQL Server instance
- **Trusted_Connection:** Uses Windows Authentication
- **MultipleActiveResultSets:** Enables multiple queries
- **TrustServerCertificate:** Required for local SQL Server 2025

---

## ? Verification Steps

### 1. Check Database Exists
```sql
-- Connect to SQL Server Management Studio
-- Server: BVUKAS5080\MSSQL2025

-- Verify database exists
SELECT name FROM sys.databases WHERE name = 'FinancialAnalysisDB';

-- Should return: FinancialAnalysisDB
```

### 2. Check Tables Created
```sql
USE FinancialAnalysisDB;

-- List all tables
SELECT TABLE_NAME 
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_TYPE = 'BASE TABLE'
ORDER BY TABLE_NAME;

-- Should return:
-- AIAnalyses
-- FinancialData
-- FinancialDocuments
-- Reports
-- __EFMigrationsHistory (EF Core tracking table)
```

### 3. Check Table Structure
```sql
-- Check FinancialDocuments table
SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'FinancialDocuments'
ORDER BY ORDINAL_POSITION;

-- Check FinancialData table
SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'FinancialData'
ORDER BY ORDINAL_POSITION;
```

### 4. Verify Foreign Keys
```sql
-- Check foreign key relationships
SELECT 
    fk.name AS ForeignKey,
    OBJECT_NAME(fk.parent_object_id) AS TableName,
    COL_NAME(fkc.parent_object_id, fkc.parent_column_id) AS ColumnName,
    OBJECT_NAME(fk.referenced_object_id) AS ReferencedTable,
    COL_NAME(fkc.referenced_object_id, fkc.referenced_column_id) AS ReferencedColumn
FROM sys.foreign_keys AS fk
INNER JOIN sys.foreign_key_columns AS fkc 
    ON fk.object_id = fkc.constraint_object_id
WHERE OBJECT_NAME(fk.parent_object_id) IN ('FinancialData', 'AIAnalyses', 'Reports')
ORDER BY TableName;
```

---

## ?? Testing the Setup

### Test 1: Application Startup
1. Start the application: `dotnet run`
2. Check logs for: "Applying database migrations..."
3. Should see: "Database migrations applied successfully"

### Test 2: Upload Test Data
1. Navigate to: `https://localhost:5001/upload-test`
2. Upload: `SampleFinancialData.csv`
3. Verify success message with Document ID

### Test 3: Query Database
```sql
-- Check uploaded documents
SELECT * FROM FinancialDocuments ORDER BY UploadDate DESC;

-- Check imported data
SELECT * FROM FinancialData ORDER BY Id DESC;

-- Count records by category
SELECT Category, COUNT(*) as Count, SUM(Amount) as Total
FROM FinancialData
GROUP BY Category
ORDER BY Category;
```

---

## ?? Future Migrations

### When to Create a New Migration

Create a new migration when you:
- Add/remove entities
- Add/remove properties
- Change data types
- Add/remove relationships
- Add indexes or constraints

### How to Create a Migration

```powershell
# 1. Make changes to your entity classes
# 2. Create migration
cd C:\source\CustomFinancialPlanningAssistant\CustomFinancialPlanningAssistant
dotnet ef migrations add [MigrationName]

# Example:
dotnet ef migrations add AddUserTable
dotnet ef migrations add AddIndexToFinancialData

# 3. Apply migration (automatic on startup, or manually):
dotnet ef database update
```

### How to Rollback a Migration

```powershell
# Rollback to previous migration
dotnet ef database update [PreviousMigrationName]

# Remove last migration (if not applied)
dotnet ef migrations remove
```

---

## ??? Troubleshooting

### Issue: "Database does not exist"
**Solution:** The app will create it automatically on first run with migrations enabled.

### Issue: "Login failed" or "SSL Provider error"
**Solution:** Add `TrustServerCertificate=True` to connection string (already done).

### Issue: "Migration already applied"
**Solution:** This is normal - EF Core tracks which migrations are applied in `__EFMigrationsHistory` table.

### Issue: "Cannot drop database because it is in use"
**Solution:** 
1. Stop the application
2. Close all connections in SSMS
3. Try again

### Issue: Need to reset database completely
**Solution:**
```sql
-- WARNING: This deletes ALL data!
USE master;
GO
DROP DATABASE FinancialAnalysisDB;
GO
```
Then restart the application - it will recreate everything.

---

## ?? Current Database State

After successful setup:

```
FinancialAnalysisDB
??? Tables
?   ??? FinancialDocuments (0 rows) ?
?   ??? FinancialData (0 rows) ?
?   ??? AIAnalyses (0 rows) ?
?   ??? Reports (0 rows) ?
?   ??? __EFMigrationsHistory (1 row: InitialCreate) ?
??? Foreign Keys
?   ??? FinancialData ? FinancialDocuments ?
?   ??? AIAnalyses ? FinancialDocuments ?
?   ??? Reports ? FinancialDocuments ?
??? Indexes
    ??? Primary Keys on all tables ?
```

---

## ?? Ready for Testing!

Your database is now:
- ? Created and configured
- ? Schema fully applied
- ? Foreign keys in place
- ? Ready to accept data
- ? Auto-migrates on startup

**Next step:** Upload your CSV file and watch the data flow into the database!

---

## ?? Related Documentation

- **Entity Framework Core:** https://docs.microsoft.com/en-us/ef/core/
- **Migrations Guide:** https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations/
- **SQL Server Setup:** `DatabaseSeeding.md`
- **Upload Testing:** `Phase4-Testing-QuickStart.md`

---

**Created:** 2025-01-18  
**Migration:** InitialCreate (20251118134429)  
**Status:** ? Complete and Tested  
**Auto-Migrate:** Enabled
