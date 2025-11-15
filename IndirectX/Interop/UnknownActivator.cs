using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace IndirectX.Interop;

static class UnknownActivator<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>
    where T : Unknown
{
    static readonly Func<ComPtr, bool, T?> Ctor;

    static UnknownActivator()
    {
        if (typeof(T).GetConstructor([typeof(ComPtr), typeof(bool)]) is { } ctor)
        {
            var comPtr = Expression.Parameter(typeof(ComPtr), "comPtr");
            var addRef = Expression.Parameter(typeof(bool), "addRef");
            Ctor = Expression.Lambda<Func<ComPtr, bool, T?>>(
                Expression.New(ctor, comPtr, addRef),
                comPtr,
                addRef).Compile();
        }
        else
        {
            Ctor = (_, _) => default;
        }
    }

    public static T? CreateInstance(ComPtr nativePtr, bool addRef = false) => Ctor(nativePtr, addRef);
}
