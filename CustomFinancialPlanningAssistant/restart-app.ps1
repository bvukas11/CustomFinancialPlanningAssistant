# Quick Restart Script for Blazor App
# This script cleans, rebuilds, and restarts your application

Write-Host "?? Cleaning build artifacts..." -ForegroundColor Yellow
Set-Location "C:\source\CustomFinancialPlanningAssistant"
dotnet clean

Write-Host "`n?? Building application..." -ForegroundColor Yellow
dotnet build

if ($LASTEXITCODE -eq 0) {
    Write-Host "`n? Build successful!" -ForegroundColor Green
    Write-Host "`n?? Starting application..." -ForegroundColor Cyan
    Write-Host "?? Navigate to: https://localhost:7265" -ForegroundColor Cyan
    Write-Host "??  IMPORTANT: Close ALL browser windows first, then open a new one" -ForegroundColor Yellow
    Write-Host "??  If still not working, use Incognito/Private mode" -ForegroundColor Yellow
    Write-Host "`nPress Ctrl+C to stop the application`n" -ForegroundColor Gray
    
    dotnet run --project CustomFinancialPlanningAssistant
} else {
    Write-Host "`n? Build failed! Check errors above." -ForegroundColor Red
}
