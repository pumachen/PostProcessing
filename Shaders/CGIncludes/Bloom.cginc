#ifndef BLOOM_INCLUDED
#define BLOOM_INCLUDED

#include "CGIncludes/StdLib.cginc"

sampler2D _BloomTex;

/*
	x: scatter
	y: clamp
	z: threshold (linear)
	w: threshold knee
*/
float4 _FilterParams;

#define Scatter       _FilterParams.x
#define ClampMax      _FilterParams.y
#define Threshold     _FilterParams.z
#define ThresholdKnee _FilterParams.w

#define FILTER_EXP    _FilterParams.x

float4 _BloomParams;

#define BLOOM_MIP_MIN   _BloomParams.x
#define BLOOM_MIP_MAX   _BloomParams.y
#define BLOOM_INV_STEPS _BloomParams.z
#define BLOOM_INTENSITY _BloomParams.w

//
// Quadratic color thresholding
// curve = (threshold - knee, knee * 2, 0.25 / knee)
//
half4 QuadraticThreshold(half4 color, half threshold, half3 curve)
{
	// Pixel brightness
	half br = Max3(color.r, color.g, color.b);

	// Under-threshold part: quadratic curve
	half rq = clamp(br - curve.x, 0.0, curve.y);
	rq = curve.z * rq * rq;

	// Combine and apply the brightness response curve.
	color *= max(rq, br - threshold) / max(br, EPSILON);

	return color;
}

fixed4 DownSample(sampler2D mainTex, float2 uv)
{
	fixed4 col = tex2D(mainTex, uv);
	float brightness = Max3(col.r, col.g, col.b);
	col.rgb *= saturate(pow(brightness, FILTER_EXP));
	return col;
}

fixed4 frag_DownSample(v2f_img i) : SV_Target
{
	return DownSample(_MainTex, i.uv);
}

fixed4 UpSample(float2 uv)
{
	float4 uvLod = float4(uv, 0, 0);
	fixed4 bloom = 0;
	for (int mip = BLOOM_MIP_MIN; mip <= BLOOM_MIP_MAX; ++mip)
	{
		uvLod.zw = mip;
		bloom += tex2Dlod(_BloomTex, uvLod) * BLOOM_INV_STEPS;
	}
	return bloom;
}

fixed4 frag_UpSample(v2f_img i) : SV_Target
{
	return UpSample(i.uv);
}

fixed4 ApplyBloom(fixed4 col, float2 uv)
{
	fixed4 bloom = UpSample(uv);
	return saturate(col + BLOOM_INTENSITY * bloom);
}

#endif //BLOOM_INCLUDED