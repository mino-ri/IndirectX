using System;

namespace IndirectX.Helper;

[Flags]
public enum ShaderStages
{
    None = 0x00,
    VertexShader = 0x01,
    HullShader = 0x02,
    DomainShader = 0x04,
    GeometryShader = 0x08,
    PixelShader = 0x10,
    ComputeShader = 0x20,
}
