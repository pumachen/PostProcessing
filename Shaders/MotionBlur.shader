﻿Shader "Hidden/PostProcess/MotionBlur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_MotionBlurFactor("Blur Factor", Range(0, 1)) = 0.05
    }
    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag_MotionBlur

			#define BLUR_STEPS 7.0
			#define DEPTH_TEXTURE

            sampler2D _MainTex;

			#include "UnityCG.cginc"
			#include "CGIncludes/MotionBlur.cginc"

            ENDCG
        }
    }
}
