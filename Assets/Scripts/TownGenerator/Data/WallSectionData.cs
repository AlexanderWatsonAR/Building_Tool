using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable]
public class WallSectionData : IData
{
    #region Member Variables
    [SerializeField] private Vector2Int m_ID;
    [SerializeField] private Vector3[] m_ControlPoints;
    [SerializeField] private float m_WallDepth;
    [SerializeField] private Vector3 m_FaceNormal;
    [SerializeField] private WallElement m_WallElement;

    [SerializeField] private WindowData m_WindowData;
    [SerializeField] private DoorData m_DoorData;
    [SerializeField] private DoorData m_ArchDoorData;
    [SerializeField] private DoorFrameData m_DoorFrameData;

    [SerializeField] WindowOpeningData m_WindowOpeningData;
    [SerializeField] DoorwayData m_DoorwayData;
    [SerializeField] ArchwayData m_ArchwayData;
    [SerializeField] ExtensionData m_ExtensionData;
    #endregion

    #region Accessors
    public WallElement WallElement { get { return m_WallElement; } set { m_WallElement = value; } }
    public WindowData WindowData { get { return m_WindowData; } set { m_WindowData = value; } }
    public DoorData DoorData { get { return m_DoorData; } set { m_DoorData = value; } }
    public DoorData ArchDoorData { get { return m_ArchDoorData; } set { m_ArchDoorData = value; } } 
    public DoorFrameData DoorFrameData { get { return m_DoorFrameData; } set { m_DoorFrameData = value; } }
    public Vector2Int ID { get { return m_ID; } set { m_ID = value; } }
    public Vector3[] ControlPoints { get { return m_ControlPoints; } set { m_ControlPoints = value; } }
    public float WallDepth { get { return m_WallDepth; } set { m_WallDepth = value; } }
    public Vector3 FaceNormal { get { return m_FaceNormal; } set { m_FaceNormal = value; } }
    public WindowOpeningData WindowOpening { get { return m_WindowOpeningData; } set { m_WindowOpeningData = value; } }
    public DoorwayData Doorway { get { return m_DoorwayData; } set { m_DoorwayData = value; } }
    public ArchwayData Archway { get { return m_ArchwayData; } set { m_ArchwayData = value; } }
    public ExtensionData Extension { get { return m_ExtensionData; } set { m_ExtensionData = value; } }
    #endregion

    // Food for thought
    // Should we have an empty constructor?
    // We could give it some dummy data, but the data would need re-defining anyway.
    // If it were null, checking the data would be simpler.

    // 15.02.2024
    // Instantiate the windowData, doorData, archDoorData in the constructors.

    public WallSectionData() : this (WallElement.Wall, null, Vector2Int.zero, 0, Vector3.zero)
    {

    }

    public WallSectionData(WallElement wallElement, Vector3[] controlPoints, Vector2Int id, float depth, Vector3 normal) : this(wallElement, controlPoints, id, depth, normal, new WindowOpeningData(), new DoorwayData(), new ArchwayData(), new ExtensionData(), new WindowData(), new DoorData(), new DoorData(), new DoorFrameData())
    {

    }

    public WallSectionData(WallElement wallElement, Vector3[] controlPoints, Vector2Int id, float depth, Vector3 normal, WindowOpeningData windowOpeningData, DoorwayData doorwayData, ArchwayData archwayData, ExtensionData extensionData, WindowData windowData, DoorData doorData, DoorData archDoorData, DoorFrameData doorFrameData)
    {
        m_WallElement = wallElement;
        m_ControlPoints = controlPoints;
        m_ID = id;
        m_WallDepth = depth;
        m_FaceNormal = normal;
        m_WindowOpeningData = new (windowOpeningData);
        m_DoorwayData = new (doorwayData);
        m_ArchwayData = new (archwayData);
        m_ExtensionData = new (extensionData);
        m_WindowData = windowData;
        m_DoorData = doorData;
        m_ArchDoorData = archDoorData;
        m_DoorFrameData = doorFrameData;
    }

    public WallSectionData(WallSectionData data) : this(data.WallElement, data.ControlPoints, data.ID, data.WallDepth, data.FaceNormal, data.WindowOpening, data.Doorway, data.Archway, data.Extension, data.WindowData, data.DoorData, data.ArchDoorData, data.DoorFrameData)
    {
    }
}
