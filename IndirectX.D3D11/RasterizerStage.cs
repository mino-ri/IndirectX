namespace IndirectX.D3D11;

partial class DeviceContext
{
    private RasterizerStage? _rasterizer;
    public RasterizerStage Rasterizer => _rasterizer ??= new RasterizerStage(this);

    public class RasterizerStage
    {
        private readonly DeviceContext _context;
        public RasterizerStage(DeviceContext context) => _context = context;

        public void SetState(RasterizerState? rasterizerState) => _context.RSSetState(rasterizerState);
        public RasterizerState? GetState() => _context.RSGetState();

        public void SetViewports(params Viewport[] viewports) => _context.RSSetViewports(viewports);
        public Viewport[] GetViewports(int getCount) => _context.RSGetViewports(ref getCount)[..getCount];

        public void SetScissorRects(params Rect[] rects) => _context.RSSetScissorRects(rects);
        public Rect[] GetScissorRects(int getCount) => _context.RSGetScissorRects(ref getCount)[..getCount];

        public RasterizerState? State { get => GetState(); set => SetState(value); }
    }
}
