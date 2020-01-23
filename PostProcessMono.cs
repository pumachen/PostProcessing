using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

#if UNITY_EDITOR
using UnityEditor;
#endif //UNITY_EDITOR

namespace Omega.Rendering.PostProcessing
{
    [RequireComponent(typeof(Camera))]
    [ExecuteInEditMode]
    public class PostProcessMono : MonoBehaviour
    {
        public const float MIN_SCALE = 0.01f;
        public const float MAX_SCALE = 1f;
        protected float m_renderScale = MAX_SCALE;
        public float renderScale
        {
            get => m_renderScale;
            set
            {
                value = Mathf.Clamp(value, MIN_SCALE, MAX_SCALE);
                if (m_renderScale != value)
                {
                    m_renderScale = value;

                    camera.targetTexture = null;
                    if (m_bufferRT != null)
                    {
                        Object.Destroy(m_bufferRT);
                    }
                    int width = (int)(1920 * renderScale);
                    int height = (int)(1080 * renderScale);
                    m_bufferRT = new RenderTexture(width, height, 24, RenderTextureFormat.Default);
                    m_bufferRT.name = "ScaleDownRT";
                    camera.targetTexture = m_bufferRT;
                }
            }
        }
        protected Camera m_camera = null;
        public new Camera camera
        {
            get
            {
                if (m_camera == null)
                {
                    m_camera = GetComponent<Camera>();
                }
                return m_camera;
            }
        }

        private RenderTexture m_bufferRT = null;

        protected RenderTexture scaledownRT
        {
            get
            {
                if (m_bufferRT == null)
                {
                    int width = (int)(Screen.width * renderScale);
                    int height = (int)(Screen.height * renderScale);
                    m_bufferRT = new RenderTexture(width, height, 24, RenderTextureFormat.Default);
                    m_bufferRT.name = "ScaleDownRT";
                    camera.targetTexture = m_bufferRT;
                }
                return m_bufferRT;
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
        }

        IEnumerable<PostProcessPass> passes
        {
            get
            {
                yield return uber;
            }
        }

        protected void Start()
        {
            uber.Init();
            camera.forceIntoRenderTexture = true;
            camera.SetTargetBuffers(scaledownRT.colorBuffer, scaledownRT.depthBuffer);
        }

        protected void OnEnable()
        {
            enabled = uber.enabled;
        }

        void OnPostRender()
        {
            camera.targetTexture = null;
            if(uber.enabled)
            {
                uber.Process(scaledownRT);
                Graphics.Blit(scaledownRT, null as RenderTexture, uber.material);
            }
            else
            {
                Graphics.Blit(scaledownRT, null as RenderTexture);
            }
            camera.SetTargetBuffers(scaledownRT.colorBuffer, scaledownRT.depthBuffer);
        }

#if UNITY_EDITOR
        [CustomEditor(typeof(PostProcessMono))]
        class PostProcessMonoEditor : Editor
        {
            new PostProcessMono target;

            private void OnEnable()
            {
                target = base.target as PostProcessMono;
            }

            public override void OnInspectorGUI()
            {
                PostProcessEffect.debugMode = EditorGUILayout.ToggleLeft("Debug", PostProcessEffect.debugMode);
                EditorGUILayout.ObjectField("ScaleDownRT", target.scaledownRT, typeof(RenderTexture), true, GUILayout.Height(20f));
                Undo.RecordObject(target, "Post Process Settings Modification");
                target.renderScale = EditorGUILayout.Slider("Render Scale", target.renderScale, MIN_SCALE, MAX_SCALE);
                foreach (var pass in target.passes)
                {
                    pass.InspectorGUI();
                }
                serializedObject.ApplyModifiedProperties();
            }
        }
#endif //UNITY_EDITORd
    }
}