using Kahrad.Pricing.BuildingBlocks.Infrastructure.Hosting;
using Kahrad.Pricing.Media.Worker;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostLogging(builder.Configuration, builder.Environment.ApplicationName);
builder.Services.AddWindowsService(options => options.ServiceName = "Kahrad Media Worker");
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
