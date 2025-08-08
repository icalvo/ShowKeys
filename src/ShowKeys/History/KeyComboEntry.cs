namespace ShowKeys.History;

// Represents a keyboard combination (e.g. Ctrl+C)
public class KeyComboEntry : KeyHistoryEntry
{
    public KeyComboEntry(string keyCombo)
    {
        KeyCombo = keyCombo;
    }

    public string KeyCombo { get; }

    public override string ToString() => KeyCombo;
}