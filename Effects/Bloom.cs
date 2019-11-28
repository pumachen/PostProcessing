using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Omega.Rendering.PostProcessing
{
    [System.Serializable]
    public class Bloom : PostProcessEffect
    {
        public override string name { get { return "Bloom"; } }
        protected override Shader shader
        {
            get { return Shader.Find("Hidden/PostProcess/Bloom"); }
        }

        protected RenderTexture luminanceRT
        {
            get
            {
                if(m_luminanceRT == null)
                {
                    m_luminanceRT = new RenderTexture(1024, 512, 0)
                    {
                        useMipMap = true,
                        filterMode = FilterMode.Bilinear
                    };
                }
                return m_luminanceRT;
            }
        }
        private RenderTexture m_luminanceRT;

        public override void Process(RenderTexture src, RenderTexture dest)
        {
            Graphics.Blit(src, luminanceRT, material, 0);
            Graphics.Blit(src, dest, material, 1);
        }

        protected override void OnEnable()
        {
            material.SetTexture("_Luminance", luminanceRT);
        }

#if UNITY_EDITOR
        protected override void OnInspectorGUI()
        {
            EditorGUILayout.ObjectField(luminanceRT, typeof(RenderTexture), true);
        }
#endif //UNITY_EDITOR
    }
}