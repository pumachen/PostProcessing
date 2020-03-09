using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Fuxi.Rendering.PostProcessing
{
    public interface IPostProcess
    {
        bool enabled { get; set; }

#if UNITY_EDITOR
        string name { get; }
        void InspectorGUI();
#endif
    }
}