using System.Text.Json.Serialization;

namespace Elysium.Core.Configuration.Elysium;

public class ElysiumConfiguration
{
    [JsonPropertyName("permissions")]
    public string Permissions { get; set; } = "./permissions.json";

    [JsonPropertyName("resources")]
    public ResourcesConfiguration Resources { get; set; } = new();

    [JsonPropertyName("spawnWorldIdentifier")]
    public string SpawnWorldIdentifier { get; set; } = "default";
    
    [JsonPropertyName("movementValidation")]
    public bool MovementValidation { get; set; } = true;

    [JsonPropertyName("movementHorizontalThreshold")]
    public double? MovementHorizontalThreshold { get; set; } = 0.4;
    
    [JsonPropertyName("movementVerticalThreshold")]
    public double? MovementVerticalThreshold { get; set; } = 0.6;
    
    [JsonPropertyName("shutdownMessage")]
    public string ShutdownMessage { get; set; } = "Server is shutting down...";

    [JsonPropertyName("ticksPerSecond")] 
    public int? TicksPerSecond { get; set; } = 20;
    
    [JsonPropertyName("offlineMode")] 
    public bool OfflineMode { get; set; } = false;
}