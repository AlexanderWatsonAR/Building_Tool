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
using UnityEngine.Rendering;

public class WallSection : MonoBehaviour, IBuildable
{
    [SerializeField] private WallElement m_WallElement;

    [SerializeField, HideInInspector] private ProBuilderMesh m_ProBuilderMesh;
    [SerializeField] private Vector3 m_FaceNormal;
    [SerializeField] private WallSectionData m_Data;

    public WallElement WallElement => m_WallElement;
    public WallSectionData Data => m_Data;

    // Temp variables for testing
    List<IList<Vector3>> m_Points;
    List<Vector3> m_Grid;
    Vector3 m_Centre;
    // end of temp


    public IBuildable Initialize(IData wallSectionData)
    {
        m_Data = wallSectionData as WallSectionData;
        m_FaceNormal = m_Data.DoorData.Forward;
        m_ProBuilderMesh = GetComponent<ProBuilderMesh>();
        return this;
    }

    public void Build()
    {
        transform.DeleteChildren();

        switch(m_WallElement)
        {
            case WallElement.Wall:
                Rebuild(MeshMaker.Cube(m_Data.ControlPoints, m_Data.WallDepth));
                break;
            case WallElement.Doorway:
                {
                    List<List<Vector3>> doorGridControlPoints;

                    Vector3 doorScale = new Vector3(m_Data.SideWidth, m_Data.PedimentHeight);
                    ProBuilderMesh doorGridA = MeshMaker.NPolyHoleGrid(m_Data.ControlPoints, doorScale, m_Data.DoorColumns, m_Data.DoorRows, 4, 0, out doorGridControlPoints, Vector3.right * m_Data.SideOffset, new Vector3(0, -0.999f)); // Outside
                    doorGridA.Extrude(new Face[] { doorGridA.faces[0] }, ExtrudeMethod.FaceNormal, m_Data.WallDepth);
                    ProBuilderMesh doorGridB = MeshMaker.NPolyHoleGrid(m_Data.ControlPoints, doorScale, m_Data.DoorColumns, m_Data.DoorRows, 4, 0, out _, Vector3.right * m_Data.SideOffset, new Vector3(0, -0.999f), true); // Inside
                    CombineMeshes.Combine(new ProBuilderMesh[] { doorGridA, doorGridB }, doorGridA);
                    Rebuild(doorGridA);
                    DestroyImmediate(doorGridB.gameObject);

                    CreateDoors(doorGridControlPoints);
                }
                break;
            case WallElement.Archway:
                {
                    List<List<Vector3>> doorGridControlPoints;

                    ProBuilderMesh archedDoorGridA = MeshMaker.ArchedDoorHoleGrid(m_Data.ControlPoints, m_Data.SideWidth, m_Data.DoorColumns, m_Data.DoorRows, m_Data.PedimentHeight, m_Data.ArchHeight, m_Data.ArchSides, out doorGridControlPoints, Vector3.right * m_Data.SideOffset); // Outside
                    archedDoorGridA.Extrude(new Face[] { archedDoorGridA.faces[0] }, ExtrudeMethod.FaceNormal, m_Data.WallDepth);
                    ProBuilderMesh archedDoorGridB = MeshMaker.ArchedDoorHoleGrid(m_Data.ControlPoints, m_Data.SideWidth, m_Data.DoorColumns, m_Data.DoorRows, m_Data.PedimentHeight, m_Data.ArchHeight, m_Data.ArchSides, out doorGridControlPoints, Vector3.right * m_Data.SideOffset, true); // Inside
                    CombineMeshes.Combine(new ProBuilderMesh[] { archedDoorGridA, archedDoorGridB }, archedDoorGridA);
                    Rebuild(archedDoorGridA);
                    DestroyImmediate(archedDoorGridB.gameObject);

                    CreateDoors(doorGridControlPoints);
                }
                break;
            case WallElement.Window:
                {
                    List<List<Vector3>> holePoints;
                    Vector3 winScale = new Vector3(m_Data.WindowWidth, m_Data.WindowHeight);
                    ProBuilderMesh polyHoleGridA = MeshMaker.NPolyHoleGrid(m_Data.ControlPoints, winScale, m_Data.WindowColumns, m_Data.WindowRows, m_Data.WindowSides, m_Data.WindowAngle, out holePoints, false);
                    ProBuilderMesh polyHoleGridB = MeshMaker.NPolyHoleGrid(m_Data.ControlPoints, winScale, m_Data.WindowColumns, m_Data.WindowRows, m_Data.WindowSides, m_Data.WindowAngle, out _, true); // Would instantiating be quicker?
                    polyHoleGridA.Extrude(polyHoleGridA.faces, ExtrudeMethod.FaceNormal, m_Data.WallDepth);

                    CombineMeshes.Combine(new ProBuilderMesh[] { polyHoleGridA, polyHoleGridB }, polyHoleGridA);
                    Rebuild(polyHoleGridA);
                    DestroyImmediate(polyHoleGridB.gameObject);

                    if (m_Data.WindowData.ActiveElements == WindowElement.Nothing)
                        return;

                    WindowData windowData = new WindowData(m_Data.WindowData);
                    windowData.SetControlPoints(holePoints[0]);

                    ProBuilderMesh win = ProBuilderMesh.Create();
                    win.name = "Window";
                    win.transform.SetParent(transform, false);
                    Window window = win.AddComponent<Window>();
                    window.Initialize(windowData).Build();
                    Vector3 winPosition = ProMaths.Average(holePoints[0]);

                    for (int i = 1; i < holePoints.Count; i++)
                    {
                        Window instanceWin = Instantiate(window);
                        Vector3 position = ProMaths.Average(holePoints[i]);
                        instanceWin.WindowData.SetControlPoints(holePoints[i]);
                        instanceWin.SetPosition(position - winPosition);
                        instanceWin.transform.SetParent(transform, false);
                    }

                }
                break;
            case WallElement.Extension:
                {
                    List<List<Vector3>> wallHole;
                    Vector3 scale = new Vector3(m_Data.ExtendWidth, m_Data.ExtendHeight);
                    ProBuilderMesh extensionA = MeshMaker.NPolyHoleGrid(m_Data.ControlPoints, scale, 1, 1, 4, 0, out wallHole, null, new Vector3(0, -0.999f)); // Outside
                    Vector3 normal = extensionA.GetVertices(extensionA.faces[0].distinctIndexes)[0].normal;
                    extensionA.Extrude(new Face[] { extensionA.faces[0] }, ExtrudeMethod.FaceNormal, m_Data.WallDepth);
                    ProBuilderMesh extensionB = MeshMaker.NPolyHoleGrid(m_Data.ControlPoints, scale, 1, 1, 4, 0, out _, null, new Vector3(0, -0.999f), true); // Inside
                    CombineMeshes.Combine(new ProBuilderMesh[] { extensionA, extensionB }, extensionA);
                    Rebuild(extensionA);
                    DestroyImmediate(extensionB.gameObject);

                    ControlPoint[] points = new ControlPoint[4];
                    points[0] = new ControlPoint(wallHole[0][0]);
                    points[1] = new ControlPoint(wallHole[0][0] + normal * m_Data.ExtendDistance);
                    points[2] = new ControlPoint(wallHole[0][3] + normal * m_Data.ExtendDistance);
                    points[3] = new ControlPoint(wallHole[0][3]);

                    for (int i = 0; i < points.Length; i++)
                    {
                        int next = points.GetNext(i);
                        int previous = points.GetPrevious(i);

                        Vector3 nextForward = Vector3Extensions.DirectionToTarget(points[i].Position, points[next].Position);
                        Vector3 previousForward = Vector3Extensions.DirectionToTarget(points[i].Position, points[previous].Position);

                        points[i].SetForward(Vector3.Lerp(nextForward, previousForward, 0.5f));
                    }

                    float wallHeight = Vector3.Distance(wallHole[0][0], wallHole[0][1]);

                    Storey wallSectionStorey;

                    if (TryGetComponent(out Storey storey))
                    {
                        wallSectionStorey = storey;
                    }
                    else
                    {
                        wallSectionStorey = gameObject.AddComponent<Storey>().Initialize(new StoreyData()) as Storey;
                    }

                    wallSectionStorey.Data.ControlPoints = points;
                    wallSectionStorey.Data.WallData.Height = wallHeight;

                    GameObject storeyGO = new GameObject("Storey", typeof(Storey));
                    storeyGO.transform.SetParent(transform, true);
                    storeyGO.GetComponent<Storey>().Initialize(wallSectionStorey.Data).Build();

                    Roof wallSectionRoof;

                    if (TryGetComponent(out Roof roof))
                    {
                        wallSectionRoof = roof;
                    }
                    else
                    {
                        gameObject.AddComponent<Roof>().Initialize(new RoofData());
                        wallSectionRoof = gameObject.GetComponent<Roof>();
                    }

                    wallSectionRoof.Data.ControlPoints = points;

                    GameObject roofGO = new GameObject("Roof", typeof(Roof));
                    roofGO.transform.SetParent(transform, true);
                    roofGO.transform.localPosition = new Vector3(0, wallHeight, 0);
                    Roof roofExtension = roofGO.GetComponent<Roof>();
                    roofExtension.Initialize(wallSectionRoof.Data).Build();
                }
                break;
            case WallElement.Empty:
                m_ProBuilderMesh.Clear();
                m_ProBuilderMesh.ToMesh();
                m_ProBuilderMesh.Refresh();
                break;      
        }

        if(m_WallElement != WallElement.Extension)
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

    private void CreateDoors(List<List<Vector3>> doorGridControlPoints)
    {
        foreach (List<Vector3> controlPoints in doorGridControlPoints)
        {
            if (m_Data.ActiveDoorElements.IsElementActive(DoorElement.Door))
            {
                ProBuilderMesh doorPro = ProBuilderMesh.Create();
                doorPro.name = "Door";
                doorPro.transform.SetParent(transform, false);
                Door door = doorPro.AddComponent<Door>();
                DoorData data = new DoorData(m_Data.DoorData);
                {
                    data.ControlPoints = controlPoints.ToArray();
                    data.ActiveElements = m_Data.ActiveDoorElements;
                }

                door.Initialize(data).Build();
            }

            if (!m_Data.ActiveDoorElements.IsElementActive(DoorElement.Frame))
                continue;

            // Frame
            Vector3[] scaledControlPoints = controlPoints.ScalePolygon(m_Data.DoorFrameInsideScale);
            List<IList<Vector3>> holePoints = new();
            holePoints.Add(scaledControlPoints);

            ProBuilderMesh doorFrameMesh = ProBuilderMesh.Create();
            doorFrameMesh.name = "Frame";
            doorFrameMesh.transform.SetParent(transform, false);
            doorFrameMesh.CreateShapeFromPolygon(controlPoints, 0, false, holePoints);
            doorFrameMesh.ToMesh();
            doorFrameMesh.Refresh();
            doorFrameMesh.MatchFaceToNormal(m_FaceNormal);
            doorFrameMesh.Extrude(doorFrameMesh.faces, ExtrudeMethod.FaceNormal, m_Data.DoorFrameDepth);
            doorFrameMesh.ToMesh();
            doorFrameMesh.Refresh();
            //doorFrameMesh.GetComponent<Renderer>().sharedMaterial = m_Data.DoorFrameMaterial;
        }
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

        if (m_Grid == null)
            return;

        for(int x = 0; x < m_Grid.Count; x++) // cols
        {
            Handles.color = Color.yellow;
            Handles.DotHandleCap(0, m_Grid[x], Quaternion.identity, 0.02f, EventType.Repaint);

            //for(int y = 0; y < m_Grid.GetLength(1)-1; y++) // rows
            //{
            //    Vector3 bl = m_Grid[x,y];
            //    Vector3 tl = m_Grid[x, y + 1];
            //    Vector3 tr = m_Grid[x + 1, y + 1];
            //    Vector3 br = m_Grid[x + 1, y];

            //    Vector3[] quad = new Vector3[] { bl, tl, tr, br };

            //    Handles.DrawAAPolyLine(quad);
            //    Handles.DrawAAPolyLine(quad[0], quad[3]);
            //}
        }

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
