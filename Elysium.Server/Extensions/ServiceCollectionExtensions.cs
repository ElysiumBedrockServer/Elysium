using System.Reflection;
using Elysium.Core.Attributes;
using Elysium.Core.Configuration;
using Elysium.Core.Packets;
using Elysium.Core.Packets.Ack;
using Elysium.Core.Packets.Connect;
using Elysium.RakNet.Hosts;
using Elysium.RakNet.Sockets;
using Elysium.RakNet.Store;
using Elysium.Server.Hosts;
using Microsoft.Extensions.DependencyInjection;

namespace Elysium.Server.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPacket<T>(this IServiceCollection service) where T : RakNetPacket
    {
        var type = typeof(T);

        var attribute = type.GetCustomAttribute<RakNetPacketAttribute>();

        if (attribute == null)
            throw new InvalidOperationException($"RakNetPacket attribute not found for {type.Name}");

        service.AddSingleton(new PacketConfig(type, attribute.Id));

        return service;
    }

    public static IServiceCollection AddPacket(this IServiceCollection service, Type type)
    {
        if (!type.IsSubclassOf(typeof(RakNetPacket)))
            throw new InvalidOperationException($"RakNetPacket type {type.Name} is not a RakNetPacket");

        var attribute = type.GetCustomAttribute<RakNetPacketAttribute>();

        if (attribute == null)
            throw new InvalidOperationException($"RakNetPacket attribute not found for {type.Name}");

        service.AddSingleton(new PacketConfig(type, attribute.Id));

        return service;
    }

    public static IServiceCollection AddRakNetPackets(this IServiceCollection services, params Assembly[] assemblies)
    {
        var targetAssemblies = assemblies.Length == 0
            ? new[] { Assembly.GetCallingAssembly() }
            : assemblies;

        foreach (var assembly in targetAssemblies)
        {
            var typesWithPacketId = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.IsPublic && t.IsSubclassOf(typeof(RakNetPacket)))
                .Select(t => new { Type = t, Attribute = t.GetCustomAttribute<RakNetPacketAttribute>() })
                .Where(x => x.Attribute != null);

            foreach (var item in typesWithPacketId)
            {
                var config = new PacketConfig(item.Type, item.Attribute!.Id);

                services.AddSingleton(config);
            }
        }

        return services;
    }

    public static IServiceCollection AddConsoleInput(this IServiceCollection services)
    {
        return services.AddHostedService<ConsoleInputHostedService>();
    }

    internal static IServiceCollection AddRakNetServer(this IServiceCollection services)
    {
        services.AddHostedService<RakNetHostedService>();
        services.AddSingleton<IRakNetConnectionStore, RakNetConnectionStore>();
        services.AddSingleton<IRakNetSocket, RakNetSocket>();

        services.AddRekNetDefaultsPackets()
            .AddLogging();

        return services;
    }

    private static IServiceCollection AddRekNetDefaultsPackets(this IServiceCollection services)
    {
        services.AddSingleton<RakNetPacketMap>();

        services.AddPacket<AckPacket>();
        services.AddPacket<NackPacket>();

        services.AddPacket<ConnectedPing>();
        services.AddPacket<ConnectedPong>();
        services.AddPacket<ConnectionRequest>();


        return services;
    }
}