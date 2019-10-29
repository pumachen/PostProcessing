using UnityEngine;

namespace Omega.Rendering.PostProcessing
{
    public abstract class PostProcessEffect
    {
        public abstract string name { get; }
        protected bool m_enabled;
        public bool enabled
        {
            get { return m_enabled; }
            set
            {
                if (value == m_enabled)
                    return;
                m_enabled = value;
                if (value == true)
                {
                    OnEnable();
                }
                else
                {
                    OnDisable();
                }
            }
        }

        protected virtual void OnEnable() {}
        protected virtual void OnDisable() {}
        public abstract void Init(Material material);
        public virtual void BeforeProcess(Material material) {}
        public virtual void AfterProcess(Material material) {}
    }
}