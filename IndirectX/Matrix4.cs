using System.Numerics;
using System.Runtime.InteropServices;

namespace IndirectX;

/// <summary>4×4行列を表すクラス。この行列は、内部的に列優先配置を使用しているため、転置せずにDirectXに渡すことができます。</summary>
/// <remarks>各要素を指定して、<see cref="Matrix4"/>クラスの新しいインスタンスを初期化します。</remarks>
/// <param name="R1C1">(1, 1)要素。</param>
/// <param name="R1C2">(1, 2)要素。</param>
/// <param name="R1C3">(1, 3)要素。</param>
/// <param name="R1C4">(1, 4)要素。</param>
/// <param name="R2C1">(2, 1)要素。</param>
/// <param name="R2C2">(2, 2)要素。</param>
/// <param name="R2C3">(2, 3)要素。</param>
/// <param name="R2C4">(2, 4)要素。</param>
/// <param name="R3C1">(3, 1)要素。</param>
/// <param name="R3C2">(3, 2)要素。</param>
/// <param name="R3C3">(3, 3)要素。</param>
/// <param name="R3C4">(3, 4)要素。</param>
/// <param name="R4C1">(4, 1)要素。</param>
/// <param name="R4C2">(4, 2)要素。</param>
/// <param name="R4C3">(4, 3)要素。</param>
/// <param name="R4C4">(4, 4)要素。</param>
[StructLayout(LayoutKind.Sequential)]
public struct Matrix4(float r1c1, float r1c2, float r1c3, float r1c4,
               float r2c1, float r2c2, float r2c3, float r2c4,
               float r3c1, float r3c2, float r3c3, float r3c4,
               float r4c1, float r4c2, float r4c3, float r4c4)
{
    #region メンバー変数定義
    /// <summary>(1,1)要素。</summary>
    public float R1C1 = r1c1;
    /// <summary>(2,1)要素。</summary>
    public float R2C1 = r2c1;
    /// <summary>(3,1)要素。</summary>
    public float R3C1 = r3c1;
    /// <summary>(4,1)要素。</summary>
    public float R4C1 = r4c1;

    /// <summary>(1,2)要素。</summary>
    public float R1C2 = r1c2;
    /// <summary>(2,2)要素。</summary>
    public float R2C2 = r2c2;
    /// <summary>(3,2)要素。</summary>
    public float R3C2 = r3c2;
    /// <summary>(4,2)要素。</summary>
    public float R4C2 = r4c2;

    /// <summary>(1,3)要素。</summary>
    public float R1C3 = r1c3;
    /// <summary>(2,3)要素。</summary>
    public float R2C3 = r2c3;
    /// <summary>(3,3)要素。</summary>
    public float R3C3 = r3c3;
    /// <summary>(4,3)要素。</summary>
    public float R4C3 = r4c3;

    /// <summary>(1,4)要素。</summary>
    public float R1C4 = r1c4;
    /// <summary>(2,4)要素。</summary>
    public float R2C4 = r2c4;
    /// <summary>(3,4)要素。</summary>
    public float R3C4 = r3c4;
    /// <summary>(4,4)要素。</summary>
    public float R4C4 = r4c4;

    #endregion
    #region インスタンスメソッド

    /// <summary>現在のインスタンスを単位行列に初期化します。</summary>
    public void Initialize()
    {
        R1C1 = 1f;
        R2C1 = 0f;
        R3C1 = 0f;
        R4C1 = 0f;
        R1C2 = 0f;
        R2C2 = 1f;
        R3C2 = 0f;
        R4C2 = 0f;
        R1C3 = 0f;
        R2C3 = 0f;
        R3C3 = 1f;
        R4C3 = 0f;
        R1C4 = 0f;
        R2C4 = 0f;
        R3C4 = 0f;
        R4C4 = 1f;
    }

    /// <summary>現在の行列にX軸を中心とした回転を表す行列を乗算します。</summary>
    /// <param name="radian">ラジアンで計測した角度。</param>
    public void RotateX(float radian)
    {
        float cosf = (float)Math.Cos(radian);
        float sinf = (float)Math.Sin(radian);

        var i = R1C2 * cosf + R1C3 * -sinf;
        R1C3 = R1C2 * sinf + R1C3 * cosf;
        R1C2 = i;

        i = R2C2 * cosf + R2C3 * -sinf;
        R2C3 = R2C2 * sinf + R2C3 * cosf;
        R2C2 = i;

        i = R3C2 * cosf + R3C3 * -sinf;
        R3C3 = R3C2 * sinf + R3C3 * cosf;
        R3C2 = i;

        i = R4C2 * cosf + R4C3 * -sinf;
        R4C3 = R4C2 * sinf + R4C3 * cosf;
        R4C2 = i;
    }

    /// <summary>現在の行列にY軸を中心とした回転を表す行列を乗算します。</summary>
    /// <param name="radian">ラジアンで計測した角度。</param>
    public void RotateY(float radian)
    {
        float cosf = (float)Math.Cos(radian);
        float sinf = (float)Math.Sin(radian);

        var i = R1C1 * cosf + R1C3 * sinf;
        R1C3 = R1C1 * -sinf + R1C3 * cosf;
        R1C1 = i;

        i = R2C1 * cosf + R2C3 * sinf;
        R2C3 = R2C1 * -sinf + R2C3 * cosf;
        R2C1 = i;

        i = R3C1 * cosf + R3C3 * sinf;
        R3C3 = R3C1 * -sinf + R3C3 * cosf;
        R3C1 = i;

        i = R4C1 * cosf + R4C3 * sinf;
        R4C3 = R4C1 * -sinf + R4C3 * cosf;
        R4C1 = i;
    }

    /// <summary>現在の行列にZ軸を中心とした回転を表す行列を乗算します。</summary>
    /// <param name="radian">ラジアンで計測した角度。</param>
    public void RotateZ(float radian)
    {
        float cosf = (float)Math.Cos(radian);
        float sinf = (float)Math.Sin(radian);

        var i = R1C1 * cosf + R1C2 * -sinf;
        R1C2 = R1C1 * sinf + R1C2 * cosf;
        R1C1 = i;

        i = R2C1 * cosf + R2C2 * -sinf;
        R2C2 = R2C1 * sinf + R2C2 * cosf;
        R2C1 = i;

        i = R3C1 * cosf + R3C2 * -sinf;
        R3C2 = R3C1 * sinf + R3C2 * cosf;
        R3C1 = i;

        i = R4C1 * cosf + R4C2 * -sinf;
        R4C2 = R4C1 * sinf + R4C2 * cosf;
        R4C1 = i;
    }

    /// <summary>現在の行列にXY平面を中心とした4次元空間上の回転を表す行列を乗算します。</summary>
    /// <param name="radian">ラジアンで計測した角度。</param>
    public void RotateXY(float radian)
    {
        float cosf = (float)Math.Cos(radian);
        float sinf = (float)Math.Sin(radian);

        var i = R1C3 * cosf + R1C4 * sinf;
        R1C4 = R1C3 * -sinf + R1C4 * cosf;
        R1C3 = i;

        i = R2C3 * cosf + R2C4 * sinf;
        R2C4 = R2C3 * -sinf + R2C4 * cosf;
        R2C3 = i;

        i = R3C3 * cosf + R3C4 * sinf;
        R3C4 = R3C3 * -sinf + R3C4 * cosf;
        R3C3 = i;

        i = R4C3 * cosf + R4C4 * sinf;
        R4C4 = R4C3 * -sinf + R4C4 * cosf;
        R4C3 = i;
    }

    /// <summary>現在の行列にXZ平面を中心とした4次元空間上の回転を表す行列を乗算します。</summary>
    /// <param name="radian">ラジアンで計測した角度。</param>
    public void RotateXZ(float radian)
    {
        float cosf = (float)Math.Cos(radian);
        float sinf = (float)Math.Sin(radian);

        var i = R1C2 * cosf + R1C4 * sinf;
        R1C4 = R1C2 * -sinf + R1C4 * cosf;
        R1C2 = i;

        i = R2C2 * cosf + R2C4 * sinf;
        R2C4 = R2C2 * -sinf + R2C4 * cosf;
        R2C2 = i;

        i = R3C2 * cosf + R3C4 * sinf;
        R3C4 = R3C2 * -sinf + R3C4 * cosf;
        R3C2 = i;

        i = R4C2 * cosf + R4C4 * sinf;
        R4C4 = R4C2 * -sinf + R4C4 * cosf;
        R4C2 = i;
    }

    /// <summary>現在の行列にYZ平面を中心とした4次元空間上の回転を表す行列を乗算します。</summary>
    /// <param name="radian">ラジアンで計測した角度。</param>
    public void RotateYZ(float radian)
    {
        float cosf = (float)Math.Cos(radian);
        float sinf = (float)Math.Sin(radian);

        var i = R1C1 * cosf + R1C4 * sinf;
        R1C4 = R1C1 * -sinf + R1C4 * cosf;
        R1C1 = i;

        i = R2C1 * cosf + R2C4 * sinf;
        R2C4 = R2C1 * -sinf + R2C4 * cosf;
        R2C1 = i;

        i = R3C1 * cosf + R3C4 * sinf;
        R3C4 = R3C1 * -sinf + R3C4 * cosf;
        R3C1 = i;

        i = R4C1 * cosf + R4C4 * sinf;
        R4C4 = R4C1 * -sinf + R4C4 * cosf;
        R4C1 = i;
    }

    /// <summary>現在の行列に指定した行列を乗算します。</summary>
    /// <param name="target">乗算する行列。</param>
    public void MultiplyTo(in Matrix4 target)
    {
        var i = R1C1 * target.R1C1 + R1C2 * target.R2C1 + R1C3 * target.R3C1 + R1C4 * target.R4C1;
        var j = R1C1 * target.R1C2 + R1C2 * target.R2C2 + R1C3 * target.R3C2 + R1C4 * target.R4C2;
        var k = R1C1 * target.R1C3 + R1C2 * target.R2C3 + R1C3 * target.R3C3 + R1C4 * target.R4C3;
        R1C4 = R1C1 * target.R1C4 + R1C2 * target.R2C4 + R1C3 * target.R3C4 + R1C4 * target.R4C4;
        R1C1 = i;
        R1C2 = j;
        R1C3 = k;

        i = R2C1 * target.R1C1 + R2C2 * target.R2C1 + R2C3 * target.R3C1 + R2C4 * target.R4C1;
        j = R2C1 * target.R1C2 + R2C2 * target.R2C2 + R2C3 * target.R3C2 + R2C4 * target.R4C2;
        k = R2C1 * target.R1C3 + R2C2 * target.R2C3 + R2C3 * target.R3C3 + R2C4 * target.R4C3;
        R2C4 = R2C1 * target.R1C4 + R2C2 * target.R2C4 + R2C3 * target.R3C4 + R2C4 * target.R4C4;
        R2C1 = i;
        R2C2 = j;
        R2C3 = k;

        i = R3C1 * target.R1C1 + R3C2 * target.R2C1 + R3C3 * target.R3C1 + R3C4 * target.R4C1;
        j = R3C1 * target.R1C2 + R3C2 * target.R2C2 + R3C3 * target.R3C2 + R3C4 * target.R4C2;
        k = R3C1 * target.R1C3 + R3C2 * target.R2C3 + R3C3 * target.R3C3 + R3C4 * target.R4C3;
        R3C4 = R3C1 * target.R1C4 + R3C2 * target.R2C4 + R3C3 * target.R3C4 + R3C4 * target.R4C4;
        R3C1 = i;
        R3C2 = j;
        R3C3 = k;

        i = R4C1 * target.R1C1 + R4C2 * target.R2C1 + R4C3 * target.R3C1 + R4C4 * target.R4C1;
        j = R4C1 * target.R1C2 + R4C2 * target.R2C2 + R4C3 * target.R3C2 + R4C4 * target.R4C2;
        k = R4C1 * target.R1C3 + R4C2 * target.R2C3 + R4C3 * target.R3C3 + R4C4 * target.R4C3;
        R4C4 = R4C1 * target.R1C4 + R4C2 * target.R2C4 + R4C3 * target.R3C4 + R4C4 * target.R4C4;
        R4C1 = i;
        R4C2 = j;
        R4C3 = k;
    }

    /// <summary>指定した二つの行列を乗算し、現在のインスタンスに代入します。</summary>
    /// <param name="left">左辺。</param>
    /// <param name="right">右辺。</param>
    public void Multiply(in Matrix4 left, in Matrix4 right)
    {
        R1C1 = left.R1C1 * right.R1C1 + left.R1C2 * right.R2C1 + left.R1C3 * right.R3C1 + left.R1C4 * right.R4C1;
        R1C2 = left.R1C1 * right.R1C2 + left.R1C2 * right.R2C2 + left.R1C3 * right.R3C2 + left.R1C4 * right.R4C2;
        R1C3 = left.R1C1 * right.R1C3 + left.R1C2 * right.R2C3 + left.R1C3 * right.R3C3 + left.R1C4 * right.R4C3;
        R1C4 = left.R1C1 * right.R1C4 + left.R1C2 * right.R2C4 + left.R1C3 * right.R3C4 + left.R1C4 * right.R4C4;

        R2C1 = left.R2C1 * right.R1C1 + left.R2C2 * right.R2C1 + left.R2C3 * right.R3C1 + left.R2C4 * right.R4C1;
        R2C2 = left.R2C1 * right.R1C2 + left.R2C2 * right.R2C2 + left.R2C3 * right.R3C2 + left.R2C4 * right.R4C2;
        R2C3 = left.R2C1 * right.R1C3 + left.R2C2 * right.R2C3 + left.R2C3 * right.R3C3 + left.R2C4 * right.R4C3;
        R2C4 = left.R2C1 * right.R1C4 + left.R2C2 * right.R2C4 + left.R2C3 * right.R3C4 + left.R2C4 * right.R4C4;

        R3C1 = left.R3C1 * right.R1C1 + left.R3C2 * right.R2C1 + left.R3C3 * right.R3C1 + left.R3C4 * right.R4C1;
        R3C2 = left.R3C1 * right.R1C2 + left.R3C2 * right.R2C2 + left.R3C3 * right.R3C2 + left.R3C4 * right.R4C2;
        R3C3 = left.R3C1 * right.R1C3 + left.R3C2 * right.R2C3 + left.R3C3 * right.R3C3 + left.R3C4 * right.R4C3;
        R3C4 = left.R3C1 * right.R1C4 + left.R3C2 * right.R2C4 + left.R3C3 * right.R3C4 + left.R3C4 * right.R4C4;

        R4C1 = left.R4C1 * right.R1C1 + left.R4C2 * right.R2C1 + left.R4C3 * right.R3C1 + left.R4C4 * right.R4C1;
        R4C2 = left.R4C1 * right.R1C2 + left.R4C2 * right.R2C2 + left.R4C3 * right.R3C2 + left.R4C4 * right.R4C2;
        R4C3 = left.R4C1 * right.R1C3 + left.R4C2 * right.R2C3 + left.R4C3 * right.R3C3 + left.R4C4 * right.R4C3;
        R4C4 = left.R4C1 * right.R1C4 + left.R4C2 * right.R2C4 + left.R4C3 * right.R3C4 + left.R4C4 * right.R4C4;
    }

    /// <summary>頂点に変換行列を適用した新しい頂点を生成します。</summary>
    /// <param name="target">変換対象の頂点。</param>
    public readonly Vector4 Transform(Vector4 target)
    {
        return new Vector4(target.X * R1C1 + target.Y * R2C1 + target.Z * R3C1 + target.W * R4C1,
                           target.X * R1C2 + target.Y * R2C2 + target.Z * R3C2 + target.W * R4C2,
                           target.X * R1C3 + target.Y * R2C3 + target.Z * R3C3 + target.W * R4C3,
                           target.X * R1C4 + target.Y * R2C4 + target.Z * R3C4 + target.W * R4C4);
    }

    /// <summary>頂点に変換行列を適用した新しい頂点を生成します。</summary>
    /// <param name="target">変換対象の頂点。</param>
    public readonly Vector3 Transform(Vector3 target)
    {
        return new Vector3(target.X * R1C1 + target.Y * R2C1 + target.Z * R3C1 + R4C1,
                           target.X * R1C2 + target.Y * R2C2 + target.Z * R3C2 + R4C2,
                           target.X * R1C3 + target.Y * R2C3 + target.Z * R3C3 + R4C3);
    }

    /// <summary>
    /// このインスタンスを、それと等価な文字列に変換します。文字列は改行を含みます。
    /// </summary>
    public override readonly string ToString()
    {
        return string.Format(
@"[{0,7:F3}, {1,7:F3}, {2,7:F3}, {3,7:F3},   0.000]
[{4,7:F3}, {5,7:F3}, {6,7:F3}, {7,7:F3},   0.000]
[{8,7:F3}, {9,7:F3}, {10,7:F3}, {11,7:F3},   0.000]
[{12,7:F3}, {13,7:F3}, {14,7:F3}, {15,7:F3},   0.000]", R1C1, R1C2, R1C3, R1C4, R2C1, R2C2, R2C3, R2C4, R3C1, R3C2, R3C3, R3C4, R4C1, R4C2, R4C3, R4C4);
    }
    #endregion

    #region 静的メソッド・プロパティ
    /// <summary>単位行列を格納した<see cref="Matrix4"/>のインスタンスを取得します。</summary>
    public static readonly Matrix4 Identity = new(
        1f, 0f, 0f, 0f,
        0f, 1f, 0f, 0f,
        0f, 0f, 1f, 0f,
        0f, 0f, 0f, 1f);

    /// <summary>X軸を中心とした3次元空間上の回転を表す<see cref="Matrix4"/>の新しいインスタンスを生成します。</summary>
    /// <param name="radian">ラジアンで計測した角度。</param>
    public static Matrix4 RotationX(float radian)
    {
        float cosf = (float)Math.Cos(radian);
        float sinf = (float)Math.Sin(radian);

        return new Matrix4(
            1f, 0f, 0f, 0f,
            0f, cosf, sinf, 0f,
            0f, -sinf, cosf, 0f,
            0f, 0f, 0f, 1f);
    }

    /// <summary>Y軸を中心とした3次元空間上の回転を表す<see cref="Matrix4"/>の新しいインスタンスを生成します。</summary>
    /// <param name="radian">ラジアンで計測した角度。</param>
    public static Matrix4 RotationY(float radian)
    {
        float cosf = (float)Math.Cos(radian);
        float sinf = (float)Math.Sin(radian);

        return new Matrix4(
            cosf, 0f, -sinf, 0f,
              0f, 1f, 0f, 0f,
            sinf, 0f, cosf, 0f,
              0f, 0f, 0f, 1f);
    }

    /// <summary>Z軸を中心とした3次元空間上の回転を表す<see cref="Matrix4"/>の新しいインスタンスを生成します。</summary>
    /// <param name="radian">ラジアンで計測した角度。</param>
    public static Matrix4 RotationZ(float radian)
    {
        float cosf = (float)Math.Cos(radian);
        float sinf = (float)Math.Sin(radian);

        return new Matrix4(
             cosf, sinf, 0f, 0f,
            -sinf, cosf, 0f, 0f,
               0f, 0f, 1f, 0f,
               0f, 0f, 0f, 1f);
    }

    /// <summary>
    /// XY平面を中心とした4次元空間上の回転を表す<see cref="Matrix4"/>の新しいインスタンスを生成します。
    /// </summary>
    /// <param name="radian">ラジアンで計測した角度。</param>
    public static Matrix4 RotationXY(float radian)
    {
        float cosf = (float)Math.Cos(radian);
        float sinf = (float)Math.Sin(radian);

        return new Matrix4(
            1f, 0f, 0f, 0f,
            0f, 1f, 0f, 0f,
            0f, 0f, cosf, -sinf,
            0f, 0f, sinf, cosf);
    }

    /// <summary>
    /// XZ平面を中心とした4次元空間上の回転を表す<see cref="Matrix4"/>の新しいインスタンスを生成します。
    /// </summary>
    /// <param name="radian">ラジアンで計測した角度。</param>
    public static Matrix4 RotationXZ(float radian)
    {
        float cosf = (float)Math.Cos(radian);
        float sinf = (float)Math.Sin(radian);

        return new Matrix4(
            1f, 0f, 0f, 0f,
            0f, cosf, 0f, -sinf,
            0f, 0f, 1f, 0f,
            0f, sinf, 0f, cosf);
    }

    /// <summary>
    /// YZ平面を中心とした4次元空間上の回転を表す<see cref="Matrix4"/>の新しいインスタンスを生成します。
    /// </summary>
    /// <param name="radian">ラジアンで計測した角度。</param>
    public static Matrix4 RotationYZ(float radian)
    {
        float cosf = (float)Math.Cos(radian);
        float sinf = (float)Math.Sin(radian);

        return new Matrix4(
            cosf, 0f, 0f, -sinf,
              0f, 1f, 0f, 0f,
              0f, 0f, 1f, 0f,
            sinf, 0f, 0f, cosf);
    }

    /// <summary>拡大・縮小を表す<see cref="Matrix4"/>の新しいインスタンスを生成します。</summary>
    /// <param name="x">X方向の拡大・縮小倍率。</param>
    /// <param name="y">Y方向の拡大・縮小倍率。</param>
    /// <param name="z">Z方向の拡大・縮小倍率。</param>
    /// <param name="w">W方向の拡大・縮小倍率。</param>
    public static Matrix4 Scaling(float x, float y, float z, float w)
    {
        return new Matrix4(
             x, 0f, 0f, 0f,
            0f, y, 0f, 0f,
            0f, 0f, z, 0f,
            0f, 0f, 0f, w);
    }

    /// <summary>同次W座標を前提とした、4次元 → 2次元の透視射影行列を生成します。</summary>
    /// <param name="distanceZ">Z方向の距離感係数。</param>
    /// <param name="distanceW">W方向の距離感係数。</param>
    /// <param name="scale">拡大率。</param>
    /// <param name="nearZ">Z座標クリップ範囲の最近点。</param>
    /// <param name="farZ">Z座標クリップ範囲の最遠点。</param>
    public static Matrix4 Project4(float distanceZ, float distanceW, float scale, float nearZ, float farZ)
    {
        var sZ = Math.Sqrt((double)distanceZ * distanceZ - 1d);
        var sW = Math.Sqrt((double)distanceW * distanceW - 1d);

        var scaleFactor = (float)(scale * sZ * sW);
        var zFactor = ((double)farZ + distanceZ) / ((double)farZ - nearZ);

        return new Matrix4(
            scaleFactor, 0f, 0f, 0f,
                     0f, scaleFactor, 0f, 0f,
                     0f, 0f, (float)(sW * zFactor), (float)sW,
                     0f, 0f, (float)(-nearZ * zFactor), distanceZ);
    }

    /// <summary>一般的な正射影変換行列を生成します。</summary>
    /// <param name="zNear">最近点のZ座標。</param>
    /// <param name="zFar">最遠点のZ座標。</param>
    public static Matrix4 OrtoLH(float zNear, float zFar, float viewWidth, float viewHeight)
    {
        var q = zFar - zNear;
        var w = 2f / viewWidth;
        var h = -2f / viewHeight;
        return new Matrix4(
                    w, 0f, 0f, 0f,
                   0f, h, 0f, 0f,
                   0f, 0f, 1f / q, 0f,
            -0.5f * w, -0.5f * h, -zNear / q, 1f);
    }

    /// <summary>一般的な射影変換行列を生成します。</summary>
    /// <param name="zBase">X座標・Y座標が正しく表示されるZ座標。</param>
    /// <param name="zNear">最近点のZ座標。</param>
    /// <param name="zFar">最遠点のZ座標。</param>
    public static Matrix4 PerspectiveLH(float zBase, float zNear, float zFar, float viewWidth, float viewHeight)
    {
        var q = zFar / (zFar - zNear);
        var w = 2f / viewWidth * zBase;
        var h = -2f / viewHeight * zBase;
        return new Matrix4(
                    w, 0f, 0f, 0f,
                   0f, h, 0f, 0f,
                   0f, 0f, q, 1f,
            -0.5f * w, -0.5f * h, -q * zNear, 0f);
    }

    /// <summary>一般的な正射影変換行列を生成します。</summary>
    /// <param name="zNear">最近点のZ座標。</param>
    /// <param name="zFar">最遠点のZ座標。</param>
    public static Matrix4 OrtoLHZ(float zNear, float zFar, float viewWidth, float viewHeight)
    {
        var q = zFar - zNear;
        var w = 2f / viewWidth;
        var h = -2f / viewHeight;
        return new Matrix4(
             w, 0f, 0f, 0f,
            0f, h, 0f, 0f,
            0f, 0f, 1f / q, 0f,
            0f, 0f, -zNear / q, 1f);
    }

    /// <summary>一般的な射影変換行列を生成します。</summary>
    /// <param name="zBase">X座標・Y座標が正しく表示されるZ座標。</param>
    /// <param name="zNear">最近点のZ座標。</param>
    /// <param name="zFar">最遠点のZ座標。</param>
    public static Matrix4 PerspectiveLHZ(float zBase, float zNear, float zFar, float viewWidth, float viewHeight)
    {
        var q = zFar - zNear;
        var w = 2f / viewWidth * zBase;
        var h = -2f / viewHeight * zBase;
        return new Matrix4(
             w, 0f, 0f, 0f,
            0f, h, 0f, 0f,
            0f, 0f, zFar / q, 1f,
            0f, 0f, -zNear * zFar / q, 0f);
    }

    /// <summary>平行移動の変換行列を生成します。</summary>
    /// <param name="x">移動するX量。</param>
    /// <param name="y">移動するY量。</param>
    /// <param name="z">移動するZ量。</param>
    public static Matrix4 Translation(float x, float y, float z)
    {
        return new Matrix4(
            1f, 0f, 0f, 0f,
            0f, 1f, 0f, 0f,
            0f, 0f, 1f, 0f,
             x, y, z, 1f);
    }

    /// <summary>指定座標を指定距離・角度から見るビュー座標変換行列を生成します。</summary>
    /// <param name="x">カメラが注目する点のX座標。</param>
    /// <param name="y">カメラが注目する点のY座標。</param>
    /// <param name="z">カメラが注目する点のZ座標。</param>
    /// <param name="distance">注目点からのカメラの距離。</param>
    public static Matrix4 LookAtLH(float x, float y, float z, float distance)
    {
        return new Matrix4(
            1f, 0f, 0f, 0f,
            0f, 1f, 0f, 0f,
            0f, 0f, 1f, 0f,
            -x, -y, -z + distance, 1f);
    }

    /// <summary>指定座標を指定距離・角度から見るビュー座標変換行列を生成します。</summary>
    /// <param name="x">カメラが注目する点のX座標。</param>
    /// <param name="y">カメラが注目する点のY座標。</param>
    /// <param name="z">カメラが注目する点のZ座標。</param>
    /// <param name="yRotate">カメラの、Y軸を中心とした回転角度。</param>
    /// <param name="distance">注目点からのカメラの距離。</param>
    public static Matrix4 LookAtLH(float x, float y, float z, float yRotate, float distance)
    {
        float sinY = (float)Math.Sin(-yRotate);
        float cosY = (float)Math.Cos(-yRotate);
        var dx = -x * cosY - z * sinY;
        var dz = x * sinY - z * cosY + distance;

        return new Matrix4(
            cosY, 0f, -sinY, 0f,
              0f, 1f, 0f, 0f,
            sinY, 0f, cosY, 0f,
              dx, -y, dz, 1f);
    }

    /// <summary>2つの<see cref="Matrix4"/>を乗算します。</summary>
    /// <param name="left">左辺。</param>
    /// <param name="right">右辺。</param>
    public static Matrix4 operator *(in Matrix4 left, in Matrix4 right)
    {
        return new Matrix4(
        left.R1C1 * right.R1C1 + left.R1C2 * right.R2C1 + left.R1C3 * right.R3C1 + left.R1C4 * right.R4C1,
        left.R1C1 * right.R1C2 + left.R1C2 * right.R2C2 + left.R1C3 * right.R3C2 + left.R1C4 * right.R4C2,
        left.R1C1 * right.R1C3 + left.R1C2 * right.R2C3 + left.R1C3 * right.R3C3 + left.R1C4 * right.R4C3,
        left.R1C1 * right.R1C4 + left.R1C2 * right.R2C4 + left.R1C3 * right.R3C4 + left.R1C4 * right.R4C4,

        left.R2C1 * right.R1C1 + left.R2C2 * right.R2C1 + left.R2C3 * right.R3C1 + left.R2C4 * right.R4C1,
        left.R2C1 * right.R1C2 + left.R2C2 * right.R2C2 + left.R2C3 * right.R3C2 + left.R2C4 * right.R4C2,
        left.R2C1 * right.R1C3 + left.R2C2 * right.R2C3 + left.R2C3 * right.R3C3 + left.R2C4 * right.R4C3,
        left.R2C1 * right.R1C4 + left.R2C2 * right.R2C4 + left.R2C3 * right.R3C4 + left.R2C4 * right.R4C4,

        left.R3C1 * right.R1C1 + left.R3C2 * right.R2C1 + left.R3C3 * right.R3C1 + left.R3C4 * right.R4C1,
        left.R3C1 * right.R1C2 + left.R3C2 * right.R2C2 + left.R3C3 * right.R3C2 + left.R3C4 * right.R4C2,
        left.R3C1 * right.R1C3 + left.R3C2 * right.R2C3 + left.R3C3 * right.R3C3 + left.R3C4 * right.R4C3,
        left.R3C1 * right.R1C4 + left.R3C2 * right.R2C4 + left.R3C3 * right.R3C4 + left.R3C4 * right.R4C4,

        left.R4C1 * right.R1C1 + left.R4C2 * right.R2C1 + left.R4C3 * right.R3C1 + left.R4C4 * right.R4C1,
        left.R4C1 * right.R1C2 + left.R4C2 * right.R2C2 + left.R4C3 * right.R3C2 + left.R4C4 * right.R4C2,
        left.R4C1 * right.R1C3 + left.R4C2 * right.R2C3 + left.R4C3 * right.R3C3 + left.R4C4 * right.R4C3,
        left.R4C1 * right.R1C4 + left.R4C2 * right.R2C4 + left.R4C3 * right.R3C4 + left.R4C4 * right.R4C4);
    }

    /// <summary>ベクトルと行列を乗算します。</summary>
    /// <param name="left">乗算されるベクトル。</param>
    /// <param name="right">乗算する行列。</param>
    public static Vector4 operator *(Vector4 left, in Matrix4 right)
    {
        return new Vector4(left.X * right.R1C1 + left.Y * right.R2C1 + left.Z * right.R3C1 + left.W * right.R4C1,
                           left.X * right.R1C2 + left.Y * right.R2C2 + left.Z * right.R3C2 + left.W * right.R4C2,
                           left.X * right.R1C3 + left.Y * right.R2C3 + left.Z * right.R3C3 + left.W * right.R4C3,
                           left.X * right.R1C4 + left.Y * right.R2C4 + left.Z * right.R3C4 + left.W * right.R4C4);
    }

    /// <summary>ベクトルと行列を乗算します。</summary>
    /// <param name="left">乗算されるベクトル。</param>
    /// <param name="right">乗算する行列。</param>
    public static Vector3 operator *(Vector3 left, in Matrix4 right)
    {
        return new Vector3(left.X * right.R1C1 + left.Y * right.R2C1 + left.Z * right.R3C1 + right.R4C1,
                           left.X * right.R1C2 + left.Y * right.R2C2 + left.Z * right.R3C2 + right.R4C2,
                           left.X * right.R1C3 + left.Y * right.R2C3 + left.Z * right.R3C3 + right.R4C3);
    }
    #endregion
}
