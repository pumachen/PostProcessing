using UnityEngine;

namespace Omega.Rendering.PostProcessing
{
    public class ObjectSpaceMotionBlur : PostProcessEffect
    {
        public override string name { get { return "Motion Blur"; } }
        [SerializeField]
        [Range(0f, 0.1f)]
        private float blurFactor = 0.05f;

        [SerializeField]
        private Camera camera;
        [SerializeField]
        private Transform target;

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

        public override void BeforeProcess(Material material)
        {
            var MVP = this.MVP;
            material.SetMatrix("_CurrentToPrevProjPos", prevMVP * MVP.inverse);
            material.SetFloat("_BlurFactor", blurFactor);
            prevMVP = MVP;
        }

        public override void Init(Material material)
        {
            
        }
    }
}