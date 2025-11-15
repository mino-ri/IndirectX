using System.Numerics;
using System.Runtime.InteropServices;
using IndirectX;

namespace SimpleImage;

/// <summary>GraphicsSharpにおける頂点を表す構造体。</summary>
[StructLayout(LayoutKind.Sequential)]
public struct Vertex
{
    /// <summary>4次元上の座標。</summary>
    public Vector4 Vector;

    /// <summary>色。</summary>
    public Color Color;

    public Vector2 Texture;

    /// <summary>Vertex構造体の新しいインスタンスを生成します。</summary>
    /// <param name="vector">頂点座標。</param>
    /// <param name="color">色。</param>
    public Vertex(Vector4 vector, Color color, Vector2 texture)
    {
        Vector = vector;
        Color = color;
        Texture = texture;
    }

    /// <summary>Vertex構造体の新しいインスタンスを生成します。</summary>
    /// <param name="x">X座標。</param>
    /// <param name="y">Y座標。</param>
    /// <param name="z">Z座標。</param>
    /// <param name="w">W座標。</param>
    /// <param name="color">色。</param>
    public Vertex(float x, float y, float z, float w, Color color, Vector2 texture)
    {
        Vector = new Vector4(x, y, z, w);
        Color = color;
        Texture = texture;
    }

    /// <summary>インスタンスを、それと等価な文字列に変換します。</summary>
    public override readonly string ToString()
    {
        return $"({Vector.X:F3}, {Vector.Y:F3}, {Vector.Z:F3}, {Vector.W:F3})#{(int)(Color.A * 255f):x2}{(int)(Color.R * 255f):x2}{(int)(Color.G * 255f):x2}{(int)(Color.B * 255f):x2}";
    }
}
