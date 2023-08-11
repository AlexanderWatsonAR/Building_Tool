using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RoofTileData
{
    [SerializeField, Range(0, 10)] private float m_Height;
    [SerializeField, Range(0, 10)] private float m_Extend;
    [SerializeField, HideInInspector] private bool m_ExtendHeightBeginning, m_ExtendHeightEnd, m_ExtendWidthBeginning, m_ExtendWidthEnd;
    [SerializeField] private Material m_Material;

    [SerializeField, Range(1, 10)] private int m_Columns, m_Rows;

    public float Height => m_Height;
    public float Extend => m_Extend;
    public bool ExtendHeightBeginning => m_ExtendHeightBeginning;
    public bool ExtendHeightEnd => m_ExtendHeightEnd;
    public bool ExtendWidthBeginning => m_ExtendWidthBeginning;
    public bool ExtendWidthEnd => m_ExtendWidthEnd;
    public Material Material => m_Material;
    public int Columns => m_Columns;
    public int Rows => m_Rows;


    public RoofTileData() : this (0.25f, 1, 1, 0.25f)
    {

    }

    public RoofTileData(RoofTileData data) : this (data.Height, data.Columns, data.Rows, data.m_Extend, data.ExtendHeightBeginning, data.ExtendHeightEnd, data.ExtendWidthBeginning, data.ExtendWidthEnd)
    {

    }
    public RoofTileData(float height, int columns, int rows, float extend, bool heightBeginning = false, bool heightEnd = true, bool widthBeginning = true, bool widthEnd = true)
    {
        m_Height = height;
        m_Columns = columns;
        m_Rows = rows;
        m_Extend = extend;
        m_ExtendHeightBeginning = heightBeginning;
        m_ExtendHeightEnd = heightEnd;
        m_ExtendWidthBeginning = widthBeginning;
        m_ExtendWidthEnd = widthEnd;
    }


}
