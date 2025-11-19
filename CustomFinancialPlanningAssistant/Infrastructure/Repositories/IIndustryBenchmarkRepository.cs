using CustomFinancialPlanningAssistant.Core.Entities;
using CustomFinancialPlanningAssistant.Core.Enums;
using CustomFinancialPlanningAssistant.Core.DTOs;

namespace CustomFinancialPlanningAssistant.Infrastructure.Repositories;

/// <summary>
/// Repository interface for industry benchmark data
/// </summary>
public interface IIndustryBenchmarkRepository
{
    /// <summary>
    /// Gets all benchmark data for a specific industry
    /// </summary>
    Task<Dictionary<string, IndustryBenchmarkDto>> GetBenchmarksForIndustryAsync(IndustryType industry);

    /// <summary>
    /// Gets all available benchmarks
    /// </summary>
    Task<IEnumerable<IndustryBenchmark>> GetAllBenchmarksAsync();

    /// <summary>
    /// Gets benchmarks for a specific metric across all industries
    /// </summary>
    Task<IEnumerable<IndustryBenchmark>> GetBenchmarksByMetricAsync(string metricName);

    /// <summary>
    /// Updates an existing benchmark record
    /// </summary>
    Task UpdateBenchmarkAsync(IndustryBenchmark benchmark);

    /// <summary>
    /// Adds a new benchmark record
    /// </summary>
    Task AddBenchmarkAsync(IndustryBenchmark benchmark);

    /// <summary>
    /// Deletes a benchmark record
    /// </summary>
    Task DeleteBenchmarkAsync(int id);

    /// <summary>
    /// Gets benchmark data for multiple industries
    /// </summary>
    Task<Dictionary<IndustryType, Dictionary<string, IndustryBenchmarkDto>>> GetBenchmarksForIndustriesAsync(IEnumerable<IndustryType> industries);

    /// <summary>
    /// Checks if benchmark data exists for a specific industry
    /// </summary>
    Task<bool> HasBenchmarksForIndustryAsync(IndustryType industry);

    /// <summary>
    /// Gets the last update date for industry benchmarks
    /// </summary>
    Task<DateTime?> GetLastUpdateDateAsync(IndustryType industry);
}