using System;
using System.Numerics;
using IndirectX.D3D11;
using IndirectX.Dxgi;
using IndirectX.Helper;
using IndirectX;
using IndirectX.Helper.Wpf;

namespace WpfImage;

internal class TestRenderer : IDisposable
{
    private int _count;
    private readonly Graphics _graphics;
    private readonly Vertex[] _vertices;
    private readonly ArrayBuffer<Vertex> _vertexBuffer;

    public TestRenderer(DirectXHost host)
    {
        var (width, height) = host.GetActualPixelSize();
        _graphics = new Graphics(host.Handle, width, height, true, 60, 2, useStencil: true, sampleCount: 16);

        _vertices =
        [
                new Vertex(-250f, -200f, 0.5f, 1f, Color.White),
                new Vertex(+200f, -250f, 0.5f, 1f, Color.White),
                new Vertex(-200f, +250f, 0.5f, 1f, Color.White),
                new Vertex(+250f, +200f, 0.5f, 1f, Color.White),
        ];

        var texCoords = new[]
        {
            new Vector2(0f, 0f),
            new Vector2(1f, 0f),
            new Vector2(0f, 1f),
            new Vector2(1f, 1f),
        };

        var indices = new ushort[]
        {
                0, 1, 2,
                2, 1, 3,
        };

        _graphics.SetVertexShader(ShaderSource.LoadVertexShader);
        _graphics.SetPixelShader(ShaderSource.LoadPixelShader);
        _graphics.SetInputLayout(ShaderSource.LoadInputLayout,
            new InputElementDesc { SemanticName = "POSITION", Format = Format.R32G32B32A32Float },
            new InputElementDesc { SemanticName = "COLOR", Format = Format.R32G32B32A32Float, AlignedByteOffset = 16 },
            new InputElementDesc { SemanticName = "TEXCOORD", Format = Format.R32G32Float, InputSlot = 1 });

        _vertexBuffer = _graphics.RegisterVertexBuffer<Vertex>(0, 4);
        _graphics.RegisterVertexBufferWriter<Vector2>(1, 4)
            .Write(texCoords);
        _graphics.RegisterConstantBuffer<Matrix4>(0, ShaderStages.VertexShader)
            .WriteByRef(Matrix4.OrtoLH(0f, 1f, 512f, 512f));
        _graphics.RegisterIndexBufferWriter(6)
            .Write(indices);

        using var texture = _graphics.CreateResourceTexture(ShaderSource.GetStream("image.png"));
        _graphics.SetTexture(0, texture);
        _graphics.SetBorderSampler(0);
        using var alphaBlendState = _graphics.CreateAlphaBlendState();
        _graphics.SetBlendState(alphaBlendState);
    }

    public void Frame()
    {
        _count++;
        _graphics.Clear(Color.Black);
        _vertexBuffer.Write(_vertices);
        _graphics.DrawIndexed(6);
        _graphics.Present();
    }

    public void Dispose() => _graphics.Dispose();
}
