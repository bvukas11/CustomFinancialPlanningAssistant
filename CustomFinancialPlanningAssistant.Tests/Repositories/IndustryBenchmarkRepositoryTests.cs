using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using CustomFinancialPlanningAssistant.Core.Entities;
using CustomFinancialPlanningAssistant.Core.Enums;
using CustomFinancialPlanningAssistant.Core.DTOs;
using CustomFinancialPlanningAssistant.Infrastructure.Data;
using CustomFinancialPlanningAssistant.Infrastructure.Repositories;

namespace CustomFinancialPlanningAssistant.Tests.Repositories;

public class IndustryBenchmarkRepositoryTests
{
    private readonly AppDbContext _context;
    private readonly IndustryBenchmarkRepository _repository;

    public IndustryBenchmarkRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestIndustryBenchmarkDb")
            .Options;

        _context = new AppDbContext(options);
        _repository = new IndustryBenchmarkRepository(_context, Mock.Of<ILogger<IndustryBenchmarkRepository>>());

        // Seed test data
        SeedTestData();
    }

    private void SeedTestData()
    {
        var benchmarks = new List<IndustryBenchmark>
        {
            new IndustryBenchmark
            {
                Industry = IndustryType.Technology,
                MetricName = "GrossMargin",
                AverageValue = 65.0m,
                MedianValue = 68.0m,
                Percentile25 = 55.0m,
                Percentile75 = 75.0m,
                LastUpdated = DateTime.UtcNow
            },
            new IndustryBenchmark
            {
                Industry = IndustryType.Technology,
                MetricName = "OperatingMargin",
                AverageValue = 15.0m,
                MedianValue = 18.0m,
                Percentile25 = 5.0m,
                Percentile75 = 25.0m,
                LastUpdated = DateTime.UtcNow
            },
            new IndustryBenchmark
            {
                Industry = IndustryType.Healthcare,
                MetricName = "GrossMargin",
                AverageValue = 58.0m,
                MedianValue = 60.0m,
                Percentile25 = 45.0m,
                Percentile75 = 70.0m,
                LastUpdated = DateTime.UtcNow
            }
        };

        _context.IndustryBenchmarks.AddRange(benchmarks);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetBenchmarksForIndustryAsync_ReturnsCorrectBenchmarks()
    {
        // Act
        var result = await _repository.GetBenchmarksForIndustryAsync(IndustryType.Technology);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Contains("GrossMargin", result.Keys);
        Assert.Contains("OperatingMargin", result.Keys);

        var grossMargin = result["GrossMargin"];
        Assert.Equal(65.0m, grossMargin.IndustryAverage);
        Assert.Equal(68.0m, grossMargin.IndustryMedian);
    }

    [Fact]
    public async Task GetAllBenchmarksAsync_ReturnsAllBenchmarks()
    {
        // Act
        var result = await _repository.GetAllBenchmarksAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count());
    }

    [Fact]
    public async Task HasBenchmarksForIndustryAsync_ReturnsTrueForExistingIndustry()
    {
        // Act
        var result = await _repository.HasBenchmarksForIndustryAsync(IndustryType.Technology);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task HasBenchmarksForIndustryAsync_ReturnsFalseForNonExistingIndustry()
    {
        // Act
        var result = await _repository.HasBenchmarksForIndustryAsync(IndustryType.Agriculture);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task GetBenchmarksForIndustriesAsync_ReturnsMultipleIndustries()
    {
        // Act
        var industries = new[] { IndustryType.Technology, IndustryType.Healthcare };
        var result = await _repository.GetBenchmarksForIndustriesAsync(industries);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Contains(IndustryType.Technology, result.Keys);
        Assert.Contains(IndustryType.Healthcare, result.Keys);
    }
}