Shader "Hidden/PostProcess/Uber"
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
            #pragma multi_compile COLORGRADING_ENABLED _
            #pragma vertex vert_img
            #pragma fragment frag

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;


            #include "UnityCG.cginc"
            //#include "CGIncludes/MotionBlur.cginc"
			#include "CGIncludes/Bloom.cginc"
            #include "CGIncludes/ChromaticAberration.cginc"
            #include "CGIncludes/ColorGrading.cginc"

            fixed4 frag (v2f_img i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
            #if BLOOM_ENABLED
                col = ApplyBloom(col, i.uv);
            #endif

            #if COLORGRADING_ENABLED
                col = ApplyLut(col);
            #endif
                return col;
            }
            ENDCG
        }
    }
}
