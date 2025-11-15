namespace IndirectX.Interop;

public static class InteropUtilities
{
    public static void HandleResult(this HResult result)
    {
        if (result != HResult.Ok) throw new IndirectXException(result);
    }
}
