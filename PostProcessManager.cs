using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Omega.Rendering.PostProcessing
{
    [ExecuteInEditMode]
    public class PostProcessManager : MonoBehaviour
    {
        public const float MIN_SCALE = 0.1f;
        public const float MAX_SCALE = 1f;
        protected static float m_renderScale = MAX_SCALE;
        public static float renderScale
        {
            get => m_renderScale;
            set
            {
                value = Mathf.Clamp(value, MIN_SCALE, MAX_SCALE);
                if (m_renderScale != value)
                {
                    m_renderScale = value;

                    if (m_scaledownRT != null)
                    {
                        Object.DestroyImmediate(m_scaledownRT);
                    }
                    int width = (int)(Screen.width * renderScale);
                    int height = (int)(Screen.height * renderScale);
                    m_scaledownRT = new RenderTexture(width, height, 24, RenderTextureFormat.Default);
                    m_scaledownRT.name = "ScaleDownRT";
                }
            }
        }

        private static RenderTexture m_scaledownRT = null;

        public static RenderTexture scaledownRT
        {
            get
            {
                if (m_scaledownRT == null)
                {
                    int width = (int)(Screen.width * renderScale);
                    int height = (int)(Screen.height * renderScale);
                    m_scaledownRT = new RenderTexture(width, height, 24, RenderTextureFormat.Default);
                    m_scaledownRT.name = "ScaleDownRT";
                }
                return m_scaledownRT;
            }
        }

        protected static PostProcessManager m_Instance;
        public static PostProcessManager Instance
        {
            get
            {
                if(m_Instance == null)
                {
                    var go = GameObject.Find("PostProcessManager");
                    if (!go)
                    {
                        go = new GameObject("PostProcessManager", typeof(PostProcessManager));
                    }
                    m_Instance = go.GetComponent<PostProcessManager>();
                }
                return m_Instance;
            }
        }

        [SerializeField]
        protected Uber m_uber;
        public Uber uber
        {
            get
            {
                if (m_uber == null)
                {
                    m_uber = new Uber();
                }
                return m_uber;
            }
            protected set
            {
                if(m_uber != value)
                {
                    m_uber = value;
                    value.Init();
                }
            }
        }

        private void Awake()
        {
            m_Instance = this;
            uber.Init();
        }

        public static void Process(RenderTexture src)
        {
            Instance?.uber.Process(src);
        }

        public static Material material => Instance?.uber.material;

#if UNITY_EDITOR
        IEnumerable<PostProcessPass> passes
        {
            get
            {
                yield return uber;
            }
        }

        [CustomEditor(typeof(PostProcessManager))]
        class PostProcessMonoEditor : Editor
        {
            new PostProcessManager target;

            private void OnEnable()
            {
                target = base.target as PostProcessManager;
            }

            public override void OnInspectorGUI()
            {
                target.uber = (Uber)EditorGUILayout.ObjectField("Profile", target.uber, typeof(Uber), false);
                renderScale = EditorGUILayout.Slider("Render Scale", renderScale, MIN_SCALE, MAX_SCALE);
                PostProcessEffect.debugMode = EditorGUILayout.ToggleLeft("Debug", PostProcessEffect.debugMode);
                Undo.RecordObject(target, "Post Process Settings Modification");
                foreach (var pass in target.passes)
                {
                    pass.InspectorGUI();
                }
                serializedObject.ApplyModifiedProperties();
            }
        }
#endif //UNITY_EDITOR

    }
}

