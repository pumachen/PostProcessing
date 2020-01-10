using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Omega.Rendering.PostProcessing
{
    [System.Serializable]
    public class Uber : PostProcessPass
    {
        [SerializeField]
        protected MotionBlur m_motionBlur = new MotionBlur();
        public MotionBlur motionBlur => m_motionBlur;

        [SerializeField]
        protected FastBloom m_bloom = new FastBloom();
        public FastBloom bloom => m_bloom;

        [SerializeField]
        protected ColorGrading m_colorGrading = new ColorGrading();
        public ColorGrading colorGrading => m_colorGrading;
        protected override IEnumerable<PostProcessEffect> effects
        {
            get
            {
                yield return motionBlur;
                yield return bloom;
                yield return colorGrading;
            }
        }

        protected override Shader shader
        {
            get => Shader.Find("Hidden/PostProcess/Uber");
        }

#if UNITY_EDITOR
        public override string name { get => "Uber"; }

        protected override void OnInspectorGUI()
        {
            
        }

#endif //UNITY_EDITOR
    }
}