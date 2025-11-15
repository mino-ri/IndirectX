using System;
using System.Text;

namespace IndirectX.D3DCompiler;

/// <summary>
/// Common interface for ID3DBlob, D3D_SHADER_DATA or unmanaged bytecode data. This needs to be disposed.
/// </summary>
public abstract partial class Bytecode : IDisposable
{
    public abstract nint NativePtr { get; }

    public abstract nint Length { get; }

    public unsafe Span<byte> AsSpan() => new Span<byte>((void*)NativePtr, (int)Length);

    public unsafe ReadOnlySpan<byte> AsReadOnlySpan() => new ReadOnlySpan<byte>((void*)NativePtr, (int)Length);

    protected abstract void Dispose(bool disposing);

    internal unsafe ShaderData ToShaderData() => new ShaderData((void*)NativePtr, Length);

    internal unsafe void WriteToShaderData(ref ShaderData shaderData)
    {
        shaderData.PBytecode = (byte*)NativePtr;
        shaderData.BytecodeLength = Length;
    }

    ~Bytecode() => Dispose(false);

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
