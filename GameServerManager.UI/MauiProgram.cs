using CommunityToolkit.Maui;
using GameServerManager.BAL;
using GameServerManager.BAL.Services;
using GameServerManager.DAL.Services;
using GameServerManager.UI.Services;
using Microsoft.Extensions.Logging;
using MudBlazor;
using MudBlazor.Services;

namespace GameServerManager.UI
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            // Set default window size and add resizing handler
            Microsoft.Maui.Handlers.WindowHandler.Mapper.AppendToMapping(nameof(IWindow), (handler, view) =>
            {
#if WINDOWS
                        var nativeWindow = handler.PlatformView;
                        nativeWindow.Activate();
        
                        // Get the window handle
                        IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(nativeWindow);
                        var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
                        var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
        
                        // Set initial size
                        appWindow.Resize(new Windows.Graphics.SizeInt32(1920, 1080));
        
                        // Add a size changed event handler
                        appWindow.Changed += (sender, args) =>
                        {
                            if (args.DidSizeChange && appWindow.Size.Width > 1920)
                            {
                                // If width exceeds max, resize back to max
                                appWindow.Resize(new Windows.Graphics.SizeInt32(1920, appWindow.Size.Height));
                            }
                        };
#endif
            });


            builder.Services.AddMauiBlazorWebView();
            builder.Services.AddMudServices(config =>
            {
                config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.TopRight;
                config.SnackbarConfiguration.PreventDuplicates = false;
                config.SnackbarConfiguration.NewestOnTop = false;
                config.SnackbarConfiguration.ShowCloseIcon = true;
                config.SnackbarConfiguration.VisibleStateDuration = 10000;
                config.SnackbarConfiguration.HideTransitionDuration = 100;
                config.SnackbarConfiguration.ShowTransitionDuration = 100;
                config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
            });

            builder.Services.AddScoped<DialogService>();

            // Register the new database service
            builder.Services.AddSingleton<DatabaseService>(new DatabaseService(FileSystem.AppDataDirectory));
            
            // Register file picker service
            builder.Services.AddSingleton<IFilePickerService, FilePickerService>();
            
            // Keep the old Database class for backward compatibility if needed
            //builder.Services.AddSingleton(new Database(FileSystem.AppDataDirectory));
            builder.Services.AddSingleton<GameServerService>();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif

            var app = builder.Build();

            // Initialize the service locator with the database service
            var databaseService = app.Services.GetRequiredService<DatabaseService>();
            ServiceLocator.RegisterDatabaseService(databaseService);

            // Initialize the database (create tables if they don't exist)
            Task.Run(async () => await databaseService.Init());

            return app;
        }
    }
}
