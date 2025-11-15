using System;
using System.Collections.Generic;

namespace IndirectX.D3D11;

internal static class Utilities
{
    public static (T1, T2)[] Zip<T1, T2>(this (T1[], T2[]) source)
    {
        var (source1, source2) = source;
        var result = new (T1, T2)[source1.Length];
        for (var i = 0; i < source1.Length; i++)
            result[i] = (source1[i], source2[i]);
        return result;
    }

    public static (T1, T2, T3)[] Zip<T1, T2, T3>(this (T1[], T2[], T3[]) source)
    {
        var (source1, source2, source3) = source;
        var result = new (T1, T2, T3)[source1.Length];
        for (var i = 0; i < source1.Length; i++)
            result[i] = (source1[i], source2[i], source3[i]);
        return result;
    }

    public static (T1[], T2[]) Unzip<T1, T2>(this (T1, T2)[] source)
    {
        var result1 = new T1[source.Length];
        var result2 = new T2[source.Length];
        for (var i = 0; i < source.Length; i++)
            (result1[i], result2[i]) = source[i];
        return (result1, result2);
    }

    public static (T1[], T2[], T3[]) Unzip<T1, T2, T3>(this (T1, T2, T3)[] source)
    {
        var result1 = new T1[source.Length];
        var result2 = new T2[source.Length];
        var result3 = new T3[source.Length];
        for (var i = 0; i < source.Length; i++)
            (result1[i], result2[i], result3[i]) = source[i];
        return (result1, result2, result3);
    }
}
