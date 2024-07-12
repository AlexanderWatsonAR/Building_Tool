using OnlyInvalid.ProcGenBuilding.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class OpeningDataList : IList<OpeningData>
{
    [SerializeField] List<OpeningData> m_Openings;
    [SerializeField] bool m_IsDirty;

    int ICollection<OpeningData>.Count => m_Openings.Count;

    public bool IsReadOnly => false;
    public OpeningData this[int index] { get => m_Openings[index]; set => m_Openings[index] = value; }
    public int Count => m_Openings.Count;
    public int ActiveCount => m_Openings.FindAll(x => x.IsActive).Count;
    public bool IsDirty { get => m_IsDirty; set => m_IsDirty = value; }

    public UnityEvent OnAdd = new UnityEvent();
    public UnityEvent OnRemove = new UnityEvent();
    public UnityEvent OnShiftUp = new UnityEvent();
    public UnityEvent OnShiftDown = new UnityEvent();
    public UnityEvent OnListChanged = new UnityEvent();

    public OpeningDataList()
    {
        m_Openings = new List<OpeningData>();
        m_IsDirty = false;
    }
    public void Add(OpeningData opening)
    {
        m_Openings.Add(opening);
        OnAdd.Invoke();
        OnListChanged.Invoke();
    }
    public void Clear()
    {
        m_Openings.Clear();
        OnListChanged.Invoke();
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
        OnListChanged.Invoke();
    }
    public void Remove(OpeningData opening)
    {
        if (opening.HasContent)
            opening.Polygon3D.Demolish();

        m_Openings.Remove(opening);
        OnRemove.Invoke();
        OnListChanged.Invoke();
    }
    bool ICollection<OpeningData>.Remove(OpeningData opening)
    {
        return m_Openings.Remove(opening);
    }
    public void RemoveAt(int index)
    {
        m_Openings.RemoveAt(index);
        OnListChanged.Invoke();
    }
    public void Reverse()
    {
        m_Openings.Reverse();
        OnListChanged.Invoke();
    }
    public void ShiftUp()
    {
        m_Openings.ShiftUp();
        OnShiftUp.Invoke();
        OnListChanged.Invoke();
    }
    public void ShiftDown()
    {
        m_Openings.ShiftDown();
        OnShiftDown.Invoke();
        OnListChanged.Invoke();
    }
}
