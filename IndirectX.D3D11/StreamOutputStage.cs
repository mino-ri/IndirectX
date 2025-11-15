namespace IndirectX.D3D11;

partial class DeviceContext
{
    private StreamOutputStage? _streamOutput;
    public StreamOutputStage StreamOutput => _streamOutput ??= new StreamOutputStage(this);

    public class StreamOutputStage
    {
        private readonly DeviceContext _context;
        public StreamOutputStage(DeviceContext context) => _context = context;

        public void SetTargets(params (Buffer soTarget, int offset)[] targets) => _context.SOSetTargets(targets);

        public Buffer[] GetTargets(int getCount) => _context.SOGetTargets(getCount);
    }
}
