Shader "Hidden/PostProcess/Lut2DBaker"
{
	Properties
	{
		_Balance("Balance", Vector) = (0,0,0,0)

		_Lift("Lift", Vector) = (0,0,0,0)
		_InvGamma("InvGamma", Vector) = (0,0,0,0)
		_Gain("Gain", Vector) = (0,0,0,0)

		_Offset("Offset", Vector) = (0,0,0,0)
		_Power("Power", Vector) = (0,0,0,0)
		_Slope("Slope", Vector) = (0,0,0,0)

		_HueShift("HueShift", Float) = 0
		_Saturation("Saturation", Float) = 0
		_Contrast("Contrast", Float) = 0

		_ChannelMixerRed("ChannelMixerRed", Vector) = (0,0,0,0)
		_ChannelMixerGreen("ChannelMixerGreen", Vector) = (0,0,0,0)
		_ChannelMixerBlue("ChannelMixerBlue", Vector) = (0,0,0,0)

		_NeutralTonemapperParams1("NeutralTonemapperParams1", Vector) = (0,0,0,0)
		_NeutralTonemapperParams2("NeutralTonemapperParams2", Vector) = (0,0,0,0)

		_HueVsHue("HueVsHue", 2D) = "grey"
		_HueVsSat("HueVsSat", 2D) = "grey"
		_SatVsSat("SatVsSat", 2D) = "grey"
		_LumVsSat("LumVsSat", 2D) = "grey"

		_Master("Master", 2D) = "white"
		_Red("Red", 2D) = "white"
		_Green("Green", 2D) = "white"
		_Blue("Blue", 2D) = "white"

		_LutParams("LutParams", Vector) = (0,0,0,0)
	}
	CGINCLUDE

	#pragma target 3.0
	#pragma multi_compile __ TONEMAPPING_NEUTRAL TONEMAPPING_FILMIC

	#include "UnityCG.cginc"
	#include "CGIncludes/ACES.cginc"
	#include "CGIncludes/StdLib.cginc"
	#include "CGIncludes/ColorGrading.cginc"
	#include "CGIncludes/Tonemapping.cginc"

	half3 _Balance;

	half3 _Lift;
	half3 _InvGamma;
	half3 _Gain;

	half3 _Offset;
	half3 _Power;
	half3 _Slope;

	half _HueShift;
	half _Saturation;
	half _Contrast;

	half3 _ChannelMixerRed;
	half3 _ChannelMixerGreen;
	half3 _ChannelMixerBlue;

	half4 _NeutralTonemapperParams1;
	half4 _NeutralTonemapperParams2;

	sampler2D _HueVsHue;
	sampler2D _HueVsSat;
	sampler2D _SatVsSat;
	sampler2D _LumVsSat;

	sampler2D _Master;
	sampler2D _Red;
	sampler2D _Green;
	sampler2D _Blue;

	half4 _LutParams;

	//
	// Returns the default value for a given position on a 2D strip-format color lookup table
	// params = (lut_height, 0.5 / lut_width, 0.5 / lut_height, lut_height / lut_height - 1)
	//
	float3 GetLutStripValue(float2 uv, float4 params)
	{
		uv -= params.yz;
		float3 color;
		color.r = frac(uv.x * params.x);
		color.b = uv.x - color.r / params.x;
		color.g = uv.y;
		return color * params.w;
	}

	half3 ColorGrade(half3 color)
	{
		half3 aces = unity_to_ACES(color);

		// ACEScc (log) space
		half3 acescc = ACES_to_ACEScc(aces);

		acescc = OffsetPowerSlope(acescc, _Offset, _Power, _Slope);

		half2 hs = RgbToHsv(acescc).xy;
		half satMultiplier = SecondaryHueSat(hs.x, _HueVsSat);
		satMultiplier *= SecondarySatSat(hs.y, _SatVsSat);
		satMultiplier *= SecondaryLumSat(AcesLuminance(acescc), _LumVsSat);

		acescc = Saturation(acescc, _Saturation * satMultiplier);
		acescc = ContrastLog(acescc, _Contrast);

		aces = ACEScc_to_ACES(acescc);

		// ACEScg (linear) space
		half3 acescg = ACES_to_ACEScg(aces);

		acescg = WhiteBalance(acescg, _Balance);
		acescg = LiftGammaGain(acescg, _Lift, _InvGamma, _Gain);

		half3 hsv = RgbToHsv(max(acescg, 0.0));
		hsv.x = SecondaryHueHue(hsv.x + _HueShift, _HueVsHue);
		acescg = HsvToRgb(hsv);

		acescg = ChannelMixer(acescg, _ChannelMixerRed, _ChannelMixerGreen, _ChannelMixerBlue);

#if TONEMAPPING_FILMIC

		aces = ACEScg_to_ACES(acescg);
		color = FilmicTonemap(aces);

#elif TONEMAPPING_NEUTRAL

		color = ACEScg_to_unity(acescg);
		color = NeutralTonemap(color, _NeutralTonemapperParams1, _NeutralTonemapperParams2);

#else

		color = ACEScg_to_unity(acescg);

#endif

		// YRGB curves (done in linear/LDR for now)
		color = YrgbCurve(color, _Master, _Red, _Green, _Blue);

		return color;
	}

	half4 FragCreateLut(v2f_img i) : SV_Target
	{
		// 2D strip lut
		half2 uv = i.uv - _LutParams.yz;
		half3 color;
		color.r = frac(uv.x * _LutParams.x);
		color.b = uv.x - color.r / _LutParams.x;
		color.g = uv.y;

		// Lut is in LogC
		half3 colorLogC = color * _LutParams.w;

		// Switch back to unity linear and color grade
		half3 colorLinear = (colorLogC);
		half3 graded = ColorGrade(colorLinear);

		return half4(graded, 1.0);
	}

	ENDCG

	SubShader
	{
		Cull Off ZWrite Off ZTest Always

		// (0)
		Pass
		{
			CGPROGRAM

				#pragma vertex vert_img
				#pragma fragment FragCreateLut

			ENDCG
		}
	}
}
