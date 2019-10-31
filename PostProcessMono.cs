using UnityEngine;

namespace Omega.Rendering.PostProcessing
{
    [RequireComponent(typeof(Camera))]
    [ExecuteInEditMode]
    public abstract class PostProcessMono : MonoBehaviour, IPostProcess
    {
        public Material material { get; protected set; }
        protected Camera m_camera;
        public virtual new Camera camera
        {
            get
            {
                if (m_camera == null)
                    m_camera = GetComponent<Camera>();
                return m_camera;
            }
            protected set { m_camera = value; }
        }
        public virtual DepthTextureMode depthTextureMode
        {
            get { return camera.depthTextureMode; }
            protected set { camera.depthTextureMode = value; }
        }
        public abstract Shader shader { get; }

        protected virtual void Awake()
        {
            Init();
        }

        protected virtual void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            Process(src, dest);
        }

        public virtual void Init()
        {
            material = new Material(shader);
        }
        public abstract void Process(RenderTexture src, RenderTexture dest);
        
    }
}