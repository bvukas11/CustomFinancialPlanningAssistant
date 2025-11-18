# AI Service Testing Guide

## Quick Start - Testing AI Integration

This guide will help you verify that the AI service is properly configured and working.

---

## Prerequisites

Before testing, ensure you have:

1. ? **Ollama Installed**
   - Download from: https://ollama.ai
   - Verify: `ollama --version`

2. ? **Required Models Downloaded**
   ```bash
   ollama pull llama3.2
   ollama pull llama3.2-vision
   ollama pull qwen2.5:8b
   ```

3. ? **Ollama Service Running**
   - Test: Open browser to http://localhost:11434
   - Should show: "Ollama is running"

---

## Testing Methods

### Method 1: Automated PowerShell Test (Recommended)

Run the test script to verify all components:

```powershell
cd C:\source\CustomFinancialPlanningAssistant\CustomFinancialPlanningAssistant
.\test-ai-service.ps1
```

**What it checks:**
- ? Ollama installation
- ? Service availability
- ? Available models
- ? AI generation test
- ? Application health endpoint

---

### Method 2: Web UI Test Page

1. **Start the application:**
   ```bash
   cd C:\source\CustomFinancialPlanningAssistant\CustomFinancialPlanningAssistant
   dotnet run
   ```

2. **Open browser to:**
   ```
   https://localhost:5001/ai-test
   ```

3. **Test features:**
   - Check service status (green = online)
   - View available models
   - Run quick AI test with custom prompts
   - Test financial analysis functions

---

### Method 3: Health Check Endpoint

**Check overall application health:**
```bash
curl https://localhost:5001/health -k
```

**Check AI service specifically:**
```bash
curl https://localhost:5001/health/ai -k
```

**Expected Response (Healthy):**
```json
{
  "status": "Healthy",
  "results": {
    "ai_service": {
      "status": "Healthy",
      "description": "AI service is healthy with 3 model(s) available",
      "data": {
        "OllamaStatus": "Running",
        "AvailableModels": 3,
        "Models": "llama3.2, llama3.2-vision, qwen2.5:8b",
        "CheckedAt": "2024-01-15T10:30:00.000Z"
      }
    }
  }
}
```

---

### Method 4: Direct Ollama API Test

Test Ollama directly without the application:

```bash
curl http://localhost:11434/api/generate -d '{
  "model": "llama3.2",
  "prompt": "Explain cash flow in one sentence",
  "stream": false
}'
```

---

## Test Scenarios

### Scenario 1: Basic AI Response
**Purpose:** Verify AI can generate text responses

1. Navigate to AI Test page
2. Enter prompt: "What is financial analysis?"
3. Click "Test AI"
4. Verify response appears (2-10 seconds)

### Scenario 2: Financial Summary
**Purpose:** Test domain-specific analysis

1. Select "Financial Summary" from analysis type
2. Click "Run Analysis"
3. Verify structured financial analysis appears

### Scenario 3: Different Models
**Purpose:** Test multiple AI models

1. Try with llama3.2 (fastest)
2. Try with qwen2.5:8b (alternative)
3. Compare response quality and speed

### Scenario 4: Error Handling
**Purpose:** Verify graceful error handling

1. Stop Ollama service
2. Try to run analysis
3. Verify error message appears with troubleshooting tips

---

## Troubleshooting

### Issue: "Ollama is not running"

**Solutions:**
```bash
# Check if Ollama process is running
# Windows: Check Task Manager for "ollama"
# Mac/Linux: ps aux | grep ollama

# Restart Ollama (Windows)
# It should auto-start, check system tray

# Start Ollama (Mac/Linux)
ollama serve
```

### Issue: "Model not found"

**Solutions:**
```bash
# List currently installed models
ollama list

# Pull missing model
ollama pull llama3.2

# Verify model downloaded
ollama list | grep llama3.2
```

### Issue: "Request timeout"

**Possible Causes:**
- First request to a model (loading time)
- System resources (CPU/RAM)
- Model size too large

**Solutions:**
1. Wait for first request to complete (can take 15-30 seconds)
2. Subsequent requests will be faster
3. Increase timeout in appsettings.json:
   ```json
   "AISettings": {
     "TimeoutSeconds": 180
   }
   ```

### Issue: "Cannot connect to localhost:11434"

**Solutions:**
1. Check firewall settings
2. Verify Ollama is bound to localhost
3. Check port 11434 is not in use:
   ```bash
   # Windows
   netstat -ano | findstr 11434
   
   # Mac/Linux
   lsof -i :11434
   ```

### Issue: "Out of memory"

**Solutions:**
1. Close other applications
2. Use smaller model:
   - llama3.2 (~4.7GB)
   - qwen2.5:8b (~4.7GB)
3. Consider using quantized versions
4. Increase system RAM if possible

---

## Performance Expectations

### Response Times

| Scenario | First Request | Subsequent Requests |
|----------|---------------|---------------------|
| Simple prompt | 5-15 seconds | 2-5 seconds |
| Financial analysis | 8-20 seconds | 3-8 seconds |
| With GPU | 1-5 seconds | 1-3 seconds |

### Model Comparison

| Model | Size | Speed | Quality | Use Case |
|-------|------|-------|---------|----------|
| llama3.2 | ~4.7GB | Fast | Good | General analysis |
| qwen2.5:8b | ~4.7GB | Medium | Better | Complex reasoning |
| llama3.2-vision | ~4.7GB | Fast | Good | Image analysis |

---

## Success Indicators

### ? Everything is Working When:

1. **Service Status Card shows:**
   - Ollama Service: Online (green)
   - Available Models: 3 (or more)
   - Models list includes: llama3.2, qwen2.5:8b, llama3.2-vision

2. **Quick Test:**
   - Response appears within 5-15 seconds
   - No error messages
   - Response is coherent and relevant

3. **Financial Analysis:**
   - Analysis completes successfully
   - Results are structured and detailed
   - Contains specific financial insights

4. **Health Endpoint:**
   - Returns status: "Healthy"
   - Shows available models
   - Response time < 1 second

---

## Next Steps After Testing

Once AI service is confirmed working:

1. ? **Proceed to Phase 4** - Document Processing
2. ? **Test with real financial data** - Upload actual files
3. ? **Optimize prompts** - Refine for better results
4. ? **Monitor performance** - Track response times
5. ? **Explore features** - Try different analysis types

---

## Sample Test Prompts

### Quick Tests:
- "What is financial analysis?"
- "Explain cash flow in 2 sentences"
- "Define EBITDA"

### Financial Scenarios:
- "Analyze this company's revenue growth"
- "What are red flags in financial statements?"
- "How do you calculate working capital?"

### Domain-Specific:
- "Compare revenue and expenses"
- "Identify anomalies in spending"
- "Forecast next quarter revenue"

---

## Monitoring and Logs

### View Application Logs:
```bash
# Run application with verbose logging
dotnet run --verbosity detailed
```

### Check Ollama Logs:
```bash
# Mac/Linux
tail -f ~/.ollama/logs/server.log

# Windows
# Check: %LOCALAPPDATA%\Ollama\logs\
```

### Enable Detailed AI Logging:
In `appsettings.json`:
```json
{
  "Features": {
    "LogAIRequests": true
  },
  "Logging": {
    "LogLevel": {
      "CustomFinancialPlanningAssistant.Services.AI": "Debug"
    }
  }
}
```

---

## Support and Resources

### Documentation:
- Ollama Docs: https://github.com/ollama/ollama/blob/main/docs/
- LLaMA Models: https://ai.meta.com/llama/
- Project Wiki: [Add your wiki link]

### Community:
- Ollama Discord: https://discord.gg/ollama
- GitHub Issues: [Your repo issues]

---

## Test Checklist

Before moving to next phase, verify:

- [ ] PowerShell test script runs successfully
- [ ] All 5 tests in script pass
- [ ] AI Test page loads without errors
- [ ] Service status shows "Online"
- [ ] All 3 models are available
- [ ] Quick test generates response
- [ ] Financial analysis works
- [ ] Health endpoint returns healthy status
- [ ] No errors in application logs
- [ ] Response times are acceptable

---

**Last Updated:** ${new Date().toISOString().split('T')[0]}  
**Test Duration:** ~5-10 minutes  
**Difficulty:** Beginner-Friendly  
**Status:** Ready for Production Testing
