using System;
using System.IO;
using System.Reflection;

namespace SimpleImage;

internal static partial class ShaderSource
{
    public static Stream GetStream(string name)
    {
        return Assembly.GetExecutingAssembly().GetManifestResourceStream("SimpleImage." + name) ??
             throw new ArgumentException("Invalid resource", nameof(name));
    }
}
