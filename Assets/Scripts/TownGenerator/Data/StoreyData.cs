using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StoreyData : IData
{
    [SerializeField, HideInInspector] private int m_ID;
    [SerializeField] private ControlPoint[] m_ControlPoints;
    [SerializeField] private StoreyElement m_ActiveElements;

    [SerializeField] private WallData m_Wall;
    [SerializeField] private PillarData m_Pillar;
    [SerializeField] private CornerData m_Corner;
    [SerializeField] private FloorData m_Floor;

    public WallData WallData => m_Wall;
    public PillarData PillarData => m_Pillar;
    public CornerData CornerData => m_Corner;
    public FloorData FloorData => m_Floor;

    public ControlPoint[] ControlPoints { get { return m_ControlPoints; } set { m_ControlPoints = value; } }
    public StoreyElement ActiveElements => m_ActiveElements;
    public int ID { get{ return m_ID; } set { m_ID = value; } }

    public Vector3[] InsidePoints
    {
        get
        {
            Vector3[] insidePoints = new Vector3[m_ControlPoints.Length];

            for (int i = 0; i < insidePoints.Length; i++)
            {
                float w = Mathf.Lerp(-1, 1, m_Wall.Depth);
                insidePoints[i] = m_ControlPoints[i].Position + m_ControlPoints[i].Forward + (m_ControlPoints[i].Forward * w);
            }

            return insidePoints;
        }
    }

    public StoreyData() : this (0, new ControlPoint[0], StoreyElement.Everything, new WallData(), new PillarData(), new CornerData(), new FloorData())
    {

    }

    public StoreyData(StoreyData data) : this(data.ID, data.ControlPoints, data.ActiveElements, data.WallData, data.PillarData, data.CornerData, data.FloorData)
    {

    }

    public StoreyData(int id, ControlPoint[] controlPoints, StoreyElement activeElements, WallData wallData, PillarData pillarData, CornerData cornerData, FloorData floorData)
    {
        m_ID = id;
        m_ControlPoints = controlPoints == null? new ControlPoint[0] : controlPoints;
        m_ActiveElements = activeElements;
        m_Wall = wallData;
        m_Pillar = pillarData;
        m_Pillar.Height = m_Wall.Height;
        m_Corner = cornerData;
        m_Floor = floorData;
    }
}
