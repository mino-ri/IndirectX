using System;
using IndirectX.Interop;
using System.Runtime.InteropServices;

namespace IndirectX.D3D11;

[StructLayout(LayoutKind.Sequential)]
public partial struct InputElementDesc
{
    public string SemanticName;
    public int SemanticIndex;
    public Dxgi.Format Format;
    public int InputSlot;
    public int AlignedByteOffset;
    public InputClassification InputSlotClass;
    public int InstanceDataStepRate;

    internal InputElementDesc(string semanticName, int semanticIndex, Dxgi.Format format, int inputSlot, int alignedByteOffset, InputClassification inputSlotClass, int instanceDataStepRate)
    {
        SemanticName = semanticName;
        SemanticIndex = semanticIndex;
        Format = format;
        InputSlot = inputSlot;
        AlignedByteOffset = alignedByteOffset;
        InputSlotClass = inputSlotClass;
        InstanceDataStepRate = instanceDataStepRate;
    }

    internal static InteropInputElementDesc[] ToInterop(ReadOnlySpan<InputElementDesc> source)
    {
        var result = new InteropInputElementDesc[source.Length];
        for (var i = 0; i < source.Length; i++)
        {
            result[i].SemanticName = new AnsiString(source[i].SemanticName);
            result[i].SemanticIndex = source[i].SemanticIndex;
            result[i].Format = source[i].Format;
            result[i].InputSlot = source[i].InputSlot;
            result[i].AlignedByteOffset = source[i].AlignedByteOffset;
            result[i].InputSlotClass = source[i].InputSlotClass;
            result[i].InstanceDataStepRate = source[i].InstanceDataStepRate;
        }

        return result;
    }
}
