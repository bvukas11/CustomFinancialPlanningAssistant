using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CustomFinancialPlanningAssistant.Core.Entities;
using CustomFinancialPlanningAssistant.Core.Enums;
using CustomFinancialPlanningAssistant.Core.DTOs;
using CustomFinancialPlanningAssistant.Infrastructure.Data;

namespace CustomFinancialPlanningAssistant.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for industry benchmark data
/// </summary>
public class IndustryBenchmarkRepository : IIndustryBenchmarkRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<IndustryBenchmarkRepository> _logger;

    /// <summary>
    /// Initializes a new instance of the IndustryBenchmarkRepository class
    /// </summary>
    public IndustryBenchmarkRepository(
        AppDbContext context,
        ILogger<IndustryBenchmarkRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Dictionary<string, IndustryBenchmarkDto>> GetBenchmarksForIndustryAsync(IndustryType industry)
    {
        _logger.LogInformation("Getting benchmark data for industry: {Industry}", industry);

        var benchmarks = await _context.IndustryBenchmarks
            .Where(b => b.Industry == industry)
            .ToListAsync();

        var result = new Dictionary<string, IndustryBenchmarkDto>();

        foreach (var benchmark in benchmarks)
        {
            result[benchmark.MetricName] = new IndustryBenchmarkDto
            {
                MetricName = benchmark.MetricName,
                IndustryAverage = benchmark.AverageValue,
                IndustryMedian = benchmark.MedianValue,
                MetricDescription = GetMetricDescription(benchmark.MetricName)
            };
        }

        _logger.LogInformation("Retrieved {Count} benchmark metrics for {Industry}", result.Count, industry);
        return result;
    }

    public async Task<IEnumerable<IndustryBenchmark>> GetAllBenchmarksAsync()
    {
        _logger.LogInformation("Getting all industry benchmarks");
        return await _context.IndustryBenchmarks
            .OrderBy(b => b.Industry)
            .ThenBy(b => b.MetricName)
            .ToListAsync();
    }

    public async Task<IEnumerable<IndustryBenchmark>> GetBenchmarksByMetricAsync(string metricName)
    {
        _logger.LogInformation("Getting benchmarks for metric: {MetricName}", metricName);
        return await _context.IndustryBenchmarks
            .Where(b => b.MetricName == metricName)
            .OrderBy(b => b.Industry)
            .ToListAsync();
    }

    public async Task UpdateBenchmarkAsync(IndustryBenchmark benchmark)
    {
        _logger.LogInformation("Updating benchmark for {Industry} - {MetricName}",
            benchmark.Industry, benchmark.MetricName);

        benchmark.LastUpdated = DateTime.UtcNow;
        _context.Entry(benchmark).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task AddBenchmarkAsync(IndustryBenchmark benchmark)
    {
        _logger.LogInformation("Adding new benchmark for {Industry} - {MetricName}",
            benchmark.Industry, benchmark.MetricName);

        benchmark.LastUpdated = DateTime.UtcNow;
        await _context.IndustryBenchmarks.AddAsync(benchmark);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteBenchmarkAsync(int id)
    {
        _logger.LogInformation("Deleting benchmark with ID: {Id}", id);

        var benchmark = await _context.IndustryBenchmarks.FindAsync(id);
        if (benchmark != null)
        {
            _context.IndustryBenchmarks.Remove(benchmark);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<Dictionary<IndustryType, Dictionary<string, IndustryBenchmarkDto>>> GetBenchmarksForIndustriesAsync(IEnumerable<IndustryType> industries)
    {
        _logger.LogInformation("Getting benchmarks for {Count} industries", industries.Count());

        var industryList = industries.ToList();
        var benchmarks = await _context.IndustryBenchmarks
            .Where(b => industryList.Contains(b.Industry))
            .ToListAsync();

        var result = new Dictionary<IndustryType, Dictionary<string, IndustryBenchmarkDto>>();

        foreach (var industry in industryList)
        {
            var industryBenchmarks = benchmarks.Where(b => b.Industry == industry);
            var benchmarkDict = new Dictionary<string, IndustryBenchmarkDto>();

            foreach (var benchmark in industryBenchmarks)
            {
                benchmarkDict[benchmark.MetricName] = new IndustryBenchmarkDto
                {
                    MetricName = benchmark.MetricName,
                    IndustryAverage = benchmark.AverageValue,
                    IndustryMedian = benchmark.MedianValue,
                    MetricDescription = GetMetricDescription(benchmark.MetricName)
                };
            }

            result[industry] = benchmarkDict;
        }

        return result;
    }

    public async Task<bool> HasBenchmarksForIndustryAsync(IndustryType industry)
    {
        return await _context.IndustryBenchmarks
            .AnyAsync(b => b.Industry == industry);
    }

    public async Task<DateTime?> GetLastUpdateDateAsync(IndustryType industry)
    {
        return await _context.IndustryBenchmarks
            .Where(b => b.Industry == industry)
            .MaxAsync(b => (DateTime?)b.LastUpdated);
    }

    /// <summary>
    /// Provides descriptions for common financial metrics
    /// </summary>
    private string GetMetricDescription(string metricName)
    {
        return metricName switch
        {
            "GrossMargin" => "Percentage of revenue remaining after cost of goods sold",
            "OperatingMargin" => "Percentage of revenue remaining after operating expenses",
            "NetMargin" => "Percentage of revenue remaining as net profit",
            "CurrentRatio" => "Ability to pay short-term obligations with current assets",
            "QuickRatio" => "Ability to pay short-term obligations with liquid assets",
            "DebtToEquity" => "Proportion of debt relative to shareholder equity",
            "ReturnOnAssets" => "Efficiency of using assets to generate profit",
            "ReturnOnEquity" => "Efficiency of using equity to generate profit",
            "AssetTurnover" => "Efficiency of using assets to generate revenue",
            "InventoryTurnover" => "Speed of selling and replacing inventory",
            "AccountsReceivableTurnover" => "Speed of collecting receivables",
            _ => $"{metricName} - Financial performance metric"
        };
    }
}