using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fuxi.Rendering.PostProcessing
{
    [System.Serializable]
    public class RampMap
    {
        public AnimationCurve curve { get; protected set; }
        protected Texture2D m_texture;
        public Texture2D texture
        {
            get
            {
                if(m_texture == null)
                {
                    m_texture = new Texture2D(size, 1, TextureFormat.R8, false)
                    {
                        wrapMode = TextureWrapMode.Clamp
                    };
                    Update();
                }
                return m_texture;
            }
        }
        [SerializeField]
        protected int m_size = 128;
        public int size
        {
            get => m_size;
            set
            {
                if(m_size != value)
                {
                    m_size = value;
                    Object.Destroy(texture);
                    m_texture = new Texture2D(m_size, 1, TextureFormat.R8, false);
                    Update();
                }
            }
        }
        public RampMap(AnimationCurve curve = null, int size = 128)
        {
            this.curve = curve == null ? AnimationCurve.Constant(0f, 1f, 1f) : curve;
        }

        public void Update()
        {
            for (int u = 0; u < texture.width; ++u)
            {
                float val = curve.Evaluate((u + 0.5f) / texture.width);
                texture.SetPixel(u, 0, new Color(val, val, val));
            }
            texture.Apply();
        }

#if UNITY_EDITOR
        public void OnInspectorGUI(string label)
        {
            curve = UnityEditor.EditorGUILayout.CurveField(label, curve);
            Update();
        }
#endif //UNITY_EDITOR

        public static implicit operator RampMap(AnimationCurve curve)
        {
            return new RampMap(curve);
        }

        public static implicit operator Texture2D(RampMap rampMap)
        {
            if (rampMap != null && rampMap.texture != null)
                return rampMap.texture;
            else return null;
        }
    }
}