using System;

namespace IndirectX.D3D11;

public partial class Device
{
    internal const int SdkVersion = 7;

    public InputLayout CreateInputLayout(ReadOnlySpan<InputElementDesc> inputElementDescs, ReadOnlySpan<byte> shaderBytecode)
    {
        var interopArray = InputElementDesc.ToInterop(inputElementDescs);
        try
        {
            return CreateInputLayout(interopArray, shaderBytecode);
        }
        finally
        {
            foreach (var interop in interopArray)
                interop.SemanticName.Dispose();
        }
    }

    public GeometryShader CreateGeometryShaderWithStreamOutput(ReadOnlySpan<byte> shaderBytecode, ReadOnlySpan<SoDeclarationEntry> sODeclaration, ReadOnlySpan<int> bufferStrides, int rasterizedStream, ClassLinkage? classLinkage)
    {
        var interopArray = SoDeclarationEntry.ToInterop(sODeclaration);
        try
        {
            return CreateGeometryShaderWithStreamOutputCore(shaderBytecode, interopArray, bufferStrides, rasterizedStream, classLinkage);
        }
        finally
        {
            foreach (var interop in interopArray)
                interop.SemanticName.Dispose();
        }
    }

    public GeometryShader CreateGeometryShaderWithStreamOutput(ReadOnlySpan<byte> shaderBytecode, int rasterizedStream)
    {
        return CreateGeometryShaderWithStreamOutputCore(shaderBytecode, rasterizedStream);
    }

    public static (Device device, DeviceContext immediateContext) Create(
        DriverType driverType,
        CreateDeviceFlags flags,
        Dxgi.Adapter? adapter = null,
        nint software = default,
        ReadOnlySpan<FeatureLevel> featureLevels = default)
    {
        var (device, _, context) = WinApis.CreateDevice(adapter, driverType, software, flags, featureLevels);
        return (device, context);
    }

    public static (Dxgi.SwapChain swapChain, Device device, DeviceContext immediateContext) CreateWithSwapChain(
        DriverType driverType,
        CreateDeviceFlags flags,
        in Dxgi.SwapChainDesc swapChainDesc,
        Dxgi.Adapter? adapter = null,
        nint software = default,
        ReadOnlySpan<FeatureLevel> featureLevels = default)
    {
        var (swapChain, device, _, context) = WinApis.CreateDeviceAndSwapChain(adapter, driverType, software, flags, featureLevels, in swapChainDesc);
        return (swapChain, device, context);
    }
}
