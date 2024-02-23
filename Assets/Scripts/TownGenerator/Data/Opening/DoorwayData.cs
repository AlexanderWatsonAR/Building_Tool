using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class DoorwayData : OpeningData
{
    // Opening
    [SerializeField, Range(-0.999f, 0.999f)] protected float m_PositionOffset;

    [SerializeField] protected DoorwayElement m_ActiveElements;

    [SerializeField] protected FrameData[] m_Frames;

    [SerializeField] protected DoorData[] m_Doors;

    public DoorwayElement ActiveElements { get { return m_ActiveElements; } set { m_ActiveElements = value; } }
    public float PositionOffset { get { return m_PositionOffset; } set { m_PositionOffset = value; } }
    public FrameData[] Frames { get { return m_Frames; } set { m_Frames = value; } }
    public DoorData[] Doors { get { return m_Doors; } set { m_Doors = value; } }
    public IList<List<Vector3>> DoorHoles => Doors.Select(door => door.ControlPoints.ToList()).ToList();

    public DoorwayData() : this(0, DoorwayElement.Everything, null, null)
    {
    }

    public DoorwayData(float posOffset, DoorwayElement activeElements, FrameData[] frames, DoorData[] doors) : base()
    {
        m_PositionOffset = posOffset;
        m_ActiveElements = activeElements;
        m_Frames = frames;
        m_Doors = doors;
    }

    public DoorwayData(float height, float width, int cols, int rows, float posOffset, DoorwayElement activeElements, FrameData[] frames, DoorData[] doors) : base (height, width, cols, rows)
    {
        m_PositionOffset = posOffset;
        m_ActiveElements = activeElements;
        m_Frames = frames;
        m_Doors = doors;
    }

    public DoorwayData(DoorwayData data) : this (data.Height, data.Width, data.Columns, data.Rows, data.PositionOffset, data.ActiveElements, data.Frames, data.Doors)
    {

    }
}
