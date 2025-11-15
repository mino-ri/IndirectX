using System.Collections.Generic;
using IndirectX.Dxgi;
using IndirectX.D3D11;
using Device = IndirectX.D3D11.Device;

namespace IndirectX.Helper;

public sealed partial class Graphics
{
    public unsafe ValueBuffer<T> CreateBuffer<T>(BindFlags bindFlags) where T : unmanaged
    {
        var buffer = Device.CreateBuffer(new BufferDesc
        {
            ByteWidth = sizeof(T),
            BindFlags = bindFlags,
        });
        return new ValueBuffer<T>(Context, buffer);
    }

    public unsafe ArrayBuffer<T> CreateBuffer<T>(BindFlags bindFlags, int length) where T : unmanaged
    {
        var buffer = Device.CreateBuffer(new BufferDesc
        {
            ByteWidth = sizeof(T) * length,
            StructureByteStride = sizeof(T),
            BindFlags = bindFlags,
        });
        return new ArrayBuffer<T>(Context, buffer, length);
    }

    public unsafe ValueBufferWriter<T> CreateBufferWriter<T>(BindFlags bindFlags) where T : unmanaged
    {
        var buffer = Device.CreateBuffer(new BufferDesc
        {
            ByteWidth = sizeof(T),
            BindFlags = bindFlags,
        });
        return new ValueBufferWriter<T>(Context, buffer);
    }

    public unsafe ArrayBufferWriter<T> CreateBufferWriter<T>(BindFlags bindFlags, int length) where T : unmanaged
    {
        var buffer = Device.CreateBuffer(new BufferDesc
        {
            ByteWidth = sizeof(T) * length,
            StructureByteStride = sizeof(T),
            BindFlags = bindFlags,
        });
        return new ArrayBufferWriter<T>(Context, buffer, length);
    }

    public unsafe void SetVertexBuffer<T>(int slot, ArrayBuffer<T> buffer) where T : unmanaged
    {
        Context.InputAssembler.VertexBuffers[slot] = (buffer.D3DBuffer, sizeof(T), 0);
    }

    public unsafe void SetVertexBuffer<T>(int slot, ArrayBufferWriter<T> buffer) where T : unmanaged
    {
        Context.InputAssembler.VertexBuffers[slot] = (buffer.D3DBuffer, sizeof(T), 0);
    }

    public unsafe ArrayBuffer<T> RegisterVertexBuffer<T>(int slot, int length) where T : unmanaged
    {
        var wrapper = CreateBuffer<T>(BindFlags.VertexBuffer, length);
        _disposables.Add(wrapper.D3DBuffer);
        Context.InputAssembler.VertexBuffers[slot] = (wrapper.D3DBuffer, sizeof(T), 0);
        return wrapper;
    }

    public unsafe ArrayBufferWriter<T> RegisterVertexBufferWriter<T>(int slot, int length) where T : unmanaged
    {
        var wrapper = CreateBufferWriter<T>(BindFlags.VertexBuffer, length);
        _disposables.Add(wrapper.D3DBuffer);
        Context.InputAssembler.VertexBuffers[slot] = (wrapper.D3DBuffer, sizeof(T), 0);
        return wrapper;
    }

    public unsafe void SetIndexBuffer(ArrayBuffer<ushort> buffer)
    {
        Context.InputAssembler.IndexBuffer = (buffer.D3DBuffer, Format.R16UInt, 0);
    }

    public unsafe void SetIndexBuffer(ArrayBufferWriter<ushort> buffer)
    {
        Context.InputAssembler.IndexBuffer = (buffer.D3DBuffer, Format.R16UInt, 0);
    }

    public ArrayBuffer<ushort> RegisterIndexBuffer(int length)
    {
        var wrapper = CreateBuffer<ushort>(BindFlags.IndexBuffer, length);
        _disposables.Add(wrapper.D3DBuffer);
        Context.InputAssembler.IndexBuffer = (wrapper.D3DBuffer, Format.R16UInt, 0);
        return wrapper;
    }

    public ArrayBufferWriter<ushort> RegisterIndexBufferWriter(int length)
    {
        var wrapper = CreateBufferWriter<ushort>(BindFlags.IndexBuffer, length);
        _disposables.Add(wrapper.D3DBuffer);
        Context.InputAssembler.IndexBuffer = (wrapper.D3DBuffer, Format.R16UInt, 0);
        return wrapper;
    }

    public void SetConstantBuffer(int slot, BufferBase buffer, ShaderStages bindingStages = ShaderStages.VertexShader | ShaderStages.PixelShader)
    {
        foreach (var shaderStage in GetShaderStages(bindingStages))
            shaderStage.ConstantBuffers[slot] = buffer.D3DBuffer;
    }

    public ValueBuffer<T> RegisterConstantBuffer<T>(int slot, ShaderStages bindingStages = ShaderStages.VertexShader | ShaderStages.PixelShader) where T : unmanaged
    {
        var wrapper = CreateBuffer<T>(BindFlags.ConstantBuffer);
        _disposables.Add(wrapper.D3DBuffer);
        foreach (var shaderStage in GetShaderStages(bindingStages))
            shaderStage.ConstantBuffers[slot] = wrapper.D3DBuffer;
        return wrapper;
    }

    public unsafe ArrayBuffer<T> RegisterConstantBuffer<T>(int slot, int length, ShaderStages bindingStages = ShaderStages.VertexShader | ShaderStages.PixelShader) where T : unmanaged
    {
        var wrapper = CreateBuffer<T>(BindFlags.ConstantBuffer, length);
        _disposables.Add(wrapper.D3DBuffer);
        foreach (var shaderStage in GetShaderStages(bindingStages))
            shaderStage.ConstantBuffers[slot] = wrapper.D3DBuffer;
        return wrapper;
    }

    public ValueBufferWriter<T> RegisterConstantBufferWriter<T>(int slot, ShaderStages bindingStages = ShaderStages.VertexShader | ShaderStages.PixelShader) where T : unmanaged
    {
        var wrapper = CreateBufferWriter<T>(BindFlags.ConstantBuffer);
        _disposables.Add(wrapper.D3DBuffer);
        foreach (var shaderStage in GetShaderStages(bindingStages))
            shaderStage.ConstantBuffers[slot] = wrapper.D3DBuffer;
        return wrapper;
    }

    public unsafe ArrayBufferWriter<T> RegisterConstantBufferWriter<T>(int slot, int length, ShaderStages bindingStages = ShaderStages.VertexShader | ShaderStages.PixelShader) where T : unmanaged
    {
        var wrapper = CreateBufferWriter<T>(BindFlags.ConstantBuffer, length);
        _disposables.Add(wrapper.D3DBuffer);
        foreach (var shaderStage in GetShaderStages(bindingStages))
            shaderStage.ConstantBuffers[slot] = wrapper.D3DBuffer;
        return wrapper;
    }

    private IEnumerable<DeviceContext.ShaderStage> GetShaderStages(ShaderStages shaderStages)
    {
        if (shaderStages.HasFlag(ShaderStages.VertexShader)) yield return Context.VertexShader;
        if (shaderStages.HasFlag(ShaderStages.HullShader)) yield return Context.HullShader;
        if (shaderStages.HasFlag(ShaderStages.DomainShader)) yield return Context.DomainShader;
        if (shaderStages.HasFlag(ShaderStages.GeometryShader)) yield return Context.GeometryShader;
        if (shaderStages.HasFlag(ShaderStages.PixelShader)) yield return Context.PixelShader;
        if (shaderStages.HasFlag(ShaderStages.ComputeShader)) yield return Context.ComputeShader;
    }
}
