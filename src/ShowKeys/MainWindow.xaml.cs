using System;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using ShowKeys.History;

namespace ShowKeys;

public partial class MainWindow
{
    private readonly KeyHistory _keyHistory;
    private readonly NotifyIcon _trayIcon;

    public MainWindow()
    {
        InitializeComponent();
        _keyHistory = new KeyHistory();
        AppSettings.OnMaxKeyHistoryEntriesChanged = () => _keyHistory.MaxEntries = AppSettings.MaxKeyHistoryEntries;
        AppSettings.Load();
        KeyHistoryList.ItemsSource = _keyHistory.History;

        // Prevent the window from getting focus when started
        ShowActivated = false;

        PositionWindow();
        LowLevelKeyboardHooks.SetKeyboardHook(
            KeyComboBuilder.HandleKeyDown,
            KeyComboBuilder.HandleKeyUp,
            keyCombo => Dispatcher.Invoke(() => _keyHistory.Add(keyCombo)));

        _trayIcon = TrayIconBuilder.CreateTrayIcon();
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

        LowLevelKeyboardHooks.UnhookWindowsHook();
        AppSettings.Save();
        base.OnClosed(e);
    }
}