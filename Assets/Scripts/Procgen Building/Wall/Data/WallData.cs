using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using OnlyInvalid.ProcGenBuilding.Common;


namespace OnlyInvalid.ProcGenBuilding.Wall
{
    [System.Serializable]
    public class WallData : DirtyData, ICloneable
    {
        #region Member Variables
        [SerializeField, HideInInspector] int m_ID;
        [SerializeField, HideInInspector] ControlPoint m_Start, m_End;
        [SerializeField, HideInInspector] Vector3 m_Normal;

        [SerializeField, Range(1, 5)] int m_Columns, m_Rows;
        [SerializeField, Range(1, 50)] float m_Height;
        [SerializeField, Range(0, 1)] float m_Depth;
        [SerializeField, HideInInspector] bool m_IsTriangle;
        [SerializeField, HideInInspector] Material m_Material;
        [SerializeField] WallSectionData m_SectionData;
        [SerializeField] WallSectionData[] m_Sections;
        #endregion

        #region Accessors
        public WallSectionData SectionData { get { return m_SectionData; } set { m_SectionData = value; } }
        public int ID { get { return m_ID; } set { m_ID = value; } }
        public Vector3 StartPosition
        {
            get
            {
                float d = Mathf.Lerp(-1, 1, m_Depth);
                Vector3 start = m_Start.Position + m_Start.Forward + (m_Start.Forward * d);
                return start;
            }
        }
        public Vector3 EndPosition
        {
            get
            {
                float d = Mathf.Lerp(-1, 1, m_Depth);
                Vector3 end = m_End.Position + m_End.Forward + (m_End.Forward * d);
                return end;
            }
        }
        public ControlPoint Start { get { return m_Start; } set { m_Start = value; } }
        public ControlPoint End { get { return m_End; } set { m_End = value; } }
        public Vector3 Normal { get { return m_Normal; } set { m_Normal = value; } }
        public Material Material { get { return m_Material; } set { m_Material = value; } }
        public float Height { get { return m_Height; } set { m_Height = value; } }
        public float Depth { get { return m_Depth; } set { m_Depth = value; } }
        public int Columns { get { return m_Columns; } set { m_Columns = value; } }
        public int Rows => m_Rows;
        public bool IsTriangle { get { return m_IsTriangle; } set { m_IsTriangle = value; } }
        public WallSectionData[] Sections { get { return m_Sections; } set { m_Sections = value; } }
        #endregion

        public WallData() : this(null, null, Vector3.zero, 1, 1, 4, 0.25f, new WallSectionData[1], null)
        {

        }
        public WallData(WallData data) : this(data.Start, data.End, data.Normal, data.Columns, data.Rows, data.Height, data.Depth, data.Sections, data.Material)
        {

        }
        public WallData(ControlPoint wallStart, ControlPoint wallEnd, Vector3 normal, int columns, int rows, float height, float depth, WallSectionData[] sections, Material material)
        {
            m_Start = wallStart;
            m_End = wallEnd;
            m_Normal = normal;
            m_Columns = columns;
            m_Rows = rows;
            m_Height = height;
            m_Depth = depth;
            m_Material = material;
            m_Sections = sections;
            m_IsTriangle = false;
        }

        public object Clone()
        {
            WallData clone = MemberwiseClone() as WallData;

            return Clone();
        }
    }
}
