# Nuclear Option - Complete Clean Restart
# Use this if normal restart doesn't work

Write-Host "?? DEEP CLEAN - This will remove all build artifacts" -ForegroundColor Yellow
Set-Location "C:\source\CustomFinancialPlanningAssistant"

# Stop any running processes (if applicable)
Write-Host "Stopping any running processes..." -ForegroundColor Gray

# Clean
Write-Host "`n?? Running dotnet clean..." -ForegroundColor Yellow
dotnet clean

# Delete bin and obj folders manually
Write-Host "???  Deleting bin and obj folders..." -ForegroundColor Yellow
Get-ChildItem -Path . -Include bin,obj -Recurse -Directory | Remove-Item -Recurse -Force -ErrorAction SilentlyContinue

# Restore packages
Write-Host "`n?? Restoring packages..." -ForegroundColor Yellow
dotnet restore

# Build
Write-Host "`n?? Building..." -ForegroundColor Yellow
dotnet build

if ($LASTEXITCODE -eq 0) {
    Write-Host "`n? Build successful!" -ForegroundColor Green
    Write-Host "`n??  BEFORE STARTING:" -ForegroundColor Red
    Write-Host "   1. Close ALL browser windows" -ForegroundColor Yellow
    Write-Host "   2. Clear browser cache (Ctrl+Shift+Delete)" -ForegroundColor Yellow
    Write-Host "   3. Select 'All time' and clear 'Cached images and files'" -ForegroundColor Yellow
    Write-Host "`nPress ENTER when ready to start the application..." -ForegroundColor Cyan
    Read-Host
    
    Write-Host "`n?? Starting application..." -ForegroundColor Cyan
    Write-Host "?? Navigate to: https://localhost:7265" -ForegroundColor Cyan
    Write-Host "?? Use Incognito/Private mode for guaranteed fresh start" -ForegroundColor Green
    Write-Host "`nPress Ctrl+C to stop`n" -ForegroundColor Gray
    
    dotnet run --project CustomFinancialPlanningAssistant
} else {
    Write-Host "`n? Build failed!" -ForegroundColor Red
    Write-Host "Check the errors above and fix them before restarting." -ForegroundColor Yellow
}
