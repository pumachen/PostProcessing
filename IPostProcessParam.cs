using UnityEngine;
using UnityEngine.Events;

namespace Fuxi.Rendering.PostProcessing
{
    public interface IPostProcessParam<T> where T : struct, IPostProcessParam<T>
    {
        event UnityAction<T> onValueChange;

#if UNITY_EDITOR
        void OnInspectorGUI();
#endif // UNITY_EDITOR
    }

}
