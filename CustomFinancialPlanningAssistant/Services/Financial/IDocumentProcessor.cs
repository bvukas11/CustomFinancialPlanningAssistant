using CustomFinancialPlanningAssistant.Core.DTOs;
using CustomFinancialPlanningAssistant.Core.Entities;
using CustomFinancialPlanningAssistant.Core.Enums;

namespace CustomFinancialPlanningAssistant.Services.Financial;

/// <summary>
/// Interface for processing financial documents from various file formats
/// </summary>
public interface IDocumentProcessor
{
    /// <summary>
    /// Main entry point for processing any document type
    /// Automatically routes to correct processor based on file type
    /// </summary>
    /// <param name="fileStream">Stream containing the file data</param>
    /// <param name="fileName">Name of the file</param>
    /// <param name="fileType">Type of file to process</param>
    /// <returns>Upload result with processing details</returns>
    Task<UploadResultDto> ProcessDocumentAsync(Stream fileStream, string fileName, string fileType);

    /// <summary>
    /// Processes Excel files (.xlsx, .xls) and extracts financial data
    /// </summary>
    /// <param name="fileStream">Stream containing Excel file</param>
    /// <param name="fileName">Name of the Excel file</param>
    /// <returns>Upload result with processing details</returns>
    Task<UploadResultDto> ProcessExcelAsync(Stream fileStream, string fileName);

    /// <summary>
    /// Processes CSV files and extracts financial data
    /// </summary>
    /// <param name="fileStream">Stream containing CSV file</param>
    /// <param name="fileName">Name of the CSV file</param>
    /// <returns>Upload result with processing details</returns>
    Task<UploadResultDto> ProcessCsvAsync(Stream fileStream, string fileName);

    /// <summary>
    /// Processes PDF files and extracts financial data
    /// </summary>
    /// <param name="fileStream">Stream containing PDF file</param>
    /// <param name="fileName">Name of the PDF file</param>
    /// <returns>Upload result with processing details</returns>
    Task<UploadResultDto> ProcessPdfAsync(Stream fileStream, string fileName);

    /// <summary>
    /// Extracts raw financial data from Excel file without saving to database
    /// </summary>
    /// <param name="fileStream">Stream containing Excel file</param>
    /// <returns>List of extracted financial data records</returns>
    Task<List<FinancialData>> ExtractDataFromExcelAsync(Stream fileStream);

    /// <summary>
    /// Extracts raw financial data from CSV file without saving to database
    /// </summary>
    /// <param name="fileStream">Stream containing CSV file</param>
    /// <returns>List of extracted financial data records</returns>
    Task<List<FinancialData>> ExtractDataFromCsvAsync(Stream fileStream);

    /// <summary>
    /// Extracts text content from PDF file
    /// </summary>
    /// <param name="fileStream">Stream containing PDF file</param>
    /// <returns>Extracted text content</returns>
    Task<string> ExtractTextFromPdfAsync(Stream fileStream);

    /// <summary>
    /// Determines file type from filename extension
    /// </summary>
    /// <param name="fileName">Name of the file</param>
    /// <returns>Detected file type</returns>
    FileType DetectFileType(string fileName);

    /// <summary>
    /// Validates that file is not corrupt and meets requirements
    /// </summary>
    /// <param name="fileStream">Stream containing file to validate</param>
    /// <param name="fileName">Name of the file</param>
    /// <returns>True if file is valid, false otherwise</returns>
    Task<bool> ValidateFileAsync(Stream fileStream, string fileName);
}
