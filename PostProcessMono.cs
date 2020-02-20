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
        protected new Camera camera = null;

        public float renderScale => PostProcessManager.renderScale;

        public RenderTexture scaledownRT => PostProcessManager.scaledownRT;

        protected void Start()
        {
            camera = GetComponent<Camera>();
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
            Graphics.Blit(scaledownRT, null as RenderTexture, PostProcessManager.material);
            camera.SetTargetBuffers(scaledownRT.colorBuffer, scaledownRT.depthBuffer);
        }
    }
}