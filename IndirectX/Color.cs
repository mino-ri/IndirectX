using System.Runtime.InteropServices;

namespace IndirectX;

/// <summary>IndirectXにおけるfloat×4カラーを表します。この構造体は不変です。</summary>
[StructLayout(LayoutKind.Sequential)]
public struct Color
{
    /// <summary>色の赤成分。</summary>
    public float R;

    /// <summary>色の緑成分。</summary>
    public float G;

    /// <summary>色の青成分。</summary>
    public float B;

    /// <summary>色のα成分。</summary>
    public float A;

    /// <summary>各色の成分から、Color 構造体を生成します。</summary>
    /// <param name="a">色のα成分。</param>
    /// <param name="r">色の赤成分。</param>
    /// <param name="g">色の緑成分。</param>
    /// <param name="b">色の青成分。</param>
    public Color(float a, float r, float g, float b)
    {
        A = a;
        R = r;
        G = g;
        B = b;
    }

    /// <summary>
    /// 各色の成分から、Color 構造体を生成します。
    /// </summary>
    /// <param name="r">色の赤成分。</param>
    /// <param name="g">色の緑成分。</param>
    /// <param name="b">色の青成分。</param>
    public Color(float r, float g, float b)
    {
        A = 1f;
        R = r;
        G = g;
        B = b;
    }

    /// <summary>各色の成分から、Color 構造体を生成します。</summary>
    /// <param name="a">色のα成分。</param>
    /// <param name="r">色の赤成分。</param>
    /// <param name="g">色の緑成分。</param>
    /// <param name="b">色の青成分。</param>
    public Color(byte a, byte r, byte g, byte b)
    {
        A = a / 255f;
        R = r / 255f;
        G = g / 255f;
        B = b / 255f;
    }

    /// <summary>各色の成分から、Color 構造体を生成します。</summary>
    /// <param name="a">色のα成分。</param>
    /// <param name="r">色の赤成分。</param>
    /// <param name="g">色の緑成分。</param>
    /// <param name="b">色の青成分。</param>
    public Color(byte r, byte g, byte b)
    {
        A = 1f;
        R = r / 255f;
        G = g / 255f;
        B = b / 255f;
    }

    /// <summary>32bit の色を表す値から、Color 構造体を生成します。</summary>
    /// <param name="argb"></param>
    public Color(uint argb)
    {
        A = ((argb >> 24) & 0xFF) / 255f;
        R = ((argb >> 16) & 0xFF) / 255f;
        G = ((argb >> 8) & 0xFF) / 255f;
        B = (argb & 0xFF) / 255f;
    }

    public readonly override string ToString() => $"#{(int)(A * 255f):x2}{(int)(R * 255f):x2}{(int)(G * 255f):x2}{(int)(B * 255f):x2}";

    /// <summary>
    /// α値を持つ白を生成します。
    /// </summary>
    /// <param name="a">不透明度を表すα値。</param>
    /// <returns>生成された Color 構造体。</returns>
    public static Color Alpha(float a) => new(a, 1f, 1f, 1f);

    /// <summary>
    /// 不透明な黒を表します。
    /// </summary>
    public static readonly Color Black = new(1f, 0f, 0f, 0f);

    /// <summary>
    /// 不透明な白を表します。
    /// </summary>
    public static readonly Color White = new(1f, 1f, 1f, 1f);
}
