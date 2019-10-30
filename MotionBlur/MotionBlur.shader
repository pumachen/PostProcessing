Shader "Hidden/MotionBlur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_MotionBlurFactor("Blur Factor", Range(0, 0.1)) = 0.05
    }
    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment fragMotionBlur

			#define NUM_SAMPLERS 9
			#define DEPTH_TEXTURE

            #include "UnityCG.cginc"
			#include "MotionBlur.cginc"

            ENDCG
        }
    }
}
