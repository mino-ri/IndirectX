cbuffer cbWorldTransform : register(b0)
{
    matrix World;
    matrix WorldViewProj;
};

cbuffer cbLight : register(b1)
{
    float AmbientFactor;
    float DiffuseFactor;
    float SpecularFactor;
    float SpecularIndex;
    float4 LightDirection;
}

struct VS_INPUT
{
    float4 Pos : POSITION;
    float4 Col : COLOR;
};

struct GS_INPUT
{
    float4 Pos : POSITION0;
    float4 Posw : POSITION1;
    float4 Col : COLOR;
};

struct PS_INPUT
{
    float4 Pos : SV_POSITION;
    float4 Col : COLOR;
};

GS_INPUT VS(VS_INPUT input)
{
    GS_INPUT output;

    output.Pos = mul(input.Pos, WorldViewProj);
    output.Posw = mul(input.Pos, World);
    output.Col = input.Col;

    return output;
}

[maxvertexcount(3)]
void GS(triangle GS_INPUT input[3], inout TriangleStream<PS_INPUT> triStream)
{
    float3 norm = normalize(cross(input[1].Posw.xyz - input[0].Posw.xyz, input[2].Posw.xyz - input[0].Posw.xyz));
    float3 direction = LightDirection.xyz;
    float diffuse = max(0, dot(norm, direction)) * DiffuseFactor;
    float specular = pow(max(0, dot(norm, direction)), SpecularIndex) * SpecularFactor;
    
    PS_INPUT output;

    [unroll(3)]
    for (int i = 0; i < 3; ++i)
    {
        output.Pos = input[i].Pos;
        output.Col = input[i].Col;
        output.Col.rgb = input[i].Col.rgb * (AmbientFactor + diffuse) + specular;
        triStream.Append(output);
    }
    triStream.RestartStrip();
}

float4 PS(PS_INPUT input) : SV_TARGET
{
    return input.Col;
}
