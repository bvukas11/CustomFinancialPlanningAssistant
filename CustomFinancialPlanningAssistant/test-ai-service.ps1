# AI Service Test Script
# This script helps verify Ollama installation and AI service functionality

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  AI Service Test - Financial Assistant" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Test 1: Check if Ollama is installed
Write-Host "[Test 1] Checking Ollama Installation..." -ForegroundColor Yellow
try {
    $ollamaVersion = ollama --version 2>$null
    if ($LASTEXITCODE -eq 0) {
        Write-Host "? Ollama is installed: $ollamaVersion" -ForegroundColor Green
    } else {
        Write-Host "? Ollama is not installed or not in PATH" -ForegroundColor Red
        Write-Host "  Download from: https://ollama.ai" -ForegroundColor Yellow
        exit 1
    }
} catch {
    Write-Host "? Error checking Ollama: $_" -ForegroundColor Red
    exit 1
}

Write-Host ""

# Test 2: Check if Ollama service is running
Write-Host "[Test 2] Checking Ollama Service..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "http://localhost:11434" -TimeoutSec 5 -UseBasicParsing
    if ($response.StatusCode -eq 200) {
        Write-Host "? Ollama service is running at http://localhost:11434" -ForegroundColor Green
    }
} catch {
    Write-Host "? Ollama service is not running" -ForegroundColor Red
    Write-Host "  Start Ollama service and try again" -ForegroundColor Yellow
    Write-Host "  Windows: Ollama should auto-start" -ForegroundColor Gray
    Write-Host "  Mac/Linux: Run 'ollama serve'" -ForegroundColor Gray
    exit 1
}

Write-Host ""

# Test 3: List available models
Write-Host "[Test 3] Checking Available Models..." -ForegroundColor Yellow
$models = ollama list 2>$null
if ($LASTEXITCODE -eq 0) {
    Write-Host "? Available models:" -ForegroundColor Green
    Write-Host $models -ForegroundColor Gray
    
    # Check for required models
    $requiredModels = @("llama3.2", "qwen-offline", "llama3.2-vision")
    $missingModels = @()
    
    foreach ($model in $requiredModels) {
        if ($models -notmatch $model) {
            $missingModels += $model
        }
    }
    
    if ($missingModels.Count -gt 0) {
        Write-Host ""
        Write-Host "? Missing required models:" -ForegroundColor Yellow
        foreach ($model in $missingModels) {
            Write-Host "  - $model" -ForegroundColor Yellow
        }
        Write-Host ""
        Write-Host "Note: Your configuration expects these models:" -ForegroundColor Cyan
        foreach ($model in $requiredModels) {
            Write-Host "  - $model" -ForegroundColor White
        }
    } else {
        Write-Host "? All required models are available!" -ForegroundColor Green
    }
} else {
    Write-Host "? Error listing models" -ForegroundColor Red
}

Write-Host ""

# Test 4: Test AI generation
Write-Host "[Test 4] Testing AI Generation..." -ForegroundColor Yellow
Write-Host "Sending test prompt to llama3.2..." -ForegroundColor Gray

$testPrompt = "Explain financial analysis in one sentence."
$requestBody = @{
    model = "llama3.2"
    prompt = $testPrompt
    stream = $false
} | ConvertTo-Json

try {
    $startTime = Get-Date
    $response = Invoke-RestMethod -Uri "http://localhost:11434/api/generate" `
                                   -Method Post `
                                   -Body $requestBody `
                                   -ContentType "application/json" `
                                   -TimeoutSec 60
    $endTime = Get-Date
    $duration = ($endTime - $startTime).TotalSeconds
    
    Write-Host "? AI generation successful!" -ForegroundColor Green
    Write-Host "  Response time: $([math]::Round($duration, 2)) seconds" -ForegroundColor Gray
    Write-Host "  Response: $($response.response)" -ForegroundColor Cyan
} catch {
    Write-Host "? Error testing AI generation: $_" -ForegroundColor Red
}

Write-Host ""

# Test 5: Check Application Health Endpoint
Write-Host "[Test 5] Checking Application Health Endpoint..." -ForegroundColor Yellow
Write-Host "Note: This requires the application to be running" -ForegroundColor Gray

try {
    $healthResponse = Invoke-WebRequest -Uri "https://localhost:5001/health/ai" `
                                        -SkipCertificateCheck `
                                        -TimeoutSec 5 `
                                        -UseBasicParsing
    
    if ($healthResponse.StatusCode -eq 200) {
        Write-Host "? Application health endpoint is responding" -ForegroundColor Green
        $healthData = $healthResponse.Content | ConvertFrom-Json
        Write-Host "  Status: $($healthData.status)" -ForegroundColor Cyan
    }
} catch {
    if ($_.Exception.Message -like "*Unable to connect*") {
        Write-Host "? Application is not running" -ForegroundColor Yellow
        Write-Host "  Start the application to test this endpoint" -ForegroundColor Gray
    } else {
        Write-Host "? Could not reach health endpoint: $_" -ForegroundColor Yellow
    }
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Test Summary" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Next Steps:" -ForegroundColor Yellow
Write-Host "1. If Ollama is not installed, download from https://ollama.ai" -ForegroundColor White
Write-Host "2. Verify your models match the configuration:" -ForegroundColor White
Write-Host "   - llama3.2" -ForegroundColor Gray
Write-Host "   - qwen-offline" -ForegroundColor Gray
Write-Host "   - llama3.2-vision" -ForegroundColor Gray
Write-Host "3. Run the application: dotnet run" -ForegroundColor White
Write-Host "4. Navigate to: https://localhost:5001/ai-test" -ForegroundColor White
Write-Host "5. Or check health: https://localhost:5001/health/ai" -ForegroundColor White
Write-Host ""
