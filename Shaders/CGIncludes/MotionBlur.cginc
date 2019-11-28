#ifndef POST_PROCESS_MOTION_BLUR_INCLUDED
#define POST_PROCESS_MOTION_BLUR_INCLUDED

sampler2D _MainTex;

#ifdef DEPTH_TEXTURE
sampler2D _CameraDepthTexture;
#elif DEPTH_NORMAL_TEXTURE
sampler2D _CameraDepthNormalsTexture;
#endif

float4x4 _CurrentToPrevProjPos;
float _MotionBlurFactor;

fixed4 frag_MotionBlur(v2f_img i) : SV_Target
{
#ifdef DEPTH_TEXTURE
	float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv);
#elif DEPTH_NORMAL_TEXTURE
	float depth;
	float3 normal;
	DecodeDepthNormal(tex2D(_CameraDepthNormalsTexture, i.uv), depth, normal);
#endif
	float4 projPos = float4(i.uv.xy * 2.0 - 1.0, depth, 1.0);
	float4 prevPos = mul(_CurrentToPrevProjPos, projPos);
	prevPos = lerp(projPos, prevPos, step(0, depth));
	prevPos /= prevPos.w;
	float2 blurVec = (prevPos.xy - projPos.xy) * _MotionBlurFactor;
	float2 dv = blurVec / NUM_SAMPLERS;

	float2 uv = i.uv;
	fixed4 col = tex2D(_MainTex, uv);
	if (depth > 0)
	{
		for (int si = 1; si < NUM_SAMPLERS; ++si)
		{
			uv += dv;
			col += tex2D(_MainTex, uv);
		}
		col /= NUM_SAMPLERS;
	}
	return col;
}
#endif //POST_PROCESS_MOTION_BLUR_INCLUDED