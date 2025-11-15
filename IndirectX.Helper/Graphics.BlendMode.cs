using IndirectX.D3D11;

namespace IndirectX.Helper;

public sealed partial class Graphics
{
    public BlendState CreateBlendState(Blend sourceBlend, Blend destinationBlend, BlendOp operation = BlendOp.Add)
    {
        return Device.CreateBlendState(new BlendDesc
        {
            AlphaToCoverageEnable = false,
            IndependentBlendEnable = false,
            RenderTarget0 =
            {
                BlendEnable = true,
                RenderTargetWriteMask = (byte)ColorWriteEnable.All,
                BlendOp = operation,
                SrcBlend = sourceBlend,
                DestBlend = destinationBlend,
                BlendOpAlpha = operation,
                SrcBlendAlpha = sourceBlend,
                DestBlendAlpha = destinationBlend,
            },
        });
    }

    public void SetBlendState(BlendState? blendState)
    {
        Context.OutputMerger.SetBlendState(blendState, default, -1);
    }

    public BlendState CreateAlphaBlendState() => CreateBlendState(Blend.SrcAlpha, Blend.InvSrcAlpha);

    public BlendState CreateAddBlendState() => CreateBlendState(Blend.SrcAlpha, Blend.One);

    public BlendState CreateScreenBlendState() => CreateBlendState(Blend.InvDestColor, Blend.One);
}
