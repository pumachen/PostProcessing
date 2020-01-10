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
                        wrapMode = TextureWrapMode.Clamp
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
                if(m_spectralLut != value)
                {
                    m_spectralLut = value; 
                }
            }
        }

        protected float m_intensity = 0f;
        public float intensity
        {
            get => m_intensity;
            set
            {
                if(m_intensity != value)
                {
                    m_intensity = Mathf.Clamp01(value);
                }
            }
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

