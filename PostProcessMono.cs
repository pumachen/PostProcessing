using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

#if UNITY_EDITOR
using UnityEditor;
#endif //UNITY_EDITOR

namespace Fuxi.Rendering.PostProcessing
{
    [RequireComponent(typeof(Camera))]
    [ExecuteInEditMode]
    public class PostProcessMono : MonoBehaviour
    {
        protected Camera m_camera;
        public new Camera camera
        {
            get
            {
                if(m_camera == null)
                {
                    m_camera = GetComponent<Camera>();
                }
                return m_camera;
            }
        }

        public float renderScale => PostProcessManager.renderScale;

        public RenderTexture scaledownRT => PostProcessManager.scaledownRT;

        protected void Start()
        {
            if(!camera)
            {
                enabled = false;
                return;
            }
            camera.forceIntoRenderTexture = true;
            camera.SetTargetBuffers(scaledownRT.colorBuffer, scaledownRT.depthBuffer);
        }


        protected void OnDisable()
        {
            camera.targetTexture = null;
        }

        void OnPostRender()
        {
            camera.targetTexture = null;
            PostProcessManager.Process(scaledownRT);
            if(PostProcessManager.material)
            {
                Graphics.Blit(scaledownRT, null as RenderTexture, PostProcessManager.material);
            }
            else
            {
                Graphics.Blit(scaledownRT, null as RenderTexture);
            }
            camera.SetTargetBuffers(scaledownRT.colorBuffer, scaledownRT.depthBuffer);
        }
    }
}