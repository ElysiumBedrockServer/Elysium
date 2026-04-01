using Autofac;
using Autofac.Extensions.DependencyInjection;
using Elysium.Server.Server;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Elysium.Server.Builder;

public class RakNetBuilder
{
    public IServiceCollection Services { get; private set; }
    public IConfiguration Configuration { get; private set; }

    private readonly string[] _args;

    private RakNetBuilder(IServiceCollection services, string[] args)
    {
        Services = services;
        _args = args;
        
        AddConfiguration();
        ConfigureDefaultServices();
    }
    
    public static RakNetBuilder CreateNetBuilder(string[] args)
        => new RakNetBuilder(new ServiceCollection(), args);
    
    private void ConfigureDefaultServices()
    {
        Services.AddLogging();
    }

    private void AddConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddEnvironmentVariables();

#if DEBUG
        builder.AddJsonFile("appsettings.Development.json", optional: true);
#else
        builder.AddJsonFile("appsettings.json", optional: true);
#endif
        Configuration = builder.Build();
    }

    public Raknet Build()
    {
        var builder = new ContainerBuilder();
        
        builder.Populate(Services);
        
        return new Raknet(builder.Build(), Configuration, _args);
    }
}