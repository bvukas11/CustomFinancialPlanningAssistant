using CustomFinancialPlanningAssistant.Core.Entities;

namespace CustomFinancialPlanningAssistant.Infrastructure.Repositories;

/// <summary>
/// Repository interface for FinancialData entity operations
/// </summary>
public interface IFinancialDataRepository
{
    /// <summary>
    /// Gets a financial data record by ID
    /// </summary>
    /// <param name="id">Record identifier</param>
    /// <returns>Financial data record or null if not found</returns>
    Task<FinancialData?> GetByIdAsync(int id);

    /// <summary>
    /// Gets all financial data records
    /// </summary>
    /// <returns>Collection of all records</returns>
    Task<IEnumerable<FinancialData>> GetAllAsync();

    /// <summary>
    /// Gets all financial data for a specific document
    /// </summary>
    /// <param name="documentId">Document identifier</param>
    /// <returns>Collection of financial data records</returns>
    Task<IEnumerable<FinancialData>> GetByDocumentIdAsync(int documentId);

    /// <summary>
    /// Gets financial data for a specific period
    /// </summary>
    /// <param name="period">Period identifier (e.g., "2024-Q1")</param>
    /// <returns>Collection of financial data records</returns>
    Task<IEnumerable<FinancialData>> GetByPeriodAsync(string period);

    /// <summary>
    /// Gets financial data for a specific category
    /// </summary>
    /// <param name="category">Category name</param>
    /// <returns>Collection of financial data records</returns>
    Task<IEnumerable<FinancialData>> GetByCategoryAsync(string category);

    /// <summary>
    /// Gets financial data within a date range
    /// </summary>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">End date</param>
    /// <returns>Collection of financial data records</returns>
    Task<IEnumerable<FinancialData>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);

    /// <summary>
    /// Calculates the total amount for a category in a specific period
    /// </summary>
    /// <param name="category">Category name</param>
    /// <param name="period">Period identifier</param>
    /// <returns>Total amount</returns>
    Task<decimal> GetTotalByCategoryAsync(string category, string period);

    /// <summary>
    /// Calculates the total amount for a specific period
    /// </summary>
    /// <param name="period">Period identifier</param>
    /// <returns>Total amount</returns>
    Task<decimal> GetTotalByPeriodAsync(string period);

    /// <summary>
    /// Gets a summary of amounts grouped by category for a period
    /// </summary>
    /// <param name="period">Period identifier</param>
    /// <returns>Dictionary with category names and their totals</returns>
    Task<Dictionary<string, decimal>> GetCategorySummaryAsync(string period);

    /// <summary>
    /// Gets the top expenses for a specific period
    /// </summary>
    /// <param name="count">Number of records to retrieve</param>
    /// <param name="period">Period identifier</param>
    /// <returns>Collection of top expense records</returns>
    Task<IEnumerable<FinancialData>> GetTopExpensesAsync(int count, string period);

    /// <summary>
    /// Adds multiple financial data records at once
    /// </summary>
    /// <param name="dataRecords">Collection of records to add</param>
    /// <returns>Added records</returns>
    Task<IEnumerable<FinancialData>> AddRangeAsync(IEnumerable<FinancialData> dataRecords);

    /// <summary>
    /// Updates an existing financial data record
    /// </summary>
    /// <param name="data">Record to update</param>
    Task UpdateAsync(FinancialData data);

    /// <summary>
    /// Deletes all financial data for a specific document
    /// </summary>
    /// <param name="documentId">Document identifier</param>
    Task DeleteByDocumentIdAsync(int documentId);

    /// <summary>
    /// Saves all pending changes to the database
    /// </summary>
    /// <returns>Number of affected records</returns>
    Task<int> SaveChangesAsync();
}
