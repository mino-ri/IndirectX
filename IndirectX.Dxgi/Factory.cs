using System;
using System.Diagnostics.CodeAnalysis;
using IndirectX.Interop;

namespace IndirectX.Dxgi;

public partial class Factory
{
    public static T Create<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>()
        where T : Unknown
        => Create<T>(WinApis.CreateDXGIFactory(GetIid<T>()))!;

    public Factory() : base(WinApis.CreateDXGIFactory(GetIid<Factory>())) { }
}

public partial class Factory1
{
    public static new T Create<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>()
        where T : Unknown
        => Create<T>(WinApis.CreateDXGIFactory1(GetIid<T>()))!;

    public Factory1() : base(WinApis.CreateDXGIFactory1(GetIid<Factory1>())) { }
}
