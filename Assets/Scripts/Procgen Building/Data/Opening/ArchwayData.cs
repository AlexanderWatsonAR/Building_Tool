using System;
using System.Collections;
using UnityEngine;
using OnlyInvalid.ProcGenBuilding.Polygon3D;
using OnlyInvalid.ProcGenBuilding.Door;

[System.Serializable]
public class ArchwayData : DoorwayData, ICloneable
{
    // Maybe add an animation curve?
    [SerializeField, Range(0, 1)] private float m_ArchHeight;
    [SerializeField, Range(3, 16)] private int m_ArchSides;
    public float ArchHeight { get { return m_ArchHeight; } set { m_ArchHeight = value; } }
    public int ArchSides { get { return m_ArchSides; } set { m_ArchSides = value; } }

    public ArchwayData() : this(1, 4)
    {
    }

    public ArchwayData(float archHeight, int archSides) : base()
    {
        m_ArchHeight = archHeight;
        m_ArchSides = archSides;
    }

    public ArchwayData(float height, float width, int cols, int rows, float posOffset, DoorwayElement activeElements, FrameData[] frames, DoorData[] doors, float archHeight, int archSides) :
        base(height, width, cols, rows, posOffset, activeElements, frames, doors)
    {
        m_ArchHeight = archHeight;
        m_ArchSides = archSides;
    }

    public ArchwayData(ArchwayData data) : this(data.Height, data.Width, data.Columns, data.Rows, data.PositionOffset, data.ActiveElements, data.Frames, data.Doors, data.m_ArchHeight, data.m_ArchSides)
    {
    }

    public new object Clone()
    {
        ArchwayData copy = base.Clone() as ArchwayData;
        copy.ArchHeight = m_ArchHeight;
        copy.ArchSides = m_ArchSides;
        
        return copy;
    }
}