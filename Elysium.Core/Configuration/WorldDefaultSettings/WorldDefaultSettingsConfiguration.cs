namespace Elysium.Core.Configuration.WorldDefaultSettings;

public class WorldDefaultSettingsConfiguration
{
    public string Identifier { get; set; } = "default";
    public ulong Seed { get; set; } = 0;
    public string Gamemode { get; set; } = "survival";
    public string Difficulty { get; set; } = "normal";
    public int SaveInterval { get; set; } = 5;

    public ICollection<DimensionConfiguration> Dimensions { get; set; } = new List<DimensionConfiguration>();
    public GamerulesConfiguration Gamerules { get; set; } = new GamerulesConfiguration();
}