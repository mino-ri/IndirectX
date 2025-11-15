using System;
using IndirectX.D3D11;
using Buffer = IndirectX.D3D11.Buffer;

namespace IndirectX.Helper;

public interface IBufferWrapper
{
    void Flush();
}

public interface IBufferWriter<T>
{
    void Write(T source);
}

public abstract class BufferBase : IDisposable
{
    internal readonly DeviceContext Context;
    internal readonly Buffer D3DBuffer;

    internal BufferBase(DeviceContext context, Buffer buffer)
    {
        Context = context;
        D3DBuffer = buffer;
    }

    public void Dispose()
    {
        D3DBuffer.Dispose();
        GC.SuppressFinalize(this);
    }
}

public class ArrayBuffer<T> : BufferBase, IBufferWrapper, IBufferWriter<T[]> where T : unmanaged
{
    public T[] Buffer;
    public int Length => Buffer.Length;

    public ref T this[int index] => ref Buffer[index];

    internal ArrayBuffer(DeviceContext context, Buffer buffer, int length)
        : base(context, buffer)
    {
        Buffer = new T[length];
    }

    public void Flush() => Context.UpdateSubresource(Buffer, D3DBuffer);

    public void Write(T[] source)
    {
        source.CopyTo(Buffer, 0);
        Flush();
    }
}

public class ValueBuffer<T> : BufferBase, IBufferWrapper, IBufferWriter<T> where T : unmanaged
{
    public T Value;

    internal ValueBuffer(DeviceContext context, Buffer buffer)
        : base(context, buffer)
    {
    }

    public void Flush() => Context.UpdateSubresource(in Value, D3DBuffer);

    public void Write(T value)
    {
        Value = value;
        Flush();
    }

    public void WriteByRef(in T value)
    {
        Value = value;
        Flush();
    }
}

public class ArrayBufferWriter<T> : BufferBase, IBufferWriter<T[]> where T : unmanaged
{
    public int Length { get; }

    internal ArrayBufferWriter(DeviceContext context, Buffer buffer, int length)
        : base(context, buffer)
    {
        Length = length;
    }

    public void Write(T[] source)
    {
        if (source.Length != Length) throw new ArgumentException(null, nameof(source));
        Context.UpdateSubresource(source, D3DBuffer);
    }

    public void Write(Span<T> source)
    {
        if (source.Length != Length) throw new ArgumentException(null, nameof(source));
        Context.UpdateSubresource(source, D3DBuffer);
    }

    public void Write(ReadOnlySpan<T> source)
    {
        if (source.Length != Length) throw new ArgumentException(null, nameof(source));
        Context.UpdateSubresource(source, D3DBuffer);
    }
}

public class ValueBufferWriter<T> : BufferBase, IBufferWriter<T> where T : unmanaged
{
    internal ValueBufferWriter(DeviceContext context, Buffer buffer)
        : base(context, buffer) { }

    public void Write(T value)
    {
        Context.UpdateSubresource(in value, D3DBuffer);
    }

    public void WriteByRef(in T value)
    {
        Context.UpdateSubresource(in value, D3DBuffer);
    }
}
