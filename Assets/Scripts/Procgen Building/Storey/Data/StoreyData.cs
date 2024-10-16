using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using OnlyInvalid.ProcGenBuilding.Common;
using OnlyInvalid.ProcGenBuilding.Corner;
using OnlyInvalid.ProcGenBuilding.Pillar;
using OnlyInvalid.ProcGenBuilding.Floor;
using OnlyInvalid.ProcGenBuilding.Wall;

namespace OnlyInvalid.ProcGenBuilding.Storey
{
    [System.Serializable]
    public class StoreyData : DirtyData
    {
        #region Member Variables
        [SerializeField, HideInInspector] int m_ID;
        [SerializeField, HideInInspector] string m_Name;
        [SerializeField, HideInInspector] ControlPoint[] m_ControlPoints; // static?
        [SerializeField, HideInInspector] WallPoints[] m_WallPoints;
        [SerializeField] StoreyElement m_ActiveElements;

        // These serializable data variables are viewable in the inspector & are for providing data overrides for their array counterparts.
        [SerializeField] WallData m_Wall;
        [SerializeField] PillarData m_Pillar;
        [SerializeField] CornerData m_Corner;
        [SerializeField] FloorData m_Floor;

        [SerializeField, HideInInspector] WallData[] m_Walls;
        [SerializeField, HideInInspector] PillarData[] m_Pillars;
        [SerializeField, HideInInspector] CornerData[] m_Corners;
        #endregion

        #region Accessors
        public WallData WallData => m_Wall;
        public PillarData Pillar => m_Pillar;
        public CornerData CornerData => m_Corner;
        public FloorData FloorData => m_Floor;

        public WallData[] Walls { get { return m_Walls; } set { m_Walls = value; } }
        public PillarData[] Pillars { get { return m_Pillars; } set { m_Pillars = value; } }
        public CornerData[] Corners { get { return m_Corners; } set { m_Corners = value; } }

        public ControlPoint[] ControlPoints { get { return m_ControlPoints; } set { m_ControlPoints = value; } }
        public WallPoints[] WallPoints { get { return m_WallPoints; } set { m_WallPoints = value; } }
        public StoreyElement ActiveElements { get { return m_ActiveElements; } set { m_ActiveElements = value; } }
        public string Name { get { return m_Name; } set { m_Name = value; } }
        public int ID { get { return m_ID; } set { m_ID = value; } }
        #endregion

        public StoreyData() : this(0, new ControlPoint[0], new WallPoints[0], StoreyElement.Everything, new WallData(), new PillarData(), new CornerData(), new FloorData(), null, null, null) { }
        public StoreyData(StoreyData data) : this(data.ID, data.ControlPoints, data.WallPoints, data.ActiveElements, data.WallData, data.Pillar, data.CornerData, data.FloorData, data.Walls, data.Pillars, data.Corners) { }
        public StoreyData(int id, ControlPoint[] controlPoints, WallPoints[] wallPoints, StoreyElement activeElements,
            WallData wallData, PillarData pillarData, CornerData cornerData, FloorData floorData,
            WallData[] walls, PillarData[] pillars, CornerData[] corners)
        {
            m_ID = id;
            m_ControlPoints = controlPoints;
            m_ActiveElements = activeElements;
            m_WallPoints = wallPoints;
            m_Wall = new WallData(wallData);
            m_Pillar = new PillarData(pillarData);
            m_Corner = new CornerData(cornerData);
            m_Floor = new FloorData(floorData);
            m_Walls = walls;
            m_Pillars = pillars;
            m_Corners = corners;
        }
    }
}