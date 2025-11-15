using System.Text;

namespace IndirectX.HlslCodeGenerator;

partial class SourceTemplate
{
    private string NamespaceName { get; }

    private string TypeName { get; }

    private IEnumerable<BytecodeMethod> Methods { get; }

    internal SourceTemplate(string namespaceName, string typeName, IEnumerable<BytecodeMethod> bytecodeMethods)
    {
        NamespaceName = namespaceName;
        TypeName = typeName;
        Methods = bytecodeMethods;
    }

    public string Geterate()
    {
        var builder = new StringBuilder();
        builder.Append($$"""
            namespace {{NamespaceName}};

            partial class {{TypeName}}
            {

            """);
        foreach (var method in Methods)
        {
            var isInputLayout = method.ShaderName == "InputLayout";
            var additionalParameter = isInputLayout ? ", params global::IndirectX.D3D11.InputElementDesc[] descs" : "";
            builder.Append($$"""
                    public static global::IndirectX.D3D11.{{method.ShaderName}} {{method.MethodName}}(global::IndirectX.D3D11.Device device{{additionalParameter}})
                    {
                        global::System.ReadOnlySpan<byte> bytecode = [
                """);

            foreach (var b in method.Bytecode)
            {
                builder.Append($"0x{b:x2}, ");
            }

            builder.Append($$"""
                ];
                        return device.Create{{method.ShaderName}}({{(isInputLayout ? "descs, " : "")}}bytecode);
                    }
                
                """);
        }

        builder.Append("}\n");
        return builder.ToString();
    }
}
