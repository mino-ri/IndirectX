using System.Windows;
using System.Windows.Media;

namespace IndirectX.Helper.Wpf;

public static class FrameworkElementExtensions
{
    public static (int width, int height) GetPixelSize(this FrameworkElement element)
    {
        var dpi = VisualTreeHelper.GetDpi(element);
        return ((int)Math.Round(element.Width * dpi.DpiScaleX),
                (int)Math.Round(element.Height * dpi.DpiScaleY));
    }

    public static (int width, int height) GetActualPixelSize(this FrameworkElement element)
    {
        var dpi = VisualTreeHelper.GetDpi(element);
        return ((int)Math.Round(element.ActualWidth * dpi.DpiScaleX),
                (int)Math.Round(element.ActualHeight * dpi.DpiScaleY));
    }
}
