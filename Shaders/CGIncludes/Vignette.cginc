#ifndef VIGNETTE_INCLUDED
#define VIGNETTE_INCLUDED

#include "UnityShaderVariables.cginc"
#include "Color.cginc"

sampler2D _Vignette_Mask;
fixed4 _Vignette_Color;
float _Vignette_Opacity;

float2 _Vignette_Center;
float4 _Vignette_Params;
// x: intensity * 3
// y: smoothness * 5
// z: Roundness
// w: aspect ratio

fixed4 frag_VignetteMaskBaker(v2f_img i) : SV_Target
{
    half2 d = abs(i.uv - _Vignette_Center) * _Vignette_Params.x;
    d.x *= _Vignette_Params.w;
    d = pow(saturate(d), _Vignette_Params.z); // Roundness
    return pow(saturate(1.0 - dot(d, d)), _Vignette_Params.y);
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