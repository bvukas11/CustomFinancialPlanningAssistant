using Microsoft.EntityFrameworkCore;
using CustomFinancialPlanningAssistant.Core.Entities;

namespace CustomFinancialPlanningAssistant.Infrastructure.Data;

/// <summary>
/// Database context for Financial Analysis Assistant application
/// Manages all entity sets and database configuration
/// </summary>
public class AppDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the AppDbContext class
    /// </summary>
    /// <param name="options">Database context options</param>
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Financial documents uploaded to the system
    /// </summary>
    public DbSet<FinancialDocument> FinancialDocuments { get; set; }

    /// <summary>
    /// Financial data records extracted from documents
    /// </summary>
    public DbSet<FinancialData> FinancialDataRecords { get; set; }

    /// <summary>
    /// AI analyses performed on financial documents
    /// </summary>
    public DbSet<AIAnalysis> AIAnalyses { get; set; }

    /// <summary>
    /// Generated financial reports
    /// </summary>
    public DbSet<Report> Reports { get; set; }

    /// <summary>
    /// Configures the database schema using Fluent API
    /// </summary>
    /// <param name="modelBuilder">Model builder for entity configuration</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure FinancialDocument entity
        modelBuilder.Entity<FinancialDocument>(entity =>
        {
            entity.HasKey(e => e.Id);

            // Create index on Status for faster status-based queries
            entity.HasIndex(e => e.Status)
                .HasDatabaseName("IX_FinancialDocuments_Status");

            // Create index on UploadDate (descending) for recent document queries
            entity.HasIndex(e => e.UploadDate)
                .IsDescending()
                .HasDatabaseName("IX_FinancialDocuments_UploadDate");

            // Set default value for UploadDate
            entity.Property(e => e.UploadDate)
                .HasDefaultValueSql("GETUTCDATE()");

            // Configure one-to-many relationship with FinancialData
            entity.HasMany(e => e.FinancialDataRecords)
                .WithOne(e => e.Document)
                .HasForeignKey(e => e.DocumentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure one-to-many relationship with AIAnalysis
            entity.HasMany(e => e.AIAnalyses)
                .WithOne(e => e.Document)
                .HasForeignKey(e => e.DocumentId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure FinancialData entity
        modelBuilder.Entity<FinancialData>(entity =>
        {
            entity.HasKey(e => e.Id);

            // Foreign key configuration
            entity.HasOne(e => e.Document)
                .WithMany(e => e.FinancialDataRecords)
                .HasForeignKey(e => e.DocumentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure decimal precision for Amount
            entity.Property(e => e.Amount)
                .HasPrecision(18, 2);

            // Create index on Period for time-based queries
            entity.HasIndex(e => e.Period)
                .HasDatabaseName("IX_FinancialData_Period");

            // Create index on Category for category-based queries
            entity.HasIndex(e => e.Category)
                .HasDatabaseName("IX_FinancialData_Category");

            // Create composite index for common query patterns (DocumentId + Period)
            entity.HasIndex(e => new { e.DocumentId, e.Period })
                .HasDatabaseName("IX_FinancialData_DocumentId_Period");
        });

        // Configure AIAnalysis entity
        modelBuilder.Entity<AIAnalysis>(entity =>
        {
            entity.HasKey(e => e.Id);

            // Foreign key configuration
            entity.HasOne(e => e.Document)
                .WithMany(e => e.AIAnalyses)
                .HasForeignKey(e => e.DocumentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Create index on CreatedDate (descending) for recent analysis queries
            entity.HasIndex(e => e.CreatedDate)
                .IsDescending()
                .HasDatabaseName("IX_AIAnalyses_CreatedDate");

            // Create index on AnalysisType for type-based queries
            entity.HasIndex(e => e.AnalysisType)
                .HasDatabaseName("IX_AIAnalyses_AnalysisType");

            // Set default value for CreatedDate
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("GETUTCDATE()");
        });

        // Configure Report entity
        modelBuilder.Entity<Report>(entity =>
        {
            entity.HasKey(e => e.Id);

            // Create index on ReportType for type-based queries
            entity.HasIndex(e => e.ReportType)
                .HasDatabaseName("IX_Reports_ReportType");

            // Create index on GeneratedDate (descending) for recent report queries
            entity.HasIndex(e => e.GeneratedDate)
                .IsDescending()
                .HasDatabaseName("IX_Reports_GeneratedDate");

            // Set default value for GeneratedDate
            entity.Property(e => e.GeneratedDate)
                .HasDefaultValueSql("GETUTCDATE()");
        });
    }
}
