using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PillarsData : DirtyData
{
    [SerializeField] PillarData m_Pillar;
    [SerializeField] PillarData[] m_Pillars;

    public PillarData Pillar { get { return m_Pillar; } set { m_Pillar = value; } }
    public PillarData[] Pillars { get { return m_Pillars; } set { m_Pillars = value; } }

    public PillarsData(PillarData[] pillars, PillarData pillar)
    {
        m_Pillar = pillar;
        m_Pillars = pillars;
    }
    public PillarsData(PillarsData data) : this(data.Pillars, data.Pillar)
    {

    }
}
