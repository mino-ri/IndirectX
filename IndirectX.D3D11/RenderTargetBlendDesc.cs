using System;
using IndirectX.Interop;
using System.Runtime.InteropServices;

namespace IndirectX.D3D11;

[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct BlendDesc
{
    public Bool AlphaToCoverageEnable;
    public Bool IndependentBlendEnable;
    public RenderTargetBlendDesc RenderTarget0;
    public RenderTargetBlendDesc RenderTarget1;
    public RenderTargetBlendDesc RenderTarget2;
    public RenderTargetBlendDesc RenderTarget3;
    public RenderTargetBlendDesc RenderTarget4;
    public RenderTargetBlendDesc RenderTarget5;
    public RenderTargetBlendDesc RenderTarget6;
    public RenderTargetBlendDesc RenderTarget7;
}
