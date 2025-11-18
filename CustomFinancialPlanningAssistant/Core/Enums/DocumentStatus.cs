namespace CustomFinancialPlanningAssistant.Core.Enums;

/// <summary>
/// Represents the status of a financial document in the system
/// </summary>
public enum DocumentStatus
{
    /// <summary>
    /// Document has been uploaded but not yet processed
    /// </summary>
    Uploaded = 0,

    /// <summary>
    /// Document is currently being processed
    /// </summary>
    Processing = 1,

    /// <summary>
    /// Document has been successfully analyzed
    /// </summary>
    Analyzed = 2,

    /// <summary>
    /// An error occurred during processing
    /// </summary>
    Error = 3,

    /// <summary>
    /// Document has been archived
    /// </summary>
    Archived = 4
}
