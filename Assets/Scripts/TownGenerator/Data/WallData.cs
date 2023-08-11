using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder;

[System.Serializable]
public class WallData
{
    [SerializeField] private Vector3[] m_ControlPoints;
    [SerializeField] private List<Vector3[]> m_SubPoints; // Grid points, based on control points, columns & rows.
    [SerializeField, Range(1, 10)] private int m_Columns, m_Rows;
    [SerializeField, Range(1, 100)] private float m_Height;
    [SerializeField, Range(0, 1)] private float m_Depth;
    [SerializeField] Material m_Material;

    public Vector3[] ControlPoints => m_ControlPoints;
    public List<Vector3[]> SubPoints => m_SubPoints;
    public Material Material => m_Material;
    public float Height => m_Height;
    public float Depth => m_Depth;
    public int Columns => m_Columns;
    public int Rows => m_Rows;

    public WallData(): this(null, null, 1, 1, 1, 1, null)
    {

    }
    public WallData(WallData data) : this (data.ControlPoints, data.SubPoints, data.Columns, data.Rows, data.Height, data.Depth, data.Material)
    {

    }
    public WallData(IEnumerable<Vector3> controlPoints, List<Vector3[]> subPoints, int columns, int rows, float height, float depth, Material material)
    {
        m_ControlPoints = controlPoints == null ? new Vector3[4] : controlPoints.ToArray();
        m_SubPoints = subPoints == null ? new List<Vector3[]>() : subPoints;
        m_Columns = columns;
        m_Rows = rows;
        m_Height = height;
        m_Depth = depth;
        m_Material = material;
    }
    public void SetControlPoints(IEnumerable<Vector3> controlPoints)
    {
        m_ControlPoints = controlPoints.ToArray();
    }

    public void SetMaterial(Material material)
    {
        m_Material = material;
    }

    public void SetSubPoints(List<Vector3[]> subPoints)
    {
        m_SubPoints = subPoints;
    }

    public void SetHeight(float height)
    {
        m_Height = height;
    }
}
