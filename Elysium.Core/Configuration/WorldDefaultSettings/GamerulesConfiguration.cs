namespace Elysium.Core.Configuration.WorldDefaultSettings;

public class GamerulesConfiguration
{
    public bool ShowCoordinates { get; set; } = false;
    public bool ShowDaysPlayed { get; set; } = false;
    public bool DoDayLightCycle { get; set; } = true;
    public bool DoImmediateRespawn { get; set; } = false;
    public bool DoTileDrops { get; set; } = true;
    public bool KeepInventory { get; set; } = false;
    public bool FallDamage { get; set; } = true;
    public bool FireDamage { get; set; } = true;
    public bool DrowningDamage { get; set; } = true;
    public int RandomTickSpeed { get; set; } = 1;
    public bool LocatorBar { get; set; } = false;
}