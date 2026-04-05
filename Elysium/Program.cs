using Elysium.Server;
using Elysium.Server.Extensions;
using Microsoft.Extensions.Hosting;

var builder = RakNet.CreateApplicationBuilder(args);

builder.Services.AddConsoleInput();

var app = builder.Build();

await app.RunAsync();