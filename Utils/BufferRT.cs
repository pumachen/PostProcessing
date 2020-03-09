using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif //UNITY_EDITOR

namespace Fuxi.Rendering.PostProcessing
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
        protected Camera m_camera;
        public Camera camera
        {
            get => m_camera;
            set
            {
                if(m_camera != value)
                {
                    m_camera = value;
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
                    Resolution res;
                    if (camera != null)
                    {
                        res = new Resolution()
                        {
                            width = camera.pixelWidth,
                            height = camera.pixelHeight
                        };
                    }
                    else
                    {
                        res = Screen.currentResolution;
                    }
                    return new Vector2Int()
                    {
                        x = Mathf.Max(1, (int)(res.width  * scale)),
                        y = Mathf.Max(1, (int)(res.height * scale))
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
                    UpdateRT();
                }
            }
        }

        [SerializeField]
        protected string m_name = "";

        public string name
        {
            get => m_name;
            set
            {
                if(m_name != value)
                {
                    m_name = value;
                    if (m_RT != null)
                    {
                        m_RT.name = m_name;
                    }
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
            RenderTextureDescriptor descriptor = new RenderTextureDescriptor(res.x, res.y, format, 0)
            {
                useMipMap = useMipMap,
            };
            m_RT = new RenderTexture(descriptor) { name = name };
            m_RT.useMipMap = m_useMipMap;
            onValueChange?.Invoke();

            if (oldRT != null)
            {
                oldRT.Release();
                Object.DestroyImmediate(oldRT);
            }
        }

        public BufferRT(
            Resolution resolution,
            RenderTextureFormat format = RenderTextureFormat.Default, 
            bool useMipMap = false)
        {
            m_resolution = new Vector2Int(resolution.width, resolution.height);
            m_resolutionMode = ResolutionMode.Absolute;
            m_format = format;
            m_useMipMap = useMipMap;
        }

        public BufferRT(
            float scale, 
            RenderTextureFormat format = RenderTextureFormat.Default, 
            bool useMipMap = false)
        {
            m_resolutionMode = ResolutionMode.Relative;
            m_scale = scale;
            m_format = format;
            m_useMipMap = useMipMap;
        }

        public static implicit operator RenderTexture(BufferRT bufferRT)
        {
            return bufferRT.RT;
        }

#if UNITY_EDITOR
        public void OnGUI()
        {
            EditorGUILayout.LabelField(name);
            using (new GUILayout.HorizontalScope())
            {
                EditorGUILayout.Space();
                using (new GUILayout.VerticalScope())
                {
                    resolutionMode = (ResolutionMode)EditorGUILayout.EnumPopup("Resolution Mode", resolutionMode);
                    if (resolutionMode == ResolutionMode.Absolute)
                    {
                        resolution = EditorGUILayout.Vector2IntField("Resolution", resolution);
                    }
                    else
                    {
                        camera = EditorGUILayout.ObjectField("Camera", camera, typeof(Camera), true) as Camera;
                        scale = EditorGUILayout.Slider("Scale", scale, 0, 1);
                    }
                }
            }
        }

        public void OnDebugGUI()
        {
            OnGUI();
            
            format = (RenderTextureFormat)EditorGUILayout.EnumPopup("Format", format);
            useMipMap = EditorGUILayout.Toggle("MipMap", useMipMap);

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