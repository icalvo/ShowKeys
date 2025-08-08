using System;
using System.Threading;
using System.Windows;

namespace ShowKeys;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
    private static readonly Mutex Mutex = new(false, "ShowKeysInstance");
    private bool _mutexAcquired;

    protected override void OnStartup(StartupEventArgs e)
    {
        // Check if another instance is already running
        if (!Mutex.WaitOne(TimeSpan.Zero, false))
        {
            // Another instance is already running
            Shutdown();
            return;
        }

        _mutexAcquired = true;
        base.OnStartup(e);
    }

    protected override void OnExit(ExitEventArgs e)
    {
        // Only release the mutex if we actually acquired it
        if (_mutexAcquired)
        {
            Mutex.ReleaseMutex();
        }
        
        base.OnExit(e);
    }
}