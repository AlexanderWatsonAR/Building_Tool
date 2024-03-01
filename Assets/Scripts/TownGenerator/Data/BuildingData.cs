using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class BuildingData : IData
{
    // Thoughts on data structures & persistance of data.
    // Would changing IData to an abstract base class effect serilization?
    // How can I maintain object reference after serilization without invoking Building.Build().
    // Am I structuring my data correctly?
    // I am currently using a hierachical data model. Is that correct?
    // Is there a reason for storey to be a monobehaviour?

    [SerializeField] private PolyPath m_Path;
    [SerializeField] private List<StoreyData> m_Storeys;
    [SerializeField] private RoofData m_Roof;

    public PolyPath Path => m_Path;
    public List<StoreyData> Storeys { get { return m_Storeys; } set { m_Storeys = value; } }
    public RoofData RoofData { get{ return m_Roof;} set{ m_Roof = value; } }

    public BuildingData() : this(new PolyPath(), new List<StoreyData>(), new RoofData())
    {

    }
    public BuildingData(PolyPath path) : this(path, new List<StoreyData> { new StoreyData() { ControlPoints = path.ControlPoints.ToArray(), Name = "Ground" } }, new RoofData() { ControlPoints = path.ControlPoints.ToArray() })
    {
        m_Path.CalculateForwards();
    }
    public BuildingData(BuildingData data) : this (data.Path, data.Storeys, data.RoofData)
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
