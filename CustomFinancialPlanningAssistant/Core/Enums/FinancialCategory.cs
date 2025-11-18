namespace CustomFinancialPlanningAssistant.Core.Enums;

/// <summary>
/// Represents the category of financial data
/// </summary>
public enum FinancialCategory
{
    /// <summary>
    /// Income or revenue accounts
    /// </summary>
    Revenue = 0,

    /// <summary>
    /// Cost or expense accounts
    /// </summary>
    Expense = 1,

    /// <summary>
    /// Asset accounts (what the company owns)
    /// </summary>
    Asset = 2,

    /// <summary>
    /// Liability accounts (what the company owes)
    /// </summary>
    Liability = 3,

    /// <summary>
    /// Equity accounts (owner's interest)
    /// </summary>
    Equity = 4
}
