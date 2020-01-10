using System.Collections;
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

        protected abstract IEnumerable<PostProcessEffect> effects { get; }
        protected List<PostProcessEffect> effectList;

        protected virtual void OnEnable()  {}
        protected virtual void OnDisable() {}

        public virtual void Init()
        {
            effectList = new List<PostProcessEffect>(10);
            foreach(var effect in effects)
            {
                effectList.Add(effect);
                effect.Init(material);
            }
        }

        public virtual void Process(RenderTexture src, RenderTexture dest)
        {
            foreach (PostProcessEffect effect in effectList)
            {
                if (effect.enabled)
                {
                    effect.Process(src);
                }
            }
            Graphics.Blit(src, dest, material);
        }

#if UNITY_EDITOR

        public abstract string name { get; }
        private bool unfold = true;
        public bool debugMode = false;

        public void InspectorGUI()
        {
            using (new GUILayout.HorizontalScope())
            {
                EditorGUILayout.ToggleLeft(" ", enabled, GUILayout.Width(20f));
                EditorGUILayout.LabelField(" ", GUILayout.Width(5f));
                unfold = EditorGUILayout.Foldout(unfold, name);
            }


            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Space(15f); // horizontal indent size of 20 (pixels)
                if (unfold)
                {
                    using (new GUILayout.VerticalScope())
                    {
                        OnInspectorGUI();
                        foreach (var effect in effectList)
                        {
                            effect.InspectorGUI();
                        }
                    }
                }
            }
        }
        protected abstract void OnInspectorGUI();
#endif //UNITY_EDITOR
    }
}
