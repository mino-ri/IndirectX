using System;
using System.Numerics;
using IndirectX;
using IndirectX.D3D11;
using IndirectX.Dxgi;
using IndirectX.Helper;
using IndirectX.Helper.Wpf;

namespace WpfCube.Models;

internal class TestRenderer : IDisposable
{
    private int _count;
    private float _zFactor = 0.2f;
    private float _zNear = -4f;
    private float _zFar = 4f;
    private readonly Graphics _graphics;
    private readonly Vertex[] _vertices;
    private readonly ushort[] _indices;
    private readonly ArrayBuffer<Vertex> _vertexBuffer;
    private readonly ArrayBuffer<Matrix4> _matrixBuffer;
    private readonly ValueBuffer<Light> _lightBuffer;
    private Matrix4 _scaling = Matrix4.Identity;
    private ref Matrix4 World => ref _matrixBuffer.Buffer[0];
    private ref Matrix4 WorldViewProj => ref _matrixBuffer.Buffer[1];
    private Matrix4 _view;
    private Matrix4 _proj;

    private readonly ChangeBox _dummy = new();
    private readonly ChangeBox _lightChange = new();
    private readonly ChangeBox _projectionChange = new();

    public FloatViewModel[] Parameters { get; }

    public TestRenderer(DirectXHost host)
    {
        var (width, height) = host.GetActualPixelSize();
        _graphics = new Graphics(host.Handle, width, height, true, 60, cullMode: CullMode.Back, sampleCount: 16);
        _vertices =
        [
            new Vertex(-1f, -1f, -1f, 1f, new Color(0xFFFF9900)),
            new Vertex(+1f, -1f, -1f, 1f, new Color(0xFFFF9900)),
            new Vertex(-1f, +1f, -1f, 1f, new Color(0xFFFF9900)),
            new Vertex(+1f, +1f, -1f, 1f, new Color(0xFFFF9900)),

            new Vertex(-1f, -1f, +1f, 1f, new Color(0xFFF23818)),
            new Vertex(+1f, -1f, +1f, 1f, new Color(0xFFF23818)),
            new Vertex(-1f, +1f, +1f, 1f, new Color(0xFFF23818)),
            new Vertex(+1f, +1f, +1f, 1f, new Color(0xFFF23818)),

            new Vertex(-1f, -1f, -1f, 1f, new Color(0xFFFAF500)),
            new Vertex(+1f, -1f, -1f, 1f, new Color(0xFFFAF500)),
            new Vertex(-1f, -1f, +1f, 1f, new Color(0xFFFAF500)),
            new Vertex(+1f, -1f, +1f, 1f, new Color(0xFFFAF500)),

            new Vertex(-1f, +1f, -1f, 1f, new Color(0xFF399926)),
            new Vertex(+1f, +1f, -1f, 1f, new Color(0xFF399926)),
            new Vertex(-1f, +1f, +1f, 1f, new Color(0xFF399926)),
            new Vertex(+1f, +1f, +1f, 1f, new Color(0xFF399926)),

            new Vertex(-1f, -1f, -1f, 1f, new Color(0xFF245BF2)),
            new Vertex(-1f, +1f, -1f, 1f, new Color(0xFF245BF2)),
            new Vertex(-1f, -1f, +1f, 1f, new Color(0xFF245BF2)),
            new Vertex(-1f, +1f, +1f, 1f, new Color(0xFF245BF2)),

            new Vertex(+1f, -1f, -1f, 1f, new Color(0xFFF2F2F4)),
            new Vertex(+1f, +1f, -1f, 1f, new Color(0xFFF2F2F4)),
            new Vertex(+1f, -1f, +1f, 1f, new Color(0xFFF2F2F4)),
            new Vertex(+1f, +1f, +1f, 1f, new Color(0xFFF2F2F4)),
        ];

        _indices =
        [
            0, 1, 2, 2, 1, 3,
            4, 6, 5, 5, 6, 7,
            8, 10, 9, 9, 10, 11,
            12, 13, 14, 14, 13, 15,
            16, 17, 18, 18, 17, 19,
            20, 22, 21, 21, 22, 23,
        ];

        _graphics.SetVertexShader(ShaderSource.LoadVertexShader);
        _graphics.SetGeometryShader(ShaderSource.LoadGeometryShader);
        _graphics.SetPixelShader(ShaderSource.LoadPixelShader);
        _graphics.SetInputLayout(ShaderSource.LoadInputLayout,
            new InputElementDesc { SemanticName = "POSITION", Format = Format.R32G32B32A32Float },
            new InputElementDesc { SemanticName = "COLOR", Format = Format.R32G32B32A32Float, AlignedByteOffset = 16 });

        _vertexBuffer = _graphics.RegisterVertexBuffer<Vertex>(0, 24);
        _vertexBuffer.Write(_vertices);
        _matrixBuffer = _graphics.RegisterConstantBuffer<Matrix4>(0, 4, ShaderStages.VertexShader);
        World = _scaling;
        _view = Matrix4.LookAtLH(0f, 0f, 0f, 0f);
        _proj = LerpProjection(4f, 4f, _zNear, _zFar, _zFactor);
        WorldViewProj = World * _view * _proj;
        _matrixBuffer.Flush();

        _lightBuffer = _graphics.RegisterConstantBuffer<Light>(1, ShaderStages.GeometryShader);
        _lightBuffer.Value.AmbientFactor = 0.15f;
        _lightBuffer.Value.DiffuseFactor = 0.90f;
        _lightBuffer.Value.LightDirection = Vector4.Normalize(new Vector4(1f, 1f, 10f, 1f));
        _lightBuffer.Value.SpecularFactor = 0.25f;
        _lightBuffer.Value.SpecularIndex = 5f;
        _lightBuffer.Flush();

        _graphics.RegisterIndexBuffer(36).Write(_indices);
        Parameters = CreateParameters();
    }

    private FloatViewModel[] CreateParameters()
    {
        return
        [
            FloatViewModel.Create("Scale X", -1f, 1f, this, t => ref t._scaling.R1C1, _dummy),
            FloatViewModel.Create("Scale Y", -1f, 1f, this, t => ref t._scaling.R2C2, _dummy),
            FloatViewModel.Create("Scale Z", -1f, 1f, this, t => ref t._scaling.R3C3, _dummy),
            FloatViewModel.Create("Ambient Factor", 0f, 1f, _lightBuffer, t => ref t.Value.AmbientFactor, _lightChange),
            FloatViewModel.Create("Diffuse Factor", 0f, 1f, _lightBuffer, t => ref t.Value.DiffuseFactor, _lightChange),
            FloatViewModel.Create("Specular Factor", 0f, 1f, _lightBuffer, t => ref t.Value.SpecularFactor, _lightChange),
            FloatViewModel.Create("Specular Index", 0f, 20f, _lightBuffer, t => ref t.Value.SpecularIndex, _lightChange),
            FloatViewModel.Create("Z Factor", 0f, 0.5f, this, t => ref t._zFactor, _projectionChange),
            FloatViewModel.Create("Z Near", -5f, 5f, this, t => ref t._zNear, _projectionChange),
            FloatViewModel.Create("Z Far", -5f, 5f, this, t => ref t._zFar, _projectionChange),
        ];
    }

    private static Matrix4 LerpProjection(float width, float height, float zNear, float zFar, float perspectiveFactor)
    {
        var w = 2f / width;
        var h = -2f / height;
        var q = (1f + perspectiveFactor * zFar) / (zFar - zNear);
        return new Matrix4(
             w, 0f, 0f, 0f,
            0f, h, 0f, 0f,
            0f, 0f, q, perspectiveFactor,
            0f, 0f, q * -zNear, 1f);
    }

    public void Frame()
    {
        _count++;
        _graphics.Clear(Color.Black);

        var rotation = Matrix4.RotationX((float)Math.PI / 300f * (_count % 600)) *
                       Matrix4.RotationY((float)Math.PI / 200f * (_count % 400));

        if (_lightChange.IsChanged)
            _lightBuffer.Flush();

        if (_projectionChange.IsChanged)
        {
            _proj = LerpProjection(4f, 4f, _zNear, _zFar, _zFactor);
        }

        for (var i = -2; i <= 2; i++)
        {
            World = _scaling * Matrix4.Translation(0f, i, 0f) * rotation;
            WorldViewProj = World * _view * _proj;
            _matrixBuffer.Flush();
            _graphics.DrawIndexedList(_indices.Length);
        }

        _graphics.Present();
    }

    public void Dispose() => _graphics.Dispose();
}
