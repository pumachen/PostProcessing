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
        [SerializeField]
        [HideInInspector]
        MotionBlur motionBlur;

        [SerializeField]
        [HideInInspector]
        Bloom bloom;

        IEnumerable<PostProcessEffect> effects
        {
            get
            {
                yield return motionBlur;
                yield return bloom;
            }
        }

        RenderTexture m_tmpBuffer;

        RenderTexture tmpBuffer
        {
            get
            {
                if (m_tmpBuffer == null)
                    m_tmpBuffer = new RenderTexture(2048, 1024, 0, RenderTextureFormat.ARGB32);
                return m_tmpBuffer;
            }
        }

        List<PostProcessEffect> enabledEffects = new List<PostProcessEffect>(2);

        protected void Start()
        {
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

        protected virtual void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            if (enabledEffects.Count == 0)
            {
                Graphics.Blit(src, dest);
                return;
            }
            int passIdx = 0;
            RenderTexture GetSrcRT()
            {
                if (passIdx % 2 == 0)
                    return src;
                return tmpBuffer;
            }
            RenderTexture GetDestRT()
            {
                if (passIdx == enabledEffects.Count - 1)
                    return dest;
                else
                    return passIdx % 2 == 1 ? src : tmpBuffer;
            }
            for(passIdx = 0; passIdx < enabledEffects.Count; ++passIdx)
            {
                enabledEffects[passIdx].Process(GetSrcRT(), GetDestRT());
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