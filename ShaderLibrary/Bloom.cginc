#ifndef POST_PROCESS_BLOOM_INCLUDED
#define POST_PROCESS_BLOOM_INCLUDED

#define Max3(a,b,c) max(max((a),(b)),(c))

sampler2D _MainTex;
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
#define thresholdKnee _FilterParams.w

#define FILTER_EXP    _FilterParams.x

float4 _BloomParams;

#define MIP_MAX       _BloomParams.x
#define INV_STEPS     _BloomParams.y

fixed4 frag_DownSample(v2f_img i) : SV_Target
{
	fixed4 col = tex2D(_MainTex, i.uv);
	float brightness = Max3(col.r, col.g, col.b);
	return saturate(pow(brightness, FILTER_EXP)) * col;
}

fixed4 frag_UpSample(v2f_img i) : SV_Target
{
	float4 uv = float4(i.uv, 0, 0);
	fixed4 col = tex2D(_MainTex, i.uv);
	fixed4 brightness = 0;
	for (int mip = 0; mip <= MIP_MAX; ++mip)
	{
		uv.zw = mip;
		brightness += tex2Dlod(_BloomTex, uv) * INV_STEPS;
	}
	return saturate(col + brightness);
}

fixed4 ApplyBloom(fixed4 col, float2 uv)
{
	float4 uvLod = float4(uv, 0, 0);
	fixed4 brightness = 0;
	for (int mip = 0; mip <= MIP_MAX; ++mip)
	{
		uvLod.zw = mip;
		brightness += tex2Dlod(_BloomTex, uvLod) * INV_STEPS;
	}
	return saturate(col + brightness);
}

#endif //POST_PROCESS_BLOOM_INCLUDED