using System;
using System.Threading.Tasks;

namespace IndirectX.Helper;

public class RenderLoop(Action action)
{
    private Task? _drawThread = null;
    private bool _endFlag = false;
    private readonly Action _action = action;

    /// <summary>描画スレッドを開始します。</summary>
    public void Start()
    {
        if (_drawThread != null) throw new ApplicationException("描画スレッドは動作中です。");
        _endFlag = false;
        _drawThread = Task.Run(Run);
    }

    /// <summary>描画スレッドを終了します。</summary>
    public void Stop()
    {
        if (_drawThread == null) return;
        _endFlag = true;
        _drawThread.Wait();
        _drawThread = null;
    }

    /// <summary>描画スレッドを終了します。</summary>
    public async Task StopAsync()
    {
        if (_drawThread == null) return;
        _endFlag = true;
        await _drawThread.ConfigureAwait(false);
        _drawThread = null;
    }

    private void Run()
    {
        while (!_endFlag) _action();
    }

    /// <summary>描画スレッドを開始します。</summary>
    public static RenderLoop Run(Action action)
    {
        var renderLoop = new RenderLoop(action);
        renderLoop.Start();
        return renderLoop;
    }
}
