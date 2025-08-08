using System;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace ShowKeys.History;

public class KeyHistory
{
    public const int DefaultMaxEntries = 15;
    private static readonly Stopwatch PaddingStopwatch = new();
    public ObservableCollection<KeyHistoryEntry> History { get; } = [];
    private int _maxEntries = DefaultMaxEntries;
    private int _keyComboCount;

    public int MaxEntries
    {
        get => _maxEntries;
        set
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(value, 1, nameof(value));
            _maxEntries = value;
            // Trim existing entries if needed
            while (_keyComboCount > _maxEntries)
            {
                if (History[0] is KeyComboEntry)
                    _keyComboCount--;
                History.RemoveAt(0);
            }
            if (History.Count > 0 && History[0] is TimePaddingEntry)
                History.RemoveAt(0);
        }
    }

    public void Add(string keyCombo)
    {
        if (PaddingStopwatch.Elapsed > TimeSpan.FromMilliseconds(1500))
        {
            History.Add(new TimePaddingEntry());
        }

        // Add appropriate entry based on whether keyCombo was successfully determined
        History.Add(new KeyComboEntry(keyCombo));
        PaddingStopwatch.Restart();

        if (_keyComboCount < MaxEntries)
        {
            _keyComboCount++;
            return;
        }

        // Remove oldest entry and following time padding if exists 
        History.RemoveAt(0);
        if (History[0] is TimePaddingEntry)
            History.RemoveAt(0);
    }
}