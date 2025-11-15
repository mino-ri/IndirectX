namespace IndirectX;

[Serializable]
public class IndirectXException : Exception
{
    public HResult Result { get; }

    public IndirectXException(HResult result) : base(result.ToString()) => Result = result;
    public IndirectXException(HResult result, string message) : base(message) => Result = result;
    public IndirectXException(HResult result, string message, Exception inner) : base(message, inner) => Result = result;
}

public enum HResult : uint
{
    Ok = 0x0,
    False = 0x1,
    NotImpl = 0x80004001,
    OutOfMemory = 0x8007000E,
    InvalidArg = 0x80070057,
    Fail = 0x80004005,
    WasStillDrawing = 0x887A000A,
    InvalidCall = 0x887A0001,
    DeferredContextMapWithoutInitialDiscard = 0x887C0004,
    TooManyUniqueViewObjects = 0x887C0003,
    TooManyUniqueStateObjects = 0x887C0001,
    FileNotFound = 0x887C0002,
}
