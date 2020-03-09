using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Props = Fuxi.Rendering.PostProcessing.PostProcessProperties;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif //UNITY_EDITOR

namespace Fuxi.Rendering.PostProcessing
{
    [System.Serializable]
    public class Vignette : PostProcessEffect
    {
        protected override Shader shader
        {
            get => Shader.Find("Hidden/PostProcess/VignetteMaskBaker");
        }

        protected RenderTexture m_proceduralMask;
        public RenderTexture proceduralMask
        {
            get
            {
                if (m_proceduralMask == null)
                {
                    m_proceduralMask = new RenderTexture(
                    Screen.width / 2,
                    Screen.height / 2,
                    0,
                    RenderTextureFormat.R8)
                    { name = "Procedural Mask" };
                }
                return m_proceduralMask;
            }
        }
        [SerializeField]
        protected Texture m_mask;
        public Texture mask
        {
            get
            {
                if (m_mask != null)
                {
                    return m_mask;
                }
                return proceduralMask;
            }
            set
            {
                if(m_mask != value && m_proceduralMask != value)
                {
                    m_mask = value;
                    destMat.SetTexture(Props.vignetteMask, mask);
                }
            }
        }

        [SerializeField]
        public VignetteParams vignetteParams = VignetteParams.Default;

        [SerializeField]
        public VignetteMaskParams maskParams = VignetteMaskParams.Default;

        protected override void Init()
        {
            base.Init();
            maskParams.onValueChange += (param) =>
            {
                UpdateProceduralMask();
                destMat.SetTexture(Props.vignetteMask, proceduralMask);
            };
            vignetteParams.onValueChange += (param) =>
            {
                destMat.SetColor(Props.vignetteColor, vignetteParams.color);
                destMat.SetFloat(Props.vignetteOpacity, vignetteParams.opacity);
            };
        }

        protected override void SetProperties()
        {
            destMat.SetTexture(Props.vignetteMask, mask);
            destMat.SetColor(Props.vignetteColor, vignetteParams.color);
            destMat.SetFloat(Props.vignetteOpacity, vignetteParams.opacity);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            destMat.EnableKeyword("VIGNETTE_ENABLED");
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            destMat.DisableKeyword("VIGNETTE_ENABLED");
        }

        protected void UpdateProceduralMask()
        {
            material.SetVector(Props.vignetteCenter, maskParams.center);
            material.SetVector(Props.vignetteParams, maskParams);
            Graphics.Blit(null, m_proceduralMask, material);
        }

        public override void Process(RenderTexture src)
        {
            base.Process(src);
            if(proceduralMask.updateCount < 1)
            {
                UpdateProceduralMask();
            }
        }

#if UNITY_EDITOR
        public override string name => "Vignette";
        protected override void OnInspectorGUI()
        {
            mask = EditorGUILayout.ObjectField("Vignette Mask", mask, typeof(Texture), false) as Texture;
            vignetteParams.OnInspectorGUI();
            if(mask == proceduralMask)
            {
                EditorGUILayout.LabelField("Procedural Mask");
                maskParams.OnInspectorGUI();
            }
        }
#endif
        [System.Serializable]
        public struct VignetteParams : IPostProcessParam<VignetteParams>
        {
            [SerializeField]
            private Color m_color;
            public Color color
            {
                get => m_color;
                set
                {
                    if(value != m_color)
                    {
                        m_color = value;
                        m_onValueChange?.Invoke(this);
                    }
                }
            }

            [SerializeField]
            private float m_opacity;
            public float opacity
            {
                get => m_opacity;
                set
                {
                    if(value != m_opacity)
                    {
                        m_opacity = value;
                        m_onValueChange?.Invoke(this);
                    }
                }
            }

            private event UnityAction<VignetteParams> m_onValueChange;
            public event UnityAction<VignetteParams> onValueChange
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

            public static VignetteParams Default
            {
                get => new VignetteParams()
                {
                    m_opacity = 1.0f,
                    m_color = Color.black
                };
            }

#if UNITY_EDITOR
            public void OnInspectorGUI()
            {
                color = EditorGUILayout.ColorField("Color", color);
                opacity = EditorGUILayout.Slider("Opacity", opacity, 0f, 1f);
            }
#endif
        }

        [System.Serializable]
        public struct VignetteMaskParams : IPostProcessParam<VignetteMaskParams>
        {
            [SerializeField]
            private Vector2 m_center;

            [SerializeField]
            private Vector4 m_params;

            [SerializeField]
            private bool m_rounded;

            public Vector2 center
            {
                get => m_center;
                set
                {
                    if(m_center != value)
                    {
                        m_center = value;
                        m_onValueChange?.Invoke(this);
                    }
                }
            }

            public float intensity
            {
                get => m_params.x / 3f;
                set
                {
                    if(value != intensity)
                    {
                        m_params.x = value * 3f;
                        m_onValueChange?.Invoke(this);
                    }
                }
            }
            public float smoothness
            {
                get => m_params.y / 5f;
                set
                {
                    if(value != smoothness)
                    {
                        m_params.y = value * 5f;
                        m_onValueChange?.Invoke(this);
                    }
                }
            }
            public float roundness
            {
                get => m_params.z;
                set
                {
                    if(value != m_params.z)
                    {
                        m_params.z = value;
                        Resolution res = Screen.currentResolution;
                        m_params.w = rounded ? ((float)res.height / res.width) : 1f;
                        m_onValueChange?.Invoke(this);
                    }
                }
            }

            public bool rounded
            {
                get => m_rounded;
                set
                {
                    if(value != m_rounded)
                    {
                        m_rounded = value;
                        Resolution res = Screen.currentResolution;
                        m_params.w = rounded ? ((float)res.height / res.width) : 1f;
                        m_onValueChange?.Invoke(this);
                    }
                }
            }

            private event UnityAction<VignetteMaskParams> m_onValueChange;
            public event UnityAction<VignetteMaskParams> onValueChange
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

            public static VignetteMaskParams Default
            {
                get => new VignetteMaskParams()
                {
                    m_rounded = false,
                    m_params = new Vector4(0.3f, 5f, 1f, 1f),
                    m_center = new Vector2(0.5f, 0.5f)
                };
            }

            public static implicit operator Vector4(VignetteMaskParams param)
            {
                return param.m_params;
            }

#if UNITY_EDITOR
            public void OnInspectorGUI()
            {
                center = EditorGUILayout.Vector2Field("Center", center);
                intensity = EditorGUILayout.Slider("Intensity", intensity, 0f, 1f);
                smoothness = EditorGUILayout.Slider("Smoothness", smoothness, 0f, 1f);
                roundness = EditorGUILayout.Slider("Roundness", roundness, 0f, 1f);
                rounded = EditorGUILayout.Toggle("Rounded", rounded);
            }
#endif
        }
    }
}