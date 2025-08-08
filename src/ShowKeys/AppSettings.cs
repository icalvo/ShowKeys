using System;
using System.IO;
using System.Text.Json;
using ShowKeys.History;

namespace ShowKeys;

public static class AppSettings
{
    private static readonly string SettingsFilePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "ShowKeys",
        "settings.json");

    private static readonly JsonSerializerOptions JsonSerializerOptions = new() { WriteIndented = true };
    private static int _maxKeyHistoryEntries = KeyHistory.DefaultMaxEntries;
    private static int _fontSize;

    public static Action? OnMaxKeyHistoryEntriesChanged { get; set; }
    public static Action? OnFontSizeChanged { get; set; }

    public static int MaxKeyHistoryEntries
    {
        get => _maxKeyHistoryEntries;
        set
        {
            if (_maxKeyHistoryEntries == value) return;
            _maxKeyHistoryEntries = value;
            OnMaxKeyHistoryEntriesChanged?.Invoke();
        }
    }

    public static int FontSize
    {
        get => _fontSize;
        set
        {
            if (_fontSize == value) return;
            _fontSize = value;
            OnFontSizeChanged?.Invoke();
        }
    }

    public static void Load()
    {
        try
        {
            if (!File.Exists(SettingsFilePath)) return;
            var json = File.ReadAllText(SettingsFilePath);
            var settings = JsonSerializer.Deserialize<AppSettingsData>(json);
            if (settings != null)
            {
                MaxKeyHistoryEntries = settings.MaxKeyHistoryEntries;

                // Only set FontSize if it exists in the settings file
                if (settings.FontSize > 0)
                {
                    FontSize = settings.FontSize;
                }
            }
        }
        catch (Exception ex)
        {
            // Handle error (log or show message)
            Console.WriteLine($"Error loading settings: {ex.Message}");
        }
    }

    public static void Save()
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(SettingsFilePath)!);
            var settings = new AppSettingsData
            {
                MaxKeyHistoryEntries = MaxKeyHistoryEntries,
                FontSize = FontSize
            };
            var json = JsonSerializer.Serialize(settings, JsonSerializerOptions);
            File.WriteAllText(SettingsFilePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving settings: {ex.Message}");
        }
    }

    private class AppSettingsData
    {
        public int MaxKeyHistoryEntries { get; init; }
        public int FontSize { get; init; }
    }
}