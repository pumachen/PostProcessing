using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Omega.Rendering.PostProcessing
{
    [System.Serializable]
    public class Bloom : PostProcessPass
    {
        protected override Shader shader
        {
            get => Shader.Find("Hidden/PostProcess/Bloom");
        }

        public RenderTexture bloomRT
        {
            get
            {
                if(m_bloomRT == null)
                {
                    CreateBloomTex();
                }
                return m_bloomRT;
            }
            protected set
            {
                if (m_bloomRT != null)
                {
                    m_bloomRT.Release();
                }
                m_bloomRT = value;
            }
        }
        private RenderTexture m_bloomRT;

        public Vector2Int bloomTexSize
        {
            get => m_bloomTexSize;
            set
            {
                if (value == m_bloomTexSize)
                    return;
                if(Mathf.IsPowerOfTwo(value.x) && Mathf.IsPowerOfTwo(value.y))
                {
                    m_bloomTexSize = value;
                    CreateBloomTex();
                }
            }
        }
        private Vector2Int m_bloomTexSize = new Vector2Int(512, 256);

        public Vector4 filterParams
        {
            get => m_filterParams;
            set
            {
                if (value == m_filterParams)
                    return;
                m_filterParams = value;
                material.SetVector("_FilterParams", m_filterParams);
            }
        }
        private Vector4 m_filterParams;

        public BloomParams bloomParams;

        public override void Process(RenderTexture src, RenderTexture dest)
        {
            Graphics.Blit(src, bloomRT, material, 0);
            Graphics.Blit(src, dest, material, 1);
        }

        public override void Init()
        {
            Shader.SetGlobalTexture("_BloomTex", bloomRT);
            bloomParams.onValueChange += (param) =>
            {
                Shader.SetGlobalVector("_BloomParams", param);
            };
            material.SetVector("_FilterParams", filterParams);
        }

        protected void CreateBloomTex()
        {
            bloomRT = new RenderTexture(bloomTexSize.x, bloomTexSize.y, 0)
            {
                useMipMap = true,
                filterMode = FilterMode.Bilinear
            };
        }

        [System.Serializable]
        public class BloomParams
        {
            protected event UnityAction<Vector4> m_onValueChange;
            public event UnityAction<Vector4> onValueChange
            {
                add
                {
                    m_onValueChange += value;
                    value?.Invoke(m_params);
                }
                remove => m_onValueChange -= value;
            }

            [SerializeField]
            protected uint m_maxMipLevel = 1;
            public uint maxMipLevel
            {
                get => m_maxMipLevel;
                set
                {
                    if (value == m_maxMipLevel)
                        return;
                    m_maxMipLevel = value;
                    UpdateParams();
                }
            }

            [SerializeField]
            protected Vector4 m_params;

            protected void UpdateParams()
            {
                m_params = new Vector4()
                {
                    x = maxMipLevel,
                    y = 1f / (1 + maxMipLevel),
                    z = 0,
                    w = 0
                };
                m_onValueChange?.Invoke(m_params);
            }

            public static implicit operator Vector4(BloomParams bloomParams)
            {
                return bloomParams.m_params;
            }
        }

#if UNITY_EDITOR
        public override string name { get => "Bloom"; }

        protected override void OnInspectorGUI()
        {
            EditorGUILayout.ObjectField(bloomRT, typeof(RenderTexture), true);
            bloomTexSize = EditorGUILayout.Vector2IntField("", bloomTexSize);
            float filterExp = EditorGUILayout.Slider("Filter Exp", filterParams.x, 1f, 10f);
            filterParams = new Vector4(filterExp, 0, 0, 0);
            bloomParams.maxMipLevel = 
                (uint)(EditorGUILayout.IntSlider(
                    "Max Mipmap Level", 
                    (int)bloomParams.maxMipLevel, 
                    0, 
                    bloomRT.mipmapCount));
        }
#endif //UNITY_EDITOR
    }
}