using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BuildingData
{
    [SerializeField] private ControlPoint[] m_ControlPoints;
    [SerializeField] private StoreyData[] m_StoreysData;
    [SerializeField] private RoofData m_RoofData;

    public ControlPoint[] ControlPoints => m_ControlPoints;
    public StoreyData[] StoreysData => m_StoreysData;
    public RoofData RoofData => m_RoofData;

    public BuildingData(ControlPoint[] controlPoints) : this (controlPoints, new StoreyData[] {new StoreyData()}, new RoofData())
    {

    }
    public BuildingData(BuildingData data) : this (data.ControlPoints, data.StoreysData, data.RoofData)
    {

    }
    public BuildingData(ControlPoint[] controlPoints, StoreyData[] storeysData, RoofData roofData)
    {
        m_ControlPoints = controlPoints;
        m_StoreysData = storeysData;
        m_RoofData = roofData;
    }
}
