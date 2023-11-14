using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RoofTileData : IData
{
    [SerializeField, HideInInspector] Vector3[] m_ControlPoints;
    [SerializeField, HideInInspector] bool m_IsInside;
    [SerializeField, Range(0, 10)] private float m_Height;
    [SerializeField, Range(0, 10)] private float m_Extend;
    [SerializeField, HideInInspector] private bool m_ExtendHeightBeginning, m_ExtendHeightEnd, m_ExtendWidthBeginning, m_ExtendWidthEnd;
    [SerializeField] private Material m_Material;

    [SerializeField, Range(1, 5)] private int m_Columns, m_Rows;

    public Vector3[] ControlPoints { get { return m_ControlPoints; } set { m_ControlPoints = value;} }
    public Vector3[] ExtendedPoints => ExtendPoints(); 
    public float Height => m_Height;
    public float Extend => m_Extend;
    public bool IsInside { get { return m_IsInside; } set { m_IsInside = value; } }
    public bool ExtendHeightBeginning { get { return m_ExtendHeightBeginning; } set { m_ExtendHeightBeginning = value; } }
    public bool ExtendHeightEnd { get { return m_ExtendHeightEnd; } set { m_ExtendHeightEnd = value; } }
    public bool ExtendWidthBeginning { get { return m_ExtendWidthBeginning; } set { m_ExtendWidthBeginning = value; } }
    public bool ExtendWidthEnd { get { return m_ExtendWidthEnd; } set { m_ExtendWidthEnd = value; } }
    public Material Material { get { return m_Material; } set { m_Material = value; } }
    public int Columns => m_Columns;
    public int Rows => m_Rows;


    public RoofTileData() : this (new Vector3[0], true, 0.25f, 1, 1, 0.25f, null)
    {

    }

    public RoofTileData(RoofTileData data) : this (data.ControlPoints, data.IsInside, data.Height, data.Columns, data.Rows, data.m_Extend, data.Material, data.ExtendHeightBeginning, data.ExtendHeightEnd, data.ExtendWidthBeginning, data.ExtendWidthEnd)
    {

    }
    public RoofTileData(Vector3[] controlPoints, bool isInside, float height, int columns, int rows, float extend, Material material, bool heightBeginning = false, bool heightEnd = true, bool widthBeginning = true, bool widthEnd = true)
    {
        m_ControlPoints = controlPoints == null ? new Vector3[0] : controlPoints;
        m_IsInside = isInside;
        m_Height = height;
        m_Columns = columns;
        m_Rows = rows;
        m_Extend = extend;
        m_ExtendHeightBeginning = heightBeginning;
        m_ExtendHeightEnd = heightEnd;
        m_ExtendWidthBeginning = widthBeginning;
        m_ExtendWidthEnd = widthEnd;
        m_Material = material;
    }

    private Vector3[] ExtendPoints()
    {
        Vector3[] extendedPoints = m_ControlPoints.Clone() as Vector3[];

        int BottomLeft = 0;
        int TopLeft = 1;
        int TopRight = 2;
        int BottomRight = 3;

        Vector3 topLeftToBottomLeft = m_ControlPoints[TopLeft].DirectionToTarget(m_ControlPoints[BottomLeft]);
        Vector3 topRightToBottomRight = m_ControlPoints[TopRight].DirectionToTarget(m_ControlPoints[BottomRight]);

        Vector3 topLeftToTopRight = m_ControlPoints[TopLeft].DirectionToTarget(m_ControlPoints[TopRight]);
        Vector3 bottomLeftToBottomRight = m_ControlPoints[BottomLeft].DirectionToTarget(m_ControlPoints[BottomRight]);

        if (m_ExtendHeightBeginning)
        {
            extendedPoints[TopLeft] += -topLeftToBottomLeft * m_Extend;
            extendedPoints[TopRight] += -topRightToBottomRight * m_Extend;
        }

        if (m_ExtendHeightEnd)
        {
            extendedPoints[BottomLeft] += topLeftToBottomLeft * m_Extend;
            extendedPoints[BottomRight] += topRightToBottomRight * m_Extend;
        }

        if (m_ExtendWidthBeginning)
        {
            extendedPoints[TopLeft] += -topLeftToTopRight * m_Extend;
            extendedPoints[BottomLeft] += -bottomLeftToBottomRight * m_Extend;
        }

        if (m_ExtendWidthEnd)
        {
            extendedPoints[TopRight] += topLeftToTopRight * m_Extend;
            extendedPoints[BottomRight] += bottomLeftToBottomRight * m_Extend;
        }

        return extendedPoints;
    }

}
