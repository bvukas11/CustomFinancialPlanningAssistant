using CustomFinancialPlanningAssistant.Core.Entities;

namespace CustomFinancialPlanningAssistant.Infrastructure.Repositories;

/// <summary>
/// Repository interface for FinancialDocument entity operations
/// </summary>
public interface IFinancialDocumentRepository
{
    /// <summary>
    /// Gets a financial document by ID
    /// </summary>
    /// <param name="id">Document identifier</param>
    /// <returns>Financial document or null if not found</returns>
    Task<FinancialDocument?> GetByIdAsync(int id);

    /// <summary>
    /// Gets a document with its related financial data records
    /// </summary>
    /// <param name="id">Document identifier</param>
    /// <returns>Document with data or null if not found</returns>
    Task<FinancialDocument?> GetWithDataAsync(int id);

    /// <summary>
    /// Gets a document with its related AI analyses
    /// </summary>
    /// <param name="id">Document identifier</param>
    /// <returns>Document with analyses or null if not found</returns>
    Task<FinancialDocument?> GetWithAnalysesAsync(int id);

    /// <summary>
    /// Gets a document with all related data (financial data and analyses)
    /// </summary>
    /// <param name="id">Document identifier</param>
    /// <returns>Complete document or null if not found</returns>
    Task<FinancialDocument?> GetCompleteAsync(int id);

    /// <summary>
    /// Gets all financial documents
    /// </summary>
    /// <returns>Collection of all documents</returns>
    Task<IEnumerable<FinancialDocument>> GetAllAsync();

    /// <summary>
    /// Gets documents filtered by status
    /// </summary>
    /// <param name="status">Document status</param>
    /// <returns>Collection of matching documents</returns>
    Task<IEnumerable<FinancialDocument>> GetByStatusAsync(string status);

    /// <summary>
    /// Gets the most recent documents
    /// </summary>
    /// <param name="count">Number of documents to retrieve</param>
    /// <returns>Collection of recent documents</returns>
    Task<IEnumerable<FinancialDocument>> GetRecentDocumentsAsync(int count);

    /// <summary>
    /// Gets documents within a date range
    /// </summary>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">End date</param>
    /// <returns>Collection of documents within the range</returns>
    Task<IEnumerable<FinancialDocument>> GetDocumentsByDateRangeAsync(DateTime startDate, DateTime endDate);

    /// <summary>
    /// Gets documents filtered by file type
    /// </summary>
    /// <param name="fileType">File type (Excel, CSV, PDF)</param>
    /// <returns>Collection of matching documents</returns>
    Task<IEnumerable<FinancialDocument>> GetDocumentsByTypeAsync(string fileType);

    /// <summary>
    /// Adds a new financial document
    /// </summary>
    /// <param name="document">Document to add</param>
    /// <returns>Added document</returns>
    Task<FinancialDocument> AddAsync(FinancialDocument document);

    /// <summary>
    /// Updates an existing financial document
    /// </summary>
    /// <param name="document">Document to update</param>
    Task UpdateAsync(FinancialDocument document);

    /// <summary>
    /// Deletes a financial document by ID
    /// </summary>
    /// <param name="id">Document identifier</param>
    Task DeleteAsync(int id);

    /// <summary>
    /// Saves all pending changes to the database
    /// </summary>
    /// <returns>Number of affected records</returns>
    Task<int> SaveChangesAsync();
}
