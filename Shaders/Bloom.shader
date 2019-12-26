Shader "Hidden/PostProcess/Bloom"
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

            #include "UnityCG.cginc"
			#include "CGIncludes/Bloom.cginc"

            ENDCG
        }
    }
}
