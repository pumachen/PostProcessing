using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif //UNITY_EDITOR

namespace Omega.Rendering.PostProcessing
{
    [System.Serializable]
    public class ChromaticAberration : PostProcessEffect
    {
        protected static Texture2D m_defaultSpectralLut;
        public static Texture2D defaultSpectralLut
        {
            get
            {
                if(m_defaultSpectralLut == null)
                {
                    m_defaultSpectralLut = new Texture2D(3, 1, TextureFormat.RGB24, false)
                    {
                        filterMode = FilterMode.Bilinear,
                        wrapMode = TextureWrapMode.Clamp,
                        name = "Default SpectralLut"
                    };
                    m_defaultSpectralLut.SetPixel(0, 0, Color.red);
                    m_defaultSpectralLut.SetPixel(1, 0, Color.green);
                    m_defaultSpectralLut.SetPixel(2, 0, Color.blue);
                    m_defaultSpectralLut.Apply();
                }
                return m_defaultSpectralLut;
            }
        }

        [SerializeField]
        protected Texture2D m_spectralLut;
        public Texture2D spectralLut
        {
            get
            {
                if(m_spectralLut == null)
                {
                    return defaultSpectralLut;
                }
                else
                {
                    return m_spectralLut;
                }
            }
            set
            {
                if(m_spectralLut != value && defaultSpectralLut != value)
                {
                    m_spectralLut = value;
                    destMat.SetTexture("_ChromaticAberration_SpectralLut", m_spectralLut);
                }
            }
        }

        [SerializeField]
        protected float m_intensity = 0f;
        public float intensity
        {
            get => m_intensity;
            set
            {
                if(m_intensity != value)
                {
                    m_intensity = Mathf.Clamp01(value);
                    destMat.SetFloat("_ChromaticAberration_Amount", m_intensity * 0.05f);
                }
            }
        }

        protected override void OnEnable()
        {
            if(spectralLut == null)
            {
                enabled = false;
                return;
            }
            base.OnEnable();
            destMat.EnableKeyword("CHROMATIC_ABERRATION_ENABLED");
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            destMat.DisableKeyword("CHROMATIC_ABERRATION_ENABLED");
        }

        public override void Init(Material destMat)
        {
            base.Init(destMat);
            destMat.SetTexture("_ChromaticAberration_SpectralLut", spectralLut);
            destMat.SetFloat("_ChromaticAberration_Amount", intensity * 0.05f);
        }
#if UNITY_EDITOR
        public override string name { get => "Chromatic Aberration"; }

        protected override void OnInspectorGUI()
        {
            spectralLut = EditorGUILayout.ObjectField("Spectral LUT", spectralLut, typeof(Texture2D), true) as Texture2D;
            intensity = EditorGUILayout.Slider("Intensity", intensity, 0f, 1f);
        }

#endif //UNITY_EDITOR
    }
}

