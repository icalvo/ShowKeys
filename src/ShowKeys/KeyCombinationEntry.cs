namespace ShowKeys
{
    // Represents a keyboard combination (e.g. Ctrl+C)
    public class KeyCombinationEntry : KeyEntryItem
    {
        public KeyCombinationEntry(string keyCombo)
        {
            Text = keyCombo;
        }

        public string Text { get; }

        public override string ToString() => Text;
    }
}
