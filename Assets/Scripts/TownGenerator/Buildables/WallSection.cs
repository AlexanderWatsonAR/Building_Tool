using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEditor;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using System.Linq;
using UnityEngine.ProBuilder.Shapes;
using ProMaths = UnityEngine.ProBuilder.Math;

public class WallSection : Polygon3D
{
    [SerializeReference] WallSectionData m_Data;

    [SerializeField] WallElement m_PreviousElement;
    [SerializeField] List<Window> m_Windows;
    [SerializeField] List<Door> m_Doors;
    [SerializeField] List<Frame> m_Frames;

    public List<Window> Windows => m_Windows;

    public WallSectionData Data => m_Data;

    public override IBuildable Initialize(IData wallSectionData)
    {
        m_Data = wallSectionData as WallSectionData;
        m_Windows ??= new List<Window>();
        base.Initialize(wallSectionData);
        return this;
    }

    public void BuildChildren()
    {
        Debug.Log("Build Children");

        switch(m_Data.WallElement)
        {
            case WallElement.Wall:
                return;
            case WallElement.Window:
                BuildWindows();
                break;
            case WallElement.Doorway:
                m_Doors.BuildCollection();
                m_Frames.BuildCollection();
                break;
            case WallElement.Archway:
                m_Doors.BuildCollection();
                m_Frames.BuildCollection();
                break;
        }
    }

    public void BuildWindows(bool rebuildOuterFrame = false, bool rebuildInnerFrame = false, bool rebuildPane = false, bool rebuildShutters = false)
    {
        if (m_Windows == null)
            return;

        foreach(Window window in m_Windows)
        {
            window.Data.DoesOuterFrameNeedRebuild = rebuildOuterFrame;
            window.Data.DoesInnerFrameNeedRebuild = rebuildInnerFrame;
            window.Data.DoesPaneNeedRebuild = rebuildPane;
            window.Data.DoShuttersNeedRebuild = rebuildShutters;
            window.Build();
        }
    }

    ///// <summary>
    /////  hmmm this isn't decoupling data from buildables.
    ///// </summary>
    ///// <param name="outerFrame"></param>
    ///// <param name="innerFrame"></param>
    ///// <param name="pane"></param>
    ///// <param name="leftShutter"></param>
    ///// <param name="rightShutter"></param>
    //public void BuildWindows(FrameData outerFrame = null, GridFrameData innerFrame = null, Polygon3DData pane = null, DoorData leftShutter = null, DoorData rightShutter = null)
    //{

    //    foreach(Window window in m_Windows)
    //    {
            
    //    }

    //}

    /// <summary>
    /// Build should only be called if changes have been made to the wall section
    /// </summary>
    public override void Build()
    {
        if (m_PreviousElement != m_Data.WallElement)
            Demolish();

        Debug.Log("Build: ", this);

        m_PreviousElement = m_Data.WallElement;

        switch(m_Data.WallElement)
        {
            case WallElement.Wall:
                m_Data.Holes = null;
                base.Build();
                break;
            case WallElement.Doorway:
                {
                    DoorwayData doorway = m_Data.Doorway;

                    int size = doorway.Columns * doorway.Rows;

                    if(doorway.Doors == null || doorway.Doors.Length == 0 || doorway.Doors.Length != size)
                    {
                        doorway.Doors = new DoorData[size];
                        doorway.Frames = new FrameData[size];
                    }

                    IList<IList<Vector3>> holePoints;

                    if (doorway.Doors[0] == null || doorway.Doors[0].Polygon.ControlPoints == null || doorway.Doors[0].Polygon.ControlPoints.Length == 0)
                    {
                        holePoints = CalculateDoorway(m_Data);
                        

                        for (int i = 0; i < size; i++)
                        {
                            if (doorway.Doors[i] == null || doorway.Doors[i].Polygon.ControlPoints == null || doorway.Doors[i].Polygon.ControlPoints.Length == 0)
                            {
                                doorway.Doors[i] = CalculateDoor(i, holePoints[i]);
                                doorway.Frames[i] = CalculateFrame(holePoints[i], m_Data.DoorFrame.Scale, m_Data.DoorFrame.Depth);

                                if (doorway.ActiveElements.IsElementActive(DoorwayElement.Door))
                                {
                                    Door door = CreateDoor(doorway.Doors[i]);
                                    m_Doors.Add(door);
                                }

                                if (!doorway.ActiveElements.IsElementActive(DoorwayElement.Frame))
                                    continue;

                                Frame frame = CreateFrame(doorway.Frames[i]);
                                m_Frames.Add(frame);

                            }
                        }
                    }
                    else
                    {
                        holePoints = new List<IList<Vector3>>();

                        for (int i = 0; i < doorway.Doors.Length; i++)
                        {
                            DoorData data = doorway.Doors[i];

                            holePoints.Add(data.Polygon.ControlPoints.ToList());

                            if (doorway.ActiveElements.IsElementActive(DoorwayElement.Door))
                            {
                                Door door = CreateDoor(data);
                                m_Doors.Add(door);
                            }   

                            if (!doorway.ActiveElements.IsElementActive(DoorwayElement.Frame))
                                continue;

                            Frame frame = CreateFrame(doorway.Frames[i]);
                            m_Frames.Add(frame);
                        }
                    }

                    m_Data.SetHoles(holePoints);
                    base.Build();
                }
                break;
            case WallElement.Archway:
                {
                    ArchwayData archway = m_Data.Archway;

                    int size = archway.Columns * archway.Rows;

                    if (archway.Doors == null || archway.Doors.Length == 0 || archway.Doors.Length != size)
                    {
                        archway.Doors = new DoorData[size];
                        archway.Frames = new FrameData[size];
                    }

                    IList<IList<Vector3>> holePoints;

                    if (archway.Doors[0] == null || archway.Doors[0].Polygon.ControlPoints == null || archway.Doors[0].Polygon.ControlPoints.Length == 0)
                    {
                        holePoints = CalculateArchway(m_Data);

                        for (int i = 0; i < size; i++)
                        {
                            if (archway.Doors[i] == null || archway.Doors[i].Polygon.ControlPoints == null || archway.Doors[i].Polygon.ControlPoints.Length == 0)
                            {
                                archway.Doors[i] = CalculateDoor(i, holePoints[i]);
                                archway.Frames[i] = CalculateFrame(holePoints[i], m_Data.DoorFrame.Scale, m_Data.DoorFrame.Depth);

                                if (archway.ActiveElements.IsElementActive(DoorwayElement.Door))
                                {
                                    Door door = CreateDoor(archway.Doors[i]);
                                    m_Doors.Add(door);
                                } 

                                if (!archway.ActiveElements.IsElementActive(DoorwayElement.Frame))
                                    continue;

                                Frame frame = CreateFrame(archway.Frames[i]);
                                m_Frames.Add(frame);
                            }
                        }
                    }
                    else
                    {
                        holePoints = new List<IList<Vector3>>();
                        for (int i = 0; i < archway.Doors.Length; i++)
                        {
                            DoorData data = archway.Doors[i];

                            holePoints.Add(data.Polygon.ControlPoints.ToList());

                            if(archway.ActiveElements.IsElementActive(DoorwayElement.Door))
                            {
                                Door door = CreateDoor(data);
                                m_Doors.Add(door);
                            }

                            if (!archway.ActiveElements.IsElementActive(DoorwayElement.Frame))
                                continue;

                            Frame frame = CreateFrame(archway.Frames[i]);
                            m_Frames.Add(frame);

                        }
                    }

                    m_Data.SetHoles(holePoints);
                    base.Build();
                }
                break;
            case WallElement.Window:
                {
                    WindowOpeningData windowOpening = m_Data.WindowOpening;

                    int size = windowOpening.Columns * windowOpening.Rows;

                    if(windowOpening.Windows == null || windowOpening.Windows.Length == 0 || windowOpening.Windows.Length != size)
                    {
                        windowOpening.Windows = new WindowData[size];
                    }

                    IList<IList<Vector3>> holePoints;

                    // TODO: Stick this if statement into a function to make it more readable.

                    if (windowOpening.Windows[0] == null || windowOpening.Windows[0].Polygon.ControlPoints == null || windowOpening.Windows[0].Polygon.ControlPoints.Length == 0)
                    {
                        holePoints = CalculateWindowOpening(m_Data);

                        for (int i = 0; i < size; i++)
                        {
                            if(windowOpening.Windows[i] == null || windowOpening.Windows[i].Polygon.ControlPoints == null || windowOpening.Windows[i].Polygon.ControlPoints.Length == 0)
                            {
                                windowOpening.Windows[i] = CalculateWindow(i, holePoints[i]);
                                Window win = CreateWindow(windowOpening.Windows[i]);
                                m_Windows.Add(win);
                            }
                        }
                    }
                    else
                    {
                        holePoints = new List<IList<Vector3>>();
                        foreach (WindowData window in windowOpening.Windows)
                        {
                            holePoints.Add(window.Polygon.ControlPoints.ToList());

                            Window win = CreateWindow(window);
                            m_Windows.Add(win);
                        }
                    }

                    m_Data.SetHoles(holePoints);
                    base.Build();
                    
                }
                break;
            case WallElement.Extension:
                {
                    IList<IList<Vector3>> extensionHole = CalculateExtension(m_Data);

                    m_Data.SetHoles(extensionHole);
                    base.Build();

                    //ControlPoint[] points = new ControlPoint[4];
                    //points[0] = new ControlPoint(extensionHole[0][0]);
                    //points[1] = new ControlPoint(extensionHole[0][0] + m_Data.FaceNormal * m_Data.ExtensionDistance);
                    //points[2] = new ControlPoint(extensionHole[0][3] + m_Data.FaceNormal * m_Data.ExtensionDistance);
                    //points[3] = new ControlPoint(extensionHole[0][3]);

                    //for (int i = 0; i < points.Length; i++)
                    //{
                    //    int next = points.GetNext(i);
                    //    int previous = points.GetPrevious(i);

                    //    Vector3 nextForward = Vector3Extensions.DirectionToTarget(points[i].Position, points[next].Position);
                    //    Vector3 previousForward = Vector3Extensions.DirectionToTarget(points[i].Position, points[previous].Position);

                    //    points[i].SetForward(Vector3.Lerp(nextForward, previousForward, 0.5f));
                    //}

                    //float wallHeight = Vector3.Distance(extensionHole[0][0], extensionHole[0][1]);

                    //m_Data.ExtensionStoreyData ??= new StoreyData();
                    //m_Data.ExtensionRoofData ??= new RoofData();

                    //m_Data.ExtensionStoreyData.ControlPoints = points;
                    //m_Data.ExtensionStoreyData.WallData.Height = wallHeight;
                    //m_Data.ExtensionRoofData.ControlPoints = points;

                    //GameObject storeyGO = new GameObject("Storey", typeof(Storey));
                    //storeyGO.transform.SetParent(transform, true);
                    //storeyGO.GetComponent<Storey>().Initialize(m_Data.ExtensionStoreyData).Build();

                    //GameObject roofGO = new GameObject("Roof", typeof(Roof));
                    //roofGO.transform.SetParent(transform, true);
                    //roofGO.transform.localPosition = new Vector3(0, wallHeight, 0);
                    //Roof roofExtension = roofGO.GetComponent<Roof>();
                    //roofExtension.Initialize(m_Data.ExtensionRoofData).Build();
                }
                break;
            case WallElement.Empty:
                m_ProBuilderMesh.Clear();
                m_ProBuilderMesh.ToMesh();
                m_ProBuilderMesh.Refresh();
                break;      
        }

        BuildChildren();
    }

    #region Calculate
    private WindowData CalculateWindow(int index, IEnumerable<Vector3> controlPoints)
    {
        WindowData data = new WindowData(m_Data.Window)
        {
            ID = index,
            Polygon = new PolygonData(controlPoints.ToArray(), m_Data.Normal)
        };
        return data;
    }
    private DoorData CalculateDoor(int index, IEnumerable<Vector3> controlPoints)
    {
        DoorData data = new DoorData(m_Data.Door)
        {
            ID = index,
            Polygon = new PolygonData(controlPoints.ToArray(), m_Data.Normal),
            ActiveElements = m_Data.Doorway.ActiveElements.ToDoorElement() // Does this need changing?
        };
        return data;
    }
    private FrameData CalculateFrame(IEnumerable<Vector3> controlPoints, float insideScale, float depth)
    {
        FrameData frameData = new FrameData()
        {
            Polygon = new PolygonData(controlPoints.ToArray(), m_Data.Normal),
            Scale = insideScale,
            Depth = depth
        };
        return frameData;
    }
    private IList<IList<Vector3>> CalculateDoorway(WallSectionData data)
    {
        DoorwayData doorway = data.Doorway;
        Vector3 doorScale = new Vector3(doorway.Width, doorway.Height);
        return MeshMaker.NPolyHoleGrid(data.Polygon.ControlPoints, doorScale, doorway.Columns, doorway.Rows, 4, 0, Vector3.right * doorway.PositionOffset, new Vector3(0, -0.999f));
    }
    private IList<IList<Vector3>> CalculateWindowOpening(WallSectionData data)
    {
        WindowOpeningData windowOpening = data.WindowOpening;
        Vector3 winScale = new Vector3(windowOpening.Width, windowOpening.Height);
        return MeshMaker.NPolyHoleGrid(data.Polygon.ControlPoints, winScale, windowOpening.Columns, windowOpening.Rows, windowOpening.Sides, windowOpening.Angle);
    }
    private IList<IList<Vector3>> CalculateArchway(WallSectionData data)
    {
        ArchwayData archway = data.Archway;
        return MeshMaker.ArchedDoorHoleGrid(data.Polygon.ControlPoints, archway.Width, archway.Columns, archway.Rows, archway.Height, archway.ArchHeight, archway.ArchSides, Vector3.right * archway.PositionOffset);
    }
    private IList<IList<Vector3>> CalculateExtension(WallSectionData data)
    {
        ExtensionData extension = data.Extension;
        Vector3 scale = new Vector3(extension.Width, extension.Height);
        return MeshMaker.NPolyHoleGrid(data.Polygon.ControlPoints, scale, 1, 1, 4, 0, null, new Vector3(0, -0.999f)); // Outside
    }
    #endregion

    #region Create
    private Window CreateWindow(WindowData data)
    {
        ProBuilderMesh windowMesh = ProBuilderMesh.Create();
        windowMesh.name = "Window " + data.ID.ToString();
        windowMesh.transform.SetParent(transform, true);
        Window window = windowMesh.AddComponent<Window>();
        window.Initialize(data);
        return window;
    }
    private Door CreateDoor(DoorData data)
    {
        ProBuilderMesh doorMesh = ProBuilderMesh.Create();
        doorMesh.name = "Door " + data.ID.ToString();
        doorMesh.transform.SetParent(transform, false);
        Door door = doorMesh.AddComponent<Door>();
        door.Initialize(data);
        return door;
    }
    private Frame CreateFrame(FrameData data)
    {
        ProBuilderMesh frameMesh = ProBuilderMesh.Create();
        frameMesh.transform.SetParent(transform, false);
        Frame doorFrame = frameMesh.AddComponent<Frame>();
        doorFrame.Initialize(data);
        return doorFrame;
    }
    #endregion

    public override void Demolish()
    {
        transform.DeleteChildren();
        m_Doors?.Clear();
        m_Windows?.Clear();
        m_Frames?.Clear();
    }

}
