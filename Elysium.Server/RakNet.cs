using Elysium.Core.Configuration;
using Elysium.Core.Interfaces.Transport;
using Elysium.RakNet;
using Elysium.RakNet.Hosts;
using Elysium.Server.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Elysium.Server;

public static class RakNet
{
    private static void InitializeConfigure(this HostApplicationBuilder builder)
    {
        builder.Services.AddLogging(builder =>
        {
            builder.AddConsole();
        }).AddRakNetServer();
        
        IHostEnvironment env = builder.Environment;

        builder.Configuration
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{env.ApplicationName}.json", optional: true, reloadOnChange: true);
        
        builder.Services
            .Configure<ServerInfoConfiguration>(
                builder.Configuration.GetSection("Server"));
    }

    public static HostApplicationBuilder CreateApplicationBuilder(string[] args)
    {
        var builder = new HostApplicationBuilder();
        
        builder.InitializeConfigure();

        return builder;
    }
    
    public static IServiceCollection AddRakNetServer(this IServiceCollection services)
    {
        services.AddDefaultServices();
        services.AddHostedService<RakNetHostedService>();
        
        return services;
    } 
}