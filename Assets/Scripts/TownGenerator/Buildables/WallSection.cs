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

public class WallSection : MonoBehaviour, IBuildable
{
    [SerializeField, HideInInspector] private ProBuilderMesh m_ProBuilderMesh;
    [SerializeReference] private WallSectionData m_Data;

    [SerializeField] WallElement m_PreviousElement;

    public WallSectionData Data => m_Data;

    public IBuildable Initialize(IData wallSectionData)
    {
        m_Data = wallSectionData as WallSectionData;
        m_ProBuilderMesh = GetComponent<ProBuilderMesh>();
        return this;
    }

    public void Build()
    {
        if (m_PreviousElement != m_Data.WallElement)
            Demolish();

        Debug.Log("Build: ", this);

        m_PreviousElement = m_Data.WallElement;

        // TODO: change to use opening data

        switch(m_Data.WallElement)
        {
            case WallElement.Wall:
                Rebuild(MeshMaker.Cube(m_Data.ControlPoints, m_Data.WallDepth));
                break;
            case WallElement.Doorway:
                {
                    DoorwayData doorway = m_Data.Doorway;

                    int size = doorway.Columns * doorway.Rows;

                    if(doorway.Doors == null || doorway.Doors.Length == 0 || doorway.Doors.Length != size)
                    {
                        doorway.Doors = new DoorData[size];
                    }

                    IList<IList<Vector3>> holePoints;

                    if (doorway.Doors[0] == null || doorway.Doors[0].ControlPoints == null || doorway.Doors[0].ControlPoints.Length == 0)
                    {
                        holePoints = CalculateDoorway(m_Data);

                        for (int i = 0; i < size; i++)
                        {
                            if (doorway.Doors[i] == null || doorway.Doors[i].ControlPoints == null || doorway.Doors[i].ControlPoints.Length == 0)
                            {
                                doorway.Doors[i] = CalculateDoor(i, holePoints[i]);

                                if (doorway.ActiveElements.IsElementActive(DoorwayElement.Door))
                                    BuildDoor(doorway.Doors[i]);

                                if (!doorway.ActiveElements.IsElementActive(DoorwayElement.Frame))
                                    continue;

                                BuildFrame(doorway.Doors[i].ControlPoints, doorway.FrameScale, doorway.FrameDepth);

                            }
                        }
                    }
                    else
                    {
                        holePoints = new List<IList<Vector3>>();
                        foreach (DoorData door in doorway.Doors)
                        {
                            holePoints.Add(door.ControlPoints.ToList());

                            if (doorway.ActiveElements.IsElementActive(DoorwayElement.Door))
                                BuildDoor(door);

                            if (!doorway.ActiveElements.IsElementActive(DoorwayElement.Frame))
                                continue;

                            BuildFrame(door.ControlPoints, doorway.FrameScale, doorway.FrameDepth);
                        }
                    }

                    BuildSection(holePoints);
                }
                break;
            case WallElement.Archway:
                {
                    ArchwayData archway = m_Data.Archway;

                    int size = archway.Columns * archway.Rows;

                    if (archway.Doors == null || archway.Doors.Length == 0 || archway.Doors.Length != size)
                    {
                        archway.Doors = new DoorData[size];
                    }

                    IList<IList<Vector3>> holePoints;

                    if (archway.Doors[0] == null || archway.Doors[0].ControlPoints == null || archway.Doors[0].ControlPoints.Length == 0)
                    {
                        holePoints = CalculateArchway(m_Data);

                        for (int i = 0; i < size; i++)
                        {
                            if (archway.Doors[i] == null || archway.Doors[i].ControlPoints == null || archway.Doors[i].ControlPoints.Length == 0)
                            {
                                archway.Doors[i] = CalculateDoor(i, holePoints[i]);

                                if (archway.ActiveElements.IsElementActive(DoorwayElement.Door))
                                    BuildDoor(archway.Doors[i]);

                                if (!archway.ActiveElements.IsElementActive(DoorwayElement.Frame))
                                    continue;

                                BuildFrame(archway.Doors[i].ControlPoints, archway.FrameScale, archway.FrameDepth);
                            }
                        }
                    }
                    else
                    {
                        holePoints = new List<IList<Vector3>>();
                        foreach (DoorData door in archway.Doors)
                        {
                            holePoints.Add(door.ControlPoints.ToList());

                            if(archway.ActiveElements.IsElementActive(DoorwayElement.Door))
                                BuildDoor(door);

                            if (!archway.ActiveElements.IsElementActive(DoorwayElement.Frame))
                                continue;

                            BuildFrame(door.ControlPoints, archway.FrameScale, archway.FrameDepth);

                        }
                    }

                    BuildSection(holePoints);
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

                    if (windowOpening.Windows[0] == null || windowOpening.Windows[0].ControlPoints == null || windowOpening.Windows[0].ControlPoints.Length == 0)
                    {
                        holePoints = CalculateWindow(m_Data);

                        for (int i = 0; i < size; i++)
                        {
                            if(windowOpening.Windows[i] == null || windowOpening.Windows[i].ControlPoints == null || windowOpening.Windows[i].ControlPoints.Length == 0)
                            {
                                windowOpening.Windows[i] = CalculateWindow(i, holePoints[i]);
                                BuildWindow(windowOpening.Windows[i]);
                            }
                        }
                    }
                    else
                    {
                        holePoints = new List<IList<Vector3>>();

                        foreach (WindowData window in windowOpening.Windows)
                        {
                            holePoints.Add(window.ControlPoints.ToList());

                            BuildWindow(window);
                        }
                    }

                    BuildSection(holePoints);
                }
                break;
            case WallElement.Extension:
                {
                    IList<IList<Vector3>> extensionHole = CalculateExtension(m_Data);

                    BuildSection(extensionHole);

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
    }

    #region Calculate
    private WindowData CalculateWindow(int index, IEnumerable<Vector3> controlPoints)
    {
        WindowData data = new WindowData(m_Data.WindowData)
        {
            ID = index,
            ControlPoints = controlPoints.ToArray()
        };

        return data;
    }
    private DoorData CalculateDoor(int index, IEnumerable<Vector3> controlPoints)
    {
        DoorData data = new DoorData(m_Data.DoorData)
        {
            ID = index,
            ControlPoints = controlPoints.ToArray(),
            ActiveElements = m_Data.Doorway.ActiveElements.ToDoorElement() // Does this need changing?
        };

        return data;
    }
    public static IList<IList<Vector3>> CalculateDoorway(WallSectionData data)
    {
        DoorwayData doorway = data.Doorway;
        Vector3 doorScale = new Vector3(doorway.Width, doorway.Height);
        return MeshMaker.NPolyHoleGrid(data.ControlPoints, doorScale, doorway.Columns, doorway.Rows, 4, 0, Vector3.right * doorway.PositionOffset, new Vector3(0, -0.999f));
    }
    /// <summary>
    /// Calculates the points for the window hole(s)
    /// </summary>
    /// <returns></returns>
    public static IList<IList<Vector3>> CalculateWindow(WallSectionData data)
    {
        WindowOpeningData windowOpening = data.WindowOpening;
        Vector3 winScale = new Vector3(windowOpening.Width, windowOpening.Height);
        return MeshMaker.NPolyHoleGrid(data.ControlPoints, winScale, windowOpening.Columns, windowOpening.Rows, windowOpening.Sides, windowOpening.Angle);
    }
    public static IList<IList<Vector3>> CalculateArchway(WallSectionData data)
    {
        ArchwayData archway = data.Archway;
        return MeshMaker.ArchedDoorHoleGrid(data.ControlPoints, archway.Width, archway.Columns, archway.Rows, archway.Height, archway.ArchHeight, archway.ArchSides, Vector3.right * archway.PositionOffset);
    }
    public static IList<IList<Vector3>> CalculateExtension(WallSectionData data)
    {
        ExtensionData extension = data.Extension;
        Vector3 scale = new Vector3(extension.Width, extension.Height);
        return MeshMaker.NPolyHoleGrid(data.ControlPoints, scale, 1, 1, 4, 0, null, new Vector3(0, -0.999f)); // Outside
    }
    #endregion

    #region Build
    public void BuildSection(IList<IList<Vector3>> holePoints)
    {
        m_ProBuilderMesh.CreateShapeFromPolygon(m_Data.ControlPoints, m_Data.FaceNormal, holePoints);
        m_ProBuilderMesh.Solidify(m_Data.WallDepth);
    }
    private void BuildWindow(WindowData data)
    {
        ProBuilderMesh windowMesh = ProBuilderMesh.Create();
        windowMesh.name = "Window " + data.ID.ToString();
        windowMesh.transform.SetParent(transform, true);
        Window window = windowMesh.AddComponent<Window>();
        window.Initialize(data).Build();
    }
    private void BuildDoor(DoorData data)
    {
        ProBuilderMesh doorMesh = ProBuilderMesh.Create();
        doorMesh.name = "Door " + data.ID.ToString();
        doorMesh.transform.SetParent(transform, false);
        Door door = doorMesh.AddComponent<Door>();
        door.Initialize(data).Build();
    }
    private void BuildFrame(IList<Vector3> controlPoints, float insideScale, float depth)
    {
        Vector3[] scaledControlPoints = controlPoints.ScalePolygon(insideScale);
        List<IList<Vector3>> holePoints = new();
        holePoints.Add(scaledControlPoints);

        ProBuilderMesh doorFrameMesh = ProBuilderMesh.Create();
        doorFrameMesh.name = "Frame";
        doorFrameMesh.transform.SetParent(transform, false);
        doorFrameMesh.CreateShapeFromPolygon(controlPoints, m_Data.FaceNormal, holePoints);
        doorFrameMesh.Extrude(doorFrameMesh.faces, ExtrudeMethod.FaceNormal, depth);
        doorFrameMesh.ToMesh();
        doorFrameMesh.Refresh();
    }
    #endregion

    public void Demolish()
    {
        transform.DeleteChildren();
    }

    private WallSection Rebuild(ProBuilderMesh mesh)
    {
        m_ProBuilderMesh.RebuildWithPositionsAndFaces(mesh.positions, mesh.faces);
        m_ProBuilderMesh.ToMesh();
        m_ProBuilderMesh.Refresh();
        //m_ProBuilderMesh.CenterPivot(m_ProBuilderMesh.GetAllDistinctIndices().ToArray());

        DestroyImmediate(mesh.gameObject);
        return this;
    }
}
