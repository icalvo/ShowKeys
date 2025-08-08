using System.Diagnostics;
using System.Windows.Forms;

namespace ShowKeys;

public static class KeyComboBuilder
{
    private static bool _ctrlPressed;
    private static bool _altPressed;
    private static bool _shiftPressed;
    private static bool _winPressed;

    public static string? HandleKeyDown(Keys key)
    {
        // Track modifier states
        switch (key)
        {
            case Keys.LControlKey:
            case Keys.RControlKey:
            case Keys.ControlKey:
                _ctrlPressed = true;
                return null;
            case Keys.LMenu:
            case Keys.RMenu:
            case Keys.Menu:
                _altPressed = true;
                return null;
            case Keys.LShiftKey:
            case Keys.RShiftKey:
            case Keys.ShiftKey:
                _shiftPressed = true;
                return null;
            case Keys.LWin:
            case Keys.RWin:
                _winPressed = true;
                return null;
        }

        return BuildKeyCombo(key);
    }

    public static void HandleKeyUp(Keys key)
    {
        // Reset modifier states
        switch (key)
        {
            case Keys.LControlKey:
            case Keys.RControlKey:
            case Keys.ControlKey:
                _ctrlPressed = false;
                break;
            case Keys.LMenu:
            case Keys.RMenu:
            case Keys.Menu:
                _altPressed = false;
                break;
            case Keys.LShiftKey:
            case Keys.RShiftKey:
            case Keys.ShiftKey:
                _shiftPressed = false;
                break;
            case Keys.LWin:
            case Keys.RWin:
                _winPressed = false;
                break;
        }
    }

    private static string BuildKeyCombo(Keys key)
    {
        string combo = "";

        // Add modifiers
        if (_winPressed) combo += "WIN+";
        if (_ctrlPressed) combo += "CTRL+";
        if (_altPressed) combo += "ALT+";

        // Add the main key
        string keyString = GetKeyString(key);
        combo += keyString;
        return combo;
    }

    private static string GetKeyString(Keys key)
    {
        Debug.Print($"Key: {key}");
        return key switch
        {
            Keys.Enter => "Enter",
            Keys.Tab => "Tab",
            Keys.Back => "Backspace",
            Keys.Delete => "Delete",
            Keys.Escape => "Esc",
            Keys.Space => "Space",
            Keys.Up => "↑",
            Keys.Down => "↓",
            Keys.Left => "←",
            Keys.Right => "→",
            Keys.Home => "Home",
            Keys.End => "End",
            Keys.PageUp => "PgUp",
            Keys.PageDown => "PgDn",
            Keys.Insert => "Insert",
            Keys.PrintScreen => "PrtScr",
            Keys.Pause => "Pause",
            Keys.CapsLock => "CapsLock",
            Keys.NumLock => "NumLock",
            Keys.Scroll => "ScrollLock",
            >= Keys.F1 and <= Keys.F24 => key.ToString(),
            >= Keys.D0 and <= Keys.D9 => ((char)('0' + (key - Keys.D0))).ToString(),
            >= Keys.NumPad0 and <= Keys.NumPad9 => "Num" + (char)('0' + (key - Keys.NumPad0)),
            >= Keys.A and <= Keys.Z => LetterString(),
            Keys.OemSemicolon => _shiftPressed ? ":" : ";",
            Keys.Oemplus => _shiftPressed ? "+" : "=",
            Keys.Oemcomma => _shiftPressed ? "<" : ",",
            Keys.OemMinus => _shiftPressed ? "_" : "-",
            Keys.OemPeriod => _shiftPressed ? ">" : ".",
            Keys.OemQuestion => _shiftPressed ? "?" : "/",
            Keys.Oemtilde => _shiftPressed ? "~" : "`",
            Keys.OemOpenBrackets => _shiftPressed ? "{" : "[",
            Keys.OemPipe => _shiftPressed ? "|" : "\\",
            Keys.OemCloseBrackets => _shiftPressed ? "}" : "]",
            Keys.OemQuotes => _shiftPressed ? "\"" : "'",
            Keys.OemBackslash => _shiftPressed ? "|" : "\\",
            Keys.Add => "Num+",
            Keys.Subtract => "Num-",
            Keys.Multiply => "Num*",
            Keys.Divide => "Num/",
            Keys.Decimal => "Num.",
            _ => key.ToString()
        };

        string LetterString()
        {
            char letter = (char)('A' + (key - Keys.A));
            // Show uppercase if shift is pressed, lowercase otherwise
            if (_shiftPressed || _altPressed || _ctrlPressed || _winPressed)
                return letter.ToString();
            return letter.ToString().ToLower();
        }
    }
}