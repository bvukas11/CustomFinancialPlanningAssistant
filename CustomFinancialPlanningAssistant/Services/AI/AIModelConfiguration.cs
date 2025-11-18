using System.ComponentModel.DataAnnotations;

namespace CustomFinancialPlanningAssistant.Services.AI;

/// <summary>
/// Configuration settings for AI model integration
/// </summary>
public class AIModelConfiguration
{
    /// <summary>
    /// Base URL for Ollama API
    /// </summary>
    public string OllamaBaseUrl { get; set; } = "http://localhost:11434";

    /// <summary>
    /// Default model for text-based financial analysis
    /// </summary>
    public string DefaultTextModel { get; set; } = "llama3.2";

    /// <summary>
    /// Model for image and document vision analysis
    /// </summary>
    public string VisionModel { get; set; } = "llama3.2-vision";

    /// <summary>
    /// Alternative model for specialized tasks
    /// </summary>
    public string AlternativeModel { get; set; } = "qwen2.5:8b";

    /// <summary>
    /// Maximum number of tokens for model responses
    /// </summary>
    [Range(1, 32768)]
    public int MaxTokens { get; set; } = 4096;

    /// <summary>
    /// Controls randomness in responses (0.0 = deterministic, 1.0 = creative)
    /// </summary>
    [Range(0.0, 1.0)]
    public double Temperature { get; set; } = 0.7;

    /// <summary>
    /// Nucleus sampling parameter for response diversity
    /// </summary>
    [Range(0.0, 1.0)]
    public double TopP { get; set; } = 0.9;

    /// <summary>
    /// Request timeout in seconds
    /// </summary>
    [Range(30, 600)]
    public int TimeoutSeconds { get; set; } = 120;

    /// <summary>
    /// Number of retry attempts for failed requests
    /// </summary>
    [Range(0, 10)]
    public int MaxRetries { get; set; } = 3;

    /// <summary>
    /// Delay between retry attempts in seconds
    /// </summary>
    [Range(1, 30)]
    public int RetryDelaySeconds { get; set; } = 2;

    /// <summary>
    /// Gets the timeout as a TimeSpan
    /// </summary>
    public TimeSpan Timeout => TimeSpan.FromSeconds(TimeoutSeconds);

    /// <summary>
    /// Initializes a new instance with default values
    /// </summary>
    public AIModelConfiguration()
    {
    }
}
