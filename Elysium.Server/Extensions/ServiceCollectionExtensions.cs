using Elysium.Core.Interfaces.Services.Parser;
using Elysium.RakNet.Hosts;
using Microsoft.Extensions.DependencyInjection;

namespace Elysium.Server.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddConsoleInput(this IServiceCollection services)
        => services.AddHostedService<ConsoleInputHostedService>();
    
    internal static IServiceCollection AddPacketOnline<T>(this IServiceCollection services, string key) where T : class, IOnlinePacketParser
    {
        services.AddSingleton<IOnlinePacketParser, T>();
        services.AddSingleton<IPacketParser, T>();
            
        return services;
    }
    
    internal static IServiceCollection AddPacketOffline<T>(this IServiceCollection services, string key) where T : class, IOfflinePacketParser
    {
        services.AddSingleton<IOfflinePacketParser, T>();
        services.AddSingleton<IPacketParser, T>();
            
        return services;
    }
    
    internal static IServiceCollection AddDefaultServices(this IServiceCollection services)
    {
        services.AddLogging();
            
        return services;
    }
}