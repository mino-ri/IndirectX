namespace IndirectX.HlslCodeGenerator;

public class HlslCompilerSetting
{
    public string Namespace { get; set; } = "";

    public string ClassName { get; set; } = "";

    public HlslCompilerMethod[] Methods { get; set; } = [];

    public static HlslCompilerSetting Sample => new()
    {
        Namespace = "CSharpNamespace",
        ClassName = "CSharpClassName",
        Methods =
            [
                new HlslCompilerMethod
                {
                    MethodName = "CSharpMethodName",
                    ShaderName = "InputLayout|VertexShader|PixelShader|HullShader|DomainShader|ComputeShader|GeometryShader",
                    SourceFile = "HlslSourceFile.hlsl",
                    EntryPoint = "HlslFunctionName",
                    Profile = "vs_5_0|ps_5_0|hs_5_0|ds_5_0|cs_5_0|gs_5_0",
                },
            ],
    };
}

public class HlslCompilerMethod
{
    public string MethodName { get; set; } = "";

    public string ShaderName { get; set; } = "";

    public string SourceFile { get; set; } = "";

    public string EntryPoint { get; set; } = "";

    public string Profile { get; set; } = "";
}
