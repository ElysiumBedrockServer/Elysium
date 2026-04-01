
using Elysium.Core.Configuration;
using Elysium.Server.Builder;
using Microsoft.Extensions.Configuration;

var builder = RakNetBuilder.CreateNetBuilder(args);

builder.Configuration.GetValue<ServerInfoConfiguration>("Server");

var app = builder.Build();

await app.RunAsync();


