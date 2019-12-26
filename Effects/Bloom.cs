using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Omega.Rendering.PostProcessing
{
    [System.Serializable]
    public class Bloom : PostProcessEffect
    {
        protected override Shader shader
        {
            get => Shader.Find("Hidden/PostProcess/Bloom");
        }

        public BufferRT bloomTex = new BufferRT();

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

        public override void Process(RenderTexture src)
        {
            Graphics.Blit(src, bloomTex, material);
        }

        public override void Init(Material dest)
        {
            base.Init(dest);

            dest.SetTexture("_BloomTex", bloomTex);
            bloomTex.onValueChange += () => dest.SetTexture("_BloomTex", bloomTex);

            bloomParams.bloomParamsChanged += (param) => dest.SetVector("_BloomParams", param);
            bloomParams.filterParamsChanged += (param) => material.SetVector("_FilterParams", param);
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
                EditorGUILayout.Space(5f);
                using (new GUILayout.VerticalScope())
                {
                    bloomParams.filterExp = EditorGUILayout.Slider("Filter Exp", bloomParams.filterExp, 1f, 10f);
                    bloomParams.minMipLevel = EditorGUILayout.
                        IntSlider(
                            "Min Mipmap Level",
                            bloomParams.minMipLevel,
                            1,
                            bloomParams.maxMipLevel);
                    bloomParams.maxMipLevel = EditorGUILayout.
                        IntSlider(
                            "Max Mipmap Level",
                            bloomParams.maxMipLevel,
                            1,
                            bloomTex.RT.mipmapCount);
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

        #region FilterParams
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

        private Vector4 m_filterParams;
        public Vector4 filterParams => m_filterParams;
        private void UpdateFilterParams()
        {
            m_filterParams = new Vector4()
            {
                x = filterExp,
                y = 0,
                z = 0,
                w = 0
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

        private float m_intensity = 0.5f;
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