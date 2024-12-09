using DesktopCleaner.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using ConfigurationProvider = DesktopCleaner.Providers.ConfigurationProvider;
using SLog = Serilog.Log;

SLog.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var serviceProvider = new ServiceCollection()
    .AddSingleton(ConfigurationProvider.GetConfiguration())
    .AddScoped<IDesktopCleaner, DesktopCleaner.Services.DesktopCleaner>()
    .AddLogging(loggingBuilder =>
    {
        loggingBuilder.ClearProviders();
        loggingBuilder.AddSerilog(dispose: true);
    })
    .BuildServiceProvider();

var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Started");

var cleaner = serviceProvider.GetRequiredService<IDesktopCleaner>();
cleaner.Clean();
