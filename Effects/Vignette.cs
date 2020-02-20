using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Props = Omega.Rendering.PostProcessing.PostProcessProperties;
#if UNITY_EDITOR
using UnityEditor;
#endif //UNITY_EDITOR

namespace Omega.Rendering.PostProcessing
{
    [System.Serializable]
    public class Vignette : PostProcessEffect
    {
        protected override Shader shader
        {
            get => Shader.Find("Hidden/PostProcess/VignetteMaskBaker");
        }

        protected RenderTexture m_proceduralMask;
        protected RenderTexture proceduralMask
        {
            get
            {
                if (m_proceduralMask == null)
                {
                    UpdateProceduralMask();
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
        protected Color m_color = Color.black;
        public Color color
        {
            get => m_color;
            set
            {
                if(color != value)
                {
                    m_color = value;
                    destMat.SetColor(Props.vignetteColor, m_color);
                }
            }
        }

        [SerializeField]
        protected float m_opacity = 1.0f;
        public float opacity
        {
            get => m_opacity;
            set
            {
                if(m_opacity != value)
                {
                    m_opacity = Mathf.Clamp01(value);
                    destMat.SetFloat(Props.vignetteOpacity, m_opacity);
                }
            }
        }

        [SerializeField]
        protected Vector2 m_center = new Vector2(0.5f, 0.5f);
        public Vector2 center
        {
            get => m_center;
            set
            {
                if(m_center != value)
                {
                    m_center = value;
                    UpdateProceduralMask();
                }
            }
        }

        [SerializeField]
        protected float m_intensity = 0.1f;
        public float intensity
        {
            get => m_intensity;
            set
            {
                if(m_intensity != value)
                {
                    m_intensity = value;
                    UpdateProceduralMask();
                }
            }
        }

        [SerializeField]
        protected float m_smoothness = 1f;
        public float smoothness
        {
            get => m_smoothness;
            set
            {
                if(m_smoothness != value)
                {
                    m_smoothness = value;
                    UpdateProceduralMask();
                }
            }
        }

        [SerializeField]
        protected float m_roundness = 1f;
        public float roundness
        {
            get => m_roundness;
            set
            {
                if(m_roundness != value)
                {
                    m_roundness = value;
                    UpdateProceduralMask();
                }
            }
        }

        [SerializeField]
        protected bool m_rounded = false;
        public bool rounded
        {
            get => m_rounded;
            set
            {
                if(m_rounded != value)
                {
                    m_rounded = value;
                    UpdateProceduralMask();
                }
            }
        }

        protected override void Init()
        {
            base.Init();
            UpdateProceduralMask();
        }

        protected override void SetProperties()
        {
            destMat.SetTexture(Props.vignetteMask, mask);
            destMat.SetColor(Props.vignetteColor, color);
            destMat.SetFloat(Props.vignetteOpacity, opacity);
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
            Resolution res = Screen.currentResolution;
            if (m_proceduralMask == null)
            {
                m_proceduralMask = new RenderTexture(
                    res.width / 2,
                    res.height / 2,
                    0,
                    RenderTextureFormat.R8)
                { name = "Procedural Mask" };
            }
            material.SetVector(Props.vignetteCenter, center);
            Vector4 param = new Vector4(
                intensity * 3f,
                smoothness * 5f,
                roundness,
                rounded ? ((float)res.height / res.width) : 1f);
            material.SetVector(Props.vignetteParams, param);
            Graphics.Blit(null, m_proceduralMask, material);
            destMat?.SetTexture(Props.vignetteMask, mask);
        }

#if UNITY_EDITOR
        public override string name => "Vignette";
        protected override void OnInspectorGUI()
        {
            mask = EditorGUILayout.ObjectField("Vignette Mask", mask, typeof(Texture), false) as Texture;
            color = EditorGUILayout.ColorField("Color", color);
            opacity = EditorGUILayout.Slider("Opacity", opacity, 0f, 1f);
            if(mask == proceduralMask)
            {
                EditorGUILayout.LabelField("Procedural Mask");
                center = EditorGUILayout.Vector2Field("Center", center);
                intensity = EditorGUILayout.Slider("Intensity", intensity, 0f, 1f);
                smoothness = EditorGUILayout.Slider("Smoothness", smoothness, 0f, 1f);
                roundness = EditorGUILayout.Slider("Roundness", roundness, 0f, 1f);
                rounded = EditorGUILayout.Toggle("Rounded", rounded);
            }
            
        }
#endif
    }
}