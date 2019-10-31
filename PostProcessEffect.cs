using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif //UNITY_EDITOR

namespace Omega.Rendering.PostProcessing
{
    public abstract class PostProcessEffect
    {
        public abstract string name { get; }
        private bool m_enabled;
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
        protected Material material { get; private set; }

        private UnityAction m_init;
        private UnityAction m_beforeProcess;
        private UnityAction m_afterProcess;

        public UnityAction init
        {
            get { return enabled ? m_init : null; }
            protected set { m_init = value; }
        }
        public UnityAction beforeProcess
        {
            get { return enabled ? m_beforeProcess : null; }
            protected set { m_beforeProcess = value; }
        }
        public UnityAction afterProcess
        {
            get { return enabled ? m_afterProcess : null; }
            protected set { m_afterProcess = value; }
        }

        protected PostProcessEffect(Material material)
        {
            this.material = material;
        }

        protected virtual void OnEnable() {}
        protected virtual void OnDisable() {}
        public virtual void Init(Material material)
        {
            this.material = material;
        }
        
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