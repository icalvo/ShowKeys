using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;

namespace ShowKeys;

public partial class MainWindow
{
    private readonly LowLevelKeyboardProc _proc = HookCallback;
    private static IntPtr _hookId = IntPtr.Zero;
    private static MainWindow? _instance;

    private readonly ObservableCollection<KeyEntryItem> _keyHistory;
    private static bool _ctrlPressed;
    private static bool _altPressed;
    private static bool _shiftPressed;
    private static bool _winPressed;

    public MainWindow()
    {
        InitializeComponent();
        _instance = this;
        _keyHistory = new ObservableCollection<KeyEntryItem>();
        KeyHistoryList.ItemsSource = _keyHistory;

        // Prevent the window from getting focus when started
        ShowActivated = false;

        PositionWindow();
        _hookId = SetHook(_proc);
    }

    private void PositionWindow()
    {
        var workingArea = SystemParameters.WorkArea;
        var jediBarHeight = 80;

        Left = workingArea.Left;
        Top = workingArea.Bottom - jediBarHeight;
        Width = workingArea.Width;
        Height = jediBarHeight;
    }

    protected override void OnClosed(EventArgs e)
    {
        UnhookWindowsHookEx(_hookId);
        base.OnClosed(e);
    }

    private static IntPtr SetHook(LowLevelKeyboardProc proc)
    {
        using var currentProcess = Process.GetCurrentProcess();
        using var curModule = currentProcess.MainModule;
        Debug.Assert(curModule?.ModuleName != null, "curModule.ModuleName != null");
        return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
    }

    private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

    private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode < 0) return CallNextHookEx(_hookId, nCode, wParam, lParam);
        bool keyDown = wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN;
        bool keyUp = wParam == (IntPtr)WM_KEYUP || wParam == (IntPtr)WM_SYSKEYUP;

        if (!keyDown && !keyUp) return CallNextHookEx(_hookId, nCode, wParam, lParam);
        int vkCode = Marshal.ReadInt32(lParam);
        Keys key = (Keys)vkCode;

        if (keyDown)
        {
            HandleKeyDown(key);
        }
        else if (keyUp)
        {
            HandleKeyUp(key);
        }

        return CallNextHookEx(_hookId, nCode, wParam, lParam);
    }

    private static void HandleKeyDown(Keys key)
    {
        // Track modifier states
        switch (key)
        {
            case Keys.LControlKey:
            case Keys.RControlKey:
            case Keys.ControlKey:
                _ctrlPressed = true;
                return;
            case Keys.LMenu:
            case Keys.RMenu:
            case Keys.Menu:
                _altPressed = true;
                return;
            case Keys.LShiftKey:
            case Keys.RShiftKey:
            case Keys.ShiftKey:
                _shiftPressed = true;
                return;
            case Keys.LWin:
            case Keys.RWin:
                _winPressed = true;
                return;
        }

        string? keyCombo = BuildKeyCombo(key);
        // Process the key regardless of whether combo could be determined
        _instance?.Dispatcher.Invoke(() => _instance.AddKeyToHistory(keyCombo));
    }

    private static void HandleKeyUp(Keys key)
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

    private static bool IsSpecialKey(Keys key)
    {
        return 
           key is 
               Keys.Enter or 
               Keys.Tab or 
               Keys.Back or 
               Keys.Delete or 
               Keys.Escape or 
               Keys.Space or 
               Keys.Up or 
               Keys.Down or 
               Keys.Left or 
               Keys.Right or 
               Keys.Home or 
               Keys.End or 
               Keys.PageUp or 
               Keys.PageDown or 
               Keys.Insert or 
               Keys.PrintScreen or 
               Keys.Pause or 
               >= Keys.F1 and <= Keys.F24;
    }

    private static string? BuildKeyCombo(Keys key)
    {
        string combo = "";

        // Add modifiers
        if (_winPressed) combo += "WIN+";
        if (_ctrlPressed) combo += "CTRL+";
        if (_altPressed) combo += "ALT+";

        // Add the main key
        string? keyString = GetKeyString(key);
        if (!string.IsNullOrEmpty(keyString))
        {
            combo += keyString;
            return combo;
        }

        return null;
    }

    private static string? GetKeyString(Keys key)
    {
        Debug.Print($"Key: {key}");
        switch (key)
        {
            case Keys.Enter: return "Enter";
            case Keys.Tab: return "Tab";
            case Keys.Back: return "Backspace";
            case Keys.Delete: return "Delete";
            case Keys.Escape: return "Esc";
            case Keys.Space: return "Space";
            case Keys.Up: return "↑";
            case Keys.Down: return "↓";
            case Keys.Left: return "←";
            case Keys.Right: return "→";
            case Keys.Home: return "Home";
            case Keys.End: return "End";
            case Keys.PageUp: return "PgUp";
            case Keys.PageDown: return "PgDn";
            case Keys.Insert: return "Insert";
            case Keys.PrintScreen: return "PrtScr";
            case Keys.Pause: return "Pause";
            case Keys.CapsLock: return "CapsLock";
            case Keys.NumLock: return "NumLock";
            case Keys.Scroll: return "ScrollLock";
        }

        // Function keys
        if (key is >= Keys.F1 and <= Keys.F24)
        {
            return key.ToString();
        }

        // Number keys
        if (key is >= Keys.D0 and <= Keys.D9)
        {
            return ((char)('0' + (key - Keys.D0))).ToString();
        }

        // Numpad keys
        if (key is >= Keys.NumPad0 and <= Keys.NumPad9)
        {
            return "Num" + (char)('0' + (key - Keys.NumPad0));
        }

        // Letter keys
        if (key is >= Keys.A and <= Keys.Z)
        {
            char letter = (char)('A' + (key - Keys.A));
            // Show uppercase if shift is pressed, lowercase otherwise
            if (_shiftPressed || _altPressed || _ctrlPressed || _winPressed)
                return letter.ToString();
            return letter.ToString().ToLower();
        }

        // Special characters that Shift might affect
        switch (key)
        {
            case Keys.OemSemicolon: return _shiftPressed ? ":" : ";";
            case Keys.Oemplus: return _shiftPressed ? "+" : "=";
            case Keys.Oemcomma: return _shiftPressed ? "<" : ",";
            case Keys.OemMinus: return _shiftPressed ? "_" : "-";
            case Keys.OemPeriod: return _shiftPressed ? ">" : ".";
            case Keys.OemQuestion: return _shiftPressed ? "?" : "/";
            case Keys.Oemtilde: return _shiftPressed ? "~" : "`";
            case Keys.OemOpenBrackets: return _shiftPressed ? "{" : "[";
            case Keys.OemPipe: return _shiftPressed ? "|" : "\\";
            case Keys.OemCloseBrackets: return _shiftPressed ? "}" : "]";
            case Keys.OemQuotes: return _shiftPressed ? "\"" : "'";
            case Keys.OemBackslash: return _shiftPressed ? "|" : "\\";
        }

        // Numpad operators
        switch (key)
        {
            case Keys.Add: return "Num+";
            case Keys.Subtract: return "Num-";
            case Keys.Multiply: return "Num*";
            case Keys.Divide: return "Num/";
            case Keys.Decimal: return "Num.";
        }

        return null;
    }

    private static Stopwatch _stopwatch = new();
    private void AddKeyToHistory(string? keyCombo)
    {
        if (_stopwatch.Elapsed > TimeSpan.FromMilliseconds(1500))
        {
            _keyHistory.Add(new TimePaddingEntry());
        }

        // Add appropriate entry based on whether keyCombo was successfully determined
        if (string.IsNullOrEmpty(keyCombo))
        {
            _keyHistory.Add(new ErrorEntry());
        }
        else
        {
            _keyHistory.Add(new KeyCombinationEntry(keyCombo));
        }

        _stopwatch.Restart();

        // Keep only the last 15 entries
        while (_keyHistory.Count > 15)
        {
            _keyHistory.RemoveAt(0);
        }
    }

    // Windows API declarations
    // ReSharper disable InconsistentNaming
    private const int WH_KEYBOARD_LL = 13;
    private const int WM_KEYDOWN = 0x0100;
    private const int WM_KEYUP = 0x0101;
    private const int WM_SYSKEYDOWN = 0x0104;
    private const int WM_SYSKEYUP = 0x0105;
    // ReSharper restore InconsistentNaming

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook,
        LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
        IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);
}