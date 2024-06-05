using UnityEngine;


namespace OnlyInvalid.ProcGenBuilding.Common
{
    [System.Serializable]
    public class DirtyData
    {
        [SerializeField] bool m_IsDirty;

        public bool IsDirty { get { return m_IsDirty; } set { m_IsDirty = value; } }

    }
}
