﻿using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif //UNITY_EDITOR

namespace Fuxi.Rendering.PostProcessing
{
    [System.Serializable]
    public class MotionBlur : PostProcessEffect
    {
        [SerializeField]
        private Mode m_mode = Mode.PositionReconstruction;
        public Mode mode
        {
            get => m_mode;
            set { m_mode = value; }
        }

        [SerializeField]
        private MotionSpace m_space = MotionSpace.World;
        public MotionSpace space
        {
            get => m_space;
            set { m_space = value; }
        }

        [SerializeField]
        private float m_blurFactor;
        public float blurFactor
        {
            get => m_blurFactor;
            set
            {
                if (value != m_blurFactor)
                {
                    m_blurFactor = value;
                    destMat.SetFloat("_MotionBlurFactor", m_blurFactor);
                }
            }
        }

        [SerializeField]
        public Camera camera;

        [SerializeField]
        protected Transform targetTransform;

        Matrix4x4 MVP
        {
            get
            {
                Matrix4x4 M = targetTransform ?
                    targetTransform.localToWorldMatrix : 
                    Matrix4x4.identity;
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
            get => space == MotionSpace.Local ? MVP : VP;
        }
        Matrix4x4 prevMatrix;

        protected override void OnEnable()
        {
            if (camera == null)
            {
                enabled = false;
                return;
            }
            if (mode  == Mode.PositionReconstruction
             && space == MotionSpace.Local
             && targetTransform == null)
            {
                enabled = false;
                return;
            }
            camera.depthTextureMode = DepthTextureMode.Depth;
            base.OnEnable();
        }

        protected override void SetProperties()
        {
            var matrix = this.matrix;
            destMat.SetMatrix("_CurrentToPrevProjPos", prevMatrix * matrix.inverse);
            prevMatrix = matrix;
        }

        public enum Mode
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
        public override string name { get => "Motion Blur"; }

        protected override void OnInspectorGUI()
        {
            blurFactor = EditorGUILayout.Slider("Blur Factor", blurFactor, 0.0f, 0.1f);
            mode = (Mode)EditorGUILayout.EnumPopup("Method", mode);
            switch(mode)
            {
                case Mode.PositionReconstruction: {
                        space = (MotionSpace)EditorGUILayout.EnumPopup("Space", space);
                        camera = EditorGUILayout.ObjectField("Camera", camera, typeof(Camera), true) as Camera;
                        if (space == MotionSpace.Local)
                        {
                            targetTransform = EditorGUILayout.ObjectField(targetTransform, typeof(Transform), true) as Transform;
                        }
                        break;
                    }
            }
        }

#endif //UNITY_EDITOR
    }
}