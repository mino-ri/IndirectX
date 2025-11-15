using System;
using System.Diagnostics.CodeAnalysis;
using IndirectX.Interop;

namespace IndirectX.D3DCompiler;

unsafe partial class Bytecode
{
    private static D3DBytecode? CreateIfNotNull(ComPtr ptr) => ptr.IsNull
        ? null
        : new D3DBytecode(ptr);

    public static CompilationResult Compile(Bytecode srcData, string entrypoint, string target, string? sourceName = null, ReadOnlySpan<ShaderMacro> defines = default, CompileFlags flags1 = default, CompileEffectFlags flags2 = default)
    {
        using var p_sourceName = new InteropString(sourceName);
        using var p_defines = new InteropShaderMacroArray(defines);
        using var p_entrypoint = new InteropString(entrypoint);
        using var p_target = new InteropString(target);
        return new CompilationResult(
            NativeApi.D3DCompile((byte*)srcData.NativePtr, srcData.Length, p_sourceName, p_defines.NativePtr, ComPtr.Null, p_entrypoint, p_target, flags1, flags2, out var code, out var errorMsgs),
            CreateIfNotNull(new(code)),
            CreateIfNotNull(new(errorMsgs)));
    }

    public static CompilationResult Preprocess(Bytecode srcData, string? sourceName = null, ReadOnlySpan<ShaderMacro> defines = default)
    {
        using var p_sourceName = new InteropString(sourceName);
        using var p_defines = new InteropShaderMacroArray(defines);
        return new CompilationResult(
            NativeApi.D3DPreprocess((byte*)srcData.NativePtr, srcData.Length, p_sourceName, p_defines.NativePtr, ComPtr.Null, out var codeText, out var errorMsgs),
            CreateIfNotNull(new(codeText)),
            CreateIfNotNull(new(errorMsgs)));
    }

    public static D3DBytecode GetDebugInfo(Bytecode srcData)
    {
        NativeApi.D3DGetDebugInfo((byte*)srcData.NativePtr, srcData.Length, out var debugInfo).HandleResult();
        return new D3DBytecode(new ComPtr(debugInfo));
    }

    public static T? Reflect<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>(Bytecode srcData)
        where T : Unknown
    {
        NativeApi.D3DReflect((byte*)srcData.NativePtr, srcData.Length, Unknown.GetIid<T>(), out var reflector).HandleResult();
        return Unknown.Create<T>(new ComPtr(reflector));
    }

    public static D3DBytecode Disassemble(Bytecode srcData, string szComments, DisassembleFlags flags = default)
    {
        using var p_szComments = new InteropString(szComments);
        NativeApi.D3DDisassemble((byte*)srcData.NativePtr, srcData.Length, flags, p_szComments, out var disassembly).HandleResult();
        return new D3DBytecode(new ComPtr(disassembly));
    }

    public static D3DBytecode GetInputSignature(Bytecode srcData)
    {
        NativeApi.D3DGetInputSignatureBlob((byte*)srcData.NativePtr, srcData.Length, out var signatureBlob).HandleResult();
        return new D3DBytecode(new ComPtr(signatureBlob));
    }

    public static D3DBytecode GetOutputSignature(Bytecode srcData)
    {
        NativeApi.D3DGetOutputSignatureBlob((byte*)srcData.NativePtr, srcData.Length, out var signatureBlob).HandleResult();
        return new D3DBytecode(new ComPtr(signatureBlob));
    }

    public static D3DBytecode GetInputAndOutputSignature(Bytecode srcData)
    {
        NativeApi.D3DGetInputAndOutputSignatureBlob((byte*)srcData.NativePtr, srcData.Length, out var signatureBlob).HandleResult();
        return new D3DBytecode(new ComPtr(signatureBlob));
    }

    public static D3DBytecode StripShader(Bytecode shaderBytecode, StripFlags uStripFlags = default)
    {
        NativeApi.D3DStripShader((byte*)shaderBytecode.NativePtr, shaderBytecode.Length, uStripFlags, out var strippedBlob).HandleResult();
        return new D3DBytecode(new ComPtr(strippedBlob));
    }

    public static D3DBytecode GetBlobPart(Bytecode srcData, BlobPart part, GetBlobPartFlags flags = default)
    {
        NativeApi.D3DGetBlobPart((byte*)srcData.NativePtr, srcData.Length, part, flags, out var result).HandleResult();
        return new D3DBytecode(new ComPtr(result));
    }

    public static D3DBytecode CompressShaders(ReadOnlySpan<Bytecode> shaderData, CompressShaderFlags uFlags = default)
    {
        var intropShaderData = new ShaderData[shaderData.Length];
        for (var i = 0; i < shaderData.Length; i++)
        {
            intropShaderData[i].PBytecode = (void*)shaderData[i].NativePtr;
            intropShaderData[i].BytecodeLength = shaderData[i].Length;
        }

        fixed (ShaderData* p_shaderData = intropShaderData)
        {
            NativeApi.D3DCompressShaders(shaderData.Length, p_shaderData, uFlags, out var compressedData).HandleResult();
            return new D3DBytecode(new ComPtr(compressedData));
        }
    }

    public static (D3DBytecode[] shaders, int totalShaders) DecompressShaders(Bytecode srcData, int uStartIndex, ReadOnlySpan<int> indices, CompressShaderFlags uFlags = default)
    {
        using var shaders = new InteropArray<Blob>(indices.Length);
        fixed (int* p_indices = indices)
        {
            NativeApi.D3DDecompressShaders((byte*)srcData.NativePtr, srcData.Length, indices.Length, uStartIndex, p_indices, uFlags, (nint*)shaders.NativePtr, out var totalShaders).HandleResult();
            return (Array.ConvertAll(shaders.ToArray(), b => new D3DBytecode(b)), totalShaders);
        }
    }

    public static D3DBytecode CreateD3D(nint length)
    {
        NativeApi.D3DCreateBlob(length, out var blob).HandleResult();
        return new D3DBytecode(new ComPtr(blob));
    }

    public static CompilationResult Compile(string str, string entrypoint, string target, string? sourceName = null, ReadOnlySpan<ShaderMacro> defines = default, CompileFlags flags1 = default, CompileEffectFlags flags2 = default)
    {
        using var bytecode = Fixed(str);
        return Compile(bytecode, entrypoint, target, sourceName, defines, flags1, flags2);
    }

    public static CompilationResult Compile(byte[] array, string entrypoint, string target, string? sourceName = null, ReadOnlySpan<ShaderMacro> defines = default, CompileFlags flags1 = default, CompileEffectFlags flags2 = default)
    {
        using var bytecode = Fixed(array);
        return Compile(bytecode, entrypoint, target, sourceName, defines, flags1, flags2);
    }

    public static CompilationResult Compile(ReadOnlySpan<byte> span, string entrypoint, string target, string? sourceName = null, ReadOnlySpan<ShaderMacro> defines = default, CompileFlags flags1 = default, CompileEffectFlags flags2 = default)
    {
        fixed (byte* p = span)
        {
            using var bytecode = new UnsafeBytecode(p, span.Length);
            return Compile(bytecode, entrypoint, target, sourceName, defines, flags1, flags2);
        }
    }

    public static CompilationResult Preprocess(string str, string? sourceName = null, ReadOnlySpan<ShaderMacro> defines = default)
    {
        using var bytecode = Fixed(str);
        return Preprocess(bytecode, sourceName, defines);
    }

    public static CompilationResult Preprocess(byte[] array, string? sourceName = null, ReadOnlySpan<ShaderMacro> defines = default)
    {
        using var bytecode = Fixed(array);
        return Preprocess(bytecode, sourceName, defines);
    }

    public static CompilationResult Preprocess(ReadOnlySpan<byte> span, string? sourceName = null, ReadOnlySpan<ShaderMacro> defines = default)
    {
        fixed (byte* p = span)
        {
            using var bytecode = new UnsafeBytecode(p, span.Length);
            return Preprocess(bytecode, sourceName, defines);
        }
    }

    public static D3DBytecode GetDebugInfo(string str)
    {
        using var bytecode = Fixed(str);
        return GetDebugInfo(bytecode);
    }

    public static D3DBytecode GetDebugInfo(byte[] array)
    {
        using var bytecode = Fixed(array);
        return GetDebugInfo(bytecode);
    }

    public static D3DBytecode GetDebugInfo(ReadOnlySpan<byte> span)
    {
        fixed (byte* p = span)
        {
            using var bytecode = new UnsafeBytecode(p, span.Length);
            return GetDebugInfo(bytecode);
        }
    }

    public static T? Reflect<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>(string str) where T : Unknown
    {
        using var bytecode = Fixed(str);
        return Reflect<T>(bytecode);
    }

    public static T? Reflect<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>(byte[] array) where T : Unknown
    {
        using var bytecode = Fixed(array);
        return Reflect<T>(bytecode);
    }

    public static T? Reflect<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>(ReadOnlySpan<byte> span) where T : Unknown
    {
        fixed (byte* p = span)
        {
            using var bytecode = new UnsafeBytecode(p, span.Length);
            return Reflect<T>(bytecode);
        }
    }

    public static D3DBytecode Disassemble(string str, string szComments, DisassembleFlags flags = default)
    {
        using var bytecode = Fixed(str);
        return Disassemble(bytecode, szComments, flags);
    }

    public static D3DBytecode Disassemble(byte[] array, string szComments, DisassembleFlags flags = default)
    {
        using var bytecode = Fixed(array);
        return Disassemble(bytecode, szComments, flags);
    }

    public static D3DBytecode Disassemble(ReadOnlySpan<byte> span, string szComments, DisassembleFlags flags = default)
    {
        fixed (byte* p = span)
        {
            using var bytecode = new UnsafeBytecode(p, span.Length);
            return Disassemble(bytecode, szComments, flags);
        }
    }

    public static D3DBytecode GetInputSignature(string str)
    {
        using var bytecode = Fixed(str);
        return GetInputSignature(bytecode);
    }

    public static D3DBytecode GetInputSignature(byte[] array)
    {
        using var bytecode = Fixed(array);
        return GetInputSignature(bytecode);
    }

    public static D3DBytecode GetInputSignature(ReadOnlySpan<byte> span)
    {
        fixed (byte* p = span)
        {
            using var bytecode = new UnsafeBytecode(p, span.Length);
            return GetInputSignature(bytecode);
        }
    }

    public static D3DBytecode GetOutputSignature(string str)
    {
        using var bytecode = Fixed(str);
        return GetOutputSignature(bytecode);
    }

    public static D3DBytecode GetOutputSignature(byte[] array)
    {
        using var bytecode = Fixed(array);
        return GetOutputSignature(bytecode);
    }

    public static D3DBytecode GetOutputSignature(ReadOnlySpan<byte> span)
    {
        fixed (byte* p = span)
        {
            using var bytecode = new UnsafeBytecode(p, span.Length);
            return GetOutputSignature(bytecode);
        }
    }

    public static D3DBytecode GetInputAndOutputSignature(string str)
    {
        using var bytecode = Fixed(str);
        return GetInputAndOutputSignature(bytecode);
    }

    public static D3DBytecode GetInputAndOutputSignature(byte[] array)
    {
        using var bytecode = Fixed(array);
        return GetInputAndOutputSignature(bytecode);
    }

    public static D3DBytecode GetInputAndOutputSignature(ReadOnlySpan<byte> span)
    {
        fixed (byte* p = span)
        {
            using var bytecode = new UnsafeBytecode(p, span.Length);
            return GetInputAndOutputSignature(bytecode);
        }
    }

    public static D3DBytecode StripShader(string str, StripFlags uStripFlags = default)
    {
        using var bytecode = Fixed(str);
        return StripShader(bytecode, uStripFlags);
    }

    public static D3DBytecode StripShader(byte[] array, StripFlags uStripFlags = default)
    {
        using var bytecode = Fixed(array);
        return StripShader(bytecode, uStripFlags);
    }

    public static D3DBytecode StripShader(ReadOnlySpan<byte> span, StripFlags uStripFlags = default)
    {
        fixed (byte* p = span)
        {
            using var bytecode = new UnsafeBytecode(p, span.Length);
            return StripShader(bytecode, uStripFlags);
        }
    }

    public static D3DBytecode GetBlobPart(string str, BlobPart part, GetBlobPartFlags flags = default)
    {
        using var bytecode = Fixed(str);
        return GetBlobPart(bytecode, part, flags);
    }

    public static D3DBytecode GetBlobPart(byte[] array, BlobPart part, GetBlobPartFlags flags = default)
    {
        using var bytecode = Fixed(array);
        return GetBlobPart(bytecode, part, flags);
    }

    public static D3DBytecode GetBlobPart(ReadOnlySpan<byte> span, BlobPart part, GetBlobPartFlags flags = default)
    {
        fixed (byte* p = span)
        {
            using var bytecode = new UnsafeBytecode(p, span.Length);
            return GetBlobPart(bytecode, part, flags);
        }
    }

    public static (D3DBytecode[] shaders, int totalShaders) DecompressShaders(string str, int uStartIndex, ReadOnlySpan<int> indices, CompressShaderFlags uFlags = default)
    {
        using var bytecode = Fixed(str);
        return DecompressShaders(bytecode, uStartIndex, indices, uFlags);
    }

    public static (D3DBytecode[] shaders, int totalShaders) DecompressShaders(byte[] array, int uStartIndex, ReadOnlySpan<int> indices, CompressShaderFlags uFlags = default)
    {
        using var bytecode = Fixed(array);
        return DecompressShaders(bytecode, uStartIndex, indices, uFlags);
    }

    public static (D3DBytecode[] shaders, int totalShaders) DecompressShaders(ReadOnlySpan<byte> span, int uStartIndex, ReadOnlySpan<int> indices, CompressShaderFlags uFlags = default)
    {
        fixed (byte* p = span)
        {
            using var bytecode = new UnsafeBytecode(p, span.Length);
            return DecompressShaders(bytecode, uStartIndex, indices, uFlags);
        }
    }
}
