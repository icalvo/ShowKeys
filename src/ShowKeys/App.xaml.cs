using System;
using System.Threading;
using System.Windows;

namespace ShowKeys;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
    private static readonly Mutex Mutex = new Mutex(false, "KeyboardJediShowKeysInstance");

    protected override void OnStartup(StartupEventArgs e)
    {
        // Check if another instance is already running
        if (!Mutex.WaitOne(TimeSpan.Zero, false))
        {
            // Another instance is already running
            MessageBox.Show("ShowKeys is already running.", "ShowKeys", MessageBoxButton.OK, MessageBoxImage.Information);
            Shutdown();
            return;
        }

        base.OnStartup(e);
    }

    protected override void OnExit(ExitEventArgs e)
    {
        // Release the mutex when the application exits
        Mutex.ReleaseMutex();
        base.OnExit(e);
    }
}