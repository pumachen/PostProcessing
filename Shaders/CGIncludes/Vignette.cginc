#ifndef VIGNETTE_INCLUDED
#define VIGNETTE_INCLUDED

#include "UnityShaderVariables.cginc"
#include "Color.cginc"

sampler2D _Vignette_Mask;
fixed4 _Vignette_Color;
float _Vignette_Opacity;

float2 _Vignette_Center;
float4 _Vignette_Params;
#define _Vignette_Intensity  (_Vignette_Params.x)
#define _Vignette_Smoothness (_Vignette_Params.y)
#define _Vignette_Roundness  (_Vignette_Params.z)
#define _Vignette_Aspect     (_Vignette_Params.w)

fixed4 frag_VignetteMaskBaker(v2f_img i) : SV_Target
{
    half2 d = abs(i.uv - _Vignette_Center) * _Vignette_Intensity;
    d.x *= _Vignette_Aspect;
    d = pow(saturate(d), _Vignette_Roundness); // Roundness
    return pow(saturate(1.0 - dot(d, d)), _Vignette_Smoothness);
}

fixed4 ApplyVignette(fixed4 color, float2 uv)
{
    half2 mask = tex2D(_Vignette_Mask, uv).ra;
    half vfactor = mask.x * mask.y;

#if !UNITY_COLORSPACE_GAMMA
    //vfactor = SRGBToLinear(vfactor);
#endif

    half3 new_color = color.rgb * lerp(_Vignette_Color, (1.0).xxx, vfactor);
    color.rgb = lerp(color.rgb, new_color, _Vignette_Opacity);
    color.a = lerp(1.0, color.a, vfactor);
    return color;
}

#endif //VIGNETTE_INCLUDED