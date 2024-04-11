
using UnityEngine;
using UnityEngine.Events;

namespace OnlyInvalid.ProcGenBuilding.Common
{
    public interface IBuildable
    {
        IBuildable Initialize(DirtyData data);
        void Build();
        void Demolish();

    }

    [System.Serializable]
    public class DataEvent : UnityEvent<DirtyData> { }

    public class Buildable : MonoBehaviour
    {
        public DataEvent onDataChanged = new DataEvent();

        public virtual Buildable Initialize(DirtyData data)
        {
            return this;
        }

        public virtual void Build() { }

        public virtual void Demolish() { }
    }

    public class Poly : Buildable
    {
        Polygon3D.FrameData m_Data;

        public override Buildable Initialize(DirtyData data)
        {
            return base.Initialize(data);
        }

        public override void Build()
        {
            base.Build();
        }
        
    }
}
