using UnityEngine;

namespace Omega.Rendering.PostProcessing
{
    interface IPostProcess
    {
        Material material { get; }
        Shader shader { get; }

        void Init();
        void Process(RenderTexture src, RenderTexture dest);
    }
}