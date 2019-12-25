using System.Collections.Generic;
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
        public static PostProcessMono instance;

        [HideInInspector]
        public MotionBlur motionBlur;

        [HideInInspector]
        public Bloom bloom;

        [HideInInspector]
        public Uber uber;

        IEnumerable<PostProcessEffect> effects
        {
            get
            {
                yield return motionBlur;
                yield return bloom;
            }
        }

        IEnumerable<PostProcessPass> passes
        {
            get
            {
                yield return uber;
            }
        }

        List<PostProcessEffect> enabledEffects = new List<PostProcessEffect>(2);

        protected void Start()
        {
            instance = this;
            foreach(var effect in effects)
            {
                effect.onEnable += UpdateEffectList;
                effect.onDisable += UpdateEffectList;
            }
            UpdateEffectList();
            foreach(var effect in enabledEffects)
            {
                effect.Init();
            }
        }

        protected void UpdateEffectList()
        {
            enabledEffects.Clear();
            foreach(var effect in effects)
            {
                if (effect.enabled)
                {
                    enabledEffects.Add(effect);
                }
            }
        }

        public virtual void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            foreach(var effect in enabledEffects)
            {
                effect.Process(src);
            }
            uber.Process(src, dest);
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
                EditorGUILayout.LabelField("Effects");
                foreach (var effect in target.effects)
                {
                    effect.InspectorGUI();
                }
                EditorGUILayout.LabelField("Passes");
                foreach (var pass in target.passes)
                {
                    pass.InspectorGUI();
                }
            }
        }
#endif //UNITY_EDITORd
    }
}