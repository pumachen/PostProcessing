Shader "Hidden/PostProcess/VignetteMaskBaker"
{
    Properties 
    {
        _Vignette_Center ("Vignette Center", Vector) = (0.5, 0.5, 0, 0)
        _Vignette_Params ("Vignette Params (Intensity, Smoothness, Roundness, IsRounded)", Vector) = (0, 0, 0, 0)
    }
    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag_VignetteMaskBaker

            #include "UnityCG.cginc"
			#include "CGIncludes/Vignette.cginc"

            ENDCG
        }
    }
}