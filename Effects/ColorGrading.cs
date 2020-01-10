using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Omega.Rendering.PostProcessing
{
    [System.Serializable]
    public class ColorGrading : PostProcessEffect
    {
        [SerializeField]
        protected Texture2D m_LUT;
        public Texture2D LUT
        {
            get => m_LUT;
            set
            {
                m_LUT = value;
                destMat.SetTexture("_LUT", value);
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            destMat.EnableKeyword("COLORGRADING_ENABLED");
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            destMat.DisableKeyword("COLORGRADING_ENABLED");
        }

        public override void Init(Material dest)
        {
            base.Init(dest);
        }

#if UNITY_EDITOR
        public override string name { get => "Color Grading"; }

        protected override void OnInspectorGUI()
        {
            LUT = EditorGUILayout.ObjectField("LUT", LUT, typeof(Texture2D), false) as Texture2D;
        }
#endif //UNITY_EDITOR
    }
}