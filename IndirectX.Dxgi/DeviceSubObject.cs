using System.Diagnostics.CodeAnalysis;
using IndirectX.Interop;

namespace IndirectX.Dxgi;

public partial class DeviceSubObject
{
    public T GetDevice<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>()
        where T : Unknown
        => Create<T>(GetDevice(GetIid<T>()))!;
}
