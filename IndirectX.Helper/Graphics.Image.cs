using System;
using IndirectX.D3D11;

namespace IndirectX.Helper;

public sealed partial class Graphics
{
    public SamplerState CreateSamplerState(in SamplerDesc samplerDesc)
    {
        return Device.CreateSamplerState(in samplerDesc);
    }

    public void SetSampler(int slot, SamplerState samplerState)
    {
        Context.PixelShader.SetSamplers(slot, samplerState);
    }

    public void SetSampler(int slot, in SamplerDesc samplerDesc)
    {
        using var samplerState = CreateSamplerState(samplerDesc);
        SetSampler(slot, samplerState);
    }

    public SamplerState CreateBorderSamplerState()
    {
        return CreateSamplerState(new SamplerDesc
        {
            AddressU = TextureAddressMode.Border,
            AddressV = TextureAddressMode.Border,
            AddressW = TextureAddressMode.Border,
            BorderColor = new Float4([0f, 0f, 0f, 0f]),
            ComparisonFunc = ComparisonFunc.LessEqual,
            Filter = Filter.ComparisonMinMagMipPoint,
            MaxAnisotropy = 0,
            MipLODBias = 0f,
            MinLOD = 0f,
            MaxLOD = 3.402823466E+38f,
        });
    }

    public void SetBorderSampler(int slot)
    {
        using var samplerState = CreateBorderSamplerState();
        SetSampler(slot, samplerState);
    }

    public SamplerState CreateWrapSamplerState()
    {
        return CreateSamplerState(new SamplerDesc
        {
            AddressU = TextureAddressMode.Wrap,
            AddressV = TextureAddressMode.Wrap,
            AddressW = TextureAddressMode.Wrap,
            BorderColor = new Float4([0f, 0f, 0f, 0f]),
            ComparisonFunc = ComparisonFunc.LessEqual,
            Filter = Filter.ComparisonMinMagMipPoint,
            MaxAnisotropy = 0,
            MipLODBias = 0f,
            MinLOD = 0f,
            MaxLOD = 3.402823466E+38f,
        });
    }

    public void SetWrapSampler(int slot)
    {
        using var samplerState = CreateWrapSamplerState();
        SetSampler(slot, samplerState);
    }

    private ShaderResourceView CreateTextureSrv(Texture2D texture2D)
    {
        return Device.CreateShaderResourceView(texture2D);
    }

    public unsafe IResourceTexture CreateResourceTexture(void* dataPtr, int width, int height, int stride)
    {
        using var texture2D = Device.CreateTexture2D(new Texture2DDesc
        {
            Format = DisplayFormat,
            ArraySize = 1,
            MipLevels = 1,
            Width = width,
            Height = height,
            SampleDesc = { Count = 1, Quality = 0 },
            BindFlags = BindFlags.ShaderResource,
        },
        new SubresourceData
        {
            PSysMem = dataPtr,
            SysMemPitch = stride,
        });

        return new ResourceTexture(CreateTextureSrv(texture2D));
    }

    public unsafe IResourceTexture CreateResourceTexture(nint intPtr, int width, int height, int stride)
    {
        return CreateResourceTexture((void*)intPtr, width, height, stride);
    }

    public unsafe IResourceTexture CreateResourceTexture(ReadOnlySpan<byte> rgbaData, int width, int height, int stride)
    {
        if (rgbaData.Length < stride * height)
        {
            throw new ArgumentException($"Length of {nameof(rgbaData)} is invalid.", nameof(rgbaData));
        }

        fixed (byte* dataPtr = rgbaData)
        {
            return CreateResourceTexture(dataPtr, width, height, stride);
        }
    }

    public RenderTargetTexture CreateRenderTargetTexture(int width, int height)
    {
        using var texture2D = Device.CreateTexture2D(new Texture2DDesc
        {
            Format = DisplayFormat,
            ArraySize = 1,
            MipLevels = 1,
            Width = width,
            Height = height,
            SampleDesc = { Count = 1, Quality = 0 },
            BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
        });

        return new RenderTargetTexture(width, height, Device.CreateRenderTargetView(texture2D), CreateTextureSrv(texture2D));
    }

    public void ResetRenderTarget()
    {
        Context.OutputMerger.SetRenderTarget(RenderView, DepthView);
        Context.Rasterizer.SetViewports(new Viewport
        {
            Width = _width,
            Height = _height,
            MinDepth = 0.0f,
            MaxDepth = 1.0f,
        });
    }

    public void SetRenderTarget(RenderTargetTexture renderTargetTexture)
    {
        Context.OutputMerger.SetRenderTarget(renderTargetTexture.RenderTargetView, DepthView);
        Context.Rasterizer.SetViewports(new Viewport
        {
            Width = renderTargetTexture.Width,
            Height = renderTargetTexture.Height,
            MinDepth = 0.0f,
            MaxDepth = 1.0f,
        });
    }

    public void SetTexture(int slot, IResourceTexture texture)
    {
        Context.PixelShader.SetShaderResources(slot, texture.ShaderResourceView);
    }
}
