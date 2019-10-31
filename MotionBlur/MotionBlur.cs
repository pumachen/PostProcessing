using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif //UNITY_EDITOR

namespace Omega.Rendering.PostProcessing
{
    [System.Serializable]
    public class MotionBlur : PostProcessEffect
    {
        public override string name { get { return "Motion Blur"; } }

        [SerializeField]
        private Method m_method;
        public Method method
        {
            get { return m_method; }
            set
            {
                m_method = value;
                switch (m_method)
                {
                    case Method.PositionReconstruction:
                        {
                            PositionReconstructionInit();
                            beforeProcess = PositionReconstructionBeforeUpdate;
                            break;
                        }
                }
            }
        }

        [SerializeField]
        private MotionSpace m_space;
        public MotionSpace space
        {
            get { return m_space; }
            set { m_space = value; }
        }

        [SerializeField]
        private float m_blurFactor;
        public float blurFactor
        {
            get { return m_blurFactor; }
            set
            {
                m_blurFactor = value;
                material.SetFloat("_MotionBlurFactor", value);
            }
        }

        [SerializeField]
        public Camera camera;

        [SerializeField]
        protected Transform target;

        Matrix4x4 MVP
        {
            get
            {
                Matrix4x4 M;
                if (target != null)
                    M = target.localToWorldMatrix;
                else
                    M = Matrix4x4.identity;
                return VP * M;
            }
        }
        Matrix4x4 VP
        {
            get
            {
                if (camera != null)
                {
                    Matrix4x4 P = camera.projectionMatrix;
                    Matrix4x4 V = camera.worldToCameraMatrix;
                    return P * V;
                }
                else return Matrix4x4.identity;
            }
        }
        Matrix4x4 matrix
        {
            get { return space == MotionSpace.Local ? MVP : VP; }
        }
        Matrix4x4 prevMatrix;
        
        public MotionBlur(Material material) : base(material) {}

        void PositionReconstructionInit()
        {
            camera.depthTextureMode = DepthTextureMode.Depth;
        }

        void PositionReconstructionBeforeUpdate()
        {
            var matrix = this.matrix;
            material.SetMatrix("_CurrentToPrevProjPos", prevMatrix * matrix.inverse);
            material.SetFloat("_MotionBlurFactor", blurFactor);
            prevMatrix = matrix;
        }        

        public enum Method
        {
            //FrameBlur, 
            PositionReconstruction, 
            //MotionVector,
        }

        public enum MotionSpace
        {
            Local,
            World
        }

#if UNITY_EDITOR
        protected override void OnInspectorGUI()
        {
            blurFactor = EditorGUILayout.Slider("Blur Factor", blurFactor, 0.0f, 1.0f);
            method = (Method)EditorGUILayout.EnumPopup("Method", method);
            if (method == Method.PositionReconstruction)
            {
                space = (MotionSpace)EditorGUILayout.EnumPopup("Space", space);
                camera = EditorGUILayout.ObjectField(camera, typeof(Camera), true) as Camera;
                if (space == MotionSpace.Local)
                {
                    target = EditorGUILayout.ObjectField(target, typeof(Transform), true) as Transform;
                }
            }
        }
#endif //UNITY_EDITOR
    }
}