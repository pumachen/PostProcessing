﻿using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif //UNITY_EDITOR

namespace Omega.Rendering.PostProcessing
{
    [System.Serializable]
    public abstract class PostProcessEffect : IPostProcess
    {
        [SerializeField]
        private bool m_enabled = false;
        public bool enabled
        {
            get { return m_enabled; }
            set
            {
                if (value != m_enabled)
                {
                    m_enabled = value;
                    if (value == true)
                        OnEnable();
                    else
                        OnDisable();
                }
            }
        }

        private Material m_material;
        protected Material material
        {
            get
            {
                if (m_material == null)
                {
                    m_material = new Material(shader);
                }
                return m_material;
            }
        }

        private Material m_destmat;
        protected Material destMat => m_destmat;

        protected virtual Shader shader => null;

        protected virtual void OnEnable()  {}
        protected virtual void OnDisable() {}

        public virtual void Init(Material destMat)
        {
            m_destmat = destMat;
        }

        public abstract void Process(RenderTexture src);

#if UNITY_EDITOR

        public abstract string name { get; }
        private bool unfold = true;
        public static bool debugMode = false;

        public void InspectorGUI()
        {
            EditorGUILayout.BeginVertical();

            EditorGUILayout.BeginHorizontal();

            enabled = EditorGUILayout.ToggleLeft(" ", enabled, GUILayout.Width(20f));
            EditorGUILayout.LabelField(" ", GUILayout.Width(5f));
            unfold = EditorGUILayout.Foldout(unfold, name);

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(15f);
            using (new GUILayout.VerticalScope())
            {
                if (unfold)
                {
                    if (debugMode)
                    {
                        OnDebugGUI();
                    }
                    else
                    {
                        OnInspectorGUI();
                    }
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }
        protected abstract void OnInspectorGUI();
        protected virtual void OnDebugGUI() { OnInspectorGUI(); }

#endif //UNITY_EDITOR
    }

    public enum PostProcessEffectMask : byte
    {
        ObjSpaceMotionBlur = 1 << 1,
        ViewSpaceMotionBlur = 1 << 2
    }
}