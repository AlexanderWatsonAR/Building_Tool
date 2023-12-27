using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable]
public class WallSectionData : IData
{
    [SerializeField, HideInInspector] private Vector2Int m_ID;
    [SerializeField, HideInInspector] private Vector3[] m_ControlPoints;
    [SerializeField, HideInInspector] private float m_WallDepth;
    [SerializeField, HideInInspector] private Vector3 m_FaceNormal;

    [SerializeField] private WallElement m_WallElement;

    public WallElement WallElement => m_WallElement;

    #region Window Properties
    [SerializeField] private WindowData m_WindowData;
    [SerializeField, Range(0, 0.999f)] private float m_WindowHeight;
    [SerializeField, Range(0, 0.999f)] private float m_WindowWidth;
    [SerializeField, Range(3, 16)] private int m_WindowSides = 3;
    [SerializeField, Range(1, 5)] private int m_WindowColumns, m_WindowRows;
    [SerializeField, Range(-180, 180)] private float m_WindowAngle;
    [SerializeField] private bool m_WindowSmooth;

    [SerializeField] private WindowData[] m_Windows;

    public WindowData WindowData { get { return m_WindowData; } set { m_WindowData = value; } }
    public WindowData[] Windows { get { return m_Windows; } set { m_Windows = value; } }
    public int WindowSides => m_WindowSides;
    public float WindowAngle => m_WindowAngle;
    public float WindowHeight => m_WindowHeight;
    public float WindowWidth => m_WindowWidth;
    public int WindowColumns => m_WindowColumns;
    public int WindowRows => m_WindowRows;
    public bool WindowSmooth => m_WindowSmooth;

    #endregion

    #region Door Properties
    // Doorway
    [SerializeField, Range(0, 0.999f)] private float m_DoorPedimentHeight;
    [SerializeField, Range(0, 0.999f)] private float m_DoorSideWidth;
    [SerializeField, Range(-0.999f, 0.999f)] private float m_DoorSideOffset;
    [SerializeField, Range(1, 5)] private int m_DoorColumns, m_DoorRows;

    // Door
    [SerializeField] private DoorwayElement m_ActiveDoorwayElements;
    [SerializeField] private DoorData m_DoorData;

    // Door Frame
    [SerializeField, Range(0, 0.999f)] private float m_DoorFrameDepth;
    [SerializeField, Range(0, 0.999f)] private float m_DoorFrameInsideScale;

    [SerializeField] private DoorData[] m_Doors;

    public DoorData DoorData { get { return m_DoorData; } set { m_DoorData = value; } }
    public DoorwayElement ActiveDoorElements => m_ActiveDoorwayElements;
    public float DoorPedimentHeight => m_DoorPedimentHeight;
    public float DoorSideWidth => m_DoorSideWidth;
    public float DoorSideOffset => m_DoorSideOffset;
    public int DoorColumns => m_DoorColumns;
    public int DoorRows => m_DoorRows;
    public float DoorFrameInsideScale { get { return m_DoorFrameInsideScale; } set { m_DoorFrameInsideScale = value; } }
    public float DoorFrameDepth { get { return m_DoorFrameDepth; } set { m_DoorFrameDepth = value; } }
    public DoorData[] Doors { get { return m_Doors; } set { m_Doors = value; } }
    #endregion

    #region Arch Properties
    // Archway
    [SerializeField, Range(0, 0.999f)] private float m_ArchPedimentHeight;
    [SerializeField, Range(0, 0.999f)] private float m_ArchSideWidth;
    [SerializeField, Range(-0.999f, 0.999f)] private float m_ArchSideOffset;
    [SerializeField, Range(1, 5)] private int m_ArchColumns, m_ArchRows;
    [SerializeField, Range(0, 1)] private float m_ArchHeight;
    [SerializeField, Range(3, 16)] private int m_ArchSides;

    // Arch Door
    [SerializeField] private DoorwayElement m_ActiveArchDoorElements;
    [SerializeField] private DoorData m_ArchDoorData;

    // Arch Door Frame
    [SerializeField, Range(0, 0.999f)] private float m_ArchDoorFrameDepth;
    [SerializeField, Range(0, 0.999f)] private float m_ArchDoorFrameInsideScale;

    [SerializeField] private DoorData[] m_ArchDoors;

    public float ArchHeight => m_ArchHeight;
    public int ArchSides => m_ArchSides;
    public DoorData ArchDoorData { get { return m_ArchDoorData; } set { m_ArchDoorData = value; } }
    public DoorwayElement ActiveArchDoorElements => m_ActiveArchDoorElements;
    public float ArchPedimentHeight => m_ArchPedimentHeight;
    public float ArchSideWidth => m_ArchSideWidth;
    public float ArchSideOffset => m_ArchSideOffset;
    public int ArchColumns => m_ArchColumns;
    public int ArchRows => m_ArchRows;
    public float ArchDoorFrameInsideScale { get { return m_ArchDoorFrameInsideScale; } set { m_ArchDoorFrameInsideScale = value; } }
    public float ArchDoorFrameDepth { get { return m_ArchDoorFrameDepth; } set { m_ArchDoorFrameDepth = value; } }
    public DoorData[] ArchDoors { get { return m_ArchDoors; } set { m_ArchDoors = value; } }
    #endregion

    #region Extension Properties

    [SerializeField] private StoreyData m_ExtensionStoreyData;
    [SerializeField] private RoofData m_ExtensionRoofData;

    [SerializeField, Range(1, 10)] private float m_ExtensionDistance;
    [SerializeField, Range(0, 1)] private float m_ExtensionHeight;
    [SerializeField, Range(0, 1)] private float m_ExtensionWidth;

    public StoreyData ExtensionStoreyData { get { return m_ExtensionStoreyData; } set { m_ExtensionStoreyData = value; } }
    public RoofData ExtensionRoofData { get { return m_ExtensionRoofData; } set { m_ExtensionRoofData = value; } }
    public float ExtensionDistance { get { return m_ExtensionDistance; } set { m_ExtensionDistance = value; } }
    public float ExtensionHeight { get { return m_ExtensionHeight; } set { m_ExtensionHeight = value; } }
    public float ExtensionWidth { get { return m_ExtensionWidth; } set { m_ExtensionWidth = value; } }
    #endregion

    public Vector2Int ID { get { return m_ID; } set { m_ID = value; } }
    public Vector3[] ControlPoints { get { return m_ControlPoints; } set { m_ControlPoints = value; } }
    public float WallDepth { get { return m_WallDepth; } set { m_WallDepth = value; } }
    public Vector3 FaceNormal { get { return m_FaceNormal; } set { m_FaceNormal = value; } }

    public WallSectionData() : this
    (
        new Vector3[0], 0.5f, new WindowData(), 0.5f, 0.5f,
        4, 1, 1, 0, false, 0.75f, 0.5f, 0, 1, 1, DoorwayElement.Everything, new DoorData(), 0.999f, 0.9f,
        1, 4, 0.75f, 0.5f, 0, 1, 1, DoorwayElement.Everything, new DoorData(), 0.999f, 0.9f, 1, 1, 0.5f, Vector3.zero,
        null, null, null, null
    )
    {
    }

    public WallSectionData(Vector3[] controlPoints, float wallDepth, WindowData windowData, float windowHeight, float windowWidth, int windowSides, int windowColumns, int windowRows, float windowAngle, bool windowSmooth,
        float doorPedimentHeight, float doorSideWidth, float doorSideOffset, int doorColumns, int doorRows, DoorwayElement activeDoorElements, DoorData doorData, float doorFrameDepth, float doorFrameInsideScale,
        float archHeight, int archSides, float archPedimentHeight, float archSideWidth, float archSideOffset, int archColumns, int archRows, DoorwayElement activeArchDoorElements, DoorData archDoorData, float archDoorFrameDepth, float archDoorFrameInsideScale,
        float extendDistance, float extendHeight, float extendWidth, Vector3 normal, WindowData[] windows, DoorData[] doors, StoreyData storeyExtension, RoofData roofExtension)
    {
        m_ControlPoints = controlPoints;
        m_WallDepth = wallDepth;

        #region Window
        m_WindowData = windowData;
        m_WindowHeight = windowHeight;
        m_WindowWidth = windowWidth;
        m_WindowSides = windowSides;
        m_WindowColumns = windowColumns;
        m_WindowRows = windowRows;
        m_WindowAngle = windowAngle;
        m_WindowSmooth = windowSmooth;
        #endregion

        #region Door
        m_DoorPedimentHeight = doorPedimentHeight;
        m_DoorSideWidth = doorSideWidth;
        m_DoorSideOffset = doorSideOffset;
        m_DoorColumns = doorColumns;
        m_DoorRows = doorRows;
        m_ActiveDoorwayElements = activeDoorElements;
        m_DoorData = doorData;
        m_DoorFrameDepth = doorFrameDepth;
        m_DoorFrameInsideScale = doorFrameInsideScale;
        #endregion
        m_ArchHeight = archHeight;
        m_ArchSides = archSides;

        #region Arch
        m_ArchPedimentHeight = archPedimentHeight;
        m_ArchSideWidth = archSideWidth;
        m_ArchSideOffset = archSideOffset;
        m_ArchColumns = archColumns;
        m_ArchRows = archRows;
        m_ActiveArchDoorElements = activeArchDoorElements;
        m_ArchDoorData = archDoorData;
        m_ArchDoorFrameDepth = archDoorFrameDepth;
        m_ArchDoorFrameInsideScale = archDoorFrameInsideScale;
        #endregion

        #region Extension
        m_ExtensionDistance = extendDistance;
        m_ExtensionHeight = extendHeight;
        m_ExtensionWidth = extendWidth;
        #endregion

        m_FaceNormal = normal;
        m_Windows = windows;
        m_Doors = doors;
    }

    public WallSectionData(WallSectionData data) : this
    (
        data.ControlPoints,
        data.WallDepth,
        data.WindowData,
        data.WindowHeight,
        data.WindowWidth,
        data.WindowSides,
        data.WindowColumns,
        data.WindowRows,
        data.WindowAngle,
        data.WindowSmooth,
        data.DoorPedimentHeight,
        data.DoorSideWidth,
        data.DoorSideOffset,
        data.DoorColumns,
        data.DoorRows,
        data.ActiveDoorElements,
        data.DoorData,
        data.DoorFrameDepth,
        data.DoorFrameInsideScale,
        data.ArchHeight,
        data.ArchSides,
        data.ArchPedimentHeight,
        data.ArchSideWidth,
        data.ArchSideOffset,
        data.ArchColumns,
        data.ArchRows,
        data.ActiveArchDoorElements,
        data.ArchDoorData,
        data.ArchDoorFrameDepth,
        data.ArchDoorFrameInsideScale,
        data.ExtensionDistance,
        data.ExtensionHeight,
        data.ExtensionWidth,
        data.FaceNormal,
        data.Windows,
        data.Doors,
        data.ExtensionStoreyData,
        data.ExtensionRoofData
    )
    {
        
    }
}
