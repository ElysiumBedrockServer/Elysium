using System.Text.Json.Serialization;

namespace Elysium.Core.Configuration.Elysium;

public class ResourcesConfiguration
{
    public string Path { get; set; } = "./resource_packs";

    public bool MustAccept { get; set; } = true;
    
    public int? ChunkDownloadTimeout { get; set; } = 1;
    
    public int? ChunkMaxSize { get; set; } = 262144;
}