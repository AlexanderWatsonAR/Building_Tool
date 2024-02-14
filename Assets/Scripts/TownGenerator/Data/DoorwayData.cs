using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class DoorwayData
{
    // Doorway
    [SerializeField, Range(0, 0.999f)] protected float m_Height;
    [SerializeField, Range(0, 0.999f)] protected float m_Width;
    [SerializeField, Range(-0.999f, 0.999f)] protected float m_PositionOffset;
    [SerializeField, Range(1, 5)] protected int m_Columns, m_Rows;

    // Door
    [SerializeField] protected DoorwayElement m_ActiveElements;
    [SerializeField] protected DoorData m_DoorData;

    // Door Frame
    [SerializeField, Range(0, 0.999f)] protected float m_FrameDepth;
    [SerializeField, Range(0, 0.999f)] protected float m_FrameScale;

    [SerializeField] protected DoorData[] m_Doors;

    public DoorData DoorData { get { return m_DoorData; } set { m_DoorData = value; } }
    public DoorwayElement ActiveElements { get { return m_ActiveElements; } set { m_ActiveElements = value; } }
    public float Height { get { return m_Height; } set { m_Height = value; } }
    public float Width { get { return m_Width; } set { m_Width = value; } }
    public float PositionOffset { get { return m_PositionOffset; } set { m_PositionOffset = value; } }
    public int Columns { get { return m_Columns; } set { m_Columns = value; } }
    public int Rows { get { return m_Rows; } set { m_Rows = value; } }
    public float FrameScale { get { return m_FrameScale; } set { m_FrameScale = value; } }
    public float FrameDepth { get { return m_FrameDepth; } set { m_FrameDepth = value; } }
    public DoorData[] Doors { get { return m_Doors; } set { m_Doors = value; } }
    public IList<List<Vector3>> DoorHoles => Doors.Select(door => door.ControlPoints.ToList()).ToList();

    public DoorwayData(int cols, int rows, float height, float width, float posOffset, DoorwayElement activeElements, float frameDepth, float frameScale)
    {
        m_Columns = cols;
        m_Rows = rows;
        m_Height = height;
        m_Width = width;
        m_PositionOffset = posOffset;
        m_ActiveElements = activeElements;
        m_FrameDepth = frameDepth;
        m_FrameScale = frameScale;
    }

    public DoorwayData(DoorwayData data) : this (data.Columns, data.Rows, data.Height, data.Width, data.PositionOffset, data.ActiveElements, data.FrameDepth, data.FrameScale)
    {

    }
}

[System.Serializable]
public class ArchwayData : DoorwayData
{
    // Maybe add an animation curve?
    [SerializeField, Range(0, 1)] private float m_ArchHeight;
    [SerializeField, Range(3, 16)] private int m_ArchSides;

    public ArchwayData(int cols, int rows, float height, float width, float posOffset, DoorwayElement activeElements, float frameDepth, float frameScale, float archHeight, int archSides) :
        base(cols, rows, height, width, posOffset, activeElements, frameDepth, frameScale)
    {
        m_ArchHeight = archHeight;
        m_ArchSides = archSides;
    }

    public ArchwayData(ArchwayData data) : this (data.Columns, data.Rows, data.Height, data.Width, data.PositionOffset, data.ActiveElements, data.FrameDepth, data.FrameScale, data.m_ArchHeight, data.m_ArchSides)
    {
    }
}
