using UnityEngine;

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
        [HideInInspector]
        MotionBlur motionBlur;

        [SerializeField]
        [HideInInspector]
        Bloom bloom;

        [SerializeField]
        PostProcessEffect[] effects;

        protected void Start()
        {
            effects = new PostProcessEffect[]
                {
                    motionBlur,
                    bloom
                };
        }

        protected virtual void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            foreach (var effect in effects)
            {
                if (effect.enabled)
                {
                    effect.Process(src, dest);
                }
            }
        }

#if UNITY_EDITOR
        [CustomEditor(typeof(PostProcessMono))]
        public class PostProcessStackEditor : Editor
        {
            new PostProcessMono target;

            private void OnEnable()
            {
                target = base.target as PostProcessMono;
            }

            public override void OnInspectorGUI()
            {
                foreach (var effect in target.effects)
                {
                    effect.InspectorGUI();
                }
            }
        }
#endif //UNITY_EDITORd
    }
}