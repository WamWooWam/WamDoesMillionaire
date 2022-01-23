sampler2D  inputSampler : register(S0);
/// <summary>The strength of the effect.</summary>
/// <minValue>0</minValue>
/// <maxValue>1</maxValue>
/// <defaultValue>0</defaultValue>
float Strength : register(C0);
float4 TintColor: register(C1);

float Blend(float base, float blend)
{
	return (base <= 0.5) ? 2 * base * blend : 1 - 2 * (1 - base) * (1 - blend);
}

float4 Blend(float4 base, float4 blend)
{
	return float4(Blend(base.r, blend.r),
		Blend(base.g, blend.g),
		Blend(base.b, blend.b),
		base.a);
}

float4 main(float2 uv : TEXCOORD) : COLOR
{
    float4 srcColor = tex2D(inputSampler, uv);
    float3 rgb = srcColor.rgb;
    float3 c = (1 - Strength) * rgb + Strength * dot(rgb, float3(0.30, 0.59, 0.11));
    return Blend(float4(c, srcColor.a), TintColor);
}