using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class StoreyData : IData
{
    // Notes of serialization. I may be repeating myself.
    // When creating a new building object a StoreyData object is instantiated and a reference is passed to an instance of Storey class.
    // Upon serialization, that reference is stored as a as copy of the original. So, now inside the storey class is no longer a reference to 
    // the StoreyData inside Building but a copy of the StoreyData from building. As such, When making changes in the Storey inspector we are altering the copy
    // and not changing the original StoreyData.

    // Test note: 16-01-2023 - attempted to modify data in the storey & building inspector. data changed but then went back to the default.
    // 17-01-2023 - Removing the serialized reference to the building data stopped the storey reset from happening.
    #region Member Variables
    [SerializeField, HideInInspector] private int m_ID;
    [SerializeField, HideInInspector] private string m_Name;
    [SerializeField, HideInInspector] private ControlPoint[] m_ControlPoints; // static?
    [SerializeField, HideInInspector] private WallPoints[] m_WallPoints;
    [SerializeField] private StoreyElement m_ActiveElements;

    // These serializable data variables are viewable in the inspector & are for providing data overrides for their array counterparts.
    [SerializeField] private WallData m_Wall;
    [SerializeField] private PillarData m_Pillar;
    [SerializeField] private CornerData m_Corner;
    [SerializeField] private FloorData m_Floor;

    [SerializeField, HideInInspector] private WallData[] m_Walls;
    [SerializeField, HideInInspector] private PillarData[] m_Pillars;
    [SerializeField, HideInInspector] private CornerData[] m_Corners;
    #endregion

    //[SerializeReference] private BuildingData TheBuilding;

    //public BuildingData Building { get { return TheBuilding; } set { TheBuilding = value; } }

    #region Accessors
    public WallData WallData => m_Wall;
    public PillarData PillarData => m_Pillar;
    public CornerData CornerData => m_Corner;
    public FloorData FloorData => m_Floor;

    public WallData[] Walls { get { return m_Walls; } set { m_Walls = value; } }
    public PillarData[] Pillars { get { return m_Pillars; } set { m_Pillars = value; } }
    public CornerData[] Corners { get { return m_Corners; } set { m_Corners = value; } }

    public ControlPoint[] ControlPoints { get { return m_ControlPoints; } set { m_ControlPoints = value; } }
    public WallPoints[] WallPoints { get { return m_WallPoints; } set { m_WallPoints = value; } }
    public StoreyElement ActiveElements => m_ActiveElements;

    public string Name { get { return m_Name; } set { m_Name = value; } }
    public int ID { get{ return m_ID; } set { m_ID = value; } }
    #endregion

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

    public StoreyData() : this (0, new ControlPoint[0], new WallPoints[0], StoreyElement.Everything, new WallData(), new PillarData(), new CornerData(), new FloorData()){}
    public StoreyData(StoreyData data) : this(data.ID, data.ControlPoints, data.WallPoints, data.ActiveElements, data.WallData, data.PillarData, data.CornerData, data.FloorData){}
    public StoreyData(int id, ControlPoint[] controlPoints, WallPoints[] wallPoints, StoreyElement activeElements, WallData wallData, PillarData pillarData, CornerData cornerData, FloorData floorData)
    {
        m_ID = id;
        m_ControlPoints = controlPoints == null? new ControlPoint[0] : controlPoints;
        m_ActiveElements = activeElements;
        m_Wall = wallData;
        m_Pillar = pillarData;
        m_Pillar.Height = m_Wall.Height;
        m_Corner = cornerData;
        m_Floor = floorData;
        m_Walls = null;
        m_Pillars = null;
        m_Corners = null;
    }
}
