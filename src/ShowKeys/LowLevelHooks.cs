using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ShowKeys;

public static partial class LowLevelHooks
{
    private static IntPtr _hookId = IntPtr.Zero;

    public static void SetKeyboardHook(
        Func<Keys, string?> keyDownCallback,
        Action<Keys> keyUpCallback,
        Action<string> keyComboCallback)
    {
        _hookId = SetHook(
            (code, param, lParam) => 
                HookCallback(
                    keyDownCallback,
                    keyUpCallback,
                    keyComboCallback, 
                    code, 
                    param, 
                    lParam));
    }

    public static void UnhookWindowsHook()
    {
        UnhookWindowsHookEx(_hookId);
    }

    private static IntPtr SetHook(LowLevelKeyboardProc proc)
    {
        using var currentProcess = Process.GetCurrentProcess();
        using var curModule = currentProcess.MainModule;
        Debug.Assert(curModule?.ModuleName != null, "curModule.ModuleName != null");
        return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
    }

    private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

    private static IntPtr HookCallback(
        Func<Keys, string?> keyDownCallback,
        Action<Keys> keyUpCallback,
        Action<string> keyComboCallback, int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode < 0) return CallNextHookEx(_hookId, nCode, wParam, lParam);
        bool keyDown = wParam is WM_KEYDOWN or WM_SYSKEYDOWN;
        bool keyUp = wParam is WM_KEYUP or WM_SYSKEYUP;

        if (!keyDown && !keyUp) return CallNextHookEx(_hookId, nCode, wParam, lParam);
        int vkCode = Marshal.ReadInt32(lParam);
        Keys key = (Keys)vkCode;

        if (keyDown)
        {
            var keyCombo = keyDownCallback(key);
            if (keyCombo is not null)
            {
                keyComboCallback(keyCombo);
            }
        }
        else if (keyUp)
        {
            keyUpCallback(key);
        }

        return CallNextHookEx(_hookId, nCode, wParam, lParam);
    }

    // ReSharper disable InconsistentNaming
    private const int WH_KEYBOARD_LL = 13;
    private const int WM_KEYDOWN = 0x0100;
    private const int WM_KEYUP = 0x0101;
    private const int WM_SYSKEYDOWN = 0x0104;
    private const int WM_SYSKEYUP = 0x0105;
    // ReSharper restore InconsistentNaming

    [LibraryImport("user32.dll", EntryPoint = "SetWindowsHookExW", SetLastError = true)]
    private static partial IntPtr SetWindowsHookEx(int idHook,
        LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

    [LibraryImport("user32.dll", EntryPoint = "UnhookWindowsHookEx", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool UnhookWindowsHookEx(IntPtr hhk);

    [LibraryImport("user32.dll", EntryPoint = "CallNextHookEx", SetLastError = true)]
    private static partial IntPtr CallNextHookEx(IntPtr hhk, int nCode,
        IntPtr wParam, IntPtr lParam);

    [LibraryImport("kernel32.dll", EntryPoint = "GetModuleHandleW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    private static partial IntPtr GetModuleHandle(string lpModuleName);
}