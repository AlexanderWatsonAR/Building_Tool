using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

[System.Serializable]
public class FloorData : IData
{
    [SerializeField, Range(1, 10)] private int m_Columns, m_Rows; 
    [SerializeField, HideInInspector] private ControlPoint[] m_ControlPoints;
    [SerializeField] private float m_Height;
    [SerializeField, HideInInspector] private Material m_Material;

    public Material Material { get { return m_Material; } set { m_Material = value; } }

    public int Columns { get { return m_Columns; } set { m_Columns = value; } }
    public int Rows { get { return m_Rows; } set { m_Rows = value; } }

    public ControlPoint[] ControlPoints { get { return m_ControlPoints; } set { m_ControlPoints = value; }}
    public float Height => m_Height;

    public FloorData() : this (new ControlPoint[0], 1, 1, 0.1f, null)
    {

    }
    public FloorData(ControlPoint[] controlPoints, int columns, int rows, float height, Material material)
    {
        m_ControlPoints = controlPoints;
        m_Columns = columns;
        m_Rows = rows;
        m_Height = height;
        m_Material = material;

    }

    public FloorData(FloorData data) : this (data.ControlPoints, data.Columns, data.Columns, data.Height, data.Material)
    {

    }



}
