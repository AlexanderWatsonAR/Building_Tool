using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StoreyData
{
    [SerializeField] private WallData m_WallData;
    [SerializeField] private PillarData m_PillarData;

    public WallData WallData => m_WallData;
    public PillarData PillarData => m_PillarData;

    public StoreyData() : this (new WallData(), new PillarData())
    {

    }

    public StoreyData(StoreyData data) : this(data.WallData, data.PillarData)
    {

    }

    public StoreyData(WallData wallData, PillarData pillarData)
    {
        m_WallData = wallData;
        m_PillarData = pillarData;
    }
}
