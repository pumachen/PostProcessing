#ifndef COLORGRADIING_INCLUDED
#define COLORGRADIING_INCLUDED

#include "Color.cginc"

sampler2D _LUT;
float4 _LUT_TexelSize ;

struct ParamsLogC
{
	half cut;
	half a;
    half b;
    half c;
    half d;
    half e;
    half f;
};

static const ParamsLogC LogC = 
{
    0.011361, // cut
    5.555556, // a
    0.047996, // b
    0.244161, // c
    0.386036, // d
    5.301883, // e
    0.092819  // f
};

half LinearToLogC(half x)
{
    return LogC.c * log10(LogC.a * x + LogC.b) + LogC.d;
}

half LinearToLogC_Precise(half x)
{
    if(x >  LogC.cut)
        return LogC.c * log10(LogC.a * x + LogC.b) + LogC.d;
    else
        return LogC.e * x + LogC.f;
}

half3 LinearToLogC(half3 col)
{
    return LogC.c * log10(LogC.a * col + LogC.b) + LogC.d;
}

half3 LinearToLogC_Precise(half3 col)
{
    return half3(
        LinearToLogC_Precise(col.x),
        LinearToLogC_Precise(col.y),
        LinearToLogC_Precise(col.z)
    );
}

half LogCToLinear(half x)
{
    return (pow(10.0, (x - LogC.d) / LogC.c) - LogC.b) / LogC.a;
}

half LogCToLinear_Precise(half x)
{
    if (x > LogC.e * LogC.cut + LogC.f)
        return (pow(10.0, (x - LogC.d) / LogC.c) - LogC.b) / LogC.a;
    else
        return (x - LogC.f) / LogC.e;
}

half3 LogCToLinear(half3 col)
{
    return (pow(10.0, (col - LogC.d) / LogC.c) - LogC.b) / LogC.a;
}

half3 LogCToLinear_Precise(half3 col)
{
    return half3(
        LogCToLinear_Precise(col.x),
        LogCToLinear_Precise(col.y),
        LogCToLinear_Precise(col.z)
    );
}

half3 ApplyLut2D(sampler2D tex, half3 uvw, half3 scaleOffset)
{
    // Strip format where `height = sqrt(width)`
    uvw.z *= scaleOffset.z;
    float shift = floor(uvw.z);
    uvw.xy = uvw.xy * scaleOffset.z * scaleOffset.xy + scaleOffset.xy * 0.5;
    uvw.x += shift * scaleOffset.y;
    uvw.xyz = lerp(
        tex2D(tex, uvw.xy).rgb,
        tex2D(tex, uvw.xy + float2(scaleOffset.y, 0.0)).rgb,
        uvw.z - shift
    );
    return uvw;
}

half3 ApplyLut(half3 color)
{
    half3 scaleOffset = half3(_LUT_TexelSize.xy, _LUT_TexelSize.w - 1);
    return SRGBToLinear(ApplyLut2D(_LUT, LinearToSRGB(saturate(color)), scaleOffset));
}

half4 ApplyLut(half4 color)
{
    color.rgb = ApplyLut(color.rgb);
    return color;
}

#endif //COLORGRADIING_INCLUDED