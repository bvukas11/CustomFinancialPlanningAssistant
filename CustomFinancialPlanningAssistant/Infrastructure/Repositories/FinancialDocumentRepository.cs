using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CustomFinancialPlanningAssistant.Core.Entities;
using CustomFinancialPlanningAssistant.Infrastructure.Data;

namespace CustomFinancialPlanningAssistant.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for FinancialDocument entity
/// </summary>
public class FinancialDocumentRepository : IFinancialDocumentRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<FinancialDocumentRepository> _logger;

    /// <summary>
    /// Initializes a new instance of the FinancialDocumentRepository class
    /// </summary>
    public FinancialDocumentRepository(
        AppDbContext context,
        ILogger<FinancialDocumentRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<FinancialDocument?> GetByIdAsync(int id)
    {
        _logger.LogInformation("Getting document by ID: {DocumentId}", id);
        return await _context.FinancialDocuments.FindAsync(id);
    }

    public async Task<FinancialDocument?> GetWithDataAsync(int id)
    {
        _logger.LogInformation("Getting document with data: {DocumentId}", id);
        return await _context.FinancialDocuments
            .Include(d => d.FinancialDataRecords)
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<FinancialDocument?> GetWithAnalysesAsync(int id)
    {
        _logger.LogInformation("Getting document with analyses: {DocumentId}", id);
        return await _context.FinancialDocuments
            .Include(d => d.AIAnalyses)
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<FinancialDocument?> GetCompleteAsync(int id)
    {
        _logger.LogInformation("Getting complete document: {DocumentId}", id);
        return await _context.FinancialDocuments
            .Include(d => d.FinancialDataRecords)
            .Include(d => d.AIAnalyses)
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<IEnumerable<FinancialDocument>> GetAllAsync()
    {
        _logger.LogInformation("Getting all documents");
        return await _context.FinancialDocuments
            .OrderByDescending(d => d.UploadDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<FinancialDocument>> GetByStatusAsync(string status)
    {
        _logger.LogInformation("Getting documents by status: {Status}", status);
        return await _context.FinancialDocuments
            .Where(d => d.Status == status)
            .OrderByDescending(d => d.UploadDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<FinancialDocument>> GetRecentDocumentsAsync(int count)
    {
        _logger.LogInformation("Getting {Count} recent documents", count);
        return await _context.FinancialDocuments
            .OrderByDescending(d => d.UploadDate)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<FinancialDocument>> GetDocumentsByDateRangeAsync(
        DateTime startDate,
        DateTime endDate)
    {
        _logger.LogInformation(
            "Getting documents between {StartDate} and {EndDate}",
            startDate,
            endDate);

        return await _context.FinancialDocuments
            .Where(d => d.UploadDate >= startDate && d.UploadDate <= endDate)
            .OrderByDescending(d => d.UploadDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<FinancialDocument>> GetDocumentsByTypeAsync(string fileType)
    {
        _logger.LogInformation("Getting documents by file type: {FileType}", fileType);
        return await _context.FinancialDocuments
            .Where(d => d.FileType == fileType)
            .OrderByDescending(d => d.UploadDate)
            .ToListAsync();
    }

    public async Task<FinancialDocument> AddAsync(FinancialDocument document)
    {
        _logger.LogInformation("Adding new document: {FileName}", document.FileName);
        await _context.FinancialDocuments.AddAsync(document);
        return document;
    }

    public Task UpdateAsync(FinancialDocument document)
    {
        _logger.LogInformation("Updating document: {DocumentId}", document.Id);
        _context.Entry(document).State = EntityState.Modified;
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(int id)
    {
        _logger.LogInformation("Deleting document: {DocumentId}", id);
        var document = await _context.FinancialDocuments.FindAsync(id);
        if (document != null)
        {
            _context.FinancialDocuments.Remove(document);
        }
        else
        {
            throw new InvalidOperationException($"Document with ID {id} not found");
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
