using Xunit;
using Moq;
using CustomFinancialPlanningAssistant.Core.Enums;
using CustomFinancialPlanningAssistant.Services.AI;

namespace CustomFinancialPlanningAssistant.Tests.AI;

public class LlamaServiceTests
{
    [Fact]
    public void PerformIndustryBenchmarkingAsync_MethodExists()
    {
        // Verify that the Phase 12.2 method exists
        var method = typeof(LlamaService).GetMethod("PerformIndustryBenchmarkingAsync");
        Assert.NotNull(method);
        Assert.True(method.IsPublic);
        Assert.Equal(3, method.GetParameters().Length); // documentId, industry, cancellationToken
    }

    [Fact]
    public void GenerateInvestmentAdviceAsync_MethodExists()
    {
        // Verify that the investment advice method exists
        var method = typeof(LlamaService).GetMethod("GenerateInvestmentAdviceAsync");
        Assert.NotNull(method);
        Assert.True(method.IsPublic);
        Assert.Equal(3, method.GetParameters().Length); // documentId, riskTolerance, cancellationToken
    }

    [Fact]
    public void OptimizeCashFlowAsync_MethodExists()
    {
        // Verify that the cash flow optimization method exists
        var method = typeof(LlamaService).GetMethod("OptimizeCashFlowAsync");
        Assert.NotNull(method);
        Assert.True(method.IsPublic);
        Assert.Equal(2, method.GetParameters().Length); // documentId, cancellationToken
    }
}