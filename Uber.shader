Shader "Hidden/Uber"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Cull Off ZWrite Off ZTest Always

		Pass //Uber
		{
			Name "Uber"
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag

			#include "UnityCG.cginc"
			sampler2D _MainTex;

			fixed4 frag(v2f_img i) : SV_Target
			{
				return 1 - tex2D(_MainTex, i.uv);
			}
			ENDCG
		}

		Pass //MotionBlur
		{
			Name "MotionBlur"
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment fragMotionBlur

			#define NUM_SAMPLERS 9
			#define DEPTH_TEXTURE

			#include "UnityCG.cginc"
			#include "MotionBlur/MotionBlur.cginc"
			ENDCG
		}
    }
}
