using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Diagnostics.CodeAnalysis;

namespace IndirectX.D3DCompiler;

unsafe partial class Bytecode
{
    public static HGlobalBytecode AllocHGlobal(int length) => new HGlobalBytecode(length);

    public static ManagedBytecode Fixed(ReadOnlyMemory<byte> memory) => new ManagedBytecode(memory);

    public static ManagedBytecode Fixed(byte[] array) => Fixed(array.AsMemory());

    public static ManagedBytecode Fixed(string str) => Fixed(Encoding.ASCII.GetBytes(str));

    public unsafe static UnsafeBytecode FromSpanUnsafe(ReadOnlySpan<byte> span) => new UnsafeBytecode((nint)Unsafe.AsPointer(ref Unsafe.AsRef(in span[0])), span.Length);

    public static FileBytecode FromFile(FileStream stream) => new FileBytecode(stream);

    public static FileBytecode FromFile(string path) => new FileBytecode(path);

    public static implicit operator Span<byte>(Bytecode? bytecode) => bytecode is null ? default : bytecode.AsSpan();

    public static implicit operator ReadOnlySpan<byte>(Bytecode? bytecode) => bytecode is null ? default : bytecode.AsReadOnlySpan();

    [return: NotNullIfNotNull("bytecode")]
    public static implicit operator byte[]?(Bytecode? bytecode) => bytecode is null ? null : bytecode.AsReadOnlySpan().ToArray();

    [return: NotNullIfNotNull("bytecode")]
    public static unsafe implicit operator string?(Bytecode? bytecode) => bytecode is null ? null : Encoding.UTF8.GetString((byte*)bytecode.NativePtr, (int)bytecode.Length);
}
