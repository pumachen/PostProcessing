using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif //UNITY_EDITOR

namespace Omega.Rendering.PostProcessing
{
    [System.Serializable]
    public class ViewSpaceMotionBlur : MotionBlur
    {
        public override string name { get { return "View Space Motion Blur"; } }

        [SerializeField]
        public Camera camera;

        public Matrix4x4 prevMatrix { get; protected set; }

        public Matrix4x4 VP
        {
            get
            {
                Matrix4x4 P = camera.projectionMatrix;
                Matrix4x4 V = camera.worldToCameraMatrix;
                return P * V;
            }
        }

        public ViewSpaceMotionBlur(Material material) : base(material) {}

        public override void BeforeProcess()
        {
            var VP = this.VP;
            material.SetMatrix("_CurrentToPrevProjPos", prevMatrix * VP.inverse);
            material.SetFloat("_MotionBlurFactor", blurFactor);
            prevMatrix = VP;
        }

        public override void Init(Material material)
        {
            base.Init(material);
            camera.depthTextureMode = DepthTextureMode.Depth;
        }

#if UNITY_EDITOR
        protected override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            camera = EditorGUILayout.ObjectField(camera, typeof(Camera), true) as Camera;
        }
#endif //UNITY_EDITOR
    }
}