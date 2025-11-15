using System.Collections;
using System.Collections.Generic;

namespace IndirectX.D3D11;

partial class DeviceContext
{
    private InputAssemblerStage? _inputAssembler;
    public InputAssemblerStage InputAssembler => _inputAssembler ??= new InputAssemblerStage(this);

    public class InputAssemblerStage
    {
        public const int VertexInputResourceSlotCount = 32;

        private readonly DeviceContext _context;
        public InputAssemblerStage(DeviceContext context) => _context = context;

        public void SetInputLayout(InputLayout? inputLayout) => _context.IASetInputLayout(inputLayout);
        public InputLayout? GetInputLayout() => _context.IAGetInputLayout();

        public void SetVertexBuffer(int slot, Buffer vertexBuffer, int stride, int offset) =>
            _context.IASetVertexBuffer(slot, vertexBuffer, stride, offset);

        public void SetVertexBuffers(int startSlot, params (Buffer vertexBuffer, int stride, int offset)[] vertexBufferDeclarations) =>
            _context.IASetVertexBuffers(startSlot, vertexBufferDeclarations);

        public (Buffer vertexBuffer, int stride, int offset) GetVertexBuffer(int slot) =>
            _context.IAGetVertexBuffer(slot);

        public (Buffer vertexBuffer, int stride, int offset)[] GetVertexBuffers(int startSlot, int numBuffers) =>
            _context.IAGetVertexBuffers(startSlot, numBuffers);

        public void SetIndexBuffer(Buffer? indexBuffer, Dxgi.Format format, int offset) =>
            _context.IASetIndexBuffer(indexBuffer, format, offset);

        public (Buffer? indexBuffer, Dxgi.Format format, int offset) GetIndexBuffer() => _context.IAGetIndexBuffer();

        public void SetPrimitiveTopology(PrimitiveTopology topology) => _context.IASetPrimitiveTopology(topology);

        public PrimitiveTopology IAGetPrimitiveTopology() => _context.IAGetPrimitiveTopology();

        public InputLayout? InputLayout { get => GetInputLayout(); set => SetInputLayout(value); }

        public (Buffer? indexBuffer, Dxgi.Format format, int offset) IndexBuffer
        {
            get => GetIndexBuffer(); set => SetIndexBuffer(value.indexBuffer, value.format, value.offset);
        }

        public PrimitiveTopology PrimitiveTopology { get => IAGetPrimitiveTopology(); set => SetPrimitiveTopology(value); }

        private VertexBufferSlotSelector? _vertexBuffers;
        public VertexBufferSlotSelector VertexBuffers => _vertexBuffers ??= new VertexBufferSlotSelector(this);

        public class VertexBufferSlotSelector : IReadOnlyList<(Buffer vertexBuffer, int stride, int offset)>
        {
            private readonly InputAssemblerStage _inputAssembler;
            public VertexBufferSlotSelector(InputAssemblerStage inputAssembler) => _inputAssembler = inputAssembler;

            public int Count => InputAssemblerStage.VertexInputResourceSlotCount;

            public (Buffer vertexBuffer, int stride, int offset) this[int slot]
            {
                get => _inputAssembler.GetVertexBuffer(slot);
                set => _inputAssembler.SetVertexBuffer(slot, value.vertexBuffer, value.stride, value.offset);
            }

            public IEnumerator<(Buffer vertexBuffer, int stride, int offset)> GetEnumerator() =>
                ((IEnumerable<(Buffer vertexBuffer, int stride, int offset)>)_inputAssembler.GetVertexBuffers(0, Count)).GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
