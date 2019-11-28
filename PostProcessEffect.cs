using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif //UNITY_EDITOR

namespace Omega.Rendering.PostProcessing
{
    [System.Serializable]
    public abstract class PostProcessEffect
    {
        public abstract string name { get; }
        [SerializeField]
        private bool m_enabled = false;
        public bool enabled
        {
            get { return m_enabled; }
            set
            {
                if (value == m_enabled)
                    return;
                m_enabled = value;
                if (value == true)
                    OnEnable();
                else
                    OnDisable();
            }
        }
        protected abstract Shader shader { get; }
        protected Material m_material;
        protected virtual Material material
        {
            get
            {
                if (m_material == null)
                    m_material = new Material(shader);
                return m_material;
            }
        }

        protected abstract void OnEnable();
        protected virtual void OnDisable() {}

        public abstract void Process(RenderTexture src, RenderTexture dest);
        
#if UNITY_EDITOR

        private bool unfold = false;

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
            }
            GUILayout.EndVertical();
        }
        protected abstract void OnInspectorGUI();

#endif //UNITY_EDITOR
    }

    public enum PostProcessEffectMask : byte
    {
        ObjSpaceMotionBlur = 1 << 1,
        ViewSpaceMotionBlur = 1 << 2
    }
}