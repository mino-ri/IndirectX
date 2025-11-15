# IndirectX.HlslCodeGenerator

DirectX managed wrapper for .NET

# Installation

```shell
dotnet tool install IndirectX.HlslCodeGenerator
```

# How to Use

Put the `hlslcompile.json` file as follows:

```json
{
  "Namespace": "CSharpNamespace",
  "ClassName": "CSharpClassName",
  "Methods": [
    {
      "MethodName": "CSharpMethodName",
      "ShaderName": "InputLayout|VertexShader|PixelShader|HullShader|DomainShader|ComputeShader|GeometryShader",
      "SourceFile": "HlslSourceFile.hlsl",
      "EntryPoint": "HlslFunctionName",
      "Profile": "vs_5_0|ps_5_0|hs_5_0|ds_5_0|cs_5_0|gs_5_0"
    }
  ]
}
```

and run:

```shell
dotnet indxgen
```

# Samples

See https://github.com/mino-ri/IndirectX/tree/main/Samples .

# Notes

This tool uses the Windows SDK.  
Please install the Windows SDK if you haven't already.  
If you installed Visual Studio, it should already be installed.
