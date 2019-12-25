﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif //UNITY_EDITOR

namespace Omega.Rendering.PostProcessing
{
    [System.Serializable]
    public abstract class PostProcessPass
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
        protected abstract Shader shader { get; }
        protected Material m_material;
        protected virtual Material material
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

        private event UnityAction m_onEnable;
        private event UnityAction m_onDisable;
        public virtual event UnityAction onEnable
        {
            add { m_onEnable += value; }
            remove { m_onEnable -= value; }
        }
        public virtual event UnityAction onDisable
        {
            add { m_onDisable += value; }
            remove { m_onDisable -= value; }
        }

        protected virtual void OnEnable()
        {
            Init();
            m_onEnable?.Invoke();
        }
        protected virtual void OnDisable()
        {
            m_onDisable?.Invoke();
        }

        public abstract void Init();

        public abstract void Process(RenderTexture src, RenderTexture dest);

#if UNITY_EDITOR

        public abstract string name { get; }
        private bool unfold = false;
        public static bool debugMode = false;

        public void InspectorGUI()
        {
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();

            enabled = EditorGUILayout.ToggleLeft(" ", enabled, GUILayout.Width(20f));
            EditorGUILayout.LabelField(" ", GUILayout.Width(5f));
            unfold = EditorGUILayout.Foldout(unfold, name);
            GUILayout.EndHorizontal();

            if (unfold)
            {
                OnInspectorGUI();
                if (debugMode)
                {
                    OnDebugGUI();
                }
            }
            GUILayout.EndVertical();
        }
        protected abstract void OnInspectorGUI();
        protected virtual void OnDebugGUI() { }

#endif //UNITY_EDITOR
    }
}
