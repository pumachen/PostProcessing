using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif //UNITY_EDITOR

namespace Omega.Rendering.PostProcessing
{
    public class PostProcessPass : PostProcessMono
    {
        public PostProcessEffect[] effects;
        public MotionBlur motionBlur;
        public override Shader shader { get { return Shader.Find("Hidden/Uber"); } }

        protected override void Awake()
        {
            base.Awake();
            motionBlur.Init(material);
            if (effects == null)
                effects = new PostProcessEffect[] { motionBlur };
        }

        public override void Process(RenderTexture src, RenderTexture dest)
        {
            motionBlur.beforeProcess?.Invoke();
            Graphics.Blit(src, dest, material, 0);
            motionBlur.beforeProcess?.Invoke();
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(PostProcessPass))]
    public class PostProcessStackEditor : Editor
    {
        new PostProcessPass target;
        
        private void OnEnable()
        {
            target = base.target as PostProcessPass;
        }

        public override void OnInspectorGUI()
        {
            target.motionBlur.InspectorGUI();
        }
    }
#endif //UNITY_EDITOR
}