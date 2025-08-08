using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using Forms = System.Windows.Forms;

namespace ShowKeys;

public partial class MainWindow
{
    private readonly ObservableCollection<KeyEntryItem> _keyHistory;
    private readonly NotifyIcon _trayIcon;

    public MainWindow()
    {
        InitializeComponent();
        _keyHistory = new ObservableCollection<KeyEntryItem>();
        KeyHistoryList.ItemsSource = _keyHistory;

        // Prevent the window from getting focus when started
        ShowActivated = false;

        PositionWindow();
        LowLevelHooks.SetKeyboardHook(
            KeysLogic.HandleKeyDown,
            KeysLogic.HandleKeyUp,
            keyCombo => Dispatcher.Invoke(() => AddKeyToHistory(keyCombo)));

        // Initialize system tray icon
        var iconStream = System.Windows.Application.GetResourceStream(
                new Uri("pack://application:,,,/ShowKeys;component/Resources/ShowKeysIcon.ico"))
            ?.Stream ?? throw new InvalidOperationException("Icon not found");
        _trayIcon = new NotifyIcon
        {
            Icon = new Icon(iconStream),
            Visible = true,
            Text = "ShowKeys"
        };

        // Create context menu for the tray icon
        var contextMenu = new ContextMenuStrip();

        var settingsItem = new ToolStripMenuItem("Settings");
        settingsItem.Click += (_, _) => OpenSettingsWindow();
        contextMenu.Items.Add(settingsItem);

        contextMenu.Items.Add(new ToolStripSeparator());

        var exitItem = new ToolStripMenuItem("Exit");
        exitItem.Click += (_, _) => System.Windows.Application.Current.Shutdown();
        contextMenu.Items.Add(exitItem);

        _trayIcon.ContextMenuStrip = contextMenu;
        _trayIcon.MouseClick += (_, args) => 
        {
            if (args.Button == MouseButtons.Left)
            {
                _trayIcon.ContextMenuStrip.Show(Forms.Cursor.Position);
            }
        };
    }

    private void PositionWindow()
    {
        var workingArea = SystemParameters.WorkArea;
        var showKeysBarHeight = 80;

        Left = workingArea.Left;
        Top = workingArea.Bottom - showKeysBarHeight;
        Width = workingArea.Width;
        Height = showKeysBarHeight;
    }

    protected override void OnClosed(EventArgs e)
    {
        // Clean up tray icon when window is closed
        _trayIcon.Visible = false;
        _trayIcon.Dispose();

        LowLevelHooks.UnhookWindowsHook();
        base.OnClosed(e);
    }

    private void OpenSettingsWindow()
    {
        var settingsWindow = new SettingsWindow(AppSettings.MaxKeyHistoryEntries);
        if (settingsWindow.ShowDialog() == true)
        {
            AppSettings.MaxKeyHistoryEntries = settingsWindow.MaxEntries;

            // Trim existing entries if needed
            while (_keyHistory.Count > AppSettings.MaxKeyHistoryEntries)
            {
                _keyHistory.RemoveAt(0);
            }
        }
    }

    private static readonly Stopwatch PaddingStopwatch = new();
    private void AddKeyToHistory(string keyCombo)
    {
        if (PaddingStopwatch.Elapsed > TimeSpan.FromMilliseconds(1500))
        {
            _keyHistory.Add(new TimePaddingEntry());
        }

        // Add appropriate entry based on whether keyCombo was successfully determined
        _keyHistory.Add(new KeyCombinationEntry(keyCombo));

        PaddingStopwatch.Restart();

        // Keep only the last N entries as specified in settings
        while (_keyHistory.Count > AppSettings.MaxKeyHistoryEntries)
        {
            _keyHistory.RemoveAt(0);
        }
    }
}