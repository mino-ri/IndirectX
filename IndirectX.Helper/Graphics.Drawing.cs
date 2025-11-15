using IndirectX.D3D11;

namespace IndirectX.Helper;

public sealed partial class Graphics
{
    public PrimitiveTopology PrimitiveTopology
    {
        get => Context.InputAssembler.PrimitiveTopology;
        set => Context.InputAssembler.PrimitiveTopology = value;
    }

    /// <summary>描画領域と深度バッファをクリアします。</summary>
    public void Clear(Color color, float depth = 1.0f)
    {
        ClearDepth(depth);
        ClearRenderTarget(color);
    }

    /// <summary>描画領域の深度バッファをクリアします。</summary>
    public void ClearDepth(float depth = 1.0f) =>
        Context.ClearDepthStencilView(DepthView!, ClearFlags.Depth, depth, 0);

    /// <summary>描画領域のステンシルバッファをクリアします。</summary>
    public void ClearStencil(byte stencil = 0) =>
        Context.ClearDepthStencilView(DepthView!, ClearFlags.Stencil, 1f, stencil);

    /// <summary>描画領域の深度バッファとステンシルバッファをクリアします。</summary>
    public void ClearDepthStencil(float depth = 1.0f, byte stencil = 0) =>
        Context.ClearDepthStencilView(DepthView!, ClearFlags.Depth | ClearFlags.Stencil, depth, stencil);

    /// <summary>描画領域を指定色でクリアします。</summary>
    public void ClearRenderTarget(Color color) => Context.ClearRenderTargetView(RenderView, color);

    /// <summary>現在の内容で描画します。</summary>
    public void Draw(int vertexCount, int startIndex = 0) => Context.Draw(vertexCount, startIndex);

    /// <summary>現在の内容で描画します。</summary>
    public void DrawStrip(int vertexCount, int startIndex = 0)
    {
        Context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleStrip;
        Context.Draw(vertexCount, startIndex);
    }

    /// <summary>現在の内容で描画します。</summary>
    public void DrawList(int vertexCount, int startIndex = 0)
    {
        Context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
        Context.Draw(vertexCount, startIndex);
    }

    /// <summary>現在の内容で描画します。</summary>
    public void DrawIndexed(int indexCount, int startIndex = 0, int vertexStartIndex = 0) =>
        Context.DrawIndexed(indexCount, startIndex, vertexStartIndex);

    /// <summary>現在の内容で描画します。</summary>
    public void DrawIndexedList(int indexCount, int startIndex = 0, int vertexStartIndex = 0)
    {
        Context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
        Context.DrawIndexed(indexCount, startIndex, vertexStartIndex);
    }
}
