using System;

namespace IndirectX.D3DCompiler;

public sealed class CompilationResult : IDisposable
{
    private bool _isDisposed;

    private D3DBytecode? _errorMsg;
    public D3DBytecode? Bytecode { get; }

    public HResult Result { get; }

    public string? ErrorMessage => _errorMsg;

    public bool HasError => _errorMsg is not null;

    internal CompilationResult(HResult result, D3DBytecode? bytecode, D3DBytecode? errorMsg) =>
        (Result, Bytecode, _errorMsg) = (result, bytecode, errorMsg);

    public void Dispose()
    {
        if (_isDisposed) return;
        _isDisposed = true;

        Bytecode?.Dispose();
        _errorMsg?.Dispose();
    }
}
