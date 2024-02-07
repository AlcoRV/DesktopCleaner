using Microsoft.Extensions.DependencyInjection;
using ConfigurationProvider = DesktopCleaner.Providers.ConfigurationProvider;

var serviceProvider = new ServiceCollection()
    .AddSingleton(new ConfigurationProvider().GetConfiguration())
    .AddScoped<DesktopCleaner.Services.DesktopCleaner>()
    .BuildServiceProvider();

var cleaner = serviceProvider.GetRequiredService<DesktopCleaner.Services.DesktopCleaner>();
cleaner.Clean();
