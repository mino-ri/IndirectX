using System;
using System.Numerics;
using System.Windows.Forms;
using IndirectX;
using IndirectX.D3D11;
using IndirectX.Dxgi;
using IndirectX.Helper;
using IndirectX.Helper.Forms;

namespace SimpleImage;

class Program
{
    static void Main(string[] args)
    {
        var form = new DirectXHostForm
        {
            ClientSize = new System.Drawing.Size(512, 512),
            Text = "SimpleTriangle",
            MaximizeBox = false,
            FormBorderStyle = FormBorderStyle.FixedSingle,
        };
        var renderer = new TestRenderer(form);
        var renderLoop = new RenderLoop(renderer.Frame);
        form.Shown += (_, __) => renderLoop.Start();
        form.FormClosing += (_, __) =>
        {
            renderLoop.Stop();
            renderer.Dispose();
        };
        Application.Run(form);
    }
}

internal class TestRenderer : IDisposable
{
    private int _count;
    private readonly Graphics _graphics;
    private readonly IResourceTexture _sourceTexture;
    private readonly RenderTargetTexture _targetTexture;
    private Vertex[] _vertices = null!;
    private ushort[] _indices = null!;
    private ArrayBuffer<Vertex> _vertexBuffer = null!;

    public TestRenderer(Form form)
    {
        _graphics = new Graphics(form.Handle, form.ClientSize.Width, form.ClientSize.Height, true, 60, 2, useStencil: true);
        _vertices =
        [
            new Vertex(-200f, -200f, 0.5f, 1f, Color.White, new Vector2(0f, 0f)),
            new Vertex(+200f, -200f, 0.5f, 1f, Color.White, new Vector2(1f, 0f)),
            new Vertex(-200f, +200f, 0.5f, 1f, Color.White, new Vector2(0f, 1f)),
            new Vertex(+200f, +200f, 0.5f, 1f, Color.White, new Vector2(1f, 1f)),
        ];

        _indices =
        [
            0, 1, 2,
            2, 1, 3,
        ];

        _graphics.SetVertexShader(ShaderSource.LoadVertexShader);
        _graphics.SetPixelShader(ShaderSource.LoadPixelShader);
        _graphics.SetInputLayout(ShaderSource.LoadInputLayout,
            new InputElementDesc { SemanticName = "POSITION", Format = Format.R32G32B32A32Float },
            new InputElementDesc { SemanticName = "COLOR", Format = Format.R32G32B32A32Float, AlignedByteOffset = 16 },
            new InputElementDesc { SemanticName = "TEXCOORD", Format = Format.R32G32Float, AlignedByteOffset = 32 });

        _vertexBuffer = _graphics.RegisterVertexBuffer<Vertex>(0, 4);
        _graphics.RegisterConstantBuffer<Matrix4>(0, ShaderStages.VertexShader)
                .WriteByRef(Matrix4.OrtoLH(0f, 1f, 512f, 512f));

        _graphics.RegisterIndexBuffer(16)
                .Write(_indices);

        _targetTexture = _graphics.CreateRenderTargetTexture(256, 256);
        _sourceTexture = _graphics.CreateResourceTexture(ShaderSource.GetStream("image.png"));
        _graphics.SetTexture(0, _sourceTexture);
        _graphics.SetBorderSampler(0);
        using var alphaBlendState = _graphics.CreateAlphaBlendState();
        _graphics.SetBlendState(alphaBlendState);
    }

    public void Frame()
    {
        _count++;
        _graphics.SetRenderTarget(_targetTexture);
        _graphics.SetTexture(0, _sourceTexture);

        _graphics.Clear(Color.White);
        _graphics.Context.ClearRenderTargetView(_targetTexture.RenderTargetView, Color.Black);
        _vertexBuffer.Write(_vertices);
        _graphics.DrawIndexed(6);

        _graphics.ResetRenderTarget();

        _graphics.SetTexture(0, _targetTexture);

        _graphics.Clear(Color.White);
        _vertexBuffer.Write(_vertices);
        _graphics.DrawIndexed(6);
        _graphics.Present();
    }

    public void Dispose()
    {
        _sourceTexture.Dispose();
        _targetTexture.Dispose();
        _graphics.Dispose();
    }
}
