using System.Numerics;
using System.Runtime.InteropServices;

namespace AotCube;

[StructLayout(LayoutKind.Sequential)]
public struct Light
{
    public float AmbientFactor;
    public float DiffuseFactor;
    public float SpecularFactor;
    public float SpecularIndex;
    public Vector4 LightDirection;
}
