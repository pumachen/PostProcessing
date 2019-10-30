using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif //UNITY_EDITOR

namespace Omega.Rendering.PostProcessing
{
    [System.Serializable]
    public abstract class MotionBlur : PostProcessEffect
    {
        [SerializeField]
        private float m_blurFactor;
        public float blurFactor
        {
            get { return m_blurFactor; }
            set
            {
                m_blurFactor = value;
                material.SetFloat("_MotionBlurFactor", value);
            }
        }

        public MotionBlur(Material material) : base(material) {}

#if UNITY_EDITOR
        protected override void OnInspectorGUI()
        {
            blurFactor = EditorGUILayout.Slider("Blur Factor", blurFactor, 0.0f, 1.0f);
        }
#endif //UNITY_EDITOR
    }
}