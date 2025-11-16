using System.Text;

namespace IndirectX.ImageEmbedder;

internal record class SourceInput(string FileName, int Width, int Height, byte[] CompressedArgb);

internal class SourceTemplate(string namespaceName, string typeName, SourceInput[] inputs)
{
    private readonly string _namespaceName = namespaceName;
    private readonly string _typeName = typeName;
    private readonly SourceInput[] _inputs = inputs;

    public string Generate()
    {
        var builder = new StringBuilder();
        builder.Append($$"""
            namespace {{_namespaceName}};

            partial class {{_typeName}}
            {

            """);
        foreach (var input in _inputs)
        {
            builder.Append($$"""
                    public static global::IndirectX.Helper.IResourceTexture Load{{input.FileName}}(global::IndirectX.Helper.Graphics graphics)
                    {
                        const int width = {{input.Width}};
                        const int height = {{input.Height}};
                        global::System.ReadOnlySpan<byte> bytecode = [
                """);

            foreach (var b in input.CompressedArgb)
            {
                builder.Append($"0x{b:x2}, ");
            }

            builder.Append($$"""
                ];
                        var decompressed = new byte[width * height * 4];
                        using (var decoder = new global::System.IO.Compression.BrotliDecoder())
                        {
                            decoder.Decompress(bytecode, decompressed, out _, out _);
                        }
                        return graphics.CreateResourceTexture(decompressed, width, height, width * 4);
                    }
                
                """);
        }

        builder.Append("}\n");
        return builder.ToString();
    }
}
