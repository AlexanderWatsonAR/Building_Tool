using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class DoorwayData : OpeningData
{
    // Opening
    [SerializeField, Range(-0.999f, 0.999f)] protected float m_PositionOffset;

    // Door
    [SerializeField] protected DoorwayElement m_ActiveElements;

    // Door Frame
    [SerializeField, Range(0, 0.999f)] protected float m_FrameDepth;
    [SerializeField, Range(0, 0.999f)] protected float m_FrameScale;

    [SerializeField] protected DoorData[] m_Doors;

    public DoorwayElement ActiveElements { get { return m_ActiveElements; } set { m_ActiveElements = value; } }
    public float PositionOffset { get { return m_PositionOffset; } set { m_PositionOffset = value; } }
    public float FrameScale { get { return m_FrameScale; } set { m_FrameScale = value; } }
    public float FrameDepth { get { return m_FrameDepth; } set { m_FrameDepth = value; } }
    public DoorData[] Doors { get { return m_Doors; } set { m_Doors = value; } }
    public IList<List<Vector3>> DoorHoles => Doors.Select(door => door.ControlPoints.ToList()).ToList();

    public DoorwayData() : this(0, DoorwayElement.Everything, 0.2f, 0.95f, null)
    {
    }

    public DoorwayData(float posOffset, DoorwayElement activeElements, float frameDepth, float frameScale, DoorData[] doors) : base()
    {
        m_PositionOffset = posOffset;
        m_ActiveElements = activeElements;
        m_FrameDepth = frameDepth;
        m_FrameScale = frameScale;
        m_Doors = doors;
    }

    public DoorwayData(float height, float width, int cols, int rows, float posOffset, DoorwayElement activeElements, float frameDepth, float frameScale, DoorData[] doors) : base (height, width, cols, rows)
    {
        m_PositionOffset = posOffset;
        m_ActiveElements = activeElements;
        m_FrameDepth = frameDepth;
        m_FrameScale = frameScale;
        m_Doors = doors;
    }

    public DoorwayData(DoorwayData data) : this (data.Height, data.Width, data.Columns, data.Rows, data.PositionOffset, data.ActiveElements, data.FrameDepth, data.FrameScale, data.Doors)
    {

    }
}
