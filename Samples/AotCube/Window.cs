using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AotCube;

internal sealed partial class Window
{
    private const string ClassName = "AOTCUBE";
    private static readonly nint ModuleHandle;
    private static readonly nint InitializeError;
    private static readonly Dictionary<nint, Window> Windows = [];

    public nint Handle { get; }
    public event WindowResizedHandler? Resized;

    static Window()
    {
        var instance = Win32.GetModuleHandleW(null);
        if (instance == 0)
        {
            InitializeError = Marshal.GetLastWin32Error();
            return;
        }

        if (InitApp(instance) == 0)
        {
            InitializeError = Marshal.GetLastWin32Error();
            return;
        }

        ModuleHandle = instance;
    }

    public Window(string title, int height, int width)
    {
        if (ModuleHandle == 0)
            throw new InvalidOperationException($"Failed to initialize window class. Code: {InitializeError}");

        Handle = Win32.CreateWindowExW(
            WindowStyleEx.OverlappedWindow,
            ClassName,
            title,
            WindowStyle.OverlappedWindow,
            Win32.CwUserDefault, Win32.CwUserDefault, width, height, 0, 0, ModuleHandle, 0);

        if (Handle == 0)
            throw new InvalidOperationException($"Failed to create window. Code: {Marshal.GetLastWin32Error()}");

        Windows.Add(Handle, this);
    }

    private static unsafe ushort InitApp(nint instance)
    {
        using var className = new InteropString(ClassName);
        return Win32.RegisterClassExW(new WndClassEx
        {
            Size = (uint)Marshal.SizeOf<WndClassEx>(),
            Style = ClassStyle.VREDRAW | ClassStyle.HREDRAW,
            WndProc = &WndProc,
            ClsExtra = 0,
            WndExtra = 0,
            Instance = instance,
            Icon = nint.Zero,
            Cursor = Win32.LoadImageW(0, "#32512", 2, 0, 0, 0x00000040 | 0x00008000),
            Background = nint.Zero,
            MenuName = nint.Zero,
            ClassName = className,
            IconSm = nint.Zero,
        });
    }

    public void Show()
    {
        Win32.ShowWindow(Handle, 1);
        Win32.UpdateWindow(Handle);
    }

    private void OnResized(int width, int height)
    {
        Resized?.Invoke(this, width, height);
    }

    public static void RunMessageLoop()
    {
        while (Win32.GetMessageW(out var msg, 0, 0, 0))
        {
            Debug.Print($"#{msg.Hwnd}, {msg.Message}, {msg.WParam:X}, {msg.LParam:X}, t{msg.Time}, ({msg.Pt.X}, {msg.Pt.Y}, {msg.LPrivate})");
            Win32.DispatchMessageW(in msg);
        }
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static nint WndProc(nint hWnd, WindowMessage message, nint wParam, nint lParam)
    {
        Debug.Print($"WndProc: #{hWnd:X}, {message & WindowMessage.Mask}, {wParam:X}, {lParam:X}");
        switch (message)
        {
            case WindowMessage.Destroy:
                Win32.PostQuitMessage(0);
                return 0;

            case WindowMessage.Size:
                var width = (int)(lParam & 0xFFFF);
                var height = (int)((lParam >> 16) & 0xFFFF);
                if (Windows.TryGetValue(hWnd, out var window))
                {
                    window.OnResized(width, height);
                }
                return Win32.DefWindowProcW(hWnd, message, wParam, lParam);

            default:
                return Win32.DefWindowProcW(hWnd, message, wParam, lParam);
        }
    }
}

internal delegate void WindowResizedHandler(Window sender, int width, int height);
