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
using System.Threading.Tasks;

public class WallSection : MonoBehaviour, IBuildable
{
    [SerializeField, HideInInspector] private ProBuilderMesh m_ProBuilderMesh;
    [SerializeField] private WallSectionData m_Data;

    public WallSectionData Data => m_Data;

    public event Action<WallSectionData> OnDataChange;

    //// Temp variables for testing
    //List<IList<Vector3>> m_Points;
    //List<Vector3> m_Grid;
    //Vector3 m_Centre;
    //// end of temp

    public void OnDataChange_Invoke()
    {
        OnDataChange?.Invoke(m_Data);
    }

    public IBuildable Initialize(IData wallSectionData)
    {
        m_Data = wallSectionData as WallSectionData;
        m_ProBuilderMesh = GetComponent<ProBuilderMesh>();
        return this;
    }

    public void Build()
    {
        transform.DeleteChildren();

        switch(m_Data.WallElement)
        {
            case WallElement.Wall:
                Rebuild(MeshMaker.Cube(m_Data.ControlPoints, m_Data.WallDepth));
                break;
            case WallElement.Doorway:
                {
                    int size = m_Data.DoorColumns * m_Data.DoorRows;

                    if(m_Data.Doors == null || m_Data.Doors.Length == 0)
                    {
                        m_Data.Doors = new DoorData[size];
                    }

                    IList<IList<Vector3>> holePoints;

                    if (m_Data.Doors[0] == null || m_Data.Doors[0].ControlPoints == null || m_Data.Doors[0].ControlPoints.Length == 0)
                    {
                        Vector3 doorScale = new Vector3(m_Data.DoorSideWidth, m_Data.DoorPedimentHeight);
                        holePoints = MeshMaker.NPolyHoleGrid(m_Data.ControlPoints, doorScale, m_Data.DoorColumns, m_Data.DoorRows, 4, 0, Vector3.right * m_Data.DoorSideOffset, new Vector3(0, -0.999f));

                        for (int i = 0; i < size; i++)
                        {
                            if (m_Data.Doors[i] == null || m_Data.Doors[i].ControlPoints == null || m_Data.Doors[i].ControlPoints.Length == 0)
                            {
                                m_Data.Doors[i] = CalculateDoor(i, holePoints[i]);

                                if (m_Data.ActiveDoorElements.IsElementActive(DoorwayElement.Door))
                                    BuildDoor(m_Data.Doors[i]);

                                if (!m_Data.ActiveDoorElements.IsElementActive(DoorwayElement.Frame))
                                    continue;

                                BuildFrame(m_Data.Doors[i].ControlPoints, m_Data.DoorFrameInsideScale, m_Data.DoorFrameDepth);

                            }
                        }
                    }
                    else
                    {
                        holePoints = new List<IList<Vector3>>();
                        foreach (DoorData data in m_Data.Doors)
                        {
                            holePoints.Add(data.ControlPoints.ToList());

                            if (m_Data.ActiveArchDoorElements.IsElementActive(DoorwayElement.Door))
                                BuildDoor(data);

                            if (!m_Data.ActiveDoorElements.IsElementActive(DoorwayElement.Frame))
                                continue;

                            BuildFrame(data.ControlPoints, m_Data.ArchDoorFrameInsideScale, m_Data.ArchDoorFrameDepth);
                        }
                    }

                    BuildSection(holePoints);
                }
                break;
            case WallElement.Archway:
                {
                    int size = m_Data.ArchColumns * m_Data.ArchRows;

                    if (m_Data.ArchDoors == null || m_Data.ArchDoors.Length == 0)
                    {
                        m_Data.ArchDoors = new DoorData[size];
                    }

                    IList<IList<Vector3>> holePoints;

                    if (m_Data.ArchDoors[0] == null || m_Data.ArchDoors[0].ControlPoints == null || m_Data.ArchDoors[0].ControlPoints.Length == 0)
                    {
                        holePoints = MeshMaker.ArchedDoorHoleGrid(m_Data.ControlPoints, m_Data.ArchSideWidth, m_Data.ArchColumns, m_Data.ArchRows, m_Data.ArchPedimentHeight, m_Data.ArchHeight, m_Data.ArchSides, Vector3.right * m_Data.ArchSideOffset);

                        for (int i = 0; i < size; i++)
                        {
                            if (m_Data.ArchDoors[i] == null || m_Data.ArchDoors[i].ControlPoints == null || m_Data.ArchDoors[i].ControlPoints.Length == 0)
                            {
                                m_Data.ArchDoors[i] = CalculateDoor(i, holePoints[i]);

                                if (m_Data.ActiveArchDoorElements.IsElementActive(DoorwayElement.Door))
                                    BuildDoor(m_Data.ArchDoors[i]);

                                if (!m_Data.ActiveDoorElements.IsElementActive(DoorwayElement.Frame))
                                    continue;

                                BuildFrame(m_Data.ArchDoors[i].ControlPoints, m_Data.ArchDoorFrameInsideScale, m_Data.ArchDoorFrameDepth);
                            }
                        }
                    }
                    else
                    {
                        holePoints = new List<IList<Vector3>>();
                        foreach (DoorData data in m_Data.ArchDoors)
                        {
                            holePoints.Add(data.ControlPoints.ToList());

                            if(m_Data.ActiveArchDoorElements.IsElementActive(DoorwayElement.Door))
                                BuildDoor(data);

                            if (!m_Data.ActiveDoorElements.IsElementActive(DoorwayElement.Frame))
                                continue;

                            BuildFrame(data.ControlPoints, m_Data.ArchDoorFrameInsideScale, m_Data.ArchDoorFrameDepth);

                        }
                    }

                    BuildSection(holePoints);
                }
                break;
            case WallElement.Window:
                {
                    int size = m_Data.WindowColumns * m_Data.WindowRows;

                    if(m_Data.Windows == null || m_Data.Windows.Length == 0)
                    {
                        m_Data.Windows = new WindowData[size];
                    }

                    IList<IList<Vector3>> holePoints;

                    if (m_Data.Windows[0] == null || m_Data.Windows[0].ControlPoints == null || m_Data.Windows[0].ControlPoints.Length == 0)
                    {
                        Vector3 winScale = new Vector3(m_Data.WindowWidth, m_Data.WindowHeight);
                        holePoints = MeshMaker.NPolyHoleGrid(m_Data.ControlPoints, winScale, m_Data.WindowColumns, m_Data.WindowRows, m_Data.WindowSides, m_Data.WindowAngle);

                        for (int i = 0; i < size; i++)
                        {
                            if(m_Data.Windows[i] == null || m_Data.Windows[i].ControlPoints == null || m_Data.Windows[i].ControlPoints.Length == 0)
                            {
                                m_Data.Windows[i] = CalculateWindow(i, holePoints[i]);
                                BuildWindow(m_Data.Windows[i]);
                            }
                        }
                    }
                    else
                    {
                        holePoints = new List<IList<Vector3>>();

                        //Parallel.ForEach(m_Data.Windows, data =>
                        //{
                        //    holePoints.Add(data.ControlPoints.ToList());

                        //    BuildWindow(data);
                        //});

                        foreach(WindowData data in m_Data.Windows)
                        {
                            holePoints.Add(data.ControlPoints.ToList());

                            BuildWindow(data);
                        }
                    }

                    BuildSection(holePoints);
                }
                break;
            case WallElement.Extension:
                {
                    //List<List<Vector3>> wallHole;
                    //Vector3 scale = new Vector3(m_Data.ExtendWidth, m_Data.ExtendHeight);
                    //ProBuilderMesh extensionA = MeshMaker.NPolyHoleGrid(m_Data.ControlPoints, scale, 1, 1, 4, 0, out wallHole, null, new Vector3(0, -0.999f)); // Outside
                    //Vector3 normal = extensionA.GetVertices(extensionA.faces[0].distinctIndexes)[0].normal;
                    //extensionA.Extrude(new Face[] { extensionA.faces[0] }, ExtrudeMethod.FaceNormal, m_Data.WallDepth);
                    //ProBuilderMesh extensionB = MeshMaker.NPolyHoleGrid(m_Data.ControlPoints, scale, 1, 1, 4, 0, out _, null, new Vector3(0, -0.999f), true); // Inside
                    //CombineMeshes.Combine(new ProBuilderMesh[] { extensionA, extensionB }, extensionA);
                    //Rebuild(extensionA);
                    //DestroyImmediate(extensionB.gameObject);

                    //ControlPoint[] points = new ControlPoint[4];
                    //points[0] = new ControlPoint(wallHole[0][0]);
                    //points[1] = new ControlPoint(wallHole[0][0] + normal * m_Data.ExtendDistance);
                    //points[2] = new ControlPoint(wallHole[0][3] + normal * m_Data.ExtendDistance);
                    //points[3] = new ControlPoint(wallHole[0][3]);

                    //for (int i = 0; i < points.Length; i++)
                    //{
                    //    int next = points.GetNext(i);
                    //    int previous = points.GetPrevious(i);

                    //    Vector3 nextForward = Vector3Extensions.DirectionToTarget(points[i].Position, points[next].Position);
                    //    Vector3 previousForward = Vector3Extensions.DirectionToTarget(points[i].Position, points[previous].Position);

                    //    points[i].SetForward(Vector3.Lerp(nextForward, previousForward, 0.5f));
                    //}

                    //float wallHeight = Vector3.Distance(wallHole[0][0], wallHole[0][1]);

                    //Storey wallSectionStorey;

                    //if (TryGetComponent(out Storey storey))
                    //{
                    //    wallSectionStorey = storey;
                    //}
                    //else
                    //{
                    //    wallSectionStorey = gameObject.AddComponent<Storey>().Initialize(new StoreyData()) as Storey;
                    //}

                    //wallSectionStorey.Data.ControlPoints = points;
                    //wallSectionStorey.Data.WallData.Height = wallHeight;

                    //GameObject storeyGO = new GameObject("Storey", typeof(Storey));
                    //storeyGO.transform.SetParent(transform, true);
                    //storeyGO.GetComponent<Storey>().Initialize(wallSectionStorey.Data).Build();

                    //Roof wallSectionRoof;

                    //if (TryGetComponent(out Roof roof))
                    //{
                    //    wallSectionRoof = roof;
                    //}
                    //else
                    //{
                    //    gameObject.AddComponent<Roof>().Initialize(new RoofData());
                    //    wallSectionRoof = gameObject.GetComponent<Roof>();
                    //}

                    //wallSectionRoof.Data.ControlPoints = points;

                    //GameObject roofGO = new GameObject("Roof", typeof(Roof));
                    //roofGO.transform.SetParent(transform, true);
                    //roofGO.transform.localPosition = new Vector3(0, wallHeight, 0);
                    //Roof roofExtension = roofGO.GetComponent<Roof>();
                    //roofExtension.Initialize(wallSectionRoof.Data).Build();
                }
                break;
            case WallElement.Empty:
                m_ProBuilderMesh.Clear();
                m_ProBuilderMesh.ToMesh();
                m_ProBuilderMesh.Refresh();
                break;      
        }

        if(m_Data.WallElement != WallElement.Extension)
        {
            if(TryGetComponent(out Storey storey))
            {
                DestroyImmediate(storey);
            }

            if (TryGetComponent(out Roof roof))
            {
                DestroyImmediate(roof);
            }
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
            ActiveElements = m_Data.ActiveDoorElements.ToDoorElement()
        };

        return data;
    }
    #endregion

    #region Build
    private void BuildSection(IList<IList<Vector3>> holePoints)
    {
        ProBuilderMesh body = ProBuilderMesh.Create();
        body.CreateShapeFromPolygon(m_Data.ControlPoints, m_Data.FaceNormal, holePoints);
        ProBuilderMesh head = Instantiate(body);
        head.faces[0].Reverse();
        body.Extrude(body.faces, ExtrudeMethod.FaceNormal, m_Data.WallDepth);
        CombineMeshes.Combine(new ProBuilderMesh[] { body, head }, body);
        Rebuild(body);
        DestroyImmediate(head.gameObject);
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
        //if(m_Data.Ac)

        ProBuilderMesh doorMesh = ProBuilderMesh.Create();
        doorMesh.name = "Door " + Data.ID.ToString();
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
        doorFrameMesh.CreateShapeFromPolygon(controlPoints, 0, false, holePoints);
        doorFrameMesh.ToMesh();
        doorFrameMesh.Refresh();
        doorFrameMesh.MatchFaceToNormal(m_Data.FaceNormal);
        doorFrameMesh.Extrude(doorFrameMesh.faces, ExtrudeMethod.FaceNormal, depth);
        doorFrameMesh.ToMesh();
        doorFrameMesh.Refresh();
    }
    #endregion

    private WallSection Rebuild(ProBuilderMesh mesh)
    {
        m_ProBuilderMesh.RebuildWithPositionsAndFaces(mesh.positions, mesh.faces);
        m_ProBuilderMesh.ToMesh();
        m_ProBuilderMesh.Refresh();
        //m_ProBuilderMesh.CenterPivot(m_ProBuilderMesh.GetAllDistinctIndices().ToArray());

        DestroyImmediate(mesh.gameObject);
        return this;
    }

    private void OnDrawGizmosSelected()
    {
        //if (m_ProBuilderMesh == null)
        //    return;

        //if (m_Points == null)
        //    return;

        //for (int i = 0; i < m_Points.Count(); i++)
        //{
        //    for (int j = 0; j < m_Points[i].Count(); j++)
        //    {
        //        //Handles.DotHandleCap(0, m_Points[i][j], Quaternion.identity, 0.01f, EventType.Repaint);
        //    }

        //}

        //List<Vector3> positions = m_ProBuilderMesh.positions.ToList();

        //GUIStyle style = new GUIStyle();
        //style.fontSize = 18;
        //style.normal.textColor = Color.red;

        //for(int i = 0; i < m_ProBuilderMesh.faces[0].distinctIndexes.Count(); i++)
        //{
        //    int index = m_ProBuilderMesh.faces[0].distinctIndexes[i];

        //    Handles.Label(positions[index], index.ToString(), style);
        //}

        //if (m_Grid == null)
        //    return;

        //for(int x = 0; x < m_Grid.Count; x++) // cols
        //{
        //    Handles.color = Color.yellow;
        //    Handles.DotHandleCap(0, m_Grid[x], Quaternion.identity, 0.02f, EventType.Repaint);

        //    //for(int y = 0; y < m_Grid.GetLength(1)-1; y++) // rows
        //    //{
        //    //    Vector3 bl = m_Grid[x,y];
        //    //    Vector3 tl = m_Grid[x, y + 1];
        //    //    Vector3 tr = m_Grid[x + 1, y + 1];
        //    //    Vector3 br = m_Grid[x + 1, y];

        //    //    Vector3[] quad = new Vector3[] { bl, tl, tr, br };

        //    //    Handles.DrawAAPolyLine(quad);
        //    //    Handles.DrawAAPolyLine(quad[0], quad[3]);
        //    //}
        //}

        //Vector3 forward = Vector3.Cross(m_ControlPoints[0].DirectionToTarget(m_ControlPoints[3]), Vector3.up);
        //Quaternion rotation = Quaternion.FromToRotation(Vector3.up, forward);

        //for (int i = 0; i < m_WindowFrameColumns; i++)
        //{
        //    for(int j = 0; j < m_WindowFrameRows; j++)
        //    {
        //        Vector3 bl = m_Grid[j][i];
        //        Vector3 tl = m_Grid[j + 1][i];
        //        Vector3 tr = m_Grid[j + 1][i + 1];
        //        Vector3 br = m_Grid[j][i + 1];

        //        Vector3[] quad = new Vector3[] { bl, tl, tr, br };


        //        for (int k = 0; k < quad.Length; k++)
        //        {
        //            Vector3 euler = rotation.eulerAngles;
        //            Vector3 v = Quaternion.Euler(euler) * (quad[k] - m_Centre) + m_Centre;
        //            quad[k] = v;
        //        }

        //        Handles.DrawAAPolyLine(quad);
        //        Handles.DrawAAPolyLine(quad[0], quad[3]);
        //    }
        //}
    }
}
