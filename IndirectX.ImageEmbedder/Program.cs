using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.Json;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using IndirectX.ImageEmbedder;

var options = new JsonSerializerOptions(JsonSerializerDefaults.General)
{
    WriteIndented = true,
};
if (args.Contains("--template"))
{
    using var stream = File.Create("imageembed.json");
    await JsonSerializer.SerializeAsync(stream, ImageEmbedSetting.Sample, options);
    return;
}

var settingPaths = Directory.GetFiles("./", "imageembed.json", SearchOption.AllDirectories);
if (settingPaths is [])
{
    Console.WriteLine("Put the 'imageembed.json' file as follows:");
    Console.WriteLine(JsonSerializer.Serialize(ImageEmbedSetting.Sample, options));
}

foreach (var settingPath in settingPaths)
{
    var imageFiles = Directory
        .GetFiles(Path.GetDirectoryName(settingPath) ?? "./", "*.png")
        .Select(path =>
        {
            var (width, height, argb) = ReadPixels(path);
            return new SourceInput(Path.GetFileNameWithoutExtension(path), width, height, Compress(argb));
        })
        .ToArray();

    ImageEmbedSetting setting;
    using (var stream = File.OpenRead(settingPath))
    {
        setting = await JsonSerializer.DeserializeAsync<ImageEmbedSetting>(stream, options)
            ?? throw new InvalidOperationException($"Invalid imageembed.json file.\r\nPath: {settingPath}");
    }

    var outputPath = Path.Combine(Path.GetDirectoryName(settingPath)!, $"{setting.ClassName}.g.cs");
    var text =
            new SourceTemplate(
                setting.Namespace,
                setting.ClassName,
                imageFiles)
                .Generate();
    Console.WriteLine($"{settingPath} ==> {outputPath}");
    await File.WriteAllTextAsync(outputPath, text, Encoding.UTF8);
}

static (int width, int height, byte[] bytecode) ReadPixels(string path)
{
    using var stream = File.OpenRead(path);
    var decoder = BitmapDecoder.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
    var bitmap = new FormatConvertedBitmap(decoder.Frames[0], PixelFormats.Bgra32, null, 0);
    var width = bitmap.PixelWidth;
    var height = bitmap.PixelHeight;
    var stride = width * 4;
    var array = new byte[stride * height];
    bitmap.CopyPixels(array, stride, 0);
    return (width, height, array);
}

static byte[] Compress(byte[] data)
{
    using var output = new MemoryStream();
    using (var encoder = new BrotliStream(output, CompressionLevel.SmallestSize))
    {
        encoder.Write(data, 0, data.Length);
    }

    return output.ToArray();
}
