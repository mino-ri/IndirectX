using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace IndirectX.Helper.Wpf;

public static class GraphicsWpfExtensions
{
    public static IResourceTexture CreateResourceTexture(this Graphics graphics, BitmapDecoder decoder)
    {
        var bitmap = new FormatConvertedBitmap(decoder.Frames[0], PixelFormats.Bgra32, null, 0);
        var width = bitmap.PixelWidth;
        var height = bitmap.PixelHeight;
        var stride = width * 4;
        var array = new byte[stride * height];
        bitmap.CopyPixels(array, stride, 0);

        return graphics.CreateResourceTexture(array, width, height, stride);
    }

    public static IResourceTexture CreateResourceTexture(this Graphics graphics, Stream stream)
    {
        var decoder = BitmapDecoder.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
        return graphics.CreateResourceTexture(decoder);
    }

    public static IResourceTexture CreateResourceTexture(this Graphics graphics, string path)
    {
        using var stream = File.OpenRead(path);
        var decoder = BitmapDecoder.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
        return graphics.CreateResourceTexture(decoder);
    }
}
