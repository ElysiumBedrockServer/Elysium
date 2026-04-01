using System.Text.Json.Serialization;

namespace Elysium.Core.Configuration.Elysium;

public class ResourcesConfiguration
{
    [JsonPropertyName("path")]  
    public string Path { get; set; } = "./resource_packs";

    [JsonPropertyName("mustAccept")] 
    public bool MustAccept { get; set; } = true;
    
    [JsonPropertyName("chunkDownloadTimeout")]
    public int? ChunkDownloadTimeout { get; set; } = 1;
    
    [JsonPropertyName("chunkMaxSize")]
    public int? ChunkMaxSize { get; set; } = 262144;
}