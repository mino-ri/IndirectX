using System;
using IndirectX.D3D11;
using Device = IndirectX.D3D11.Device;

namespace IndirectX.Helper;

public sealed partial class Graphics
{
    public void SetVertexShader(Func<Device, VertexShader> shaderLoader)
    {
        using var shader = shaderLoader(Device);
        Context.VertexShader.Shader = shader;
    }

    public void SetHullShader(Func<Device, HullShader> shaderLoader)
    {
        using var shader = shaderLoader(Device);
        Context.HullShader.Shader = shader;
    }

    public void SetDomainShader(Func<Device, DomainShader> shaderLoader)
    {
        using var shader = shaderLoader(Device);
        Context.DomainShader.Shader = shader;
    }

    public void SetGeometryShader(Func<Device, GeometryShader> shaderLoader)
    {
        using var shader = shaderLoader(Device);
        Context.GeometryShader.Shader = shader;
    }

    public void SetPixelShader(Func<Device, PixelShader> shaderLoader)
    {
        using var shader = shaderLoader(Device);
        Context.PixelShader.Shader = shader;
    }

    public void SetVertexShader(ReadOnlySpan<byte> bytecode)
    {
        using var shader = Device.CreateVertexShader(bytecode);
        Context.VertexShader.Shader = shader;
    }

    public void SetHullShader(ReadOnlySpan<byte> bytecode)
    {
        using var shader = Device.CreateHullShader(bytecode);
        Context.HullShader.Shader = shader;
    }

    public void SetDomainShader(ReadOnlySpan<byte> bytecode)
    {
        using var shader = Device.CreateDomainShader(bytecode);
        Context.DomainShader.Shader = shader;
    }

    public void SetGeometryShader(ReadOnlySpan<byte> bytecode)
    {
        using var shader = Device.CreateGeometryShader(bytecode);
        Context.GeometryShader.Shader = shader;
    }

    public void SetPixelShader(ReadOnlySpan<byte> bytecode)
    {
        using var shader = Device.CreatePixelShader(bytecode);
        Context.PixelShader.Shader = shader;
    }

    public VertexShader? VertexShader
    {
        get => Context.VertexShader.Shader;
        set => Context.VertexShader.Shader = value;
    }

    public HullShader? HullShader
    {
        get => Context.HullShader.Shader;
        set => Context.HullShader.Shader = value;
    }

    public DomainShader? DomainShader
    {
        get => Context.DomainShader.Shader;
        set => Context.DomainShader.Shader = value;
    }

    public GeometryShader? GeometryShader
    {
        get => Context.GeometryShader.Shader;
        set => Context.GeometryShader.Shader = value;
    }

    public PixelShader? PixelShader
    {
        get => Context.PixelShader.Shader;
        set => Context.PixelShader.Shader = value;
    }

    public VertexShader CompileVertexShader(Span<byte> bytecode) => Device.CreateVertexShader(bytecode);

    public HullShader CompileHullShader(Span<byte> bytecode) => Device.CreateHullShader(bytecode);

    public DomainShader CompileDomainShader(Span<byte> bytecode) => Device.CreateDomainShader(bytecode);

    public GeometryShader CompileGeometryShader(Span<byte> bytecode) => Device.CreateGeometryShader(bytecode);

    public PixelShader CompilePixelShader(Span<byte> bytecode) => Device.CreatePixelShader(bytecode);
}
