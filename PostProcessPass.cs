﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif //UNITY_EDITOR

namespace Omega.Rendering.PostProcessing
{
    /*public class PostProcess
    {
        public PostProcessEffect[] effects;
        public MotionBlur motionBlur;
        RenderTexture src;
        RenderTexture dest;

        protected void Awake()
        {
            motionBlur = new MotionBlur(material);
            motionBlur.Init(material);
            if (effects == null)
                effects = new PostProcessEffect[] { motionBlur };
        }

        public void Process()
        {
            
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
#endif //UNITY_EDITOR*/
}