#ifndef POST_PROCESS_BLOOM_INCLUDED
#define POST_PROCESS_BLOOM_INCLUDED

sampler2D _MainTex;
sampler2D _Luminance;

fixed4 frag_DownSample(v2f_img i) : SV_Target
{
	fixed4 col = tex2D(_MainTex, i.uv);
	//float luminance = Luminance(col.rgb);
	float luminance = max(col.r, max(col.g, col.b));
	return saturate(pow(luminance, 8)) * col;
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