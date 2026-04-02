using System.Text.Json.Serialization;

namespace Elysium.Core.Configuration.Elysium;

public class ElysiumConfiguration
{
    public string Permissions { get; set; } = "./permissions.json";

    public ResourcesConfiguration Resources { get; set; } = new();

    public string SpawnWorldIdentifier { get; set; } = "default";
    
    public bool MovementValidation { get; set; } = true;

    public double? MovementHorizontalThreshold { get; set; } = 0.4;
    
    public double? MovementVerticalThreshold { get; set; } = 0.6;
    
    public string ShutdownMessage { get; set; } = "Server is shutting down...";

    public int? TicksPerSecond { get; set; } = 20;
    
    public bool OfflineMode { get; set; } = false;
}