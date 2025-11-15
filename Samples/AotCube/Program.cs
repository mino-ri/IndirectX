using AotCube;
using IndirectX.Helper;

var window = new Window("AOT Cube Sample", 400, 400);
window.Show();

using var renderer = new TestRenderer(window.Handle, 400, 400);
window.Resized += (_, width, height) => renderer.Resize(width, height);
var loop = new RenderLoop(renderer.Frame);

try
{
    loop.Start();
    Window.RunMessageLoop();
}
finally
{
    loop.Stop();
}
