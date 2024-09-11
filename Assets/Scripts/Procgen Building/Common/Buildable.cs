using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor.Events;

namespace OnlyInvalid.ProcGenBuilding.Common
{
    public abstract class Buildable : MonoBehaviour
    {
        [SerializeReference] protected DirtyData m_Data, m_PreviousData;

        [SerializeField] protected DirtyDataEvent m_OnDataChanged = new();

        public DirtyData Data => m_Data;

        public virtual Buildable Initialize(DirtyData data)
        {
            m_Data = data;
            return this;
        }

        public virtual void Build() { }

        public virtual void Demolish()
        {
        }

        public void AddListener(UnityAction<DirtyData> call)
        {
            UnityEventTools.AddPersistentListener(m_OnDataChanged, call);

            for(int i = 0; i < m_OnDataChanged.GetPersistentEventCount(); i++)
            {
                m_OnDataChanged.SetPersistentListenerState(i, UnityEventCallState.EditorAndRuntime);
            }
        }

        public void RemoveListener(UnityAction<DirtyData> call)
        {
            UnityEventTools.RemovePersistentListener(m_OnDataChanged, call);
        }

        public bool HasListener()
        {
            return m_OnDataChanged.GetPersistentEventCount() > 0;
        }
    }
}
