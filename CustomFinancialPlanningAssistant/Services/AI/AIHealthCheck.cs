using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace CustomFinancialPlanningAssistant.Services.AI;

/// <summary>
/// Health check for AI service (Ollama) availability
/// </summary>
public class AIHealthCheck : IHealthCheck
{
    private readonly ILlamaService _llamaService;
    private readonly ILogger<AIHealthCheck> _logger;

    /// <summary>
    /// Initializes a new instance of the AIHealthCheck class
    /// </summary>
    public AIHealthCheck(
        ILlamaService llamaService,
        ILogger<AIHealthCheck> logger)
    {
        _llamaService = llamaService ?? throw new ArgumentNullException(nameof(llamaService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Checks the health of the AI service
    /// </summary>
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Performing AI service health check");

            // Check if Ollama service is available
            var isAvailable = await _llamaService.IsServiceAvailableAsync();

            if (!isAvailable)
            {
                _logger.LogWarning("Ollama service is not available");
                return HealthCheckResult.Unhealthy(
                    "Ollama service is not available. Please ensure Ollama is installed and running.");
            }

            // Get available models
            var models = await _llamaService.GetAvailableModelsAsync();

            if (!models.Any())
            {
                _logger.LogWarning("No AI models are available");
                return HealthCheckResult.Degraded(
                    "Ollama is running but no models are available. Please download required models using 'ollama pull llama3.2'");
            }

            var data = new Dictionary<string, object>
            {
                { "OllamaStatus", "Running" },
                { "AvailableModels", models.Count },
                { "Models", string.Join(", ", models) },
                { "CheckedAt", DateTime.UtcNow.ToString("O") }
            };

            _logger.LogInformation(
                "AI service is healthy with {Count} models available",
                models.Count);

            return HealthCheckResult.Healthy(
                $"AI service is healthy with {models.Count} model(s) available",
                data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AI health check failed");
            return HealthCheckResult.Unhealthy(
                "AI service health check failed. Please verify Ollama is running at http://localhost:11434",
                ex);
        }
    }
}
