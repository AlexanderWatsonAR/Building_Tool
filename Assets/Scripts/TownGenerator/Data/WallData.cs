using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class WallData : IData
{
    [SerializeField, HideInInspector] private int m_ID;
    [SerializeField, HideInInspector] private Vector3 m_Start, m_End;
    [SerializeField, HideInInspector, Range(1, 5)] private int m_Columns, m_Rows;
    [SerializeField, Range(1, 50)] private float m_Height;
    [SerializeField, Range(0, 1)] private float m_Depth;
    [SerializeField, HideInInspector] Material m_Material;

    // This field is viewable in the inspector.
    // Changes to a property field will change every section in the 2D array.
    [SerializeField] private WallSectionData m_SectionData;

    [SerializeField] private WallSectionData[,] m_Sections;

    public WallSectionData SectionData { get{ return m_SectionData; } set { m_SectionData = value; } }

    public int ID { get { return m_ID; } set { m_ID = value; } }
    public Vector3 Start { get { return m_Start; } set { m_Start = value; } }
    public Vector3 End { get { return m_End; } set { m_End = value; } }
    public Material Material { get { return m_Material; } set { m_Material = value; } }
    public float Height { get { return m_Height; } set { m_Height = value; } }
    public float Depth { get { return m_Depth; } set { m_Depth = value; } }
    public int Columns => m_Columns;
    public int Rows => m_Rows;

    public WallSectionData[,] Sections { get { return m_Sections; } set { m_Sections = value; } }

    public WallData(): this(Vector3.zero, Vector3.zero, 1, 1, 4, 0.25f, new WallSectionData[1,1], null)
    {

    }
    public WallData(WallData data) : this (data.Start, data.End, data.Columns, data.Rows, data.Height, data.Depth, data.Sections, data.Material)
    {

    }
    public WallData(Vector3 wallStart, Vector3 wallEnd, int columns, int rows, float height, float depth, WallSectionData[,] sections, Material material)
    {
        m_Start = wallStart;
        m_End = wallEnd;
        m_Columns = columns;
        m_Rows = rows;
        m_Height = height;
        m_Depth = depth;
        m_Material = material;
        m_Sections = sections;
    }

    public Vector3[] BottomPoints()
    {

        return null;
        //int bl = 0;
        //int tl = 1;
        //int tr = 2;
        //int br = 3;

        //Vector3[] bottom = new Vector3[4];

        //bottom[bl] = m_ControlPoints[bl];
        //bottom[br] = m_ControlPoints[br];

        //Vector3 dir = bottom[bl].DirectionToTarget(bottom[br]);
        //Vector3 forward = Vector3.Cross(Vector3.up, dir);

        //bottom[tl] = bottom[bl] + (forward * m_Depth);
        //bottom[tr] = bottom[br] + (forward * m_Depth);

        //return bottom;

    }

}
