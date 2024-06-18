using OnlyInvalid.ProcGenBuilding.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OpeningDataList : IList<OpeningData>, ISerializationCallbackReceiver
{
    [SerializeField] List<OpeningData> m_Openings;
    [SerializeField] bool m_IsDirty;

    int ICollection<OpeningData>.Count => m_Openings.Count;

    public bool IsReadOnly => false;

    public OpeningData this[int index] { get => m_Openings[index]; set => m_Openings[index] = value; }

    public int Count => m_Openings.Count;

    public bool IsDirty { get => m_IsDirty; set => m_IsDirty = value; }

    public OpeningDataList()
    {
        m_Openings = new List<OpeningData>();
        m_IsDirty = false;
    }
    public void Add(OpeningData opening)
    {
        m_Openings.Add(opening);
    }
    public void Clear()
    {
        m_Openings.Clear();
    }
    public bool Contains(OpeningData opening)
    {
        return m_Openings.Contains(opening);
    }
    public void CopyTo(OpeningData[] array, int arrayIndex)
    {
        m_Openings.CopyTo(array, arrayIndex);
    }
    public IEnumerator<OpeningData> GetEnumerator()
    {
        return m_Openings.GetEnumerator();
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
        return m_Openings.GetEnumerator();
    }
    public int IndexOf(OpeningData opening)
    {
        return m_Openings.IndexOf(opening);
    }
    public void Insert(int index, OpeningData opening)
    {
        m_Openings.Insert(index, opening);
    }
    public void Remove(OpeningData opening)
    {
        m_Openings.Remove(opening);
    }
    bool ICollection<OpeningData>.Remove(OpeningData opening)
    {
        return m_Openings.Remove(opening);
    }
    public void RemoveAt(int index)
    {
        m_Openings.RemoveAt(index);
    }
    public void Reverse()
    {
        m_Openings.Reverse();
    }
    public void OnBeforeSerialize()
    {
    }
    public void OnAfterDeserialize()
    {
    }
}
