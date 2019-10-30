using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif //UNITY_EDITOR

namespace Omega.Rendering.PostProcessing
{
    public class PostProcessStack : PostProcessMono
    {
        public PostProcessEffect[] effects;
        public ObjSpaceMotionBlur motionBlur;
        public override Shader shader { get { return Shader.Find("Hidden/MotionBlur"); } }

        protected override void Awake()
        {
            base.Awake();
            motionBlur.Init(material);
            if (effects == null)
                effects = new PostProcessEffect[] { motionBlur };
        }

        public override void Process(RenderTexture src, RenderTexture dest)
        {
            motionBlur.BeforeProcess();
            Graphics.Blit(src, dest, material);
            motionBlur.AfterProcess();
        }
    }

#if UNITY_EDITOR
    /*[CustomEditor(typeof(PostProcessStack))]
    public class PostProcessStackEditor : Editor
    {
        new PostProcessStack target;
        
        private void OnEnable()
        {
            target = base.target as PostProcessStack;
        }

        public override void OnInspectorGUI()
        {
            foreach (var effect in target.effects)
            {
                effect.InspectorGUI();
            }
        }
    }*/
#endif //UNITY_EDITOR
}