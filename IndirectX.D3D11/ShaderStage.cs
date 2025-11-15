using System;
using System.Collections;
using System.Collections.Generic;

namespace IndirectX.D3D11;

partial class DeviceContext
{
    public abstract class ShaderStage
    {
        public const int ConstantBufferApiSlotCount = 14;
        public const int ConstantBufferComponents = 4;
        public const int ConstantBufferComponentBitCount = 32;
        public const int ConstantBufferHwSlotCount = 15;
        public const int ConstantBufferRegisterComponents = 4;
        public const int ConstantBufferRegisterCount = 15;
        public const int ConstantBufferRegisterReadsPerInst = 1;
        public const int ConstantBufferRegisterReadPorts = 1;

        public const int FlowcontrolNestingLimit = 64;
        public const int ImmediateConstantBufferRegisterComponents = 4;
        public const int ImmediateConstantBufferRegisterCount = 1;
        public const int ImmediateConstantBufferRegisterReadsPerInst = 1;
        public const int ImmediateConstantBufferRegisterReadPorts = 1;
        public const int ImmediateValueComponentBitCount = 32;
        public const int InputResourceRegisterComponents = 1;
        public const int InputResourceRegisterCount = 128;
        public const int InputResourceRegisterReadsPerInst = 1;
        public const int InputResourceRegisterReadPorts = 1;
        public const int InputResourceSlotCount = 128;
        public const int SamplerRegisterComponents = 1;
        public const int SamplerRegisterCount = 16;
        public const int SamplerRegisterReadsPerInst = 1;
        public const int SamplerRegisterReadPorts = 1;
        public const int SamplerSlotCount = 16;
        public const int SubroutineNestingLimit = 32;
        public const int TempRegisterComponents = 4;
        public const int TempRegisterComponentBitCount = 32;
        public const int TempRegisterCount = 4096;
        public const int TempRegisterReadsPerInst = 3;
        public const int TempRegisterReadPorts = 3;
        public const int TexcoordRangeReductionMax = 10;
        public const int TexcoordRangeReductionMin = -10;
        public const int TexelOffsetMaxNegative = -8;
        public const int TexelOffsetMaxPositive = 7;

        public abstract void SetConstantBuffers(int startSlot, params Buffer[] constantBuffers);
        public abstract void SetShaderResources(int startSlot, params ShaderResourceView[] shaderResourceViews);
        public abstract void SetSamplers(int startSlot, params SamplerState[] samplers);

        public abstract Buffer[] GetConstantBuffers(int startSlot, int count);
        public abstract ShaderResourceView[] GetShaderResources(int startSlot, int count);
        public abstract SamplerState[] GetSamplers(int startSlot, int count);

        private ConstantBufferSlotSelector? _constantBuffers;
        public ConstantBufferSlotSelector ConstantBuffers => _constantBuffers ??= new ConstantBufferSlotSelector(this);

        private ShaderResourceSlotSelector? _shaderResources;
        public ShaderResourceSlotSelector ShaderResources => _shaderResources ??= new ShaderResourceSlotSelector(this);

        private SamplerSlotSelector? _samplers;
        public SamplerSlotSelector Samplers => _samplers ??= new SamplerSlotSelector(this);

        public class ConstantBufferSlotSelector : IReadOnlyList<Buffer>
        {
            private readonly ShaderStage _shaderStage;
            public ConstantBufferSlotSelector(ShaderStage shaderStage) => _shaderStage = shaderStage;

            public int Count => ShaderStage.ConstantBufferApiSlotCount;

            public Buffer this[int slot]
            {
                get => _shaderStage.GetConstantBuffers(slot, 1)[0];
                set => _shaderStage.SetConstantBuffers(slot, value);
            }

            public IEnumerator<Buffer> GetEnumerator() => ((IEnumerable<Buffer>)_shaderStage.GetConstantBuffers(0, Count)).GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        public class ShaderResourceSlotSelector : IReadOnlyList<ShaderResourceView>
        {
            private readonly ShaderStage _shaderStage;
            public ShaderResourceSlotSelector(ShaderStage shaderStage) => _shaderStage = shaderStage;

            public int Count => ShaderStage.InputResourceSlotCount;

            public ShaderResourceView this[int slot]
            {
                get => _shaderStage.GetShaderResources(slot, 1)[0];
                set => _shaderStage.SetShaderResources(slot, value);
            }

            public IEnumerator<ShaderResourceView> GetEnumerator() => ((IEnumerable<ShaderResourceView>)_shaderStage.GetShaderResources(0, Count)).GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        public class SamplerSlotSelector : IReadOnlyList<SamplerState>
        {
            private readonly ShaderStage _shaderStage;
            public SamplerSlotSelector(ShaderStage shaderStage) => _shaderStage = shaderStage;

            public int Count => ShaderStage.SamplerSlotCount;

            public SamplerState this[int slot]
            {
                get => _shaderStage.GetSamplers(slot, 1)[0];
                set => _shaderStage.SetSamplers(slot, value);
            }

            public IEnumerator<SamplerState> GetEnumerator() => ((IEnumerable<SamplerState>)_shaderStage.GetSamplers(0, Count)).GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }

    partial class ComputeShaderStage
    {
        public UnorderedAccessView[] GetUnorderedAccessViews(int startSlot, int count) => _context.CSGetUnorderedAccessViews(startSlot, count);
        public void SetUnorderedAccessViews(int startSlot, UnorderedAccessView[] unorderedAccessViews, ReadOnlySpan<int> uavInitialCounts) =>
            _context.CSSetUnorderedAccessViews(startSlot, unorderedAccessViews, uavInitialCounts);
    }
}

partial class DeviceContext
{
    private ComputeShaderStage? _computeshader;
    public ComputeShaderStage ComputeShader => _computeshader ??= new ComputeShaderStage(this);

    private DomainShaderStage? _domainshader;
    public DomainShaderStage DomainShader => _domainshader ??= new DomainShaderStage(this);

    private HullShaderStage? _hullshader;
    public HullShaderStage HullShader => _hullshader ??= new HullShaderStage(this);

    private GeometryShaderStage? _geometryshader;
    public GeometryShaderStage GeometryShader => _geometryshader ??= new GeometryShaderStage(this);

    private PixelShaderStage? _pixelshader;
    public PixelShaderStage PixelShader => _pixelshader ??= new PixelShaderStage(this);

    private VertexShaderStage? _vertexshader;
    public VertexShaderStage VertexShader => _vertexshader ??= new VertexShaderStage(this);

    public partial class ComputeShaderStage : ShaderStage
    {
        private readonly DeviceContext _context;

        public ComputeShaderStage(DeviceContext context) => _context = context;

        public override Buffer[] GetConstantBuffers(int startSlot, int count) => _context.CSGetConstantBuffers(startSlot, count);

        public override SamplerState[] GetSamplers(int startSlot, int count) => _context.CSGetSamplers(startSlot, count);

        public override ShaderResourceView[] GetShaderResources(int startSlot, int count) => _context.CSGetShaderResources(startSlot, count);

        public override void SetConstantBuffers(int startSlot, params Buffer[] constantBuffers) => _context.CSSetConstantBuffers(startSlot, constantBuffers);

        public override void SetSamplers(int startSlot, params SamplerState[] samplers) => _context.CSSetSamplers(startSlot, samplers);

        public override void SetShaderResources(int startSlot, params ShaderResourceView[] shaderResourceViews) => _context.CSSetShaderResources(startSlot, shaderResourceViews);

        public void SetShader(ComputeShader? shader, params ClassInstance[] classInstances) => _context.CSSetShader(shader, classInstances);

        public (ComputeShader? shader, ClassInstance[] classInstances) GetShader(int classInstanceCount)
        {
            var (shader, classInstances) = _context.CSGetShader(ref classInstanceCount);
            return (shader, classInstances[..classInstanceCount]);
        }

        public ComputeShader? Shader
        {
            get
            {
                var count = 0;
                var (shader, _) = _context.CSGetShader(ref count);
                return shader;
            }
            set => _context.CSSetShader(value, Array.Empty<ClassInstance>());
        }
    }

    public partial class DomainShaderStage : ShaderStage
    {
        private readonly DeviceContext _context;

        public DomainShaderStage(DeviceContext context) => _context = context;

        public override Buffer[] GetConstantBuffers(int startSlot, int count) => _context.DSGetConstantBuffers(startSlot, count);

        public override SamplerState[] GetSamplers(int startSlot, int count) => _context.DSGetSamplers(startSlot, count);

        public override ShaderResourceView[] GetShaderResources(int startSlot, int count) => _context.DSGetShaderResources(startSlot, count);

        public override void SetConstantBuffers(int startSlot, params Buffer[] constantBuffers) => _context.DSSetConstantBuffers(startSlot, constantBuffers);

        public override void SetSamplers(int startSlot, params SamplerState[] samplers) => _context.DSSetSamplers(startSlot, samplers);

        public override void SetShaderResources(int startSlot, params ShaderResourceView[] shaderResourceViews) => _context.DSSetShaderResources(startSlot, shaderResourceViews);

        public void SetShader(DomainShader? shader, params ClassInstance[] classInstances) => _context.DSSetShader(shader, classInstances);

        public (DomainShader? shader, ClassInstance[] classInstances) GetShader(int classInstanceCount)
        {
            var (shader, classInstances) = _context.DSGetShader(ref classInstanceCount);
            return (shader, classInstances[..classInstanceCount]);
        }

        public DomainShader? Shader
        {
            get
            {
                var count = 0;
                var (shader, _) = _context.DSGetShader(ref count);
                return shader;
            }
            set => _context.DSSetShader(value, Array.Empty<ClassInstance>());
        }
    }

    public partial class HullShaderStage : ShaderStage
    {
        private readonly DeviceContext _context;

        public HullShaderStage(DeviceContext context) => _context = context;

        public override Buffer[] GetConstantBuffers(int startSlot, int count) => _context.HSGetConstantBuffers(startSlot, count);

        public override SamplerState[] GetSamplers(int startSlot, int count) => _context.HSGetSamplers(startSlot, count);

        public override ShaderResourceView[] GetShaderResources(int startSlot, int count) => _context.HSGetShaderResources(startSlot, count);

        public override void SetConstantBuffers(int startSlot, params Buffer[] constantBuffers) => _context.HSSetConstantBuffers(startSlot, constantBuffers);

        public override void SetSamplers(int startSlot, params SamplerState[] samplers) => _context.HSSetSamplers(startSlot, samplers);

        public override void SetShaderResources(int startSlot, params ShaderResourceView[] shaderResourceViews) => _context.HSSetShaderResources(startSlot, shaderResourceViews);

        public void SetShader(HullShader? shader, params ClassInstance[] classInstances) => _context.HSSetShader(shader, classInstances);

        public (HullShader? shader, ClassInstance[] classInstances) GetShader(int classInstanceCount)
        {
            var (shader, classInstances) = _context.HSGetShader(ref classInstanceCount);
            return (shader, classInstances[..classInstanceCount]);
        }

        public HullShader? Shader
        {
            get
            {
                var count = 0;
                var (shader, _) = _context.HSGetShader(ref count);
                return shader;
            }
            set => _context.HSSetShader(value, Array.Empty<ClassInstance>());
        }
    }

    public partial class GeometryShaderStage : ShaderStage
    {
        private readonly DeviceContext _context;

        public GeometryShaderStage(DeviceContext context) => _context = context;

        public override Buffer[] GetConstantBuffers(int startSlot, int count) => _context.GSGetConstantBuffers(startSlot, count);

        public override SamplerState[] GetSamplers(int startSlot, int count) => _context.GSGetSamplers(startSlot, count);

        public override ShaderResourceView[] GetShaderResources(int startSlot, int count) => _context.GSGetShaderResources(startSlot, count);

        public override void SetConstantBuffers(int startSlot, params Buffer[] constantBuffers) => _context.GSSetConstantBuffers(startSlot, constantBuffers);

        public override void SetSamplers(int startSlot, params SamplerState[] samplers) => _context.GSSetSamplers(startSlot, samplers);

        public override void SetShaderResources(int startSlot, params ShaderResourceView[] shaderResourceViews) => _context.GSSetShaderResources(startSlot, shaderResourceViews);

        public void SetShader(GeometryShader? shader, params ClassInstance[] classInstances) => _context.GSSetShader(shader, classInstances);

        public (GeometryShader? shader, ClassInstance[] classInstances) GetShader(int classInstanceCount)
        {
            var (shader, classInstances) = _context.GSGetShader(ref classInstanceCount);
            return (shader, classInstances[..classInstanceCount]);
        }

        public GeometryShader? Shader
        {
            get
            {
                var count = 0;
                var (shader, _) = _context.GSGetShader(ref count);
                return shader;
            }
            set => _context.GSSetShader(value, Array.Empty<ClassInstance>());
        }
    }

    public partial class PixelShaderStage : ShaderStage
    {
        private readonly DeviceContext _context;

        public PixelShaderStage(DeviceContext context) => _context = context;

        public override Buffer[] GetConstantBuffers(int startSlot, int count) => _context.PSGetConstantBuffers(startSlot, count);

        public override SamplerState[] GetSamplers(int startSlot, int count) => _context.PSGetSamplers(startSlot, count);

        public override ShaderResourceView[] GetShaderResources(int startSlot, int count) => _context.PSGetShaderResources(startSlot, count);

        public override void SetConstantBuffers(int startSlot, params Buffer[] constantBuffers) => _context.PSSetConstantBuffers(startSlot, constantBuffers);

        public override void SetSamplers(int startSlot, params SamplerState[] samplers) => _context.PSSetSamplers(startSlot, samplers);

        public override void SetShaderResources(int startSlot, params ShaderResourceView[] shaderResourceViews) => _context.PSSetShaderResources(startSlot, shaderResourceViews);

        public void SetShader(PixelShader? shader, params ClassInstance[] classInstances) => _context.PSSetShader(shader, classInstances);

        public (PixelShader? shader, ClassInstance[] classInstances) GetShader(int classInstanceCount)
        {
            var (shader, classInstances) = _context.PSGetShader(ref classInstanceCount);
            return (shader, classInstances[..classInstanceCount]);
        }

        public PixelShader? Shader
        {
            get
            {
                var count = 0;
                var (shader, _) = _context.PSGetShader(ref count);
                return shader;
            }
            set => _context.PSSetShader(value, Array.Empty<ClassInstance>());
        }
    }

    public partial class VertexShaderStage : ShaderStage
    {
        private readonly DeviceContext _context;

        public VertexShaderStage(DeviceContext context) => _context = context;

        public override Buffer[] GetConstantBuffers(int startSlot, int count) => _context.VSGetConstantBuffers(startSlot, count);

        public override SamplerState[] GetSamplers(int startSlot, int count) => _context.VSGetSamplers(startSlot, count);

        public override ShaderResourceView[] GetShaderResources(int startSlot, int count) => _context.VSGetShaderResources(startSlot, count);

        public override void SetConstantBuffers(int startSlot, params Buffer[] constantBuffers) => _context.VSSetConstantBuffers(startSlot, constantBuffers);

        public override void SetSamplers(int startSlot, params SamplerState[] samplers) => _context.VSSetSamplers(startSlot, samplers);

        public override void SetShaderResources(int startSlot, params ShaderResourceView[] shaderResourceViews) => _context.VSSetShaderResources(startSlot, shaderResourceViews);

        public void SetShader(VertexShader? shader, params ClassInstance[] classInstances) => _context.VSSetShader(shader, classInstances);

        public (VertexShader? shader, ClassInstance[] classInstances) GetShader(int classInstanceCount)
        {
            var (shader, classInstances) = _context.VSGetShader(ref classInstanceCount);
            return (shader, classInstances[..classInstanceCount]);
        }

        public VertexShader? Shader
        {
            get
            {
                var count = 0;
                var (shader, _) = _context.VSGetShader(ref count);
                return shader;
            }
            set => _context.VSSetShader(value, Array.Empty<ClassInstance>());
        }
    }
}
