using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StoreyData
{
    [SerializeField] private WallData m_Wall;
    [SerializeField] private PillarData m_Pillar;

    public WallData WallData => m_Wall;
    public PillarData PillarData => m_Pillar;

    public StoreyData() : this (new WallData(), new PillarData())
    {

    }

    public StoreyData(StoreyData data) : this(data.WallData, data.PillarData)
    {

    }

    public StoreyData(WallData wallData, PillarData pillarData)
    {
        m_Wall = wallData;
        m_Pillar = pillarData;
    }
}
