# Force Stop and Clean Restart
# This ensures all cached files are cleared

Write-Host "?? Killing any running dotnet processes..." -ForegroundColor Yellow
Get-Process -Name "dotnet" -ErrorAction SilentlyContinue | Stop-Process -Force -ErrorAction SilentlyContinue
Start-Sleep -Seconds 2

Write-Host "`n?? Cleaning project..." -ForegroundColor Yellow
Set-Location "C:\source\CustomFinancialPlanningAssistant"
dotnet clean

Write-Host "`n??? Removing bin and obj folders..." -ForegroundColor Yellow
Remove-Item -Path ".\CustomFinancialPlanningAssistant\bin" -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item -Path ".\CustomFinancialPlanningAssistant\obj" -Recurse -Force -ErrorAction SilentlyContinue

Write-Host "`n?? Building project..." -ForegroundColor Yellow
dotnet build

if ($LASTEXITCODE -eq 0) {
    Write-Host "`n? Build successful!" -ForegroundColor Green
    Write-Host "`n??  IMPORTANT:" -ForegroundColor Red
    Write-Host "   1. Close ALL browser windows NOW" -ForegroundColor Yellow
    Write-Host "   2. Clear browser cache (Ctrl+Shift+Delete)" -ForegroundColor Yellow
    Write-Host "   3. Or use Incognito/Private mode" -ForegroundColor Yellow
    Write-Host "`nPress ENTER when ready..." -ForegroundColor Cyan
    Read-Host
    
    Write-Host "`n?? Starting application WITHOUT hot reload..." -ForegroundColor Cyan
    Write-Host "?? Navigate to: https://localhost:7265/reports" -ForegroundColor Cyan
    Write-Host "`nPress Ctrl+C to stop`n" -ForegroundColor Gray
    
    dotnet run --project CustomFinancialPlanningAssistant --no-hot-reload
} else {
    Write-Host "`n? Build failed!" -ForegroundColor Red
}
