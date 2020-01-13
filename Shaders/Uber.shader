Shader "Hidden/PostProcess/Uber"
{
    Properties
    {
        _MainTex      ("MainTex",       2D) = "white" {}
        _BloomTex     ("BloomTex",      2D) = "black" {}
        _SpectralLut  ("Spectral LUT",  2D) = "white" {}
        _VignetteMask ("Vignette Mask", 2D) = "white" {}
    }
    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma multi_compile BLOOM_ENABLED __
            #pragma multi_compile COLORGRADING_ENABLED __
            #pragma multi_compile CHROMATIC_ABERRATION_ENABLED __
            #pragma multi_compile VIGNETTE_ENABLED __
            #pragma vertex vert_img
            #pragma fragment frag

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;


            #include "UnityCG.cginc"
            //#include "CGIncludes/MotionBlur.cginc"
			#include "CGIncludes/Bloom.cginc"
            #include "CGIncludes/ChromaticAberration.cginc"
            #include "CGIncludes/ColorGrading.cginc"
            #include "CGIncludes/Vignette.cginc"

            fixed4 frag (v2f_img i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

            #if CHROMATIC_ABERRATION_ENABLED
                col = ApplyChromatic(col, i.uv);
            #endif

            #if BLOOM_ENABLED
                col = ApplyBloom(col, i.uv);
            #endif

            #if COLORGRADING_ENABLED
                col = ApplyLut(col);
            #endif

            #if VIGNETTE_ENABLED
                col = ApplyVignette(col, i.uv);
            #endif
                return col;
            }
            ENDCG
        }
    }
}
