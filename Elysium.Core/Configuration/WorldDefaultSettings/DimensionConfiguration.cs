namespace Elysium.Core.Configuration.WorldDefaultSettings;

public class DimensionConfiguration
{
    public string Identifier { get; set; } = "overworld";
    public string Type { get; set; } = "overworld";
    public string Generator { get; set; } = "superflat";
    public int ViewDistance { get; set; } = 20;
    public int SimulationDistance { get; set; } = 10;
    public ICollection<int> SpawnPosition { get; set; } = [0, -60, 0];
}