using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CustomFinancialPlanningAssistant.Core.Entities;
using CustomFinancialPlanningAssistant.Infrastructure.Data;

namespace CustomFinancialPlanningAssistant.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for AIAnalysis entity
/// </summary>
public class AIAnalysisRepository : IAIAnalysisRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<AIAnalysisRepository> _logger;

    /// <summary>
    /// Initializes a new instance of the AIAnalysisRepository class
    /// </summary>
    public AIAnalysisRepository(
        AppDbContext context,
        ILogger<AIAnalysisRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<AIAnalysis?> GetByIdAsync(int id)
    {
        _logger.LogInformation("Getting AI analysis by ID: {Id}", id);
        return await _context.AIAnalyses.FindAsync(id);
    }

    public async Task<IEnumerable<AIAnalysis>> GetAllAsync()
    {
        _logger.LogInformation("Getting all AI analyses");
        return await _context.AIAnalyses
            .OrderByDescending(a => a.CreatedDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<AIAnalysis>> GetByDocumentIdAsync(int documentId)
    {
        _logger.LogInformation("Getting AI analyses for document: {DocumentId}", documentId);
        return await _context.AIAnalyses
            .Include(a => a.Document)
            .Where(a => a.DocumentId == documentId)
            .OrderByDescending(a => a.CreatedDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<AIAnalysis>> GetByAnalysisTypeAsync(string analysisType)
    {
        _logger.LogInformation("Getting AI analyses by type: {AnalysisType}", analysisType);
        return await _context.AIAnalyses
            .Where(a => a.AnalysisType == analysisType)
            .OrderByDescending(a => a.CreatedDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<AIAnalysis>> GetRecentAnalysesAsync(int count)
    {
        _logger.LogInformation("Getting {Count} recent AI analyses", count);
        return await _context.AIAnalyses
            .Include(a => a.Document)
            .OrderByDescending(a => a.CreatedDate)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<AIAnalysis>> GetByDateRangeAsync(
        DateTime startDate,
        DateTime endDate)
    {
        _logger.LogInformation(
            "Getting AI analyses between {StartDate} and {EndDate}",
            startDate,
            endDate);

        return await _context.AIAnalyses
            .Where(a => a.CreatedDate >= startDate && a.CreatedDate <= endDate)
            .OrderByDescending(a => a.CreatedDate)
            .ToListAsync();
    }

    public async Task<double> GetAverageExecutionTimeAsync(string analysisType)
    {
        _logger.LogInformation(
            "Calculating average execution time for type: {AnalysisType}",
            analysisType);

        var analyses = await _context.AIAnalyses
            .Where(a => a.AnalysisType == analysisType)
            .ToListAsync();

        if (!analyses.Any())
        {
            return 0;
        }

        return analyses.Average(a => a.ExecutionTime);
    }

    public async Task<AIAnalysis> AddAsync(AIAnalysis analysis)
    {
        _logger.LogInformation(
            "Adding new AI analysis for document: {DocumentId}",
            analysis.DocumentId);
        await _context.AIAnalyses.AddAsync(analysis);
        return analysis;
    }

    public async Task UpdateRatingAsync(int id, int rating)
    {
        _logger.LogInformation("Updating rating for analysis {Id} to {Rating}", id, rating);

        var analysis = await _context.AIAnalyses.FindAsync(id);
        if (analysis == null)
        {
            throw new InvalidOperationException($"Analysis with ID {id} not found");
        }

        analysis.Rating = rating;
        _context.Entry(analysis).State = EntityState.Modified;
    }

    public async Task DeleteAsync(int id)
    {
        _logger.LogInformation("Deleting AI analysis: {Id}", id);

        var analysis = await _context.AIAnalyses.FindAsync(id);
        if (analysis != null)
        {
            _context.AIAnalyses.Remove(analysis);
        }
        else
        {
            throw new InvalidOperationException($"Analysis with ID {id} not found");
        }
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
