using CommunityToolkit.Maui;
using CourceWork.MVVM.ViewModels;
using CourceWork.MVVM.Views;
using CourceWork.Services;
using CourceWork.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using System.Reflection;

namespace CourceWork
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            SetupSerilog();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
            var logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .Enrich.FromLogContext()
                .CreateLogger();
            var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            builder.Logging.ClearProviders();
            builder.Logging.AddSerilog(dispose: true);

            builder.Services.AddSingleton<ConnectPageViewModel>();
            builder.Services.AddSingleton<ConnectPage>();

            builder.Services.AddSingleton<HomePageViewModel>();
            builder.Services.AddSingleton<HomePage>();

            builder.Services.AddSingleton<AxisControlViewModel>();
            builder.Services.AddSingleton<AxisControlPage>();

            builder.Services.AddSingleton<AppShell>();
            builder.Services.AddSingleton<AppShellViewModel>();

            builder.Services.AddSingleton<MaxiGrafService>();

            return builder.Build();
        }
        private static void SetupSerilog()
        {
            var file = Path.Combine(FileSystem.Current.AppDataDirectory, "Logs\\log.log");
            var flushInterval = new TimeSpan(0, 0, 1);
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.File(file, flushToDiskInterval: flushInterval, encoding: System.Text.Encoding.UTF8, rollingInterval: RollingInterval.Day, retainedFileCountLimit: 31)
                .Enrich.FromLogContext()
                .Destructure.ToMaximumDepth(4)
                .Destructure.ToMaximumStringLength(100)
                .Destructure.ToMaximumCollectionCount(10)
                .CreateLogger();
        }

    }
}
