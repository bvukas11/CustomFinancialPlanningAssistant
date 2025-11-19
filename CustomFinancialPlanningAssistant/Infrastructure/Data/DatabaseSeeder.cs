using CustomFinancialPlanningAssistant.Core.Entities;
using CustomFinancialPlanningAssistant.Core.Enums;
using CustomFinancialPlanningAssistant.Infrastructure.Data;

namespace CustomFinancialPlanningAssistant.Infrastructure.Data;

/// <summary>
/// Database seeder for industry benchmark data
/// </summary>
public static class DatabaseSeeder
{
    /// <summary>
    /// Seeds industry benchmark data if not already present
    /// </summary>
    public static void SeedIndustryBenchmarks(AppDbContext context)
    {
        if (!context.IndustryBenchmarks.Any())
        {
            var benchmarks = new List<IndustryBenchmark>
            {
                // ==================== TECHNOLOGY INDUSTRY ====================
                new IndustryBenchmark
                {
                    Industry = IndustryType.Technology,
                    MetricName = "GrossMargin",
                    AverageValue = 65.0m,
                    MedianValue = 68.0m,
                    Percentile25 = 55.0m,
                    Percentile75 = 75.0m,
                    DataSource = "S&P Global Market Intelligence",
                    SampleSize = 250,
                    LastUpdated = DateTime.UtcNow,
                    Notes = "Software and technology services companies"
                },
                new IndustryBenchmark
                {
                    Industry = IndustryType.Technology,
                    MetricName = "OperatingMargin",
                    AverageValue = 15.0m,
                    MedianValue = 18.0m,
                    Percentile25 = 5.0m,
                    Percentile75 = 25.0m,
                    DataSource = "S&P Global Market Intelligence",
                    SampleSize = 250,
                    LastUpdated = DateTime.UtcNow,
                    Notes = "EBITDA margins for tech sector"
                },
                new IndustryBenchmark
                {
                    Industry = IndustryType.Technology,
                    MetricName = "NetMargin",
                    AverageValue = 12.0m,
                    MedianValue = 15.0m,
                    Percentile25 = 2.0m,
                    Percentile75 = 22.0m,
                    DataSource = "S&P Global Market Intelligence",
                    SampleSize = 250,
                    LastUpdated = DateTime.UtcNow,
                    Notes = "Net profit margins vary significantly by sub-sector"
                },
                new IndustryBenchmark
                {
                    Industry = IndustryType.Technology,
                    MetricName = "CurrentRatio",
                    AverageValue = 2.8m,
                    MedianValue = 2.5m,
                    Percentile25 = 1.8m,
                    Percentile75 = 3.5m,
                    DataSource = "Industry Financial Reports",
                    SampleSize = 200,
                    LastUpdated = DateTime.UtcNow,
                    Notes = "Strong liquidity position typical in tech"
                },
                new IndustryBenchmark
                {
                    Industry = IndustryType.Technology,
                    MetricName = "DebtToEquity",
                    AverageValue = 0.3m,
                    MedianValue = 0.2m,
                    Percentile25 = 0.0m,
                    Percentile75 = 0.6m,
                    DataSource = "S&P Global Market Intelligence",
                    SampleSize = 250,
                    LastUpdated = DateTime.UtcNow,
                    Notes = "Conservative leverage in technology sector"
                },
                new IndustryBenchmark
                {
                    Industry = IndustryType.Technology,
                    MetricName = "ReturnOnAssets",
                    AverageValue = 8.5m,
                    MedianValue = 9.0m,
                    Percentile25 = 4.0m,
                    Percentile75 = 14.0m,
                    DataSource = "Industry Financial Reports",
                    SampleSize = 200,
                    LastUpdated = DateTime.UtcNow,
                    Notes = "Asset efficiency varies by business model"
                },

                // ==================== HEALTHCARE INDUSTRY ====================
                new IndustryBenchmark
                {
                    Industry = IndustryType.Healthcare,
                    MetricName = "GrossMargin",
                    AverageValue = 58.0m,
                    MedianValue = 60.0m,
                    Percentile25 = 45.0m,
                    Percentile75 = 70.0m,
                    DataSource = "Healthcare Financial Management Association",
                    SampleSize = 180,
                    LastUpdated = DateTime.UtcNow,
                    Notes = "Hospitals and healthcare providers"
                },
                new IndustryBenchmark
                {
                    Industry = IndustryType.Healthcare,
                    MetricName = "OperatingMargin",
                    AverageValue = 4.5m,
                    MedianValue = 5.0m,
                    Percentile25 = 1.0m,
                    Percentile75 = 8.0m,
                    DataSource = "Healthcare Financial Management Association",
                    SampleSize = 180,
                    LastUpdated = DateTime.UtcNow,
                    Notes = "Tight margins in healthcare sector"
                },
                new IndustryBenchmark
                {
                    Industry = IndustryType.Healthcare,
                    MetricName = "CurrentRatio",
                    AverageValue = 1.9m,
                    MedianValue = 1.8m,
                    Percentile25 = 1.2m,
                    Percentile75 = 2.5m,
                    DataSource = "Industry Financial Reports",
                    SampleSize = 150,
                    LastUpdated = DateTime.UtcNow,
                    Notes = "Adequate liquidity for operations"
                },

                // ==================== FINANCE INDUSTRY ====================
                new IndustryBenchmark
                {
                    Industry = IndustryType.Finance,
                    MetricName = "NetMargin",
                    AverageValue = 18.0m,
                    MedianValue = 20.0m,
                    Percentile25 = 12.0m,
                    Percentile75 = 25.0m,
                    DataSource = "Federal Reserve Bank Data",
                    SampleSize = 300,
                    LastUpdated = DateTime.UtcNow,
                    Notes = "Strong profitability in banking sector"
                },
                new IndustryBenchmark
                {
                    Industry = IndustryType.Finance,
                    MetricName = "ReturnOnEquity",
                    AverageValue = 12.5m,
                    MedianValue = 13.0m,
                    Percentile25 = 8.0m,
                    Percentile75 = 18.0m,
                    DataSource = "Federal Reserve Bank Data",
                    SampleSize = 300,
                    LastUpdated = DateTime.UtcNow,
                    Notes = "ROE varies by bank size and business model"
                },

                // ==================== MANUFACTURING INDUSTRY ====================
                new IndustryBenchmark
                {
                    Industry = IndustryType.Manufacturing,
                    MetricName = "GrossMargin",
                    AverageValue = 35.0m,
                    MedianValue = 38.0m,
                    Percentile25 = 25.0m,
                    Percentile75 = 45.0m,
                    DataSource = "National Association of Manufacturers",
                    SampleSize = 220,
                    LastUpdated = DateTime.UtcNow,
                    Notes = "Manufacturing sector margins"
                },
                new IndustryBenchmark
                {
                    Industry = IndustryType.Manufacturing,
                    MetricName = "AssetTurnover",
                    AverageValue = 1.2m,
                    MedianValue = 1.1m,
                    Percentile25 = 0.8m,
                    Percentile75 = 1.6m,
                    DataSource = "Industry Financial Reports",
                    SampleSize = 180,
                    LastUpdated = DateTime.UtcNow,
                    Notes = "Asset utilization in manufacturing"
                },

                // ==================== RETAIL INDUSTRY ====================
                new IndustryBenchmark
                {
                    Industry = IndustryType.Retail,
                    MetricName = "GrossMargin",
                    AverageValue = 22.0m,
                    MedianValue = 25.0m,
                    Percentile25 = 15.0m,
                    Percentile75 = 30.0m,
                    DataSource = "National Retail Federation",
                    SampleSize = 160,
                    LastUpdated = DateTime.UtcNow,
                    Notes = "Retail sector gross margins"
                },
                new IndustryBenchmark
                {
                    Industry = IndustryType.Retail,
                    MetricName = "InventoryTurnover",
                    AverageValue = 4.8m,
                    MedianValue = 5.0m,
                    Percentile25 = 3.2m,
                    Percentile75 = 6.5m,
                    DataSource = "National Retail Federation",
                    SampleSize = 160,
                    LastUpdated = DateTime.UtcNow,
                    Notes = "Inventory management efficiency"
                },

                // ==================== ENERGY INDUSTRY ====================
                new IndustryBenchmark
                {
                    Industry = IndustryType.Energy,
                    MetricName = "OperatingMargin",
                    AverageValue = 8.5m,
                    MedianValue = 9.0m,
                    Percentile25 = 3.0m,
                    Percentile75 = 15.0m,
                    DataSource = "Energy Information Administration",
                    SampleSize = 140,
                    LastUpdated = DateTime.UtcNow,
                    Notes = "Energy sector profitability varies by commodity prices"
                },
                new IndustryBenchmark
                {
                    Industry = IndustryType.Energy,
                    MetricName = "DebtToEquity",
                    AverageValue = 0.8m,
                    MedianValue = 0.7m,
                    Percentile25 = 0.3m,
                    Percentile75 = 1.4m,
                    DataSource = "Energy Information Administration",
                    SampleSize = 140,
                    LastUpdated = DateTime.UtcNow,
                    Notes = "Capital intensive industry with higher leverage"
                }
            };

            context.IndustryBenchmarks.AddRange(benchmarks);
            context.SaveChanges();

            Console.WriteLine($"Seeded {benchmarks.Count} industry benchmark records across {benchmarks.Select(b => b.Industry).Distinct().Count()} industries.");
        }
        else
        {
            Console.WriteLine("Industry benchmarks already exist in database.");
        }
    }
}