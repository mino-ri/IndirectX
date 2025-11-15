using System;
using System.Collections.Generic;
using IndirectX.Dxgi;
using IndirectX.D3D11;
using Device = IndirectX.D3D11.Device;

namespace IndirectX.Helper;

public sealed partial class Graphics : IDisposable
{
    public const Format DisplayFormat = Format.B8G8R8A8UNorm;

    private readonly int _syncInterval;
    private readonly List<IDisposable> _disposables;
    private readonly bool _useZBuffer;
    private readonly bool _useStencil;
    private int _width;
    private int _height;
    private SampleDesc _sampleDesc;
    private bool _isDisposed;

    public Device Device { get; }
    public SwapChain SwapChain { get; }
    public DeviceContext Context { get; }
    public RenderTargetView RenderView { get; private set; }
    public DepthStencilView? DepthView { get; private set; }

    public Graphics(
        nint surfaceHandle,
        int width,
        int height,
        bool windowed,
        int refreshRate = 60,
        int syncInterval = 1,
        ModeScaling scalingMode = ModeScaling.Unspecified,
        CullMode cullMode = CullMode.None,
        FillMode fillMode = FillMode.Solid,
        bool useZBuffer = true,
        bool useStencil = false,
        int sampleCount = 1)
    {
        _disposables = [];
        _syncInterval = syncInterval;
        _useZBuffer = useZBuffer;
        _useStencil = useStencil;
        _width = width;
        _height = height;

        (Device, Context) = Device.Create(DriverType.Hardware, CreateDeviceFlags.None);

        _sampleDesc = CreateSampleDesc(Device, sampleCount);

        using (var factory = new Dxgi.Factory())
        {
            factory.MakeWindowAssociation(surfaceHandle, WindowAssociationFlags.IgnoreAll);
            SwapChain = CreateSwapChain(Device, width, height, surfaceHandle, windowed, refreshRate, scalingMode, _sampleDesc, factory);
        }

        // output merger
        RenderView = CreateRenderTargetView(Device, SwapChain);

        if (useZBuffer)
        {
            DepthView = CreateDepthView(Device, width, height, useStencil, _sampleDesc);

            if (useStencil)
            {
                using var stencilState = CreateStencilState(Device);
                SetDepthStencilState(stencilState, 0);
            }
        }

        using (var state = Device.CreateRasterizerState(new RasterizerDesc
        {
            CullMode = cullMode,
            FillMode = fillMode,
            DepthClipEnable = true,
            AntialiasedLineEnable = _sampleDesc.Count > 1,
            MultisampleEnable = _sampleDesc.Count > 1,
        }))
        {
            Context.Rasterizer.State = state;
        }

        // rasterizer
        Context.Rasterizer.SetViewports(new Viewport
        {
            Width = _width,
            Height = _height,
            MinDepth = 0.0f,
            MaxDepth = 1.0f,
        });

        Context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleStrip;

        Context.OutputMerger.SetRenderTarget(RenderView, DepthView);
    }

    private static DepthStencilState CreateStencilState(Device device)
    {
        return device.CreateDepthStencilState(new DepthStencilDesc
        {
            DepthEnable = true,
            StencilEnable = false,
            DepthWriteMask = DepthWriteMask.All,
            DepthFunc = ComparisonFunc.Less,
            StencilReadMask = 0xff,
            StencilWriteMask = 0xff,
            FrontFace = new DepthStencilopDesc
            {
                StencilFunc = ComparisonFunc.Always,
                StencilPassOp = StencilOp.Invert,
                StencilFailOp = StencilOp.Keep,
                StencilDepthFailOp = StencilOp.Invert,
            },
            BackFace =
            {
                StencilFunc = ComparisonFunc.Always,
                StencilPassOp = StencilOp.Invert,
                StencilFailOp = StencilOp.Keep,
                StencilDepthFailOp = StencilOp.Invert,
            },
        });
    }

    private static DepthStencilView CreateDepthView(Device device, int width, int height, bool useStencil, SampleDesc sampleDesc)
    {
        using var depthBuffer = device.CreateTexture2D(new Texture2DDesc
        {
            Format = useStencil ? Format.D24UNormS8UInt : Format.D32Float,
            ArraySize = 1,
            MipLevels = 1,
            Width = width,
            Height = height,
            SampleDesc = sampleDesc,
            BindFlags = BindFlags.DepthStencil,
        });

        return device.CreateDepthStencilView(depthBuffer);
    }

    private static SwapChain CreateSwapChain(Device device, int width, int height, nint surfaceHandle, bool windowed, int refreshRate, ModeScaling scalingMode, SampleDesc sampleDesc, Factory factory)
    {
        return factory.CreateSwapChain(device, new SwapChainDesc
        {
            BufferCount = 1,
            BufferDesc =
                {
                    Width = width,
                    Height = height,
                    RefreshRate = { Numerator = refreshRate, Denominator = 1 },
                    Format = DisplayFormat,
                    Scaling = scalingMode,
                    ScanlineOrdering = ModeScanlineOrder.Unspecified,
                },
            Windowed = windowed,
            OutputWindow = surfaceHandle,
            SampleDesc = sampleDesc,
            SwapEffect = SwapEffect.Discard,
            BufferUsage = Dxgi.Usage.RenderTargetOutput,
            Flags = SwapChainFlags.None,
        });
    }

    /// <summary>バックバッファに描画した内容を反映します。</summary>
    public void Present() => SwapChain.Present(_syncInterval, PresentFlags.None);

    public InputLayout CreateInputLayout(ReadOnlySpan<byte> bytecode, params InputElementDesc[] descs)
    {
        return Device.CreateInputLayout(descs, bytecode);
    }

    public void SetInputLayout(Func<Device, InputElementDesc[], InputLayout> layoutLoader, params InputElementDesc[] descs)
    {
        using var layout = layoutLoader(Device, descs);
        SetInputLayout(layout);
    }

    public void SetInputLayout(InputLayout layout)
    {
        Context.InputAssembler.InputLayout = layout;
    }

    public void SetInputLayout(ReadOnlySpan<byte> bytecode, params InputElementDesc[] descs)
    {
        using var layout = CreateInputLayout(bytecode, descs);
        SetInputLayout(layout);
    }

    public DepthStencilState CreateDepthStencilState(in DepthStencilDesc desc) => Device.CreateDepthStencilState(in desc);

    public void SetDepthStencilState(DepthStencilState state, int stencilRef) =>
        Context.OutputMerger.SetDepthStencilState(state, stencilRef);

    public void Resize(int width, int height)
    {
        RenderView.Dispose();
        DepthView?.Dispose();
        _width = width;
        _height = height;

        SwapChain.ResizeBuffers(1, width, height, DisplayFormat, (int)SwapChainFlags.None);

        // output merger
        RenderView = CreateRenderTargetView(Device, SwapChain);

        if (_useZBuffer)
        {
            DepthView = CreateDepthView(Device, width, height, _useStencil, _sampleDesc);

            if (_useStencil)
            {
                using var stencilState = CreateStencilState(Device);
                SetDepthStencilState(stencilState, 0);
            }
        }

        // rasterizer
        Context.Rasterizer.SetViewports(new Viewport
        {
            Width = width,
            Height = height,
            MinDepth = 0.0f,
            MaxDepth = 1.0f,
        });

        Context.OutputMerger.SetRenderTarget(RenderView, DepthView);
    }

    private static SampleDesc CreateSampleDesc(Device device, int sampleCount)
    {
        var sampleDesc = new SampleDesc { Count = sampleCount, Quality = 0 };
        for (; sampleDesc.Count > 1; sampleDesc.Count--)
        {
            var quality = device.CheckMultisampleQualityLevels(DisplayFormat, sampleCount);
            if (quality >= 1)
            {
                sampleDesc.Quality = quality - 1;
                break;
            }
        }

        return sampleDesc;
    }

    private static RenderTargetView CreateRenderTargetView(Device device, SwapChain swapChain)
    {
        using var backBuffer = swapChain.GetBuffer<Texture2D>(0);
        return device.CreateRenderTargetView(backBuffer);
    }

    public void Dispose()
    {
        if (_isDisposed) return;
        _isDisposed = true;

        foreach (var buffer in _disposables) buffer.Dispose();
        RenderView.Dispose();
        DepthView?.Dispose();
        if (Context != null)
        {
            Context.ClearState();
            Context.Flush();
            Context.Dispose();
        }
        SwapChain?.Dispose();
        Device?.Dispose();

        GC.SuppressFinalize(this);
    }
}
