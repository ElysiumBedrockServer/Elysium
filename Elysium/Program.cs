using Elysium.RakNet.Store;
using Elysium.Server;
using Elysium.Server.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = RakNet.CreateApplicationBuilder(args);

builder.Services.AddRakNetPackets()
    .AddConsoleInput();

var app = builder.Build();

var value = app.Services.GetRequiredService<IRakNetConnectionStore>();

await app.RunAsync();