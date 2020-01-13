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
                if(m_LUT != value)
                {
                    m_LUT = value;
                    destMat.SetTexture("_LUT", value);
                }
            }
        }

        [SerializeField]
        protected float m_brightness = 1.0f;
        public float brightness
        {
            get => m_brightness;
            set
            {
                if(value != m_brightness)
                {
                    m_brightness = Mathf.Clamp(value, 0f, 2f);
                    destMat.SetFloat("_Brightness", m_brightness);
                }
            }
        }

        protected override void OnEnable()
        {
            if(LUT == null)
            {
                enabled = false;
                return;
            }
            destMat.EnableKeyword("COLORGRADING_ENABLED");
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            destMat.DisableKeyword("COLORGRADING_ENABLED");
            base.OnDisable();
        }

        public override void Init(Material destMat)
        {
            base.Init(destMat);
            destMat.SetTexture("_LUT", LUT);
            destMat.SetFloat("_Brightness", m_brightness);
        }

#if UNITY_EDITOR
        public override string name { get => "Color Grading"; }

        protected override void OnInspectorGUI()
        {
            LUT = EditorGUILayout.ObjectField("LUT", LUT, typeof(Texture2D), false) as Texture2D;
            brightness = EditorGUILayout.Slider("Brightness", brightness, 0f, 2f);
        }
#endif //UNITY_EDITOR
    }
}