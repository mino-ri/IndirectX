using System;
using IndirectX.Interop;
using System.Runtime.InteropServices;

namespace IndirectX.D3D11;

[StructLayout(LayoutKind.Sequential)]
public struct SoDeclarationEntry
{
    public int Stream;
    public string SemanticName;
    public int SemanticIndex;
    public byte StartComponent;
    public byte ComponentCount;
    public byte OutputSlot;

    internal SoDeclarationEntry(int stream, string semanticName, int semanticIndex, byte startComponent, byte componentCount, byte outputSlot)
    {
        Stream = stream;
        SemanticName = semanticName;
        SemanticIndex = semanticIndex;
        StartComponent = startComponent;
        ComponentCount = componentCount;
        OutputSlot = outputSlot;
    }

    internal static InteropSoDeclarationEntry[] ToInterop(ReadOnlySpan<SoDeclarationEntry> source)
    {
        var result = new InteropSoDeclarationEntry[source.Length];
        for (var i = 0; i < source.Length; i++)
        {
            result[i].Stream = source[i].Stream;
            result[i].SemanticName = new AnsiString(source[i].SemanticName);
            result[i].SemanticIndex = source[i].SemanticIndex;
            result[i].StartComponent = source[i].StartComponent;
            result[i].ComponentCount = source[i].ComponentCount;
            result[i].OutputSlot = source[i].OutputSlot;
        }

        return result;
    }
}
