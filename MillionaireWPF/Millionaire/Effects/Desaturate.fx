sampler2D  inputSampler : register(S0);
float Strength : register(C0);

float4 main(float2 uv : TEXCOORD) : COLOR
{
    float4 srcColor = tex2D(inputSampler, uv);
    float3 rgb = srcColor.rgb;
    float3 c = (1 - Strength) * rgb + Strength * dot(rgb, float3(0.30, 0.59, 0.11));
	return float4(c, srcColor.a);
}