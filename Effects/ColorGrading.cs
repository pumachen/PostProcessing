using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Omega.Rendering.PostProcessing
{
    [System.Serializable]
    public class ColorGrading : PostProcessEffect
    {
        protected override Shader shader
        {
            get => Shader.Find("Hidden/PostProcess/ColorGrading");
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            destMat.EnableKeyword("COLORGRADING_ENABLED");
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            destMat.DisableKeyword("COLORGRADING_ENABLED");
        }

        public override void Process(RenderTexture src)
        {

        }

        public override void Init(Material dest)
        {
            base.Init(dest);
        }

#if UNITY_EDITOR
        public override string name { get => "Bloom"; }

        protected override void OnInspectorGUI()
        {
        }

        protected override void OnDebugGUI()
        {
        }
#endif //UNITY_EDITOR
    }
}