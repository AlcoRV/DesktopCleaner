using DesktopCleaner.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using ConfigurationProvider = DesktopCleaner.Providers.ConfigurationProvider;

var serviceProvider = new ServiceCollection()
    .AddSingleton(new ConfigurationProvider().GetConfiguration())
    .AddScoped<IDesktopCleaner, DesktopCleaner.Services.DesktopCleaner>()
    .BuildServiceProvider();

var cleaner = serviceProvider.GetRequiredService<IDesktopCleaner>();
cleaner.Clean();
