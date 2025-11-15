using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace IndirectX.Helper.Wpf;

public partial class DirectXHost : HwndHost
{
    protected override HandleRef BuildWindowCore(HandleRef hwndParent)
    {
        var hwnd = CreateWindowExW(
            0,
            "STATIC",
            "",
            WsChild | WsVisible,
            0,
            0,
            (int)Width,
            (int)Height,
            hwndParent.Handle,
            nint.Zero,
            nint.Zero,
            0);

        return new HandleRef(this, hwnd);
    }

    protected override void DestroyWindowCore(HandleRef hwnd)
    {
        DestroyWindow(hwnd.Handle);
    }

    const uint WsChild = 0x40000000;
    const uint WsVisible = 0x10000000;

    [LibraryImport("user32.dll")]
    private static partial nint CreateWindowExW(
        uint dwExStyle,
        [MarshalAs(UnmanagedType.LPWStr)] string pszClassName,
        [MarshalAs(UnmanagedType.LPWStr)] string pszWindowName,
        uint dwStype,
        int x,
        int y,
        int nWidth,
        int nHeight,
        nint hWndParent,
        nint hMenu,
        nint hInstance,
        nuint pParam);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool DestroyWindow(nint hwnd);
}
