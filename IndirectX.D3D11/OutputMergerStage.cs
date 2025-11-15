namespace IndirectX.D3D11;

partial class DeviceContext
{
    private OutputMergerStage? _outputMerger;
    public OutputMergerStage OutputMerger => _outputMerger ??= new OutputMergerStage(this);

    public class OutputMergerStage
    {
        private readonly DeviceContext _context;
        public OutputMergerStage(DeviceContext context) => _context = context;

        public void SetRenderTargets(RenderTargetView[] renderTargetViews, DepthStencilView? depthStencilView) =>
            _context.OMSetRenderTargets(renderTargetViews, depthStencilView);

        public void SetRenderTarget(RenderTargetView renderTargetView, DepthStencilView? depthStencilView) =>
            _context.OMSetRenderTargets(new[] { renderTargetView }, depthStencilView);

        public (RenderTargetView[] renderTargetViews, DepthStencilView depthStencilView) GetRenderTargets(int getCount) =>
            _context.OMGetRenderTargets(getCount);

        public (RenderTargetView renderTargetViews, DepthStencilView depthStencilView) GetRenderTarget()
        {
            var (renders, stencils) = _context.OMGetRenderTargets(1);
            return (renders[0], stencils);
        }

        public void SetRenderTargetsAndUnorderedAccessViews(RenderTargetView[] renderTargetViews, DepthStencilView? depthStencilView, int uavStartSlot, (UnorderedAccessView unorderedAccessView, int initialCount)[] unorderedAccessViews) =>
            _context.OMSetRenderTargetsAndUnorderedAccessViews(renderTargetViews, depthStencilView, uavStartSlot, unorderedAccessViews);

        public (RenderTargetView[] renderTargetViews, DepthStencilView depthStencilView, UnorderedAccessView[] unorderedAccessViews) GetRenderTargetsAndUnorderedAccessViews(int rtvCount, int uavStartSlot, int uavCount) =>
            _context.OMGetRenderTargetsAndUnorderedAccessViews(rtvCount, uavStartSlot, uavCount);

        public void SetBlendState(BlendState? blendState, in Color blendFactor, int sampleMask) =>
            _context.OMSetBlendState(blendState, in blendFactor, sampleMask);

        public (BlendState? blendState, Color blendFactor, int sampleMask) GetBlendState() =>
            _context.OMGetBlendState();

        public void SetDepthStencilState(DepthStencilState? depthStencilState, int stencilRef) =>
            _context.OMSetDepthStencilState(depthStencilState, stencilRef);

        public (DepthStencilState? depthStencilState, int stencilRef) GetDepthStencilState() =>
            _context.OMGetDepthStencilState();

        public (BlendState? blendState, Color blendFactor, int sampleMask) BlendState
        {
            get => GetBlendState();
            set => SetBlendState(value.blendState, in value.blendFactor, value.sampleMask);
        }

        public (DepthStencilState? depthStencilState, int stencilRef) DepthStencilState
        {
            get => GetDepthStencilState();
            set => SetDepthStencilState(value.depthStencilState, value.stencilRef);
        }
    }
}
