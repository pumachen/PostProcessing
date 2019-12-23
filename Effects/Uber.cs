using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Omega.Rendering.PostProcessing
{
    [System.Serializable]
    public class Uber : PostProcessEffect
    {
        protected override Shader shader
        {
            get => Shader.Find("Hidden/PostProcess/Uber");
        }

        public override void Init()
        {
            
        }

        public override void Process(RenderTexture src, RenderTexture dest)
        {
            Graphics.Blit(src, dest);
        }

#if UNITY_EDITOR
        public override string name { get => "Uber"; }

        protected override void OnInspectorGUI()
        {
            
        }
#endif //UNITY_EDITOR
    }
}