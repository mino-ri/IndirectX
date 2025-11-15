using System.Runtime.CompilerServices;

namespace IndirectX;

[InlineArray(4)]
public struct Int4
{
    private int _value;

    public Int4(ReadOnlySpan<int> span)
    {
        span.CopyTo(this);
    }
}

[InlineArray(4)]
public struct Float4
{
    private float _value;

    public Float4(ReadOnlySpan<float> span)
    {
        span.CopyTo(this);
    }
}

[InlineArray(8)]
public struct Float8
{
    private float _value;

    public Float8(ReadOnlySpan<float> span)
    {
        span.CopyTo(this);
    }
}

[InlineArray(16)]
public struct Float16
{
    private float _value;

    public Float16(ReadOnlySpan<float> span)
    {
        span.CopyTo(this);
    }
}

[InlineArray(1025)]
public struct Float1025
{
    private float _value;

    public Float1025(ReadOnlySpan<float> span)
    {
        span.CopyTo(this);
    }
}

[InlineArray(2)]
public struct Float2x8
{
    private Float8 _value;

    public Float2x8(ReadOnlySpan<Float8> span)
    {
        span.CopyTo(this);
    }
}

[InlineArray(2)]
public struct Float2x16
{
    private Float16 _value;

    public Float2x16(ReadOnlySpan<Float16> span)
    {
        span.CopyTo(this);
    }
}

[InlineArray(32)]
public struct Char32
{
    private char _value;

    public static implicit operator string(in Char32 char32) => new(char32);
    public static implicit operator Char32(string str)
    {
        var result = default(Char32);
        str.AsSpan().CopyTo(result);
        return result;
    }

    public static void FromString(string str, out Char32 char32)
    {
        char32 = default;
        str.AsSpan().CopyTo(char32);
    }
}

[InlineArray(128)]
public struct Char128
{
    private char _value;

    public static implicit operator string(in Char128 char32) => new(char32);
    public static implicit operator Char128(string str)
    {
        var result = default(Char128);
        str.AsSpan().CopyTo(result);
        return result;
    }

    public static void FromString(string str, out Char128 char128)
    {
        char128 = default;
        str.AsSpan().CopyTo(char128);
    }
}
