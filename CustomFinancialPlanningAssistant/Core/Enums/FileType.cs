namespace CustomFinancialPlanningAssistant.Core.Enums;

/// <summary>
/// Represents the type of uploaded file
/// </summary>
public enum FileType
{
    /// <summary>
    /// Unknown or unsupported file type
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// Microsoft Excel file (.xlsx, .xls)
    /// </summary>
    Excel = 1,

    /// <summary>
    /// Comma-separated values file (.csv)
    /// </summary>
    CSV = 2,

    /// <summary>
    /// Portable Document Format file (.pdf)
    /// </summary>
    PDF = 3
}
