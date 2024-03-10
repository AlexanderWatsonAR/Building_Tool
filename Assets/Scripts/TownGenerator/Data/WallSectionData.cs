using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable]
public class WallSectionData : Polygon3DData
{
    #region Member Variables
    [SerializeField] Vector2Int m_ID;
    [SerializeField] WallElement m_WallElement;

    [SerializeField] WindowData m_Window;
    [SerializeField] DoorData m_Door;
    [SerializeField] DoorData m_ArchDoor;
    [SerializeField] FrameData m_DoorFrame;

    [SerializeField] WindowOpeningData m_WindowOpening;
    [SerializeField] DoorwayData m_Doorway;
    [SerializeField] ArchwayData m_Archway;
    [SerializeField] ExtensionData m_Extension;
    #endregion

    #region Accessors
    public WallElement WallElement { get { return m_WallElement; } set { m_WallElement = value; } }
    public WindowData Window { get { return m_Window; } set { m_Window = value; } }
    public DoorData Door { get { return m_Door; } set { m_Door = value; } }
    public DoorData ArchDoor { get { return m_ArchDoor; } set { m_ArchDoor = value; } } 
    public FrameData DoorFrame { get { return m_DoorFrame; } set { m_DoorFrame = value; } }
    public Vector2Int ID { get { return m_ID; } set { m_ID = value; } }
    public WindowOpeningData WindowOpening { get { return m_WindowOpening; } set { m_WindowOpening = value; } }
    public DoorwayData Doorway { get { return m_Doorway; } set { m_Doorway = value; } }
    public ArchwayData Archway { get { return m_Archway; } set { m_Archway = value; } }
    public ExtensionData Extension { get { return m_Extension; } set { m_Extension = value; } }
    #endregion

    // Food for thought
    // Should we have an empty constructor?
    // We could give it some dummy data, but the data would need re-defining anyway.
    // With an empty constructor we can create an empty object and fill the data later.
    // If it were null, checking the data would be simpler.

    // 15.02.2024
    // Instantiate the windowData, doorData, archDoorData in the constructors.

    public WallSectionData()
    {
        m_Window = new WindowData();
        m_Door = new DoorData();
        m_ArchDoor = new DoorData();
        m_DoorFrame = new FrameData();

        m_WindowOpening = new WindowOpeningData();
        m_Doorway = new DoorwayData();
        m_Archway = new ArchwayData();
        m_Extension = new ExtensionData();
    }

    public WallSectionData(WallElement wallElement, PolygonData polygon, PolygonData[] holes, Vector2Int id, float height, float width, float depth, Vector3 normal, Vector3 position,
        WindowOpeningData windowOpeningData, DoorwayData doorwayData, ArchwayData archwayData, ExtensionData extensionData,
        WindowData windowData, DoorData doorData, DoorData archDoorData, FrameData doorFrameData) :base(polygon, holes, normal, height, width, depth, position)
    {
        m_WallElement = wallElement;
        m_ID = id;
        m_WindowOpening = new (windowOpeningData);
        m_Doorway = new (doorwayData);
        m_Archway = new (archwayData);
        m_Extension = new (extensionData);
        m_Window = windowData;
        m_Door = doorData;
        m_ArchDoor = archDoorData;
        m_DoorFrame = doorFrameData;
    }

    public WallSectionData(WallSectionData data) : this
    (
        data.WallElement,
        data.Polygon,
        data.Holes,
        data.ID,
        data.Height,
        data.Width,
        data.Depth,
        data.Normal,
        data.Position,
        data.WindowOpening,
        data.Doorway,
        data.Archway,
        data.Extension,
        data.Window,
        data.Door,
        data.ArchDoor,
        data.DoorFrame
    )
    {
    }
}
