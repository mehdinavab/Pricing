using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Xunit;

namespace Kahrad.Pricing.Hosts.IntegrationTests;

public sealed class HostLoggingTests
{
    [Theory]
    [InlineData("Kahrad.Pricing.Api")]
    [InlineData("Kahrad.Pricing.PriceInquiry.Api")]
    [InlineData("Kahrad.Pricing.Worker")]
    [InlineData("Kahrad.Pricing.Media.Worker")]
    public void Host_configuration_loads_and_filters_rendered_messages(string host)
    {
        var root = new DirectoryInfo(AppContext.BaseDirectory);
        while (root is not null && !File.Exists(Path.Combine(root.FullName, "Kahrad.Pricing.slnx")))
            root = root.Parent;
        var configuration = new ConfigurationBuilder()
            .AddJsonFile(Path.Combine(root!.FullName, "Src", "Hosts", host, "appsettings.json"))
            .Build();
        var sink = new CaptureSink();
        using var logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .WriteTo.Sink(sink)
            .CreateLogger();

        logger.Information("Processing {Document}", "OpenAPI");
        logger.Information("Processing {Document}", "JSON Schema");
        logger.Information("Processing {Document}", "apidom");
        logger.ForContext("SourceContext", "Microsoft.AspNetCore.Mvc.Infrastructure.ActionInvoker")
            .Information("Executing action");
        logger.Information("Pricing completed for {BrandId}", 1);

        var entry = Assert.Single(sink.Events);
        Assert.Equal(1, ((ScalarValue)entry.Properties["BrandId"]).Value);
        Assert.Contains("MachineName", entry.Properties.Keys);
        Assert.Contains("EnvironmentName", entry.Properties.Keys);
        Assert.Contains("ProcessId", entry.Properties.Keys);
        Assert.Contains("ThreadId", entry.Properties.Keys);
    }

    private sealed class CaptureSink : ILogEventSink
    {
        public List<LogEvent> Events { get; } = [];
        public void Emit(LogEvent logEvent) => Events.Add(logEvent);
    }
}
