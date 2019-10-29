using UnityEngine;

namespace Omega.Rendering.PostProcessing
{
    [ExecuteInEditMode]
    public class ObjSpaceMotionBlur : PostProcessMono
    {
        [SerializeField]
        private Transform target;
        [SerializeField]
        [Range(0f, 0.1f)]
        private float blurFactor = 0.05f;
        public override Shader shader
        {
            get { return Shader.Find("Hidden/MotionBlur"); }
        }
        public Matrix4x4 MVP
        {
            get
            {
                var P = camera.projectionMatrix;
                var V = camera.worldToCameraMatrix;
                var M = target.localToWorldMatrix;
                return P * V * M;
            }
        }
        public Matrix4x4 prevMVP { get; protected set; }

        public override void Process(RenderTexture src, RenderTexture dest)
        {
            var MVP = this.MVP;
            material.SetMatrix("_CurrentToPrevProjPos", prevMVP * MVP.inverse);
            material.SetFloat("_BlurFactor", blurFactor);
            Graphics.Blit(src, dest, material);
            prevMVP = MVP;
        }

        protected override void Awake()
        {
            base.Awake();
            depthTextureMode = DepthTextureMode.Depth;
            if (target == null)
            {
                enabled = false;
                return;
            }
            prevMVP = MVP;
        }
    }
}

