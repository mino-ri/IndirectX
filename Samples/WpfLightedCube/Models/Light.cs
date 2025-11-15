using System.Numerics;

namespace WpfLightedCube.Models;

public struct Light
{
    public float AmbientFactor;
    public float DiffuseFactor;
    public float SpecularFactor;
    public float SpecularIndex;
    public Vector4 LightSource;
    public Vector4 CameraPosition;
}
