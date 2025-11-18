using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CustomFinancialPlanningAssistant.Core.Entities;
using CustomFinancialPlanningAssistant.Infrastructure.Data;

namespace CustomFinancialPlanningAssistant.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for FinancialData entity
/// </summary>
public class FinancialDataRepository : IFinancialDataRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<FinancialDataRepository> _logger;

    /// <summary>
    /// Initializes a new instance of the FinancialDataRepository class
    /// </summary>
    public FinancialDataRepository(
        AppDbContext context,
        ILogger<FinancialDataRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<FinancialData?> GetByIdAsync(int id)
    {
        _logger.LogInformation("Getting financial data by ID: {Id}", id);
        return await _context.FinancialDataRecords.FindAsync(id);
    }

    public async Task<IEnumerable<FinancialData>> GetAllAsync()
    {
        _logger.LogInformation("Getting all financial data");
        return await _context.FinancialDataRecords
            .OrderBy(d => d.DateRecorded)
            .ToListAsync();
    }

    public async Task<IEnumerable<FinancialData>> GetByDocumentIdAsync(int documentId)
    {
        _logger.LogInformation("Getting financial data for document: {DocumentId}", documentId);
        return await _context.FinancialDataRecords
            .Include(d => d.Document)
            .Where(d => d.DocumentId == documentId)
            .OrderBy(d => d.DateRecorded)
            .ToListAsync();
    }

    public async Task<IEnumerable<FinancialData>> GetByPeriodAsync(string period)
    {
        _logger.LogInformation("Getting financial data for period: {Period}", period);
        return await _context.FinancialDataRecords
            .Where(d => d.Period == period)
            .OrderBy(d => d.AccountName)
            .ToListAsync();
    }

    public async Task<IEnumerable<FinancialData>> GetByCategoryAsync(string category)
    {
        _logger.LogInformation("Getting financial data for category: {Category}", category);
        return await _context.FinancialDataRecords
            .Where(d => d.Category == category)
            .OrderByDescending(d => d.Amount)
            .ToListAsync();
    }

    public async Task<IEnumerable<FinancialData>> GetByDateRangeAsync(
        DateTime startDate,
        DateTime endDate)
    {
        _logger.LogInformation(
            "Getting financial data between {StartDate} and {EndDate}",
            startDate,
            endDate);

        return await _context.FinancialDataRecords
            .Where(d => d.DateRecorded >= startDate && d.DateRecorded <= endDate)
            .OrderBy(d => d.DateRecorded)
            .ToListAsync();
    }

    public async Task<decimal> GetTotalByCategoryAsync(string category, string period)
    {
        _logger.LogInformation(
            "Calculating total for category {Category} in period {Period}",
            category,
            period);

        var total = await _context.FinancialDataRecords
            .Where(d => d.Category == category && d.Period == period)
            .SumAsync(d => d.Amount);

        return total;
    }

    public async Task<decimal> GetTotalByPeriodAsync(string period)
    {
        _logger.LogInformation("Calculating total for period: {Period}", period);
        return await _context.FinancialDataRecords
            .Where(d => d.Period == period)
            .SumAsync(d => d.Amount);
    }

    public async Task<Dictionary<string, decimal>> GetCategorySummaryAsync(string period)
    {
        _logger.LogInformation("Getting category summary for period: {Period}", period);

        var summary = await _context.FinancialDataRecords
            .Where(d => d.Period == period)
            .GroupBy(d => d.Category)
            .Select(g => new { Category = g.Key, Total = g.Sum(d => d.Amount) })
            .ToDictionaryAsync(x => x.Category, x => x.Total);

        return summary;
    }

    public async Task<IEnumerable<FinancialData>> GetTopExpensesAsync(int count, string period)
    {
        _logger.LogInformation(
            "Getting top {Count} expenses for period: {Period}",
            count,
            period);

        return await _context.FinancialDataRecords
            .Where(d => d.Period == period && d.Category == "Expense")
            .OrderByDescending(d => d.Amount)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<FinancialData>> AddRangeAsync(
        IEnumerable<FinancialData> dataRecords)
    {
        var recordsList = dataRecords.ToList();
        _logger.LogInformation("Adding {Count} financial data records", recordsList.Count);
        await _context.FinancialDataRecords.AddRangeAsync(recordsList);
        return recordsList;
    }

    public Task UpdateAsync(FinancialData data)
    {
        _logger.LogInformation("Updating financial data: {Id}", data.Id);
        _context.Entry(data).State = EntityState.Modified;
        return Task.CompletedTask;
    }

    public async Task DeleteByDocumentIdAsync(int documentId)
    {
        _logger.LogInformation(
            "Deleting all financial data for document: {DocumentId}",
            documentId);

        var records = await _context.FinancialDataRecords
            .Where(d => d.DocumentId == documentId)
            .ToListAsync();

        _context.FinancialDataRecords.RemoveRange(records);
    }

    public async Task<int> SaveChangesAsync()
    {
        try
        {
            var changes = await _context.SaveChangesAsync();
            _logger.LogInformation("Saved {Changes} changes to database", changes);
            return changes;
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Error saving changes to database");
            throw;
        }
    }
}
