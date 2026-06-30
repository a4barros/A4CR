using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace GUI
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var startupFiles = StartupArgs.GetFilePaths(Environment.GetCommandLineArgs().Skip(1));
                desktop.MainWindow = new MainWindow(startupFiles);
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}