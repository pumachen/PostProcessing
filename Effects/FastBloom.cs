using UnityEngine;
using UnityEngine.Events;
using Props = Omega.Rendering.PostProcessing.PostProcessProperties;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Omega.Rendering.PostProcessing
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

        [SerializeField]
        protected BloomParams m_bloomParams;
        public BloomParams bloomParams = new BloomParams();

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

            bloomParams.bloomParamsChanged += (param) => destMat.SetVector(Props.bloomParams, param);
            bloomParams.filterParamsChanged += (param) => material.SetVector(Props.filterParams, param);
        }

        protected override void SetProperties()
        {
            destMat.SetTexture(Props.bloomTex, bloomTex);
            destMat.SetVector(Props.bloomParams, bloomParams.bloomParams);

            material.SetVector(Props.filterParams, bloomParams.filterParams);
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
            BloomParamsGUI();
            bloomTex.OnGUI();
        }

        protected override void OnDebugGUI()
        {
            BloomParamsGUI();
            bloomTex.OnDebugGUI();
        }

        void BloomParamsGUI()
        {
            EditorGUILayout.LabelField("BloomParams");
            using (new GUILayout.HorizontalScope())
            {
                EditorGUILayout.Space();
                using (new GUILayout.VerticalScope())
                {
                    bloomParams.filterExp = EditorGUILayout.Slider("Filter Exp", bloomParams.filterExp, 1f, 10f);
                    bloomParams.clampMax = EditorGUILayout.FloatField("Clamp Max", bloomParams.clampMax);
                    bloomParams.threshold = EditorGUILayout.FloatField("Threshold", bloomParams.threshold);
                    bloomParams.knee = EditorGUILayout.FloatField("Knee", bloomParams.knee);

                    bloomParams.minMipLevel = EditorGUILayout.
                        IntSlider(
                            "Min Mipmap Level",
                            bloomParams.minMipLevel,
                            0,
                            bloomParams.maxMipLevel);
                    bloomParams.maxMipLevel = EditorGUILayout.
                        IntSlider(
                            "Max Mipmap Level",
                            bloomParams.maxMipLevel,
                            0,
                            8);
                    //bloomTex.RT.mipmapCount);
                    bloomParams.intensity = EditorGUILayout.
                        Slider("Intensity", bloomParams.intensity, 0, 1);
                }
            }
        }
#endif //UNITY_EDITOR
    }

    [System.Serializable]
    public class BloomParams
    {
        protected event UnityAction<Vector4> m_bloomParamsChanged;
        public event UnityAction<Vector4> bloomParamsChanged
        {
            add
            {
                if (value != null)
                {
                    m_bloomParamsChanged += value;
                    value.Invoke(bloomParams);
                }
            }
            remove => m_bloomParamsChanged -= value;
        }

        protected event UnityAction<Vector4> m_filterParamsChanged;
        public event UnityAction<Vector4> filterParamsChanged
        {
            add
            {
                if (value != null)
                {
                    m_filterParamsChanged += value;
                    value.Invoke(filterParams);
                }
            }
            remove => m_filterParamsChanged -= value;
        }

        #region FilterParams
        [SerializeField]
        private float m_filterExp = 1.0f;
        public float filterExp
        {
            get => m_filterExp;
            set
            {
                if (value != m_filterExp)
                {
                    m_filterExp = value;
                    UpdateFilterParams();
                }
            }
        }

        [SerializeField]
        private float m_clampMax = 1.0f;
        public float clampMax
        {
            get => m_clampMax;
            set
            {
                if(value != m_clampMax)
                {
                    m_clampMax = value;
                    UpdateFilterParams();
                }
            }
        }

        [SerializeField]
        private float m_threshold = 0.5f;
        public float threshold
        {
            get => m_threshold;
            set
            {
                if(value != m_threshold)
                {
                    m_threshold = value;
                    UpdateFilterParams();
                }
            }
        }

        [SerializeField]
        private float m_knee = 1.0f;
        public float knee
        {
            get => m_knee;
            set
            {
                if(value != m_knee)
                {
                    m_knee = value;
                    UpdateFilterParams();
                }
            }
        }

        private Vector4 m_filterParams;
        public Vector4 filterParams => m_filterParams;
        private void UpdateFilterParams()
        {
            m_filterParams = new Vector4()
            {
                x = filterExp,
                y = clampMax,
                z = threshold,
                w = knee
            };
            m_filterParamsChanged?.Invoke(filterParams);
        }
        #endregion //FilterParams

        #region BloomParams
        [SerializeField]
        private int m_minMipLevel = 1;
        public int minMipLevel
        {
            get => m_minMipLevel;
            set
            {
                if (value != m_minMipLevel)
                {
                    m_minMipLevel = value;
                    UpdateBloomParams();
                }
            }
        }

        [SerializeField]
        private int m_maxMipLevel = 3;
        public int maxMipLevel
        {
            get => m_maxMipLevel;
            set
            {
                if (value != m_maxMipLevel)
                {
                    m_maxMipLevel = value;
                    UpdateBloomParams();
                }
            }
        }

        [SerializeField]
        private float m_intensity = 0.3f;
        public float intensity
        {
            get => m_intensity;
            set
            {
                if (value != m_intensity)
                {
                    m_intensity = value;
                    UpdateBloomParams();
                }
            }
        }


        private Vector4 m_bloomParams;
        public Vector4 bloomParams => m_bloomParams;

        private void UpdateBloomParams()
        {
            m_bloomParams = new Vector4()
            {
                x = minMipLevel,
                y = maxMipLevel,
                z = 1f / (maxMipLevel - minMipLevel + 1),
                w = intensity
            };
            m_bloomParamsChanged?.Invoke(bloomParams);
        }
        #endregion //BloomParams

        public BloomParams()
        {
            UpdateFilterParams();
            UpdateBloomParams();
        }
    }
}