using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif //UNITY_EDITOR

namespace Omega.Rendering.PostProcessing
{
    [System.Serializable]
    public class ObjSpaceMotionBlur : ViewSpaceMotionBlur
    {
        public override string name { get { return "Obj Space Motion Blur"; } }

        [SerializeField]
        protected Transform target;

        public Matrix4x4 MVP
        {
            get
            {
                Matrix4x4 M = target.localToWorldMatrix;
                return VP * M;
            }
        }

        public ObjSpaceMotionBlur(Material material) : base(material) {}

        public override void BeforeProcess()
        {
            var MVP = this.MVP;
            material.SetMatrix("_CurrentToPrevProjPos", prevMatrix * MVP.inverse);
            material.SetFloat("_MotionBlurFactor", blurFactor);
            prevMatrix = MVP;
        }

        public override void Init(Material material)
        {
            base.Init(material);          
            prevMatrix = MVP;
        }

#if UNITY_EDITOR
        protected override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            target = EditorGUILayout.ObjectField(target, typeof(Transform), true) as Transform;
        }
#endif //UNITY_EDITOR
    }
}