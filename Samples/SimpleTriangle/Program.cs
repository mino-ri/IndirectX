using System;
using System.Windows.Forms;
using IndirectX;
using IndirectX.D3D11;
using IndirectX.Dxgi;
using IndirectX.Helper;
using IndirectX.Helper.Forms;

namespace SimpleTriangle;

class Program
{
    static void Main(string[] args)
    {
        var form = new DirectXHostForm
        {
            ClientSize = new System.Drawing.Size(512, 512),
            Text = "SimpleTriangle",
            MaximizeBox = false,
            FormBorderStyle = FormBorderStyle.Sizable,
        };
        var renderer = new TestRenderer(form);
        var renderLoop = new RenderLoop(renderer.Frame);
        form.Shown += (_, __) => renderLoop.Start();
        form.FormClosing += (_, __) =>
        {
            renderLoop.Stop();
            renderer.Dispose();
        };
        form.ResizeEnd += (_, __) => renderer.OnResize(form.ClientSize);
        Application.Run(form);
    }
}

internal class TestRenderer : IDisposable
{
    private int _count;
    private readonly Graphics _graphics;
    private Vertex[] _vertices = null!;
    private Vertex[] _vertices2 = null!;
    private ushort[] _indices = null!;
    private ArrayBuffer<Vertex> _vertexBuffer = null!;
    private System.Drawing.Size? _changedSize;

    public TestRenderer(Form form)
    {
        _graphics = new Graphics(form.Handle, form.ClientSize.Width, form.ClientSize.Height, true, 60, 2, useStencil: true);
        _vertices =
        [
            new Vertex(0f, 0f, 0.5f, 1f, Color.White),
            new Vertex(-100f, -100f, 0.5f, 1f, Color.White),
            new Vertex(+100f, -100f, 0.5f, 1f, new Color(1f, 0f, 0f)),
            new Vertex(+100f, +100f, 0.5f, 1f, Color.White),
            new Vertex(-100f, +100f, 0.5f, 1f, Color.White),
        ];

        _vertices2 =
        [
            new Vertex(100f, 100f, 0.5f, 1f, new Color(1f, 1f, 0f)),
            new Vertex(100f, 150f, 0.5f, 1f, new Color(1f, 1f, 0f)),
            new Vertex(150f, 100f, 0.5f, 1f, new Color(1f, 1f, 0f)),
            new Vertex(150f, 150f, 0.5f, 1f, new Color(1f, 1f, 0f)),
            new Vertex(),
        ];

        _indices =
        [
            0, 1, 2,
            0, 2, 3,
            0, 3, 4,
            0, 4, 1,
        ];

        _graphics.SetVertexShader(ShaderSource.LoadVertexShader);
        _graphics.SetPixelShader(ShaderSource.LoadPixelShader);
        _graphics.SetInputLayout(ShaderSource.LoadInputLayout,
            new InputElementDesc { SemanticName = "POSITION", Format = Format.R32G32B32A32Float },
            new InputElementDesc { SemanticName = "COLOR", Format = Format.R32G32B32A32Float, AlignedByteOffset = 16 });

        _vertexBuffer = _graphics.RegisterVertexBuffer<Vertex>(0, 8);
        _graphics.RegisterConstantBuffer<Matrix4>(0, ShaderStages.VertexShader)
                .WriteByRef(Matrix4.OrtoLH(0f, 1f, 512f, 512f));

        _graphics.RegisterIndexBuffer(16)
                .Write(_indices);
    }

    public void Frame()
    {
        if (_changedSize.HasValue)
        {
            var size = _changedSize.Value;
            _changedSize = null;
            _graphics.Resize(size.Width, size.Height);
        }

        _count++;
        _graphics.Clear((_count % 3) switch
        {
            0 => new Color(1.0f, 0.125f, 0.0f, 0.0f),
            1 => new Color(1.0f, 0.0f, 0.125f, 0.0f),
            _ => new Color(1.0f, 0.0f, 0.0f, 0.125f),
        });
        _vertexBuffer.Write(_vertices);
        _graphics.DrawIndexed(12);
        _vertexBuffer.Write(_vertices2);
        _graphics.DrawStrip(4);
        _graphics.Present();
    }

    public void OnResize(System.Drawing.Size newSize)
    {
        _changedSize = newSize;
    }

    public void Dispose() => _graphics.Dispose();
}
