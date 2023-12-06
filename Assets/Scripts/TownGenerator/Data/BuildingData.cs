using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class BuildingData : IData
{
    // Should this data just be for saving & loading?
    [SerializeField] private PolyPath m_Path;
    [SerializeField] private List<StoreyData> m_Storeys;
    [SerializeField] private RoofData m_Roof;

    public PolyPath Path => m_Path;
    public List<StoreyData> StoreysData => m_Storeys;
    public RoofData RoofData { get{ return m_Roof;} set{ m_Roof = value; } }

    public BuildingData() : this(new PolyPath(), new List<StoreyData>(), new RoofData())
    {

    }
    public BuildingData(PolyPath path) : this(path, new List<StoreyData> { new StoreyData() { ControlPoints = path.ControlPoints.ToArray(), Name = "Ground" } }, new RoofData() { ControlPoints = path.ControlPoints.ToArray() })
    {
        m_Path.CalculateForwards();
    }
    public BuildingData(BuildingData data) : this (data.Path, data.StoreysData, data.RoofData)
    {

    }
    public BuildingData(PolyPath path, List<StoreyData> storeysData, RoofData roofData)
    {
        m_Path = path;
        m_Storeys = storeysData;
        m_Roof = roofData;
    }
    /// <summary>
    /// Each storey data as an id equal to its index in the list.
    /// </summary>
    public void AssignStoreyID()
    {
        for(int i = 0; i < m_Storeys.Count; i++)
        {
            m_Storeys[i].ID = i;
        }
    }
}
