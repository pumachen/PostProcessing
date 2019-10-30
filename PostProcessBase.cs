using UnityEngine;

namespace Omega.Rendering.PostProcessing
{
    public abstract class PostProcessBase : Object, IPostProcess
    {
        public virtual Material material { get; protected set; }
        public abstract Shader shader { get; }

        public virtual void Init()
        {
            material = new Material(shader);
        }
        public abstract void Process(RenderTexture src, RenderTexture dest);
    }
}