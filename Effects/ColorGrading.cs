using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Omega.Rendering.PostProcessing
{
    [System.Serializable]
    public class ColorLookUp : PostProcessEffect
    {
        protected override Shader shader
        {
            get => Shader.Find("Hidden/PostProcess/ColorLookUp");
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            destMat.EnableKeyword("COLORLOOKUP_ENABLED");
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            destMat.DisableKeyword("COLORLOOKUP_ENABLED");
        }

        public override void Process(RenderTexture src)
        {

        }

        public override void Init(Material dest)
        {
            base.Init(dest);
        }

#if UNITY_EDITOR
        public override string name { get => "Color LookUp"; }

        protected override void OnInspectorGUI()
        {

        }

        protected override void OnDebugGUI()
        {
        }
#endif //UNITY_EDITOR
    }
}