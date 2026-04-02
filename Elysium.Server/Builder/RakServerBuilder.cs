using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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
        Services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Debug);
        });
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

    public ApplicationRaknet Build()
    {
        var builder = new ContainerBuilder();
        
        builder.Populate(Services);
        
        return new ApplicationRaknet(builder.Build(), Configuration, _args);
    }
}