using CustomFinancialPlanningAssistant.Core.Entities;

namespace CustomFinancialPlanningAssistant.Infrastructure.Repositories;

/// <summary>
/// Repository interface for AIAnalysis entity operations
/// </summary>
public interface IAIAnalysisRepository
{
    /// <summary>
    /// Gets an AI analysis by ID
    /// </summary>
    /// <param name="id">Analysis identifier</param>
    /// <returns>AI analysis or null if not found</returns>
    Task<AIAnalysis?> GetByIdAsync(int id);

    /// <summary>
    /// Gets all AI analyses
    /// </summary>
    /// <returns>Collection of all analyses</returns>
    Task<IEnumerable<AIAnalysis>> GetAllAsync();

    /// <summary>
    /// Gets all AI analyses for a specific document
    /// </summary>
    /// <param name="documentId">Document identifier</param>
    /// <returns>Collection of AI analyses</returns>
    Task<IEnumerable<AIAnalysis>> GetByDocumentIdAsync(int documentId);

    /// <summary>
    /// Gets analyses filtered by type
    /// </summary>
    /// <param name="analysisType">Type of analysis</param>
    /// <returns>Collection of matching analyses</returns>
    Task<IEnumerable<AIAnalysis>> GetByAnalysisTypeAsync(string analysisType);

    /// <summary>
    /// Gets the most recent AI analyses
    /// </summary>
    /// <param name="count">Number of analyses to retrieve</param>
    /// <returns>Collection of recent analyses</returns>
    Task<IEnumerable<AIAnalysis>> GetRecentAnalysesAsync(int count);

    /// <summary>
    /// Gets analyses within a date range
    /// </summary>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">End date</param>
    /// <returns>Collection of analyses within the range</returns>
    Task<IEnumerable<AIAnalysis>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);

    /// <summary>
    /// Calculates the average execution time for a specific analysis type
    /// </summary>
    /// <param name="analysisType">Type of analysis</param>
    /// <returns>Average execution time in milliseconds</returns>
    Task<double> GetAverageExecutionTimeAsync(string analysisType);

    /// <summary>
    /// Adds a new AI analysis
    /// </summary>
    /// <param name="analysis">Analysis to add</param>
    /// <returns>Added analysis</returns>
    Task<AIAnalysis> AddAsync(AIAnalysis analysis);

    /// <summary>
    /// Updates the rating for an existing analysis
    /// </summary>
    /// <param name="id">Analysis identifier</param>
    /// <param name="rating">New rating (1-5)</param>
    Task UpdateRatingAsync(int id, int rating);

    /// <summary>
    /// Deletes an AI analysis by ID
    /// </summary>
    /// <param name="id">Analysis identifier</param>
    Task DeleteAsync(int id);

    /// <summary>
    /// Saves all pending changes to the database
    /// </summary>
    /// <returns>Number of affected records</returns>
    Task<int> SaveChangesAsync();
}
