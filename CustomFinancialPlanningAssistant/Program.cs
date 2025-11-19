using CustomFinancialPlanningAssistant.Components;
using CustomFinancialPlanningAssistant.Infrastructure.Data;
using CustomFinancialPlanningAssistant.Infrastructure.FileStorage;
using CustomFinancialPlanningAssistant.Infrastructure.Repositories;
using CustomFinancialPlanningAssistant.Services.AI;
using CustomFinancialPlanningAssistant.Services.Financial;
using CustomFinancialPlanningAssistant.Services.Reports;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Configure QuestPDF License (Community - Free for non-commercial use)
QuestPDF.Settings.License = LicenseType.Community;

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Configure Database Context
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    
    if (builder.Environment.IsDevelopment())
    {
        options.UseSqlServer(connectionString)
               .EnableDetailedErrors()
               .EnableSensitiveDataLogging();
    }
    else
    {
        options.UseSqlServer(connectionString);
    }
});

// Register Repositories
builder.Services.AddScoped<IFinancialDocumentRepository, FinancialDocumentRepository>();
builder.Services.AddScoped<IFinancialDataRepository, FinancialDataRepository>();
builder.Services.AddScoped<IAIAnalysisRepository, AIAnalysisRepository>();

// Register File Storage Service
builder.Services.AddScoped<IFileStorageService, FileStorageService>();

// Register Document Processor
builder.Services.AddScoped<IDocumentProcessor, DocumentProcessor>();

// Register Financial Service
builder.Services.AddScoped<IFinancialService, FinancialService>();

// Register Report Services (Phase 7 & 9)
builder.Services.AddScoped<PdfReportService>();        // NEW - Phase 9!
builder.Services.AddScoped<ExcelReportService>();      // Phase 7
builder.Services.AddScoped<IReportService, ReportService>();

// Configure AI Settings
builder.Services.Configure<AIModelConfiguration>(
    builder.Configuration.GetSection("AISettings"));

// Register AI Service
builder.Services.AddScoped<ILlamaService, LlamaService>();

// Register HttpClient for Ollama (for health checks and manual calls if needed)
builder.Services.AddHttpClient("Ollama", client =>
{
    var ollamaUrl = builder.Configuration["AISettings:OllamaBaseUrl"] ?? "http://localhost:11434";
    client.BaseAddress = new Uri(ollamaUrl);
    client.Timeout = TimeSpan.FromSeconds(120);
});

// Add MudBlazor Services
builder.Services.AddMudServices();

// Add Health Checks
builder.Services.AddHealthChecks()
    .AddCheck<AIHealthCheck>("ai_service", tags: new[] { "ai", "ollama" });

var app = builder.Build();

// Apply database migrations automatically on startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        var logger = services.GetRequiredService<ILogger<Program>>();
        
        logger.LogInformation("Applying database migrations...");
        context.Database.Migrate();
        logger.LogInformation("Database migrations applied successfully");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database");
        // In development, you might want to throw, in production you might want to continue
        if (builder.Environment.IsDevelopment())
        {
            throw;
        }
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Map Health Check Endpoints
app.MapHealthChecks("/health");
app.MapHealthChecks("/health/ai", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ai")
});

app.Run();
