using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class BuildingData : IData
{
    [SerializeField] private PolyPath m_Path;
    [SerializeField] private List<StoreyData> m_Storeys;
    [SerializeField] private RoofData m_Roof;

    public PolyPath Path => m_Path;
    public List<StoreyData> StoreysData => m_Storeys;
    public RoofData RoofData => m_Roof;

    public BuildingData(PolyPath path) : this(path, new List<StoreyData> { new StoreyData() { ControlPoints = path.ControlPoints.ToArray() } }, new RoofData() { ControlPoints = path.ControlPoints.ToArray() })
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
    public void AddStorey(StoreyData storeyData)
    {
        storeyData.ControlPoints = m_Path.ControlPoints.ToArray();
        m_Storeys.Add(storeyData);
    }
}
