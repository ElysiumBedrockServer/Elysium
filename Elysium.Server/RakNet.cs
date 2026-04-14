using Elysium.Core.Configuration;
using Elysium.Server.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Elysium.Server;

public static class RakNet
{
    public static HostApplicationBuilder CreateApplicationBuilder(string[] args)
    {
        var builder = new HostApplicationBuilder();

        builder.InitializeConfigure();

        return builder;
    }

    private static void InitializeConfigure(this HostApplicationBuilder builder)
    {
        builder.Services.AddLogging(builder => { builder.AddConsole(); }).AddRakNetServer();

        var env = builder.Environment;

        builder.Configuration
            .AddJsonFile("appsettings.json", true, true)
            .AddJsonFile($"appsettings.{env.ApplicationName}.json", true, true);

        builder.Services
            .Configure<ServerInfoConfiguration>(
                builder.Configuration.GetSection("Server"));
    }
}