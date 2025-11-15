using System.Text;
using System.Text.Json;
using IndirectX;
using IndirectX.D3DCompiler;
using IndirectX.HlslCodeGenerator;

try
{
    
    var options = new JsonSerializerOptions(JsonSerializerDefaults.General)
    {
        WriteIndented = true,
    };
    if (args.Contains("--template"))
    {
        using var stream = File.Create("hlslcompile.json");
        await JsonSerializer.SerializeAsync(stream, HlslCompilerSetting.Sample, options);
        return;
    }

    var hlslTexts =
        Directory.GetFiles("./", "*.hlsl", SearchOption.AllDirectories)
        .ToDictionary(path => Path.GetFileName(path)!, File.ReadAllBytes);

    var settingPaths = Directory.GetFiles("./", "hlslcompile.json", SearchOption.AllDirectories);
    if (settingPaths is [])
    {
        Console.WriteLine("Put the 'hlslcompile.json' file as follows:");
        Console.WriteLine(JsonSerializer.Serialize(HlslCompilerSetting.Sample, options));
    }

    var tasks = new List<Task>(settingPaths.Length);
    var settings = new (string inputPath, string outputPath, HlslCompilerSetting setting)[settingPaths.Length];
    for (var i = 0; i < settingPaths.Length; i++)
    {
        var index = i;
        var path = settingPaths[index];
        tasks.Add(Task.Run(async () =>
        {
            try
            {
                using var stream = File.OpenRead(path);
                var setting = await JsonSerializer.DeserializeAsync<HlslCompilerSetting>(stream, options)
                    ?? throw new InvalidOperationException($"Invalid hlslcompile.setting file.\r\nPath: {path}");
                settings[index] = (
                    path,
                    Path.Combine(Path.GetDirectoryName(path)!, $"{setting.ClassName}.g.cs"),
                    setting);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Invalid hlslcompile.setting file.\r\nPath: {path}", ex);
            }
        }));
    }

    await Task.WhenAll([.. tasks]);
    tasks.Clear();

    foreach (var setting in settings)
    {
        tasks.Add(Task.Run(async () =>
        {
            var text =
                new SourceTemplate(
                    setting.setting.Namespace,
                    setting.setting.ClassName,
                    Array.ConvertAll(setting.setting.Methods, method => CompileHlsl(method, hlslTexts)))
                    .Geterate();
            Console.WriteLine($"{setting.inputPath} ==> {setting.outputPath}");
            await File.WriteAllTextAsync(setting.outputPath, text, Encoding.UTF8);
        }));
    }

    await Task.WhenAll([.. tasks]);
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine(ex);
}

static BytecodeMethod CompileHlsl(HlslCompilerMethod method, Dictionary<string, byte[]> hlslTexts)
{
    var isSignature = method.ShaderName == "InputLayout";

    if (!hlslTexts.TryGetValue(method.SourceFile, out var sourceBytecode))
    {
        throw new InvalidOperationException($"Source file '{method.SourceFile}' is not found.");
    }

    var sourceSpan = sourceBytecode[0] == 0xEF && sourceBytecode[1] == 0xBB && sourceBytecode[2] == 0xBF
        ? sourceBytecode.AsSpan()[3..]
        : sourceBytecode.AsSpan();

    using var result = Bytecode.Compile(sourceSpan, method.EntryPoint, method.Profile);

    if (result.HasError) throw new IndirectXException(result.Result, result.ErrorMessage ?? "");
    if (result.Bytecode is null) throw new IndirectXException(result.Result, "result is null.");

    if (!isSignature)
    {
        return new BytecodeMethod
        {
            MethodName = method.MethodName,
            ShaderName = method.ShaderName,
            Bytecode = result.Bytecode!,
        };
    }
    else
    {
        using var signature = Bytecode.GetInputSignature(result.Bytecode);
        return new BytecodeMethod
        {
            MethodName = method.MethodName,
            ShaderName = method.ShaderName,
            Bytecode = signature!,
        };
    }
}
