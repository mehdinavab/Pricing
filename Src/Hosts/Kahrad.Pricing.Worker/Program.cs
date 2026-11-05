using Kahrad.Pricing.BuildingBlocks.Infrastructure.Hosting;
using Hangfire;
using Kahrad.Pricing.Sms.Application;
using Kahrad.Pricing.Sms.Application.Abstractions.Services;
using Kahrad.Pricing.Sms.Infrastructure;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddWindowsService(options => options.ServiceName = "Kahrad Pricing Worker");
builder.Services.AddHostLogging(builder.Configuration, builder.Environment.ApplicationName);
builder.Services.AddSmsApplication();
builder.Services.AddSmsInfrastructure(builder.Configuration);
var connection = builder.Configuration.GetConnectionString("HangfireDb")
    ?? builder.Configuration.GetConnectionString("SmsDb")
    ?? builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Configure the worker SQL Server connection string.");
builder.Services.AddHangfire(config => config.UseSqlServerStorage(connection));
builder.Services.AddHangfireServer();

var host = builder.Build();
host.Services.GetRequiredService<IRecurringJobManager>().AddOrUpdate<ISmsOutboxProcessor>(
    "sms-outbox", processor => processor.ProcessPendingOutboxAsync(CancellationToken.None),
    "*/2 * * * *", new RecurringJobOptions { TimeZone = TimeZoneInfo.Utc });
host.Run();
