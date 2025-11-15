using System;
using IndirectX.D3D11;

namespace IndirectX.Helper;

public interface IResourceTexture : IDisposable
{
    ShaderResourceView ShaderResourceView { get; }
}

internal class ResourceTexture : IResourceTexture
{
    public ShaderResourceView ShaderResourceView { get; set; }

    internal ResourceTexture(ShaderResourceView shaderResourceView)
    {
        ShaderResourceView = shaderResourceView;
    }

    public void Dispose()
    {
        ShaderResourceView.Dispose();
    }
}

public class RenderTargetTexture : IResourceTexture
{
    public int Width { get; }

    public int Height { get; }

    public RenderTargetView RenderTargetView { get; }

    public ShaderResourceView ShaderResourceView { get; }

    internal RenderTargetTexture(int width, int height, RenderTargetView renderTargetView, ShaderResourceView shaderResourceView)
    {
        Width = width;
        Height = height;
        RenderTargetView = renderTargetView;
        ShaderResourceView = shaderResourceView;
    }

    public void Dispose()
    {
        RenderTargetView.Dispose();
        ShaderResourceView.Dispose();
        GC.SuppressFinalize(this);
    }
}
