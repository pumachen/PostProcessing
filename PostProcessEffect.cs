using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif //UNITY_EDITOR

namespace Omega.Rendering.PostProcessing
{
    public abstract class PostProcessEffect
    {
        public abstract string name { get; }
        protected bool m_enabled;
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
        public virtual void BeforeProcess() {}
        public virtual void AfterProcess() {}

#if UNITY_EDITOR

        private bool unfold = false;

        public void InspectorGUI()
        {
            GUILayout.BeginVertical();

            enabled = EditorGUILayout.ToggleLeft(name, enabled);
            unfold = EditorGUILayout.Foldout(unfold, "");

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