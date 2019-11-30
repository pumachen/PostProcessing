#ifndef POST_PROCESS_BLOOM_INCLUDED
#define POST_PROCESS_BLOOM_INCLUDED

#define Max3(a,b,c) max(max((a),(b)),(c))

sampler2D _MainTex;
sampler2D _Luminance;

/*
	x: scatter
	y: clamp
	z: threshold (linear)
	w: threshold knee
*/
float4 _FilterParams;

#define Scatter       _FilterParams.x;
#define ClampMax      _FilterParams.y;
#define Threshold     _FilterParams.z;
#define thresholdKnee _FilterParams.w;

fixed4 frag_DownSample(v2f_img i) : SV_Target
{
	fixed4 col = tex2D(_MainTex, i.uv);
	float brightness = Max3(col.r, col.g, col.b);
	return saturate(pow(brightness, 8)) * col;
}

fixed4 frag_UpSample(v2f_img i) : SV_Target
{
	float4 uv = float4(i.uv, 0, 0);
	fixed4 col = tex2D(_MainTex, i.uv);
	fixed4 luminance = 0;
	for (int mip = 7; mip > 2; --mip)
	{
		uv.zw = mip;
		luminance += tex2Dlod(_Luminance, uv);
	}
	luminance *= 0.2;
	return saturate(col + luminance);
}

#endif //POST_PROCESS_BLOOM_INCLUDED