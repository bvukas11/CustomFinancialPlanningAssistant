# AI Service Testing - Summary and Instructions

## ? Testing Components Created

### 1. **Web UI Test Page** - `/ai-test`
   - **Location:** `Components/Pages/AITest.razor`
   - **Features:**
     - Real-time service status monitoring
     - Available models display
     - Quick AI prompt testing
     - Financial analysis testing (5 types)
     - Sample data display
     - Error handling with troubleshooting tips

### 2. **PowerShell Test Script**
   - **Location:** `test-ai-service.ps1`
   - **Tests:**
     - Ollama installation check
     - Service availability check
     - Model availability verification
     - Direct API test
     - Application health endpoint test

### 3. **Testing Documentation**
   - **Location:** `Documentation/AI-Testing-Guide.md`
   - **Contents:**
     - Prerequisites checklist
     - 4 testing methods
     - Test scenarios
     - Troubleshooting guide
     - Performance expectations

---

## ?? Quick Start Testing

### Method 1: Run the Application (Recommended)

```powershell
# Navigate to project directory
cd C:\source\CustomFinancialPlanningAssistant\CustomFinancialPlanningAssistant

# Run the application
dotnet run

# Open browser to:
https://localhost:5001/ai-test
```

### Method 2: Run PowerShell Test

```powershell
# Run automated test script
cd C:\source\CustomFinancialPlanningAssistant\CustomFinancialPlanningAssistant
.\test-ai-service.ps1
```

### Method 3: Check Health Endpoint

```bash
# Test health endpoint (requires app running)
curl https://localhost:5001/health/ai -k

# Or in browser:
https://localhost:5001/health/ai
```

---

## ?? Prerequisites Checklist

Before testing, ensure:

- [ ] **Ollama installed**
  ```bash
  # Verify installation
  ollama --version
  ```

- [ ] **Ollama service running**
  ```bash
  # Test endpoint
  curl http://localhost:11434
  # Should return: "Ollama is running"
  ```

- [ ] **Required models downloaded**
  ```bash
  # Download models
  ollama pull llama3.2
  ollama pull llama3.2-vision
  ollama pull qwen2.5:8b
  
  # Verify
  ollama list
  ```

---

## ?? Test Scenarios

### Test 1: Service Status Check
**Purpose:** Verify Ollama connection

1. Navigate to `/ai-test`
2. Check "Service Status" card
3. Expected: Green "? Online" status
4. Expected: 3 models listed

**Result:** 
- ? PASS if service shows online and models are listed
- ? FAIL if offline or no models

---

### Test 2: Quick AI Response
**Purpose:** Test basic AI generation

1. Use default prompt or enter custom
2. Select model: `llama3.2`
3. Click "Test AI"
4. Wait 2-15 seconds
5. Verify response appears

**Expected Response Time:** 2-15 seconds (first time slower)

**Result:**
- ? PASS if coherent response received
- ? FAIL if timeout or error

---

### Test 3: Financial Summary Analysis
**Purpose:** Test domain-specific analysis

1. Select "Financial Summary" from dropdown
2. Click "Run Analysis"
3. Wait for analysis to complete
4. Verify structured analysis

**Expected Output:**
- Total revenue calculation
- Total expenses calculation
- Net income
- Key observations
- Recommendations

**Result:**
- ? PASS if structured analysis with specific numbers
- ? FAIL if generic or incomplete response

---

### Test 4: Different Models
**Purpose:** Compare model performance

1. Test with `llama3.2` (fastest)
2. Test with `qwen2.5:8b` (alternative)
3. Compare response quality and speed

**Expected:**
- llama3.2: Faster, good quality
- qwen2.5:8b: Slightly slower, potentially better reasoning

---

### Test 5: Error Handling
**Purpose:** Verify graceful failure

1. Stop Ollama service
2. Try to run analysis
3. Verify error message with troubleshooting

**Expected:**
- Clear error message
- Troubleshooting steps provided
- No application crash

---

## ?? Success Criteria

### ? All Tests Pass When:

1. **Service Status**
   - Shows "Online" (green)
   - Lists 3+ models
   - Last check timestamp updates

2. **AI Generation**
   - Response within 15 seconds
   - Coherent and relevant
   - No timeout errors

3. **Financial Analysis**
   - Completes successfully
   - Provides structured output
   - Includes specific numbers

4. **Error Handling**
   - Clear error messages
   - Helpful troubleshooting tips
   - No crashes or hangs

---

## ?? Troubleshooting Quick Reference

| Issue | Solution |
|-------|----------|
| "Offline" status | Start Ollama, check http://localhost:11434 |
| "No models" | Run: `ollama pull llama3.2` |
| Timeout | Increase TimeoutSeconds in appsettings.json |
| First request slow | Normal - model loading (wait 15-30s) |
| Out of memory | Close other apps, use smaller model |

---

## ?? Performance Benchmarks

### Expected Response Times

| Scenario | First Request | Subsequent | Target |
|----------|---------------|------------|--------|
| Simple prompt | 5-15s | 2-5s | <10s |
| Financial summary | 8-20s | 3-8s | <15s |
| Complex analysis | 10-30s | 5-10s | <20s |

### Model Comparison

| Model | Speed | Quality | Best For |
|-------|-------|---------|----------|
| llama3.2 | ??? | ??? | General use |
| qwen2.5:8b | ?? | ???? | Complex analysis |
| llama3.2-vision | ??? | ??? | Image processing |

---

## ? Final Checklist

Before proceeding to Phase 4, verify:

- [ ] PowerShell test script runs successfully
- [ ] All 5 tests pass
- [ ] Web UI loads without errors
- [ ] Service status shows online
- [ ] All 3 models available
- [ ] Quick test generates response
- [ ] Financial analysis works
- [ ] Health endpoint returns healthy
- [ ] Response times acceptable
- [ ] No errors in logs

---

## ?? Next Steps

Once all tests pass:

1. ? **Mark Phase 3 as Complete**
2. ? **Document any issues encountered**
3. ? **Note performance metrics**
4. ? **Proceed to Phase 4** - Document Processing

---

## ?? Support Resources

### Ollama Help
- Installation: https://ollama.ai
- Documentation: https://github.com/ollama/ollama/blob/main/docs/
- Discord: https://discord.gg/ollama

### Application Logs
```bash
# View logs while running
dotnet run --verbosity detailed

# Check specific AI logs
# Look for: CustomFinancialPlanningAssistant.Services.AI
```

### Common Commands
```bash
# Check Ollama status
curl http://localhost:11434

# List models
ollama list

# Pull model
ollama pull llama3.2

# Test generation
ollama run llama3.2 "test prompt"
```

---

**Created:** ${new Date().toISOString().split('T')[0]}  
**Test Duration:** 5-10 minutes  
**Status:** Ready for Testing  
**Next Phase:** Phase 4 - Document Processing
