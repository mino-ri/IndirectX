using System.Diagnostics.CodeAnalysis;

namespace IndirectX.Dxgi;

public partial class SwapChain
{
    public T GetBuffer<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>(int buffer)
        where T : Unknown
        => Create<T>(GetBuffer(buffer, GetIid<T>()))!;

    public unsafe bool TryPresent(int syncInterval, PresentFlags flags)
    {
        return VtblRef.Present(ComPtr, syncInterval, flags) == HResult.Ok;
    }
}
