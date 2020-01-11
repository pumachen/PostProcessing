#ifndef CHROMATIC_ABERRATION_INCLUDED
#define CHROMATIC_ABERRATION_INCLUDED

#include "LensDistortion.cginc"

#define MAX_CHROMATIC_SAMPLES 16
sampler2D _ChromaticAberration_SpectralLut;
float _ChromaticAberration_Amount;

fixed4 ApplyChromatic(fixed4 color, float2 uv)
{
#ifdef CHROMATIC_ABERRATION_LOW
    float2 coords = 2.0 * uv - 1.0;
    float2 end = uv - coords * dot(coords, coords) * _ChromaticAberration_Amount;
    float2 delta = (end - uv) / 3;

    half4 filterA = half4(tex2D(_ChromaticAberration_SpectralLut, float2(0.5 / 3, 0.0));
    half4 filterB = half4(tex2D(_ChromaticAberration_SpectralLut, float2(1.5 / 3, 0.0));
    half4 filterC = half4(tex2D(_ChromaticAberration_SpectralLut, float2(2.5 / 3, 0.0));

    half4 texelA = tex2Dlod(_MainTex, UnityStereoTransformScreenSpaceTex(Distort(uv)), 0);
    half4 texelB = tex2Dlod(_MainTex, UnityStereoTransformScreenSpaceTex(Distort(delta + uv)), 0);
    half4 texelC = tex2Dlod(_MainTex, UnityStereoTransformScreenSpaceTex(Distort(delta * 2.0 + uv)), 0);

    half4 sum = texelA * filterA + texelB * filterB + texelC * filterC;
    half4 filterSum = filterA + filterB + filterC;
    return sum / filterSum;
#else
    float2 coords = 2.0 * uv - 1.0;
    float2 end = uv - coords * dot(coords, coords) * _ChromaticAberration_Amount;

    float2 diff = end - uv;
    int samples = clamp(int(length(_MainTex_TexelSize.zw * diff / 2.0)), 3, MAX_CHROMATIC_SAMPLES);
    float2 delta = diff / samples;
    float2 pos = uv;
    half4 sum = (0.0).xxxx, filterSum = (0.0).xxxx;

    for (int i = 0; i < samples; i++)
    {
        half t = (i + 0.5) / samples;
        half4 s = tex2D(_MainTex, UnityStereoTransformScreenSpaceTex(Distort(pos)));
        half4 filter = half4(tex2D(_ChromaticAberration_SpectralLut, float2(t, 0.0)));

        sum += s * filter;
        filterSum += filter;
        pos += delta;
    }

    return sum / filterSum;
}
#endif // CHROMATIC_ABERRATION_LOW


#endif //CHROMATIC_ABERRATION_INCLUDED