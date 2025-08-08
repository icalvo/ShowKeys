using System;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using ShowKeys.History;

namespace ShowKeys;

public partial class MainWindow
{
    private readonly NotifyIcon _trayIcon;

    public MainWindow()
    {
        InitializeComponent();
        var keyHistory = new KeyHistory();
        AppSettings.OnMaxKeyHistoryEntriesChanged = 
            () => keyHistory.MaxEntries = AppSettings.MaxKeyHistoryEntries;
        AppSettings.OnFontSizeChanged = UpdateFontSize;
        AppSettings.Load();
        KeyHistoryList.ItemsSource = keyHistory.History;

        // Prevent the window from getting focus when started
        ShowActivated = false;

        PositionWindow();
        LowLevelKeyboardHooks.SetKeyboardHook(
            KeyComboBuilder.HandleKeyDown,
            KeyComboBuilder.HandleKeyUp,
            keyCombo => Dispatcher.Invoke(() => keyHistory.Add(keyCombo)));

        _trayIcon = TrayIconBuilder.CreateTrayIcon();
    }

    private void PositionWindow()
    {
        var workingArea = SystemParameters.WorkArea;

        // Set width to screen width
        Left = workingArea.Left;
        Width = workingArea.Width;

        Height = AppSettings.FontSize * 2;

        // Update layout to ensure size calculations are accurate
        UpdateLayout();
        // Position at bottom of screen
        Top = workingArea.Bottom - Height;
    }
    private void UpdateFontSize()
    {
        // This needs to be done in code since we're using a template selector
        // and can't easily bind the font size in XAML

        Resources["KeyTextFontSize"] = (double)AppSettings.FontSize;

        // Update position since size has changed
        PositionWindow();
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