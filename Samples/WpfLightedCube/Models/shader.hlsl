cbuffer cbWorldTransform : register(b0)
{
    matrix World;
    matrix View;
    matrix Proj;
    matrix WorldViewProj;
};

cbuffer cbLight : register(b1)
{
    float AmbientFactor;
    float DiffuseFactor;
    float SpecularFactor;
    float SpecularIndex;
    float4 LightSource;
    float4 CameraPosition;
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
    float4 Posw : POSITION0;
    float4 Norm : NORMAL0;
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
    PS_INPUT output;
    output.Norm.xyz = norm.xyz;
    output.Norm.w = 1;

    [unroll(3)]
    for (int i = 0; i < 3; ++i)
    {
        output.Pos = input[i].Pos;
        output.Posw = input[i].Posw;
        output.Col = input[i].Col;
        triStream.Append(output);
    }
    triStream.RestartStrip();
}

float4 PS(PS_INPUT input) : SV_TARGET
{
    float3 direction = normalize(input.Posw.xyz - LightSource.xyz);
    float3 cameraDirection = normalize(input.Posw.xyz - CameraPosition.xyz);
    float3 halfVector = normalize(direction + cameraDirection);
    float diffuse = max(0, dot(input.Norm.xyz, direction)) * DiffuseFactor;
    float specular = pow(max(0, dot(input.Norm.xyz, halfVector)), SpecularIndex) * SpecularFactor;
    float4 color = input.Col;
    color.rgb = color.rgb * (AmbientFactor + diffuse) + specular;
    return color;
}
