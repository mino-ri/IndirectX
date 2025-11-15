using System;

namespace IndirectX.Dxgi;

[Flags]
public enum WindowAssociationFlags
{
    None = 0,
    IgnoreAll = 1,
    IgnoreAltEnter = 2,
    IgnorePrintScreen = 4,
    Valid = 7,
}
