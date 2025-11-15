namespace IndirectX.D3D11;

public enum DriverType : uint
{
    Unknown = 0,
    Hardware = (Unknown + 1),
    Reference = (Hardware + 1),
    Null = (Reference + 1),
    Software = (Null + 1),
    Warp = (Software + 1),
}

public enum FeatureLevel : uint
{
    Level9_1 = 0x9100,
    Level9_2 = 0x9200,
    Level9_3 = 0x9300,
    Level10_0 = 0xa000,
    Level10_1 = 0xa100,
    Level11_0 = 0xb000,
}

public enum PrimitiveTopology : uint
{
    Undefined = 0,
    PointList = 1,
    LineList = 2,
    LineStrip = 3,
    TriangleList = 4,
    TriangleStrip = 5,
    LineListAdj = 10,
    LineStripAdj = 11,
    TriangleListAdj = 12,
    TriangleStripAdj = 13,
    ControlPointPatchList1 = 33,
    ControlPointPatchList2 = 34,
    ControlPointPatchList3 = 35,
    ControlPointPatchList4 = 36,
    ControlPointPatchList5 = 37,
    ControlPointPatchList6 = 38,
    ControlPointPatchList7 = 39,
    ControlPointPatchList8 = 40,
    ControlPointPatchList9 = 41,
    ControlPointPatchList10 = 42,
    ControlPointPatchList11 = 43,
    ControlPointPatchList12 = 44,
    ControlPointPatchList13 = 45,
    ControlPointPatchList14 = 46,
    ControlPointPatchList15 = 47,
    ControlPointPatchList16 = 48,
    ControlPointPatchList17 = 49,
    ControlPointPatchList18 = 50,
    ControlPointPatchList19 = 51,
    ControlPointPatchList20 = 52,
    ControlPointPatchList21 = 53,
    ControlPointPatchList22 = 54,
    ControlPointPatchList23 = 55,
    ControlPointPatchList24 = 56,
    ControlPointPatchList25 = 57,
    ControlPointPatchList26 = 58,
    ControlPointPatchList27 = 59,
    ControlPointPatchList28 = 60,
    ControlPointPatchList29 = 61,
    ControlPointPatchList30 = 62,
    ControlPointPatchList31 = 63,
    ControlPointPatchList32 = 64,
}

public enum Primitive : uint
{
    Undefined = 0,
    Point = 1,
    Line = 2,
    Triangle = 3,
    LineAdj = 6,
    TriangleAdj = 7,
    ControlPointPatch1 = 8,
    ControlPointPatch2 = 9,
    ControlPointPatch3 = 10,
    ControlPointPatch4 = 11,
    ControlPointPatch5 = 12,
    ControlPointPatch6 = 13,
    ControlPointPatch7 = 14,
    ControlPointPatch8 = 15,
    ControlPointPatch9 = 16,
    ControlPointPatch10 = 17,
    ControlPointPatch11 = 18,
    ControlPointPatch12 = 19,
    ControlPointPatch13 = 20,
    ControlPointPatch14 = 21,
    ControlPointPatch15 = 22,
    ControlPointPatch16 = 23,
    ControlPointPatch17 = 24,
    ControlPointPatch18 = 25,
    ControlPointPatch19 = 26,
    ControlPointPatch20 = 28,
    ControlPointPatch21 = 29,
    ControlPointPatch22 = 30,
    ControlPointPatch23 = 31,
    ControlPointPatch24 = 32,
    ControlPointPatch25 = 33,
    ControlPointPatch26 = 34,
    ControlPointPatch27 = 35,
    ControlPointPatch28 = 36,
    ControlPointPatch29 = 37,
    ControlPointPatch30 = 38,
    ControlPointPatch31 = 39,
    ControlPointPatch32 = 40,
}

public enum SrvDimension : uint
{
    Unknown = 0,
    Buffer = 1,
    Texture1D = 2,
    Texture1Darray = 3,
    Texture2D = 4,
    Texture2Darray = 5,
    Texture2Dms = 6,
    Texture2Dmsarray = 7,
    Texture3D = 8,
    Texturecube = 9,
    Texturecubearray = 10,
    Bufferex = 11,
}

public enum IncludeType : uint
{
    Local = 0,
    System = (Local + 1),
    ForceDword = 0x7fffffff,
}

public enum ShaderVariableClass : uint
{
    Scalar = 0,
    Vector = (Scalar + 1),
    MatrixRows = (Vector + 1),
    MatrixColumns = (MatrixRows + 1),
    Object = (MatrixColumns + 1),
    Struct = (Object + 1),
    InterfaceClass = (Struct + 1),
    InterfacePointer = (InterfaceClass + 1),
    ForceDword = 0x7fffffff,
}

public enum ShaderVariableFlags : uint
{
    Userpacked = 1,
    Used = 2,
    InterfacePointer = 4,
    InterfaceParameter = 8,
    ForceDword = 0x7fffffff,
}

public enum ShaderVariableType : uint
{
    Void = 0,
    Bool = 1,
    Int = 2,
    Float = 3,
    String = 4,
    Texture = 5,
    Texture1D = 6,
    Texture2D = 7,
    Texture3D = 8,
    Texturecube = 9,
    Sampler = 10,
    Sampler1D = 11,
    Sampler2D = 12,
    Sampler3D = 13,
    Samplercube = 14,
    Pixelshader = 15,
    Vertexshader = 16,
    Pixelfragment = 17,
    Vertexfragment = 18,
    Uint = 19,
    Uint8 = 20,
    Geometryshader = 21,
    Rasterizer = 22,
    Depthstencil = 23,
    Blend = 24,
    Buffer = 25,
    Cbuffer = 26,
    Tbuffer = 27,
    Texture1Darray = 28,
    Texture2Darray = 29,
    Rendertargetview = 30,
    Depthstencilview = 31,
    Texture2Dms = 32,
    Texture2Dmsarray = 33,
    Texturecubearray = 34,
    Hullshader = 35,
    Domainshader = 36,
    InterfacePointer = 37,
    Computeshader = 38,
    Double = 39,
    Rwtexture1D = 40,
    Rwtexture1Darray = 41,
    Rwtexture2D = 42,
    Rwtexture2Darray = 43,
    Rwtexture3D = 44,
    Rwbuffer = 45,
    ByteaddressBuffer = 46,
    RwbyteaddressBuffer = 47,
    StructuredBuffer = 48,
    RwstructuredBuffer = 49,
    AppendStructuredBuffer = 50,
    ConsumeStructuredBuffer = 51,
    ForceDword = 0x7fffffff,
}

public enum ShaderInputFlags : uint
{
    Userpacked = 1,
    ComparisonSampler = 2,
    TextureComponent0 = 4,
    TextureComponent1 = 8,
    TextureComponents = 12,
    ForceDword = 0x7fffffff,
}

public enum ShaderInputType : uint
{
    Cbuffer = 0,
    Tbuffer = (Cbuffer + 1),
    Texture = (Tbuffer + 1),
    Sampler = (Texture + 1),
    UavRwtyped = (Sampler + 1),
    Structured = (UavRwtyped + 1),
    UavRwstructured = (Structured + 1),
    Byteaddress = (UavRwstructured + 1),
    UavRwbyteaddress = (Byteaddress + 1),
    UavAppendStructured = (UavRwbyteaddress + 1),
    UavConsumeStructured = (UavAppendStructured + 1),
    UavRwstructuredWithCounter = (UavConsumeStructured + 1),
}

public enum ShaderCbufferFlags : uint
{
    Userpacked = 1,
    ForceDword = 0x7fffffff,
}

public enum CbufferType : uint
{
    Cbuffer = 0,
    Tbuffer = (Cbuffer + 1),
    InterfacePointers = (Tbuffer + 1),
    ResourceBindInfo = (InterfacePointers + 1),
}

public enum Name : uint
{
    Undefined = 0,
    Position = 1,
    ClipDistance = 2,
    CullDistance = 3,
    RenderTargetArrayIndex = 4,
    ViewportArrayIndex = 5,
    VertexId = 6,
    PrimitiveId = 7,
    InstanceId = 8,
    IsFrontFace = 9,
    SampleIndex = 10,
    FinalQuadEdgeTessfactor = 11,
    FinalQuadInsideTessfactor = 12,
    FinalTriEdgeTessfactor = 13,
    FinalTriInsideTessfactor = 14,
    FinalLineDetailTessfactor = 15,
    FinalLineDensityTessfactor = 16,
    Target = 64,
    Depth = 65,
    Coverage = 66,
    DepthGreaterEqual = 67,
    DepthLessEqual = 68,
}

public enum ResourceReturnType : uint
{
    UNorm = 1,
    SNorm = 2,
    SInt = 3,
    UInt = 4,
    Float = 5,
    Mixed = 6,
    Double = 7,
    Continued = 8,
}

public enum RegisterComponentType : uint
{
    Unknown = 0,
    UInt32 = 1,
    SInt32 = 2,
    Float32 = 3,
}

public enum TessellatorDomain : uint
{
    Undefined = 0,
    Isoline = 1,
    Tri = 2,
    Quad = 3,
}

public enum TessellatorPartitioning : uint
{
    Undefined = 0,
    Integer = 1,
    Pow2 = 2,
    FractionalOdd = 3,
    FractionalEven = 4,
}

public enum TessellatorOutputPrimitive : uint
{
    Undefined = 0,
    Point = 1,
    Line = 2,
    TriangleCw = 3,
    TriangleCcw = 4,
}
