using UnityEngine;
using Props = Omega.Rendering.PostProcessing.PostProcessProperties;
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
                    destMat.SetTexture(Props.LUT, value);
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
                    destMat.SetFloat(Props.brightness, m_brightness);
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
            base.OnEnable();
            destMat.EnableKeyword("COLORGRADING_ENABLED");
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            destMat.DisableKeyword("COLORGRADING_ENABLED");
        }

        protected override void SetProperties()
        {
            destMat.SetTexture(Props.LUT, LUT);
            destMat.SetFloat(Props.brightness, m_brightness);
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