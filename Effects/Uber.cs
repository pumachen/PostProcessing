using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Omega.Rendering.PostProcessing
{
    [System.Serializable]
    public class Uber : PostProcessPass
    {
        protected MotionBlur m_motionBlur;
        protected Bloom m_bloom;

        public MotionBlur motionBlur = new MotionBlur();
        public Bloom bloom = new Bloom();
        public override IEnumerable<PostProcessEffect> effects
        {
            get
            {
                yield return motionBlur;
                yield return bloom;
            }
        }

        protected override Shader shader
        {
            get => Shader.Find("Hidden/PostProcess/Uber");
        }

        public override void Init()
        {
            foreach (var effect in effects)
            {
                effect.Init(material);
            }
        }

        public override void Process(RenderTexture src, RenderTexture dest)
        {
            foreach (var effect in effects)
            {
                if(effect.enabled)
                {
                    effect.Process(src);
                }
            }
            Graphics.Blit(src, dest, material);
        }

#if UNITY_EDITOR
        public override string name { get => "Uber"; }

        protected override void OnInspectorGUI()
        {
            
        }

#endif //UNITY_EDITOR
    }
}