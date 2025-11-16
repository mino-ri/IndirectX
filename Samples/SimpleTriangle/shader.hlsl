cbuffer cbWorldTransform : register(b0)
{
    matrix WorldViewProj;
};

struct VS_INPUT
{
    float4 Pos : POSITION;
    float4 Col : COLOR;
};

struct PS_INPUT
{
    float4 Pos : SV_POSITION;
    float4 Col : COLOR;
};

PS_INPUT VS(VS_INPUT input)
{
    PS_INPUT output;

    output.Pos = mul(input.Pos, WorldViewProj);
    output.Col = input.Col;

    return output;
}

float4 PS(PS_INPUT input) : SV_TARGET
{
    return input.Col;
}
