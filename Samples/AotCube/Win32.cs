using System.Runtime.InteropServices;

namespace AotCube;

internal static partial class Win32
{
    public const int CwUserDefault = unchecked((int)0x80000000);

    [LibraryImport("kernel32.dll", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    internal static partial nint GetModuleHandleW(string? lpModuleName);

    [LibraryImport("user32.dll", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    internal static partial ushort RegisterClassExW(in WndClassEx lpwcx);

    [LibraryImport("user32.dll", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    internal static partial nint LoadImageW(nint hinst, string lpszName, uint uType, int cxDesired, int cyDesired, uint fuLoad);

    [LibraryImport("user32.dll", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    internal static partial nint CreateWindowExW(
    WindowStyleEx dwExStyle,
    string? pszClassName,
    string? pszWindowName,
    WindowStyle dwStype,
    int x,
    int y,
    int nWidth,
    int nHeight,
    nint hWndParent,
    nint hMenu,
    nint hInstance,
    nint pParam);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool ShowWindow(nint hWnd, int nCmdShow);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool UpdateWindow(nint hWnd);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool GetMessageW(out Msg lpMsg, nint hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

    [LibraryImport("user32.dll")]
    internal static partial nint DispatchMessageW(in Msg lpMsg);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool TranslateMessage(in Msg lpMsg);

    [LibraryImport("user32.dll")]
    internal static partial void PostQuitMessage(int nExitCode);

    [LibraryImport("user32.dll")]
    internal static partial nint DefWindowProcW(nint hWnd, WindowMessage uMsg, nint wParam, nint lParam);

    [LibraryImport("gdi32.dll")]
    internal static partial nint GetStockObject(int fnObject);
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct WndClassEx
{
    public uint Size;
    public ClassStyle Style;
    public delegate* unmanaged[Stdcall]<nint, WindowMessage, nint, nint, nint> WndProc;
    public int ClsExtra;
    public int WndExtra;
    public nint Instance;
    public nint Icon;
    public nint Cursor;
    public nint Background;
    public nint MenuName;
    public nint ClassName;
    public nint IconSm;
}

[Flags]
public enum ClassStyle : uint
{
    None = 0,
    VREDRAW = 0x0001,
    HREDRAW = 0x0002,
}


[StructLayout(LayoutKind.Sequential)]
public struct Msg
{
    public nint Hwnd;
    public WindowMessage Message;
    public nint WParam;
    public nint LParam;
    public uint Time;
    public Point Pt;
    public uint LPrivate;
}

[StructLayout(LayoutKind.Sequential)]
public struct Point(int x, int y)
{
    public int X = x;
    public int Y = y;
}

[Flags]
public enum WindowStyleEx : uint
{
    None = 0,
    WindowEdge = 0x00000100,
    ClientEdge = 0x00000200,
    AppWindow = 0x00040000,
    OverlappedWindow = WindowEdge | ClientEdge,
}

[Flags]
public enum WindowStyle : uint
{
    Overlapped = 0x00000000,
    TabStop = 0x00010000,
    MaximizeBox = 0x00010000,
    Group = 0x00020000,
    MinimizeBox = 0x00020000,
    ThickFrame = 0x00040000,
    SysMenu = 0x00080000,
    HScroll = 0x00100000,
    VScroll = 0x00200000,
    DlgFrame = 0x00400000,
    Border = 0x00800000,
    Caption = 0x00C00000,
    Maximize = 0x01000000,
    ClipChildren = 0x02000000,
    ClipSiblings = 0x04000000,
    Disabled = 0x08000000,
    Minimize = 0x20000000,
    Child = 0x40000000,
    Popup = 0x80000000,
    Visible = 0x10000000,
    OverlappedWindow = Border | Caption | SysMenu | ThickFrame | MinimizeBox | MaximizeBox,
}

public enum WindowMessage : uint
{
    Null = 0x0000,
    Create = 0x0001,
    Destroy = 0x0002,
    Move = 0x0003,
    Size = 0x0005,
    Enable = 0x000A,
    Close = 0x0010,
    Quit = 0x0012,
    QueryOpen = 0x0013,
    ShowWindow = 0x0018,
    ActivateApp = 0x001C,
    CancelMade = 0x001F,
    ChildActive = 0x0022,
    GetMinMaxInfo = 0x0024,
    QueryDragIcon = 0x0037,
    Compacting = 0x0041,
    WindowPosChanging = 0x0046,
    WindowPosChanged = 0x0047,
    InputLangChangeRequest = 0x0050,
    InputLangChange = 0x0051,
    UserChanged = 0x0054,
    StyleChanging = 0x007C,
    StyleChanged = 0x007D,
    GetIcon = 0x007F,
    NcCreate = 0x0081,
    NcDestroy = 0x0082,
    NcCalcSize = 0x0083,
    NcActivate = 0x0086,
    Sizing = 0x0214,
    Moving = 0x0216,
    EnterSizeMove = 0x0231,
    ExistSizeMove = 0x0232,
    ThemeChanged = 0x031A,
    Mask = 0xFFFF,
}

[StructLayout(LayoutKind.Sequential)]
public readonly ref struct InteropString
{
    public readonly nint NativePtr;

    public InteropString(int length) => NativePtr = Marshal.AllocHGlobal(length);

    public InteropString(nint length) => NativePtr = Marshal.AllocHGlobal((int)length);

    public InteropString(string? source) => NativePtr = Marshal.StringToHGlobalUni(source);

    public new string ToString() => Marshal.PtrToStringAnsi(NativePtr)!;

    public void Dispose() => Marshal.FreeHGlobal(NativePtr);

    public static implicit operator nint(InteropString str) => str.NativePtr;
}
