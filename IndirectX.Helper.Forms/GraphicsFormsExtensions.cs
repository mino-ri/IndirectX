using System.Drawing.Imaging;

namespace IndirectX.Helper.Forms;

public static class GraphicsFormsExtensions
{
    public static IResourceTexture CreateResourceTexture(this Graphics graphics, Bitmap bitmap)
    {
        const PixelFormat targetFormat = PixelFormat.Format32bppArgb;
        if (bitmap.PixelFormat != targetFormat)
        {
            using var targetImage = bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), targetFormat);
            return graphics.CreateResourceTexture(targetImage);
        }

        BitmapData? bitmapData = null;
        try
        {
            bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, targetFormat);
            return graphics.CreateResourceTexture(bitmapData.Scan0, bitmapData.Width, bitmapData.Height, bitmapData.Stride);
        }
        finally
        {
            if (bitmapData is not null)
                bitmap.UnlockBits(bitmapData);
        }
    }

    public static IResourceTexture CreateResourceTexture(this Graphics graphics, Stream stream)
    {
        using var image = Image.FromStream(stream, false, false);
        using var bitmap = new Bitmap(image);
        return graphics.CreateResourceTexture(bitmap);
    }

    public static IResourceTexture CreateResourceTexture(this Graphics graphics, string path)
    {
        using var stream = File.OpenRead(path);
        return graphics.CreateResourceTexture(stream);
    }
}
