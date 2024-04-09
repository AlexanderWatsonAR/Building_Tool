using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RoofTileData : DirtyData
{
    #region Member Variables
    [SerializeField, HideInInspector] private int m_ID;
    [SerializeField, HideInInspector] private LerpPoint[] m_ControlPoints;
    [SerializeField, HideInInspector] bool m_IsInside;
    [SerializeField, Range(0, 10)] private float m_Thickness;
    [SerializeField, Range(0, 10)] private float m_Extend;
    [SerializeField, HideInInspector] private float m_Height;
    [SerializeField, HideInInspector] private bool m_ExtendHeightBeginning, m_ExtendHeightEnd, m_ExtendWidthBeginning, m_ExtendWidthEnd;
    [SerializeField] private Material m_Material;
    [SerializeField, Range(1, 5)] private int m_Columns, m_Rows;
    [SerializeField] private RoofSectionData m_SectionData;
    [SerializeField] private RoofSectionData[,] m_Sections;
    #endregion

    #region Accessors
    public int ID { get { return m_ID; } set { m_ID = value; } }
    public LerpPoint[] ControlPoints { get { return m_ControlPoints; } set { m_ControlPoints = value; } }
    public Vector3[] ExtendedPoints => ExtendPoints(); 
    public float Height { get { return m_Height; } set { m_Height = value; } }
    public float Thickness { get { return m_Thickness; } set { m_Thickness = value; } }
    public float Extend { get { return m_Extend; } set { m_Extend = value; } }
    public bool IsInside { get { return m_IsInside; } set { m_IsInside = value; } }
    public bool ExtendHeightBeginning { get { return m_ExtendHeightBeginning; } set { m_ExtendHeightBeginning = value; } }
    public bool ExtendHeightEnd { get { return m_ExtendHeightEnd; } set { m_ExtendHeightEnd = value; } }
    public bool ExtendWidthBeginning { get { return m_ExtendWidthBeginning; } set { m_ExtendWidthBeginning = value; } }
    public bool ExtendWidthEnd { get { return m_ExtendWidthEnd; } set { m_ExtendWidthEnd = value; } }
    public Material Material { get { return m_Material; } set { m_Material = value; } }
    public int Columns => m_Columns;
    public int Rows => m_Rows;
    public RoofSectionData SectionData => m_SectionData;
    public RoofSectionData[,] Sections { get { return m_Sections; } set { m_Sections = value; } }
    public Vector3 TopLeft => Vector3.Lerp(m_ControlPoints[1].Start, m_ControlPoints[1].End, m_ControlPoints[1].T) + (Vector3.up * m_Height);
    public Vector3 TopRight => Vector3.Lerp(m_ControlPoints[2].Start, m_ControlPoints[2].End, m_ControlPoints[2].T) + (Vector3.up * m_Height);
    #endregion

    public RoofTileData() : this (new LerpPoint[0], new RoofSectionData(), true, 1f, 0.25f, 1, 1, 0.25f, null)
    {

    }

    public RoofTileData(RoofTileData data) : this (data.ControlPoints, data.SectionData, data.IsInside, data.Height, data.Thickness, data.Columns, data.Rows, data.m_Extend, data.Material, data.ExtendHeightBeginning, data.ExtendHeightEnd, data.ExtendWidthBeginning, data.ExtendWidthEnd)
    {

    }
    public RoofTileData(LerpPoint[] controlPoints, RoofSectionData sectionData, bool isInside, float height, float thickness, int columns, int rows, float extend, Material material, bool heightBeginning = false, bool heightEnd = true, bool widthBeginning = true, bool widthEnd = true)
    {
        m_ControlPoints = controlPoints == null? new LerpPoint[0] : controlPoints;
        m_SectionData = sectionData == null ? new RoofSectionData() : sectionData;
        m_IsInside = isInside;
        m_Thickness = thickness;
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
        Vector3[] extendedPoints = new Vector3[] { m_ControlPoints[0].Start, TopLeft, TopRight, m_ControlPoints[3].Start };

        int bottomLeft = 0;
        int topLeft = 1;
        int topRight = 2;
        int bottomRight = 3;

        Vector3 topLeftToBottomLeft = extendedPoints[topLeft].DirectionToTarget(extendedPoints[bottomLeft]);
        Vector3 topRightToBottomRight = extendedPoints[topRight].DirectionToTarget(extendedPoints[bottomRight]);

        Vector3 topLeftToTopRight = extendedPoints[topLeft].DirectionToTarget(extendedPoints[topRight]);
        Vector3 bottomLeftToBottomRight = extendedPoints[bottomLeft].DirectionToTarget(extendedPoints[bottomRight]);

        if (m_ExtendHeightBeginning)
        {
            extendedPoints[topLeft] += -topLeftToBottomLeft * m_Extend;
            extendedPoints[topRight] += -topRightToBottomRight * m_Extend;
        }

        if (m_ExtendHeightEnd)
        {
            extendedPoints[bottomLeft] += topLeftToBottomLeft * m_Extend;
            extendedPoints[bottomRight] += topRightToBottomRight * m_Extend;
        }

        if (m_ExtendWidthBeginning)
        {
            extendedPoints[topLeft] += -topLeftToTopRight * m_Extend;
            extendedPoints[bottomLeft] += -bottomLeftToBottomRight * m_Extend;
        }

        if (m_ExtendWidthEnd)
        {
            extendedPoints[topRight] += topLeftToTopRight * m_Extend;
            extendedPoints[bottomRight] += bottomLeftToBottomRight * m_Extend;
        }

        return extendedPoints;
    }

}
