using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Fuxi.RP;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif //UNITY_EDITOR

namespace Omega.Rendering.PostProcessing
{
    [RequireComponent(typeof(Camera))]
    [ExecuteInEditMode]
    public class PostProcessMono : MonoBehaviour
    {
        [SerializeField]
        protected Uber m_uber;
        public Uber uber
        { 
            get
            {
                if(m_uber == null)
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

        protected void Awake()
        {
            uber.Init();
            RenderPipelineManager.endCameraRendering += OnEndRendering;
        }

        void OnEndRendering(ScriptableRenderContext context, Camera camera)
        {
            if(camera == Camera.main)
            {
                OnRenderImage(
                    DeferredRP<PBRGBuffer>.instance.GetFrameBuffer(camera), 
                    camera.targetTexture);
            }
        }

        public virtual void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            uber.Process(src, dest);
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
                foreach (var pass in target.passes)
                {
                    pass.InspectorGUI();
                }
            }
        }
#endif //UNITY_EDITORd
    }
}