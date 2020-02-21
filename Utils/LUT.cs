using UnityEngine;
using UnityEditor;
using UnityEngine.Events;

namespace Omega.Rendering.PostProcessing
{
    [System.Serializable]
    public class LUT : BufferRT
    {
        public int size = 32;
        public int precision = 128;

        #region ToneMapping Params

        public ToneMappingParams tonemapping = ToneMappingParams.Default;

        [System.Serializable]
        public struct ToneMappingParams : IPostProcessParam<ToneMappingParams>
        {
            [SerializeField]
            private ToneMapper m_tonemapper;
            public ToneMapper tonemapper
            {
                get => m_tonemapper;
                set
                {
                    if(value != m_tonemapper)
                    {
                        m_tonemapper = value;
                        m_onValueChange?.Invoke(this);
                    }
                }
            }

            // Neutral settings
            [SerializeField]
            private Vector4 m_neutralInOut;

            [SerializeField]
            private Vector4 m_neutralWhiteLevel;

            //[Range(-0.1f, 0.1f)]
            public float neutralBlackIn
            {
                get => m_neutralInOut.x;
                set
                {
                    if(value != m_neutralInOut.x)
                    {
                        m_neutralInOut.x = value;
                        m_onValueChange?.Invoke(this);
                    }
                }
            }

            public float neutralWhiteIn
            {
                get => m_neutralInOut.y;
                set
                {
                    if (value != m_neutralInOut.y)
                    {
                        m_neutralInOut.y = value;
                        m_onValueChange?.Invoke(this);
                    }
                }
            }

            public float neutralBlackOut
            {
                get => m_neutralInOut.z;
                set
                {
                    if (value != m_neutralInOut.z)
                    {
                        m_neutralInOut.z = value;
                        m_onValueChange?.Invoke(this);
                    }
                }
            }

            public float neutralWhiteOut
            {
                get => m_neutralInOut.w;
                set
                {
                    if (value != m_neutralInOut.w)
                    {
                        m_neutralInOut.w = value;
                        m_onValueChange?.Invoke(this);
                    }
                }
            }

            public float neutralWhiteLevel
            {
                get => m_neutralWhiteLevel.x;
                set
                {
                    if(value != m_neutralWhiteLevel.x)
                    {
                        m_neutralWhiteLevel.x = value;
                        m_onValueChange?.Invoke(this);
                    }
                }
            }

            public float neutralWhiteClip
            {
                get => m_neutralWhiteLevel.y;
                set
                {
                    if (value != m_neutralWhiteLevel.y)
                    {
                        m_neutralWhiteLevel.y = value;
                        m_onValueChange?.Invoke(this);
                    }
                }
            }

            private event UnityAction<ToneMappingParams> m_onValueChange;
            public event UnityAction<ToneMappingParams> onValueChange
            {
                add
                {
                    if (value != null)
                    {
                        m_onValueChange += value;
                        value.Invoke(this);
                    }
                }
                remove => m_onValueChange -= value;
            }

            public static ToneMappingParams Default
            {
                get
                {
                    return new ToneMappingParams
                    {
                        m_tonemapper = ToneMapper.None,

                        m_neutralInOut = new Vector4(0.1f, 10f, 0f, 10f),
                        m_neutralWhiteLevel = new Vector4(10f, 6f)
                    };
                }
            }

#if UNITY_EDITOR
            private bool foldout;

            public void OnInspectorGUI()
            {
                EditorGUILayout.BeginHorizontal();
                foldout = EditorGUILayout.Foldout(foldout, "Tone Mapping");
                if (GUILayout.Button("Reset"))
                {
                    this = Default;
                }
                EditorGUILayout.EndHorizontal();
                if (foldout)
                {
                    tonemapper = (ToneMapper)EditorGUILayout.EnumPopup("Tone Mapper", tonemapper);
                    if (tonemapper == ToneMapper.Neutral)
                    {
                        neutralBlackIn = EditorGUILayout.Slider("BlackIn", neutralBlackIn, -0.1f, 0.1f);
                        neutralWhiteIn = EditorGUILayout.Slider("WhiteIn", neutralWhiteIn, 1f, 20f);
                        neutralBlackOut = EditorGUILayout.Slider("BlackOut", neutralBlackOut, -0.09f, 0.1f);
                        neutralWhiteOut = EditorGUILayout.Slider("WhiteOut", neutralWhiteOut, 1f, 19f);
                        neutralWhiteLevel = EditorGUILayout.Slider("WhiteLevel", neutralWhiteLevel, 0.1f, 20f);
                        neutralWhiteClip = EditorGUILayout.Slider("WhiteClip", neutralWhiteClip, 1f, 10f);
                    }
                }
            }
#endif
            public enum ToneMapper
            {
                None,
                ACES,
                Neutral
            }
        }

        #endregion //ToneMapping Params

        #region Basic Params

        public BasicParams basic = BasicParams.Default;

        [System.Serializable]
        public struct BasicParams : IPostProcessParam<BasicParams>
        {
            [SerializeField]
            private Vector4 m_params;

            [SerializeField]
            private float m_hueShift;

            public float temperature
            {
                get => m_params.x;
                set
                {
                    if(value != m_params.x)
                    {
                        m_params.x = value;
                        m_onValueChange?.Invoke(this);
                    }
                }
            }

            public float tint
            {
                get => m_params.y;
                set
                {
                    if (value != m_params.y)
                    {
                        m_params.y = value;
                        m_onValueChange?.Invoke(this);
                    }
                }
            }

            public float saturation
            {
                get => m_params.z;
                set
                {
                    if (value != m_params.z)
                    {
                        m_params.z = value;
                        m_onValueChange?.Invoke(this);
                    }
                }
            }

            public float contrast
            {
                get => m_params.w;
                set
                {
                    if (value != m_params.w)
                    {
                        m_params.w = value;
                        m_onValueChange?.Invoke(this);
                    }
                }
            }

            public float hueShift
            {
                get => m_hueShift;
                set
                {
                    if(value != m_hueShift)
                    {
                        m_hueShift = value;
                        m_onValueChange?.Invoke(this);
                    }
                }
            }

            private event UnityAction<BasicParams> m_onValueChange;
            public event UnityAction<BasicParams> onValueChange
            {
                add
                {
                    if (value != null)
                    {
                        m_onValueChange += value;
                        value.Invoke(this);
                    }
                }
                remove => m_onValueChange -= value;
            }

            public static BasicParams Default
            {
                get
                {
                    return new BasicParams
                    {
                        m_params = new Vector4(0f, 0f, 1f, 1f),
                        m_hueShift = 0f
                    };
                }
            }

#if UNITY_EDITOR
            private bool foldout;

            public void OnInspectorGUI()
            {
                EditorGUILayout.BeginHorizontal();
                foldout = EditorGUILayout.Foldout(foldout, "Basic Settings");
                if (GUILayout.Button("Reset"))
                {
                    this = Default;
                }
                EditorGUILayout.EndHorizontal();
                if (foldout)
                {
                    temperature = EditorGUILayout.Slider("Temperature", temperature, -100f, 100f);
                    tint = EditorGUILayout.Slider("Tint", tint, -100f, 100f);
                    saturation = EditorGUILayout.Slider("Saturation", saturation, 0f, 2f);
                    contrast = EditorGUILayout.Slider("Contrast", contrast, 0f, 2f);
                    hueShift = EditorGUILayout.Slider("Hue Shift", hueShift, -180f, 180f);
                }
            }
#endif
        }

        #endregion //Basic Params

        #region ChannelMixer Params

        public ChannelMixerParams channelMixer = ChannelMixerParams.Default;

        [System.Serializable]
        public struct ChannelMixerParams : IPostProcessParam<ChannelMixerParams>
        {
            [SerializeField]
            private Vector3 m_red;
            public Vector3 red
            {
                get => m_red;
                set
                {
                    if(value != m_red)
                    {
                        m_red = value;
                        m_onValueChange?.Invoke(this);
                    }
                }
            }

            [SerializeField]
            private Vector3 m_green;
            public Vector3 green
            {
                get => m_green;
                set
                {
                    if (value != m_green)
                    {
                        m_green = value;
                        m_onValueChange?.Invoke(this);
                    }
                }
            }

            [SerializeField]
            private Vector3 m_blue;
            public Vector3 blue
            {
                get => m_blue;
                set
                {
                    if (value != m_blue)
                    {
                        m_blue = value;
                        m_onValueChange?.Invoke(this);
                    }
                }
            }

            private event UnityAction<ChannelMixerParams> m_onValueChange;
            public event UnityAction<ChannelMixerParams> onValueChange
            {
                add
                {
                    if (value != null)
                    {
                        m_onValueChange += value;
                        value.Invoke(this);
                    }
                }
                remove => m_onValueChange -= value;
            }

            public static ChannelMixerParams Default
            {
                get
                {
                    return new ChannelMixerParams
                    {
                        m_red = new Vector3(1f, 0f, 0f),
                        m_green = new Vector3(0f, 1f, 0f),
                        m_blue = new Vector3(0f, 0f, 1f)
                    };
                }
            }

#if UNITY_EDITOR
            private bool foldout;

            public void OnInspectorGUI()
            {
                EditorGUILayout.BeginHorizontal();
                foldout = EditorGUILayout.Foldout(foldout, "Channel Mixer");
                if (GUILayout.Button("Reset"))
                {
                    this = Default;
                }
                EditorGUILayout.EndHorizontal();
                if (foldout)
                {
                    red = EditorGUILayout.Vector3Field("Red", red);
                    green = EditorGUILayout.Vector3Field("Green", green);
                    blue = EditorGUILayout.Vector3Field("Blue", blue);
                }
            }
#endif //UNITY_EDITOR

            
        }

        #endregion //ChannelMixer Params

        #region ColorWheels Params

        public ColorWheelParams colorWheels = ColorWheelParams.Default;

        public enum ColorWheelMode
        {
            Linear,
            Log
        }

        [System.Serializable]
        public struct ColorWheelParams : IPostProcessParam<ColorWheelParams>
        {
            [SerializeField]
            private ColorWheelMode m_mode;
            public ColorWheelMode mode
            {
                get => m_mode;
                set
                {
                    if(value != m_mode)
                    {
                        m_mode = value;
                        m_onValueChange?.Invoke(this);
                    }
                }
            }

            // Log Wheel Params
            [SerializeField]
            private Color m_slope;
            public Color slope
            {
                get => m_slope;
                set
                {
                    if (value != m_slope)
                    {
                        m_slope = value;
                        m_onValueChange?.Invoke(this);
                    }
                }
            }

            [SerializeField]
            private Color m_power;
            public Color power
            {
                get => m_power;
                set
                {
                    if (value != m_power)
                    {
                        m_power = value;
                        m_onValueChange?.Invoke(this);
                    }
                }
            }

            [SerializeField]
            private Color m_offset;
            public Color offset
            {
                get => m_offset;
                set
                {
                    if (value != m_offset)
                    {
                        m_offset = value;
                        m_onValueChange?.Invoke(this);
                    }
                }
            }


            // Linear Wheel Params
            [SerializeField]
            private Color m_lift;
            public Color lift
            {
                get => m_lift;
                set
                {
                    if (value != m_lift)
                    {
                        m_lift = value;
                        m_onValueChange?.Invoke(this);
                    }
                }
            }

            [SerializeField]
            private Color m_gamma;
            public Color gamma
            {
                get => m_gamma;
                set
                {
                    if (value != m_gamma)
                    {
                        m_gamma = value;
                        m_onValueChange?.Invoke(this);
                    }
                }
            }

            [SerializeField]
            private Color m_gain;
            public Color gain
            {
                get => m_gain;
                set
                {
                    if (value != m_gain)
                    {
                        m_gain = value;
                        m_onValueChange?.Invoke(this);
                    }
                }
            }

            private event UnityAction<ColorWheelParams> m_onValueChange;
            public event UnityAction<ColorWheelParams> onValueChange
            {
                add
                {
                    if (value != null)
                    {
                        m_onValueChange += value;
                        value.Invoke(this);
                    }
                }
                remove => m_onValueChange -= value;
            }

            public static ColorWheelParams Default
            {
                get
                {
                    return new ColorWheelParams
                    {
                        m_mode = ColorWheelMode.Linear,

                        m_slope = Color.clear,
                        m_power = Color.clear,
                        m_offset = Color.clear,

                        m_lift = Color.clear,
                        m_gamma = Color.clear,
                        m_gain = Color.clear
                    };
                }
            }

#if UNITY_EDITOR
            private bool foldout;
            public void OnInspectorGUI()
            {
                EditorGUILayout.BeginHorizontal();
                foldout = EditorGUILayout.Foldout(foldout, "Color Wheel");
                if (GUILayout.Button("Reset"))
                {
                    this = Default;
                }
                EditorGUILayout.EndHorizontal();
                if (foldout)
                {
                    mode = (ColorWheelMode)EditorGUILayout.EnumPopup("Mode", mode);
                    switch (mode)
                    {
                        case ColorWheelMode.Log:
                            {
                                slope = EditorGUILayout.ColorField("Slope", slope);
                                power = EditorGUILayout.ColorField("Power", power);
                                offset = EditorGUILayout.ColorField("Offset", offset);
                                break;
                            }
                        case ColorWheelMode.Linear:
                            {
                                lift = EditorGUILayout.ColorField("Lift", lift);
                                gamma = EditorGUILayout.ColorField("Gamma", gamma);
                                gain = EditorGUILayout.ColorField("Gain", gain);
                                break;
                            }
                    }
                }
            }
#endif
        }

        #endregion //ColorWheel Params

        #region Curve Settings

        public CurveSettings curveSettings = CurveSettings.Default;

        [System.Serializable]
        public struct CurveSettings
        {
            public RampMap HueVsHue;
            public RampMap HueVsSat;
            public RampMap SatVsSat;
            public RampMap LumVsSat;

            public RampMap Master;
            public RampMap Red;
            public RampMap Green;
            public RampMap Blue;

            public static CurveSettings Default
            {
                get
                {
                    return new CurveSettings()
                    {
                        HueVsHue = new RampMap(AnimationCurve.Constant(0, 1, 0.5f)),
                        HueVsSat = new RampMap(AnimationCurve.Constant(0, 1, 0.5f)),
                        SatVsSat = new RampMap(AnimationCurve.Constant(0, 1, 0.5f)),
                        LumVsSat = new RampMap(AnimationCurve.Constant(0, 1, 0.5f)),

                        Master = new RampMap(AnimationCurve.Linear(0, 0, 1, 1)),
                        Red = new RampMap(AnimationCurve.Linear(0, 0, 1, 1)),
                        Green = new RampMap(AnimationCurve.Linear(0, 0, 1, 1)),
                        Blue = new RampMap(AnimationCurve.Linear(0, 0, 1, 1))
                    };
                }
            }

#if UNITY_EDITOR
            private bool foldout;

            public void OnInspectorGUI()
            {
                EditorGUILayout.BeginHorizontal();
                foldout = EditorGUILayout.Foldout(foldout, "Curves");
                if(GUILayout.Button("Reset"))
                {
                    this = Default;
                }
                EditorGUILayout.EndHorizontal();
                if(foldout)
                {
                    HueVsHue.OnInspectorGUI("HueVsHue");
                    HueVsSat.OnInspectorGUI("HueVsSat");
                    SatVsSat.OnInspectorGUI("SatVsSat");
                    LumVsSat.OnInspectorGUI("LumVsSat");

                    Master.OnInspectorGUI("Master");
                    Red.OnInspectorGUI("Red");
                    Green.OnInspectorGUI("Green");
                    Blue.OnInspectorGUI("Blue");

                    HueVsHue.Update();
                    HueVsSat.Update();
                    SatVsSat.Update();
                    LumVsSat.Update();

                    Master.Update();
                    Red.Update();
                    Green.Update();
                    Blue.Update();
                }
            }
#endif
        }

#endregion //Curve Settings

        private Material m_material;
        protected Material material
        {
            get
            {
                if (!m_material)
                {
                    Shader shader = Shader.Find("Hidden/PostProcess/Lut2DBaker");
                    m_material = new Material(shader);
                }
                return m_material;
            }
        }

        public LUT(int size) : base( 
            new Resolution() { width = size * size, height = size }, 
            RenderTextureFormat.ARGB32 )
        {
            name = "LUT";
            this.size = size;
        }

#if UNITY_EDITOR
        public void OnInspectorGUI()
        {
            tonemapping.OnInspectorGUI();
            basic.OnInspectorGUI();
            channelMixer.OnInspectorGUI();
            colorWheels.OnInspectorGUI();
            curveSettings.OnInspectorGUI();

            GUIStyle style = new GUIStyle()
            {
                stretchHeight = true,
                stretchWidth = true
            };
            if(GUILayout.Button(RT, style, GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.9f)))
            {
                Selection.objects = new Object[] { RT };
            }
        }
#endif

        public void Render()
        {
            material.SetVector(Uniforms._LutParams, new Vector4(
                    size,
                    0.5f / (size * size),
                    0.5f / size,
                    size / (size - 1f)
                ));

            material.shaderKeywords = null;
            switch(tonemapping.tonemapper)
            {
                case ToneMappingParams.ToneMapper.Neutral:
                {
                    material.EnableKeyword("TONEMAPPING_NEUTRAL");

                    const float scaleFactor = 20f;
                    const float scaleFactorHalf = scaleFactor * 0.5f;

                    float inBlack = tonemapping.neutralBlackIn * scaleFactor + 1f;
                    float outBlack = tonemapping.neutralBlackOut * scaleFactorHalf + 1f;
                    float inWhite = tonemapping.neutralWhiteIn / scaleFactor;
                    float outWhite = 1f - tonemapping.neutralWhiteOut / scaleFactor;
                    float blackRatio = inBlack / outBlack;
                    float whiteRatio = inWhite / outWhite;

                    const float a = 0.2f;
                    float b = Mathf.Max(0f, Mathf.LerpUnclamped(0.57f, 0.37f, blackRatio));
                    float c = Mathf.LerpUnclamped(0.01f, 0.24f, whiteRatio);
                    float d = Mathf.Max(0f, Mathf.LerpUnclamped(0.02f, 0.20f, blackRatio));
                    const float e = 0.02f;
                    const float f = 0.30f;

                    material.SetVector(Uniforms._NeutralTonemapperParams1, new Vector4(a, b, c, d));
                    material.SetVector(Uniforms._NeutralTonemapperParams2, new Vector4(e, f, tonemapping.neutralWhiteLevel, tonemapping.neutralWhiteClip / scaleFactorHalf));
                    break;
                }

                case ToneMappingParams.ToneMapper.ACES:
                {
                    material.EnableKeyword("TONEMAPPING_FILMIC");
                    break;
                }
            }

            // Color balance & basic grading settings
            material.SetFloat(Uniforms._HueShift, basic.hueShift / 360f);
            material.SetFloat(Uniforms._Saturation, basic.saturation);
            material.SetFloat(Uniforms._Contrast, basic.contrast);
            material.SetVector(Uniforms._Balance, CalculateColorBalance(basic.temperature, basic.tint));

            // Lift / Gamma / Gain
            Vector3 lift, gamma, gain;
            CalculateLiftGammaGain(
                colorWheels.lift,
                colorWheels.gamma,
                colorWheels.gain,
                out lift, out gamma, out gain
                );

            material.SetVector(Uniforms._Lift, lift);
            material.SetVector(Uniforms._InvGamma, gamma);
            material.SetVector(Uniforms._Gain, gain);

            // Slope / Power / Offset
            Vector3 slope, power, offset;
            CalculateSlopePowerOffset(
                colorWheels.slope,
                colorWheels.power,
                colorWheels.offset,
                out slope, out power, out offset
                );

            material.SetVector(Uniforms._Slope, slope);
            material.SetVector(Uniforms._Power, power);
            material.SetVector(Uniforms._Offset, offset);

            // Channel mixer
            material.SetVector(Uniforms._ChannelMixerRed, channelMixer.red);
            material.SetVector(Uniforms._ChannelMixerGreen, channelMixer.green);
            material.SetVector(Uniforms._ChannelMixerBlue, channelMixer.blue);

            // Selective grading & YRGB curves
            material.SetTexture(Uniforms._HueVsHue, curveSettings.HueVsHue);
            material.SetTexture(Uniforms._HueVsSat, curveSettings.HueVsSat);
            material.SetTexture(Uniforms._SatVsSat, curveSettings.SatVsSat);
            material.SetTexture(Uniforms._LumVsSat, curveSettings.LumVsSat);
            material.SetTexture(Uniforms._Master, curveSettings.Master);
            material.SetTexture(Uniforms._Red, curveSettings.Red);
            material.SetTexture(Uniforms._Green, curveSettings.Green);
            material.SetTexture(Uniforms._Blue, curveSettings.Blue);

            Graphics.Blit(null, RT, material);
        }

        // An analytical model of chromaticity of the standard illuminant, by Judd et al.
        // http://en.wikipedia.org/wiki/Standard_illuminant#Illuminant_series_D
        // Slightly modifed to adjust it with the D65 white point (x=0.31271, y=0.32902).
        float StandardIlluminantY(float x)
        {
            return 2.87f * x - 3f * x * x - 0.27509507f;
        }

        // CIE xy chromaticity to CAT02 LMS.
        // http://en.wikipedia.org/wiki/LMS_color_space#CAT02
        Vector3 CIExyToLMS(float x, float y)
        {
            float Y = 1f;
            float X = Y * x / y;
            float Z = Y * (1f - x - y) / y;

            float L = 0.7328f * X + 0.4296f * Y - 0.1624f * Z;
            float M = -0.7036f * X + 1.6975f * Y + 0.0061f * Z;
            float S = 0.0030f * X + 0.0136f * Y + 0.9834f * Z;

            return new Vector3(L, M, S);
        }

        Vector3 CalculateColorBalance(float temperature, float tint)
        {
            // Range ~[-1.8;1.8] ; using higher ranges is unsafe
            float t1 = temperature / 55f;
            float t2 = tint / 55f;

            // Get the CIE xy chromaticity of the reference white point.
            // Note: 0.31271 = x value on the D65 white point
            float x = 0.31271f - t1 * (t1 < 0f ? 0.1f : 0.05f);
            float y = StandardIlluminantY(x) + t2 * 0.05f;

            // Calculate the coefficients in the LMS space.
            var w1 = new Vector3(0.949237f, 1.03542f, 1.08728f); // D65 white point
            var w2 = CIExyToLMS(x, y);
            return new Vector3(w1.x / w2.x, w1.y / w2.y, w1.z / w2.z);
        }

        static Color NormalizeColor(Color c)
        {
            float sum = (c.r + c.g + c.b) / 3f;

            if (Mathf.Approximately(sum, 0f))
                return new Color(1f, 1f, 1f, c.a);

            return new Color
            {
                r = c.r / sum,
                g = c.g / sum,
                b = c.b / sum,
                a = c.a
            };
        }

        static Vector3 ClampVector(Vector3 v, float min, float max)
        {
            return new Vector3(
                Mathf.Clamp(v.x, min, max),
                Mathf.Clamp(v.y, min, max),
                Mathf.Clamp(v.z, min, max)
                );
        }

        public static Vector3 GetLiftValue(Color lift)
        {
            const float kLiftScale = 0.1f;

            var nLift = NormalizeColor(lift);
            float avgLift = (nLift.r + nLift.g + nLift.b) / 3f;

            // Getting some artifacts when going into the negatives using a very low offset (lift.a) with non ACES-tonemapping
            float liftR = (nLift.r - avgLift) * kLiftScale + lift.a;
            float liftG = (nLift.g - avgLift) * kLiftScale + lift.a;
            float liftB = (nLift.b - avgLift) * kLiftScale + lift.a;

            return ClampVector(new Vector3(liftR, liftG, liftB), -1f, 1f);
        }

        public static Vector3 GetGammaValue(Color gamma)
        {
            const float kGammaScale = 0.5f;
            const float kMinGamma = 0.01f;

            var nGamma = NormalizeColor(gamma);
            float avgGamma = (nGamma.r + nGamma.g + nGamma.b) / 3f;

            gamma.a *= gamma.a < 0f ? 0.8f : 5f;
            float gammaR = Mathf.Pow(2f, (nGamma.r - avgGamma) * kGammaScale) + gamma.a;
            float gammaG = Mathf.Pow(2f, (nGamma.g - avgGamma) * kGammaScale) + gamma.a;
            float gammaB = Mathf.Pow(2f, (nGamma.b - avgGamma) * kGammaScale) + gamma.a;

            float invGammaR = 1f / Mathf.Max(kMinGamma, gammaR);
            float invGammaG = 1f / Mathf.Max(kMinGamma, gammaG);
            float invGammaB = 1f / Mathf.Max(kMinGamma, gammaB);

            return ClampVector(new Vector3(invGammaR, invGammaG, invGammaB), 0f, 5f);
        }

        public static Vector3 GetGainValue(Color gain)
        {
            const float kGainScale = 0.5f;

            var nGain = NormalizeColor(gain);
            float avgGain = (nGain.r + nGain.g + nGain.b) / 3f;

            gain.a *= gain.a > 0f ? 3f : 1f;
            float gainR = Mathf.Pow(2f, (nGain.r - avgGain) * kGainScale) + gain.a;
            float gainG = Mathf.Pow(2f, (nGain.g - avgGain) * kGainScale) + gain.a;
            float gainB = Mathf.Pow(2f, (nGain.b - avgGain) * kGainScale) + gain.a;

            return ClampVector(new Vector3(gainR, gainG, gainB), 0f, 4f);
        }

        public static void CalculateLiftGammaGain(Color lift, Color gamma, Color gain, out Vector3 outLift, out Vector3 outGamma, out Vector3 outGain)
        {
            outLift = GetLiftValue(lift);
            outGamma = GetGammaValue(gamma);
            outGain = GetGainValue(gain);
        }

        public static Vector3 GetSlopeValue(Color slope)
        {
            const float kSlopeScale = 0.1f;

            var nSlope = NormalizeColor(slope);
            float avgSlope = (nSlope.r + nSlope.g + nSlope.b) / 3f;

            slope.a *= 0.5f;
            float slopeR = (nSlope.r - avgSlope) * kSlopeScale + slope.a + 1f;
            float slopeG = (nSlope.g - avgSlope) * kSlopeScale + slope.a + 1f;
            float slopeB = (nSlope.b - avgSlope) * kSlopeScale + slope.a + 1f;

            return ClampVector(new Vector3(slopeR, slopeG, slopeB), 0f, 2f);
        }

        public static Vector3 GetPowerValue(Color power)
        {
            const float kPowerScale = 0.1f;
            const float minPower = 0.01f;

            var nPower = NormalizeColor(power);
            float avgPower = (nPower.r + nPower.g + nPower.b) / 3f;

            power.a *= 0.5f;
            float powerR = (nPower.r - avgPower) * kPowerScale + power.a + 1f;
            float powerG = (nPower.g - avgPower) * kPowerScale + power.a + 1f;
            float powerB = (nPower.b - avgPower) * kPowerScale + power.a + 1f;

            float invPowerR = 1f / Mathf.Max(minPower, powerR);
            float invPowerG = 1f / Mathf.Max(minPower, powerG);
            float invPowerB = 1f / Mathf.Max(minPower, powerB);

            return ClampVector(new Vector3(invPowerR, invPowerG, invPowerB), 0.5f, 2.5f);
        }

        public static Vector3 GetOffsetValue(Color offset)
        {
            const float kOffsetScale = 0.05f;

            var nOffset = NormalizeColor(offset);
            float avgOffset = (nOffset.r + nOffset.g + nOffset.b) / 3f;

            offset.a *= 0.5f;
            float offsetR = (nOffset.r - avgOffset) * kOffsetScale + offset.a;
            float offsetG = (nOffset.g - avgOffset) * kOffsetScale + offset.a;
            float offsetB = (nOffset.b - avgOffset) * kOffsetScale + offset.a;

            return ClampVector(new Vector3(offsetR, offsetG, offsetB), -0.8f, 0.8f);
        }

        public static void CalculateSlopePowerOffset(Color slope, Color power, Color offset, out Vector3 outSlope, out Vector3 outPower, out Vector3 outOffset)
        {
            outSlope = GetSlopeValue(slope);
            outPower = GetPowerValue(power);
            outOffset = GetOffsetValue(offset);
        }

        //Texture2D GetCurveTexture()
        //{
        //    /*if (m_GradingCurves == null)
        //    {
        //        m_GradingCurves = new Texture2D(k_CurvePrecision, 2, false, true)
        //        {
        //            name = "Internal Curves Texture",
        //            hideFlags = HideFlags.DontSave,
        //            anisoLevel = 0,
        //            wrapMode = TextureWrapMode.Clamp,
        //            filterMode = FilterMode.Bilinear
        //        };
        //    }*/

        //    var curves = model.settings.curves;
        //    curves.hueVShue.Cache();
        //    curves.hueVSsat.Cache();

        //    for (int i = 0; i < k_CurvePrecision; i++)
        //    {
        //        float t = i * k_CurveStep;

        //        // HSL
        //        float x = curves.hueVShue.Evaluate(t);
        //        float y = curves.hueVSsat.Evaluate(t);
        //        float z = curves.satVSsat.Evaluate(t);
        //        float w = curves.lumVSsat.Evaluate(t);
        //        m_pixels[i] = new Color(x, y, z, w);

        //        // YRGB
        //        float m = curves.master.Evaluate(t);
        //        float r = curves.red.Evaluate(t);
        //        float g = curves.green.Evaluate(t);
        //        float b = curves.blue.Evaluate(t);
        //        m_pixels[i + k_CurvePrecision] = new Color(r, g, b, m);
        //    }

        //    m_GradingCurves.SetPixels(m_pixels);
        //    m_GradingCurves.Apply(false, false);

        //    return m_GradingCurves;
        //}

        bool IsLogLutValid(RenderTexture lut)
        {
            return lut != null && lut.IsCreated() && lut.height == size;
        }

        static class Uniforms
        {
            public static readonly int _LutParams = Shader.PropertyToID("_LutParams");
            public static readonly int _NeutralTonemapperParams1 = Shader.PropertyToID("_NeutralTonemapperParams1");
            public static readonly int _NeutralTonemapperParams2 = Shader.PropertyToID("_NeutralTonemapperParams2");
            public static readonly int _HueShift = Shader.PropertyToID("_HueShift");
            public static readonly int _Saturation = Shader.PropertyToID("_Saturation");
            public static readonly int _Contrast = Shader.PropertyToID("_Contrast");
            public static readonly int _Balance = Shader.PropertyToID("_Balance");
            public static readonly int _Lift = Shader.PropertyToID("_Lift");
            public static readonly int _InvGamma = Shader.PropertyToID("_InvGamma");
            public static readonly int _Gain = Shader.PropertyToID("_Gain");
            public static readonly int _Slope = Shader.PropertyToID("_Slope");
            public static readonly int _Power = Shader.PropertyToID("_Power");
            public static readonly int _Offset = Shader.PropertyToID("_Offset");
            public static readonly int _ChannelMixerRed = Shader.PropertyToID("_ChannelMixerRed");
            public static readonly int _ChannelMixerGreen = Shader.PropertyToID("_ChannelMixerGreen");
            public static readonly int _ChannelMixerBlue = Shader.PropertyToID("_ChannelMixerBlue");
            public static readonly int _Curves = Shader.PropertyToID("_Curves");
            public static readonly int _HueVsHue = Shader.PropertyToID("_HueVsHue");
            public static readonly int _HueVsSat = Shader.PropertyToID("_HueVsSat");
            public static readonly int _SatVsSat = Shader.PropertyToID("_SatVsSat");
            public static readonly int _LumVsSat = Shader.PropertyToID("_LumVsSat");
            public static readonly int _Master = Shader.PropertyToID("_Master");
            public static readonly int _Red = Shader.PropertyToID("_Red");
            public static readonly int _Green = Shader.PropertyToID("_Green");
            public static readonly int _Blue = Shader.PropertyToID("_Blue");
            public static readonly int _LogLut = Shader.PropertyToID("_LogLut");
            public static readonly int _LogLut_Params = Shader.PropertyToID("_LogLut_Params");
            public static readonly int _ExposureEV = Shader.PropertyToID("_ExposureEV");
        }
    }
}
