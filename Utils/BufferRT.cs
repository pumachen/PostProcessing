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
    public class BufferRT
    {
        [SerializeField]
        protected ResolutionMode m_resolutionMode = ResolutionMode.Absolute;
        public ResolutionMode resolutionMode
        {
            get => m_resolutionMode;
            set
            {
                if (m_resolutionMode != value)
                {
                    m_resolutionMode = value;
                    UpdateRT();
                }
            }
        }

        [SerializeField]
        protected float m_scale = 1.0f;
        public float scale
        {
            get => m_scale;
            set
            {
                if (resolutionMode != ResolutionMode.Relative || m_scale != value)
                {
                    m_resolutionMode = ResolutionMode.Relative;
                    m_scale = value;
                    UpdateRT();
                }
            }
        }

        [SerializeField]
        protected Vector2Int m_resolution = new Vector2Int(512, 256);
        public Vector2Int resolution
        {
            get
            {
                if (resolutionMode == ResolutionMode.Absolute)
                {
                    return m_resolution;
                }
                else
                {
                    Resolution screenRes = Screen.currentResolution;
                    return new Vector2Int()
                    {
                        x = Mathf.Max(1, (int)(screenRes.width  * scale)),
                        y = Mathf.Max(1, (int)(screenRes.height * scale))
                    };
                }
            }
            set
            {
                if (resolutionMode != ResolutionMode.Absolute || m_resolution != value)
                {
                    m_resolutionMode = ResolutionMode.Absolute;
                    m_resolution = value;
                    UpdateRT();
                }
            }
        }

        [SerializeField]
        protected RenderTextureFormat m_format = RenderTextureFormat.Default;
        public RenderTextureFormat format
        {
            get => m_format;
            set
            {
                if(m_format != value)
                {
                    m_format = value;
                    UpdateRT();
                }
            }
        }

        [SerializeField]
        protected bool m_useMipMap = false;
        public bool useMipMap
        {
            get => m_useMipMap;
            set
            {
                if(m_useMipMap != value)
                {
                    m_useMipMap = value;
                    RT.useMipMap = value;
                }
            }
        }

        private RenderTexture m_RT;
        public RenderTexture RT
        {
            get
            { 
                if(m_RT == null)
                {
                    UpdateRT();
                }
                return m_RT;
            }
        }

        public event UnityAction onValueChange;

        public void UpdateRT()
        {
            var oldRT = m_RT;
            Vector2Int res = resolution;
            int mipCount = 0;
            if (useMipMap)
            {
                mipCount = (int)Mathf.Log(Mathf.Max(res.x, res.y), 2);
            }
            m_RT = new RenderTexture(res.x, res.y, 0, format, mipCount);
            m_RT.useMipMap = true;
            onValueChange?.Invoke();

            if (oldRT != null)
            {
                oldRT.Release();
                Object.DestroyImmediate(oldRT);
            }
        }

        public static implicit operator RenderTexture(BufferRT bufferRT)
        {
            return bufferRT.RT;
        }

#if UNITY_EDITOR
        public void OnGUI()
        {
            resolutionMode = (ResolutionMode)EditorGUILayout.EnumPopup("Resolution Mode", resolutionMode);
            if(resolutionMode == ResolutionMode.Absolute)
            {
                resolution = EditorGUILayout.Vector2IntField("Resolution", resolution);
            }
            else
            {
                scale = EditorGUILayout.Slider("Scale", scale, 0, 1);
            }
        }

        public void OnDebugGUI()
        {
            OnGUI();
            float width = EditorGUIUtility.currentViewWidth * 0.9f;
            float aspect = (float)RT.height / RT.width;
            float height = width * aspect;
            GUILayoutOption layoutWidth = GUILayout.Width(width);
            GUILayoutOption layoutHeight = GUILayout.Height(height);
            Color srcColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.black;
            if (GUILayout.Button(RT, layoutWidth, layoutHeight))
            {
                Selection.objects = new Object[] { RT };
            }
            GUI.backgroundColor = srcColor;
        }
#endif //UNITY_EDITOR
    }

    public enum ResolutionMode
    {
        Relative,
        Absolute
    }
}