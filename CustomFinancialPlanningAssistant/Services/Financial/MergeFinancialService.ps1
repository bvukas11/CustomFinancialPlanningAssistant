# Script to merge FinancialService parts
# Run this from PowerShell in the Services\Financial directory

# Read Part 2
$part2 = Get-Content "FinancialService_Part2.txt" -Raw

# Read Part 3  
$part3 = Get-Content "FinancialService_Part3.txt" -Raw

# Create the complete file content
$completeContent = @"
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using CustomFinancialPlanningAssistant.Core.DTOs;
using CustomFinancialPlanningAssistant.Core.Entities;
using CustomFinancialPlanningAssistant.Core.Enums;
using CustomFinancialPlanningAssistant.Infrastructure.Repositories;
using CustomFinancialPlanningAssistant.Services.AI;

namespace CustomFinancialPlanningAssistant.Services.Financial;

public class FinancialService : IFinancialService
{
    private readonly IFinancialDocumentRepository _documentRepo;
    private readonly IFinancialDataRepository _dataRepo;
    private readonly IAIAnalysisRepository _analysisRepo;
    private readonly ILlamaService _aiService;
    private readonly ILogger<FinancialService> _logger;

    public FinancialService(
        IFinancialDocumentRepository documentRepo,
        IFinancialDataRepository dataRepo,
        IAIAnalysisRepository analysisRepo,
        ILlamaService aiService,
        ILogger<FinancialService> logger)
    {
        _documentRepo = documentRepo ?? throw new ArgumentNullException(nameof(documentRepo));
        _dataRepo = dataRepo ?? throw new ArgumentNullException(nameof(dataRepo));
        _analysisRepo = analysisRepo ?? throw new ArgumentNullException(nameof(analysisRepo));
        _aiService = aiService ?? throw new ArgumentNullException(nameof(aiService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

$part2

$part3
}
"@

# Write the complete file
$completeContent | Out-File -FilePath "FinancialService.cs" -Encoding UTF8

Write-Host "? FinancialService.cs created successfully!" -ForegroundColor Green
Write-Host "File location: Services\Financial\FinancialService.cs" -ForegroundColor Cyan
