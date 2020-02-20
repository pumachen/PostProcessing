using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Props = Omega.Rendering.PostProcessing.PostProcessProperties;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Omega.Rendering.PostProcessing
{
    [System.Serializable]
    public class FXAA : PostProcessEffect
    {
        public FXAAParams fxaaParams = FXAAParams.Default;

        protected override void OnEnable()
        {
            base.OnEnable();
            destMat.EnableKeyword("FXAA_ENABLED");
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            destMat.DisableKeyword("FXAA_DISABLED");
        }

        protected override void Init()
        {
            base.Init();
            fxaaParams.onValueChange += (param) => destMat.SetVector(Props.fxaaParams, fxaaParams);
        }
        protected override void SetProperties()
        {
            destMat.SetVector(Props.fxaaParams, fxaaParams);
        }

#if UNITY_EDITOR
        public override string name => "FXAA";

        protected override void OnInspectorGUI()
        {
            fxaaParams.OnInspectorGUI();
        }
#endif // UNITY_EDITOR

        [System.Serializable]
        public struct FXAAParams : IPostProcessParam<FXAAParams>
        {
            [SerializeField]
            private Vector4 m_params;

            public float contrastThreshold
            {
                get => m_params.x;
                set
                {
                    if (value != m_params.x)
                    {
                        m_params.x = value;
                        m_onValueChange?.Invoke(this);
                    }
                }
            }
            public float relativeThreshold
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
            public float subpixelBlending
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

            private event UnityAction<FXAAParams> m_onValueChange;
            public event UnityAction<FXAAParams> onValueChange
            {
                add
                {
                    if(value != null)
                    {
                        m_onValueChange += value;
                        value.Invoke(this);
                    }
                }
                remove => m_onValueChange -= value;
            }

            public static FXAAParams Default
            {
                get => new FXAAParams() { m_params = new Vector4(0.0312f, 0.063f, 1f, 0) };
            }

            public static implicit operator Vector4(FXAAParams param)
            {
                return param.m_params;
            }

#if UNITY_EDITOR
            public void OnInspectorGUI()
            {
                contrastThreshold = EditorGUILayout.Slider("Contrast Threshold", contrastThreshold, 0.0312f, 0.0833f);
                relativeThreshold = EditorGUILayout.Slider("Relative Threshold", relativeThreshold, 0.063f, 0.333f);
                subpixelBlending = EditorGUILayout.Slider("Subpixel Blending", subpixelBlending, 0f, 1f);
            }
#endif
        }
    }
}