using System.Diagnostics.CodeAnalysis;

namespace IndirectX.Dxgi;

public partial class Surface1
{
    public T GetDC<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>(bool discard)
        where T : Unknown
        => Create<T>(GetDC(discard))!;
}
