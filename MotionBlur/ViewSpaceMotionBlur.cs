using UnityEngine;

namespace Omega.Rendering.PostProcessing
{
    public class ViewSpaceMotionBlur : PostProcessMono
    {
        public override Shader shader
        {
            get { return Shader.Find("Hidden/MotionBlur"); }
        }
        public Matrix4x4 VP
        {
            get
            {
                var V = camera.worldToCameraMatrix;
                var P = camera.projectionMatrix;
                return P * V;
            }
        }
        public Matrix4x4 prevVP { get; protected set; }

        protected override void Awake()
        {
            base.Awake();
            depthTextureMode = DepthTextureMode.Depth;
            prevVP = VP;
        }

        public override void Process(RenderTexture src, RenderTexture dest)
        {
            var VP = this.VP;
            material.SetMatrix("_CurrentToPrevProjPos", prevVP * VP.inverse);
            base.OnRenderImage(src, dest);
            prevVP = VP;
        }
    }
}

