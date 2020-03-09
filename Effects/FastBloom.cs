using UnityEngine;
using UnityEngine.Events;
using Props = Fuxi.Rendering.PostProcessing.PostProcessProperties;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Fuxi.Rendering.PostProcessing
{
    [System.Serializable]
    public class FastBloom : PostProcessEffect
    {
        protected override Shader shader
        {
            get => Shader.Find("Hidden/PostProcess/FastBloom");
        }

        [SerializeField]
        protected BufferRT m_bloomTex = new BufferRT(0.5f, RenderTextureFormat.ARGB32, true) { name = "Bloom Tex" };
        public BufferRT bloomTex
        {
            get
            {
                if(m_bloomTex == null)
                {
                    m_bloomTex = new BufferRT(0.5f, RenderTextureFormat.ARGB32, true) { name = "Bloom Tex" };
                }
                return m_bloomTex;
            }
        }

        public BloomParams bloomParams = BloomParams.Default;

        protected override void OnEnable()
        {
            base.OnEnable();
            destMat.EnableKeyword("BLOOM_ENABLED");
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            destMat.DisableKeyword("BLOOM_ENABLED");
        }
        protected override void Init()
        {
            bloomTex.onValueChange += () => destMat.SetTexture(Props.bloomTex, bloomTex);

            bloomParams.onValueChange += (param) =>
            {
                material.SetVector(Props.filterParams, param.filterParams);
                destMat.SetVector(Props.bloomParams, param.bloomParams);
            };
        }

        protected override void SetProperties()
        {
            material.SetVector(Props.filterParams, bloomParams.filterParams);

            destMat.SetTexture(Props.bloomTex, bloomTex);
            destMat.SetVector(Props.bloomParams, bloomParams.bloomParams);
        }

        public override void Process(RenderTexture src)
        {
            base.Process(src);
            bloomTex.RT.autoGenerateMips = false;
            Graphics.Blit(src, bloomTex, material);
            bloomTex.RT.GenerateMips();
        }

        

#if UNITY_EDITOR
        public override string name { get => "Bloom"; }

        protected override void OnInspectorGUI()
        {
            bloomParams.OnInspectorGUI();
            bloomTex.OnGUI();
        }

        protected override void OnDebugGUI()
        {
            bloomParams.OnInspectorGUI();
            bloomTex.OnDebugGUI();
        }
#endif //UNITY_EDITOR
    }

    [System.Serializable]
    public struct BloomParams : IPostProcessParam<BloomParams>
    {
        #region FilterParams

        [SerializeField]
        private Vector4 m_filterParams;
        public Vector4 filterParams => m_filterParams;

        public float filterExp
        {
            get => m_filterParams.x;
            set
            {
                if (value != m_filterParams.x)
                {
                    m_filterParams.x = value;
                    m_onValueChange?.Invoke(this);
                }
            }
        }

        public float clampMax
        {
            get => m_filterParams.y;
            set
            {
                if(value != m_filterParams.y)
                {
                    m_filterParams.y = value;
                    m_onValueChange?.Invoke(this);
                }
            }
        }

        public float threshold
        {
            get => m_filterParams.z;
            set
            {
                if(value != m_filterParams.z)
                {
                    m_filterParams.z = value;
                    m_onValueChange?.Invoke(this);
                }
            }
        }

        public float knee
        {
            get => m_filterParams.w;
            set
            {
                if(value != m_filterParams.w)
                {
                    m_filterParams.w = value;
                    m_onValueChange?.Invoke(this);
                }
            }
        }
        #endregion //FilterParams

        #region BloomParams

        [SerializeField]
        private Vector4 m_bloomParams;
        public Vector4 bloomParams => m_bloomParams;

        public int minMipLevel
        {
            get => (int)m_bloomParams.x;
            set
            {
                if (value != minMipLevel)
                {
                    m_bloomParams.x = value;
                    UpdateBloomParams();
                }
            }
        }

        public int maxMipLevel
        {
            get => (int)m_bloomParams.y;
            set
            {
                if (value != maxMipLevel)
                {
                    m_bloomParams.y = value;
                    UpdateBloomParams();
                }
            }
        }

        public float intensity
        {
            get => m_bloomParams.w;
            set
            {
                if (value != m_bloomParams.w)
                {
                    m_bloomParams.w = value;
                    UpdateBloomParams();
                }
            }
        }

        private void UpdateBloomParams()
        {
            m_bloomParams.z = 1f / (maxMipLevel - minMipLevel + 1);
            m_onValueChange?.Invoke(this);
        }
        #endregion //BloomParams

        private event UnityAction<BloomParams> m_onValueChange;
        public event UnityAction<BloomParams> onValueChange
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

        public static BloomParams Default
        {
            get => new BloomParams()
            {
                m_filterParams = new Vector4(1.0f, 1.0f, 0.5f, 1.0f),
                m_bloomParams = new Vector4( 1f, 3f, 0.3f)
            };
        }

#if UNITY_EDITOR
        public void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("BloomParams");
            using (new GUILayout.HorizontalScope())
            {
                EditorGUILayout.Space();
                using (new GUILayout.VerticalScope())
                {
                    filterExp = EditorGUILayout.Slider("Filter Exp", filterExp, 1f, 10f);
                    clampMax = EditorGUILayout.FloatField("Clamp Max", clampMax);
                    threshold = EditorGUILayout.FloatField("Threshold", threshold);
                    knee = EditorGUILayout.FloatField("Knee", knee);

                    minMipLevel = EditorGUILayout.IntSlider("Min Mipmap Level", minMipLevel, 0, maxMipLevel);
                    maxMipLevel = EditorGUILayout.IntSlider( "Max Mipmap Level", maxMipLevel, 0, 8);
                    intensity = EditorGUILayout.Slider("Intensity", intensity, 0, 1);
                }
            }
        }
#endif //UNITY_EDITOR
    }
}