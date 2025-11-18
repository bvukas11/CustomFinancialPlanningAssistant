# ?? Troubleshooting Button Click Issues

## Problem
Buttons in the application don't respond, and browser console shows:
```
Error: There was an unhandled exception on the current circuit
```

## Solution Steps

### Step 1: Enable Detailed Errors ?
Already done - added to `appsettings.Development.json`:
```json
{
  "DetailedErrors": true,
  ...
}
```

### Step 2: Check Service Diagnostics
Navigate to: **http://localhost:7265/diagnostics**

This page will show you which services are registered and which are missing.

### Step 3: Common Causes & Fixes

#### Cause 1: Database Not Initialized
**Symptoms:** Error on any page that accesses database
**Fix:**
```powershell
cd C:\source\CustomFinancialPlanningAssistant
dotnet ef database update --project CustomFinancialPlanningAssistant
```

#### Cause 2: Missing Service Registration
**Symptoms:** Specific pages fail to load
**Fix:** Check Program.cs - all services should be registered

#### Cause 3: Ollama Not Running (AI Service)
**Symptoms:** AI-related pages fail
**Fix:**
```powershell
# Check if Ollama is running
curl http://localhost:11434/api/version

# If not running, start it
ollama serve
```

#### Cause 4: SQL Server Not Running
**Symptoms:** All pages fail on database access
**Fix:** Start SQL Server LocalDB or your SQL Server instance

### Step 4: Restart Application
After fixing issues:
```powershell
# Stop the current instance (Ctrl+C)
# Then restart
cd C:\source\CustomFinancialPlanningAssistant
dotnet run --project CustomFinancialPlanningAssistant
```

### Step 5: Check Browser Console
With `DetailedErrors: true`, the browser console will now show the actual exception message.

Look for:
- Service resolution errors
- Database connection errors
- Null reference exceptions

### Step 6: Test Each Page Individually

1. **Dashboard** (/) - Tests: Document Repository, Financial Service
2. **Upload** (/upload) - Tests: Document Processor, File Storage
3. **Documents** (/documents) - Tests: Document Repository
4. **Analysis** (/analysis) - Tests: Financial Service
5. **Reports** (/reports) - Tests: Report Service

### Step 7: Check Logs
Look at the terminal where you ran `dotnet run` for detailed error messages.

## Quick Diagnostic Checklist

- [ ] SQL Server/LocalDB is running
- [ ] Database migrations applied (`dotnet ef database update`)
- [ ] All services registered in Program.cs
- [ ] appsettings.json has valid connection string
- [ ] Ollama is running (if using AI features)
- [ ] Build succeeded without errors
- [ ] DetailedErrors enabled
- [ ] Browser cache cleared

## Most Likely Issue

Based on your symptoms, the most likely cause is:

**Database not initialized or connection string issue**

Try this:
```powershell
# Check connection string
cat CustomFinancialPlanningAssistant/appsettings.json | Select-String "ConnectionStrings"

# Update database
dotnet ef database update --project CustomFinancialPlanningAssistant

# Restart application
dotnet run --project CustomFinancialPlanningAssistant
```

Then check the detailed error in the browser console.

## Need More Help?

1. Go to **/diagnostics** page to see service status
2. Check terminal output for detailed error messages
3. Look at browser console with DetailedErrors enabled
4. Share the specific error message for targeted help

---

**Your application is 99% complete - just need to fix this initialization issue!** ??
