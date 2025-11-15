namespace IndirectX.HlslCodeGenerator;

internal class BytecodeMethod
{
    public string MethodName { get; set; } = "";

    public string ShaderName { get; set; } = "";

    public byte[] Bytecode { get; set; } = [];
}
