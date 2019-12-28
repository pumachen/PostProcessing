﻿Shader "Hidden/PostProcess/Uber"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BloomTex ("BloomTex", 2D) = "black" {}
    }
    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma multi_compile BLOOM_ENABLED _
            #pragma vertex vert_img
            #pragma fragment frag

            #include "UnityCG.cginc"
			#include "CGIncludes/Bloom.cginc"

            //sampler2D _MainTex;

            fixed4 frag (v2f_img i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
            #if BLOOM_ENABLED
                col = ApplyBloom(col, i.uv);
            #endif
                return col;
            }
            ENDCG
        }
    }
}