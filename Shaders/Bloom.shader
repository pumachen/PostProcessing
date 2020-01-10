Shader "Hidden/PostProcess/FastBloom"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag_DownSample

            sampler2D _MainTex;

            #include "UnityCG.cginc"
			#include "CGIncludes/Bloom.cginc"

            ENDCG
        }
    }
}
