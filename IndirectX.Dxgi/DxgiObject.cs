using System.Diagnostics.CodeAnalysis;

namespace IndirectX.Dxgi;

public partial class DxgiObject
{
    public T GetParent<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>()
        where T : Unknown
        => Create<T>(GetParent(GetIid<T>()))!;
}
