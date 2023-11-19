using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class WallData : IData
{
    [SerializeField, HideInInspector] private int m_ID; // this will most likely be an index within an array.
    [SerializeField, HideInInspector] private Vector3[] m_ControlPoints;
    [SerializeField, Range(1, 5)] private int m_Columns, m_Rows;
    [SerializeField, Range(1, 50)] private float m_Height;
    [SerializeField, Range(0, 1)] private float m_Depth;
    [SerializeField] Material m_Material;

    [SerializeField] private WallSectionData[,] m_Sections;

    public int ID { get { return m_ID; } set { m_ID = value; } }
    public Vector3[] ControlPoints { get { return m_ControlPoints; } set { m_ControlPoints = value; } }
    public Material Material { get { return m_Material; } set { m_Material = value; } }
    public float Height { get { return m_Height; } set { m_Height = value; } }
    public float Depth { get { return m_Depth; } set { m_Depth = value; } }
    public int Columns => m_Columns;
    public int Rows => m_Rows;

    public WallSectionData[,] Sections { get { return m_Sections; } set { m_Sections = value; } }

    public WallData(): this(null, 1, 1, 4, 0.25f, null)
    {

    }
    public WallData(WallData data) : this (data.ControlPoints, data.Columns, data.Rows, data.Height, data.Depth, data.Material)
    {

    }
    public WallData(IEnumerable<Vector3> controlPoints, int columns, int rows, float height, float depth, Material material)
    {
        m_ControlPoints = controlPoints == null ? new Vector3[4] : controlPoints.ToArray();
        m_Columns = columns;
        m_Rows = rows;
        m_Height = height;
        m_Depth = depth;
        m_Material = material;
        m_Sections = new WallSectionData[columns, rows];
    }

    public Vector3[] BottomPoints()
    {
        int bl = 0;
        int tl = 1;
        int tr = 2;
        int br = 3;

        Vector3[] bottom = new Vector3[4];

        bottom[bl] = m_ControlPoints[bl];
        bottom[br] = m_ControlPoints[br];

        Vector3 dir = bottom[bl].DirectionToTarget(bottom[br]);
        Vector3 forward = Vector3.Cross(Vector3.up, dir);

        bottom[tl] = bottom[bl] + (forward * m_Depth);
        bottom[tr] = bottom[br] + (forward * m_Depth);

        return bottom;

    }

}
