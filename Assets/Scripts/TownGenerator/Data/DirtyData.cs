using UnityEngine;


[System.Serializable]
public abstract class DirtyData
{
    [SerializeField] bool m_IsDirty;

    public bool IsDirty { get { return m_IsDirty; } set { m_IsDirty = value; } }

}

