﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif //UNITY_EDITOR

namespace Fuxi.Rendering.PostProcessing
{
    [CreateAssetMenu(menuName = "Create/PostProcessProfile")]
    [System.Serializable]
    public class Uber : PostProcessPass
    {
        [SerializeField]
        protected FXAA m_fxaa = new FXAA();
        public FXAA fxaa => m_fxaa;

        [SerializeField]
        protected FastBloom m_bloom = new FastBloom();
        public FastBloom bloom => m_bloom;

        [SerializeField]
        protected ChromaticAberration m_chromaticAberration = new ChromaticAberration();
        public ChromaticAberration chromaticAberration => m_chromaticAberration;

        [SerializeField]
        protected ColorGrading m_colorGrading = new ColorGrading();
        public ColorGrading colorGrading => m_colorGrading;

        [SerializeField]
        protected Vignette m_vignette = new Vignette();
        public Vignette vignette => m_vignette;

        protected override IEnumerable<PostProcessEffect> effects
        {
            get
            {
                yield return fxaa;
                yield return bloom;
                yield return chromaticAberration;
                yield return colorGrading;
                yield return vignette;
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
            if(PostProcessEffect.debugMode)
            {
                EditorGUILayout.ObjectField(material, typeof(Material), true);
            }
        }

#endif //UNITY_EDITOR

        /*[CustomEditor(typeof(Uber))]
        public class UberEditor : Editor
        {
            new Uber target;

            private void OnEnable()
            {
                target = base.target as Uber;
            }

            public override void OnInspectorGUI()
            {
                Undo.RecordObject(target, "Post Process Settings Modification");
                target.InspectorGUI();
                serializedObject.ApplyModifiedProperties();
            }
        }*/
    }
}