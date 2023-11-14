using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WallSectionData : IData
{

    [SerializeField, HideInInspector] private Vector3[] m_ControlPoints;
    [SerializeField, HideInInspector] private float m_WallDepth;

    #region Window Properties
    [SerializeField] private WindowData m_WindowData;
    [SerializeField, Range(0, 0.999f)] private float m_WindowHeight;
    [SerializeField, Range(0, 0.999f)] private float m_WindowWidth;
    [SerializeField, Range(3, 16)] private int m_WindowSides = 3;
    [SerializeField, Range(1, 5)] private int m_WindowColumns, m_WindowRows;
    [SerializeField, Range(-180, 180)] private float m_WindowAngle;
    [SerializeField] private bool m_WindowSmooth;

    public WindowData WindowData { get { return m_WindowData; } set { m_WindowData = value; } }
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
    [SerializeField, Range(0, 0.999f)] private float m_PedimentHeight;
    [SerializeField, Range(0, 0.999f)] private float m_SideWidth;
    [SerializeField, Range(-0.999f, 0.999f)] private float m_SideOffset;
    [SerializeField, Range(1, 10)] private int m_DoorColumns, m_DoorRows;
    [SerializeField] private float m_ArchHeight;
    [SerializeField] private int m_ArchSides;
    // Door
    [SerializeField] private DoorElement m_ActiveDoorElements;
    [SerializeField] private DoorData m_DoorData;

    // Door Frame
    [SerializeField, Range(0, 0.999f)] private float m_DoorFrameDepth;
    [SerializeField, Range(0, 0.999f)] private float m_DoorFrameInsideScale;

    public DoorData DoorData { get { return m_DoorData; } set { m_DoorData = value; } }
    public DoorElement ActiveDoorElements => m_ActiveDoorElements;
    public float PedimentHeight => m_PedimentHeight;
    public float SideWidth => m_SideWidth;
    public float SideOffset => m_SideOffset;
    public int DoorColumns => m_DoorColumns;
    public int DoorRows => m_DoorRows;
    public float ArchHeight => m_ArchHeight;
    public int ArchSides => m_ArchSides;
    public float DoorFrameInsideScale { get { return m_DoorFrameInsideScale; } set { m_DoorFrameInsideScale = value; } }
    public float DoorFrameDepth { get { return m_DoorFrameDepth; } set { m_DoorFrameDepth = value; } }
    #endregion

    #region Extension Properties
    [SerializeField, Range(1, 10)] private float m_ExtendDistance;
    [SerializeField, Range(0, 1)] private float m_ExtendHeight;
    [SerializeField, Range(0, 1)] private float m_ExtendWidth;

    public float ExtendDistance => m_ExtendDistance;
    public float ExtendHeight => m_ExtendHeight;
    public float ExtendWidth => m_ExtendWidth;
    #endregion


    public Vector3[] ControlPoints { get { return m_ControlPoints; } set { m_ControlPoints = value; } }
    public float WallDepth { get { return m_WallDepth; } set { m_WallDepth = value; } }

    public WallSectionData()
    {
        m_ControlPoints = new Vector3[0];
        m_WallDepth = 0.5f;

        m_WindowData = new WindowData();
        m_WindowHeight = 0.5f;
        m_WindowWidth = 0.5f;
        m_WindowColumns = 1;
        m_WindowRows = 1;
        m_WindowAngle = 0;

        m_ExtendDistance = 2.5f;
        m_ExtendHeight = 0.75f;
        m_ExtendWidth = 0.75f;

        m_DoorData = new DoorData();
        m_ActiveDoorElements = DoorElement.Everything;
        m_PedimentHeight = 0.75f;
        m_SideWidth = 0.5f;
        m_DoorColumns = 1;
        m_DoorRows = 1;
        m_ArchSides = 3;
        m_ArchHeight = 1;
    }

    public WallSectionData(Vector3[] controlPoints, float wallDepth, WindowData windowData, float windowHeight, float windowWidth, int windowSides, int windowColumns, int windowRows, float windowAngle, bool windowSmooth, float pedimentHeight, float sideWidth, float sideOffset, int doorColumns, int doorRows, float archHeight, int archSides, DoorElement activeElements, DoorData doorData, float doorFrameDepth, float doorFrameInsideScale, float extendDistance, float extendHeight, float extendWidth)
    {
        m_ControlPoints = controlPoints;
        m_WallDepth = wallDepth;
        m_WindowData = windowData;
        m_WindowHeight = windowHeight;
        m_WindowWidth = windowWidth;
        m_WindowSides = windowSides;
        m_WindowColumns = windowColumns;
        m_WindowRows = windowRows;
        m_WindowAngle = windowAngle;
        m_WindowSmooth = windowSmooth;
        m_PedimentHeight = pedimentHeight;
        m_SideWidth = sideWidth;
        m_SideOffset = sideOffset;
        m_DoorColumns = doorColumns;
        m_DoorRows = doorRows;
        m_ArchHeight = archHeight;
        m_ArchSides = archSides;
        m_ActiveDoorElements = activeElements;
        m_DoorData = doorData;
        m_DoorFrameDepth = doorFrameDepth;
        m_DoorFrameInsideScale = doorFrameInsideScale;
        m_ExtendDistance = extendDistance;
        m_ExtendHeight = extendHeight;
        m_ExtendWidth = extendWidth;
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
        data.PedimentHeight,
        data.SideWidth,
        data.SideOffset,
        data.DoorColumns,
        data.DoorRows,
        data.ArchHeight,
        data.ArchSides,
        data.ActiveDoorElements,
        data.DoorData,
        data.DoorFrameDepth,
        data.DoorFrameInsideScale,
        data.ExtendDistance,
        data.ExtendHeight,
        data.ExtendWidth
    )
    {
        
    }
}
