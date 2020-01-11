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
        }

        protected void OnEnable()
        {
            uber.Init();
        }

        public virtual void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            if (uber.enabled)
            {
                uber.Process(src, dest);
            }
            else
            {
                Graphics.Blit(src, dest);
            }
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