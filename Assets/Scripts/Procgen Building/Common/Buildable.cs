using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace OnlyInvalid.ProcGenBuilding.Common
{
    public class Buildable : MonoBehaviour
    {
        protected DirtyDataEvent m_OnDataChanged = new DirtyDataEvent();

        [SerializeField] DirtyData m_Data;

        public virtual Buildable Initialize(DirtyData data)
        {
            return this;
        }

        public virtual void Build() { }

        public virtual void Demolish() { }

        public void AddDataListener(UnityAction<DirtyData> call)
        {
            m_OnDataChanged.AddListener(call);
        }

        public void RemoveDataListener(UnityAction<DirtyData> call)
        {
            m_OnDataChanged.RemoveListener(call);
        }

        private void OnValidate()
        {
            m_OnDataChanged.Invoke(m_Data);
        }
    }
}
