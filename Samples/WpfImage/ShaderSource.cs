using System;
using System.IO;
using System.Reflection;

namespace WpfImage;

internal static partial class ShaderSource
{
    public static Stream GetStream(string name)
    {
        return Assembly.GetExecutingAssembly().GetManifestResourceStream("WpfImage." + name) ??
             throw new ArgumentException("Invalid resource", nameof(name));
    }
}
