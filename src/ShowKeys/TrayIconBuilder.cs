using System;
using System.Drawing;
using System.Windows.Forms;
using Forms = System.Windows.Forms;

namespace ShowKeys;

public static class TrayIconBuilder
{
    public static NotifyIcon CreateTrayIcon()
    {
        var iconStream = System.Windows.Application.GetResourceStream(
                new Uri("pack://application:,,,/ShowKeys;component/Resources/ShowKeysIcon.ico"))
            ?.Stream ?? throw new InvalidOperationException("Icon not found");

        var trayIcon = new NotifyIcon
        {
            Icon = new Icon(iconStream),
            Visible = true,
            Text = "ShowKeys",
            ContextMenuStrip = CreateTrayIconContextMenu()
        };


        trayIcon.MouseClick += (_, args) => 
        {
            if (args.Button == MouseButtons.Left)
            {
                trayIcon.ContextMenuStrip.Show(Cursor.Position);
            }
        };
        return trayIcon;
    }

    private static ContextMenuStrip CreateTrayIconContextMenu()
    {
        var contextMenu = new ContextMenuStrip();

        var settingsItem = new ToolStripMenuItem("Settings");
        settingsItem.Click += (_, _) => OpenSettingsWindow();
        contextMenu.Items.Add(settingsItem);

        contextMenu.Items.Add(new ToolStripSeparator());

        var exitItem = new ToolStripMenuItem("Exit");
        exitItem.Click += (_, _) => System.Windows.Application.Current.Shutdown();
        contextMenu.Items.Add(exitItem);
        return contextMenu;
    }

    private static void OpenSettingsWindow()
    {
        var settingsWindow = new SettingsWindow(AppSettings.MaxKeyHistoryEntries);
        if (settingsWindow.ShowDialog() != true) return;
        AppSettings.MaxKeyHistoryEntries = settingsWindow.MaxEntries;
        AppSettings.Save();
    }
}