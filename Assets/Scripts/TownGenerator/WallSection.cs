using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEditor;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using System.Linq;
using UnityEngine.ProBuilder.Shapes;

public class WallSection : MonoBehaviour
{
    [SerializeField] private WallElement m_WallElement;
    [SerializeField, HideInInspector] private Vector3[] m_ControlPoints;
    [SerializeField, HideInInspector] private float m_WallDepth;
    [SerializeField, HideInInspector] private ProBuilderMesh m_ProBuilderMesh;

    public WallElement WallElement => m_WallElement;

    // --------- Window Properties -------------
    [SerializeField, Range(0, 0.999f)] private float m_WindowHeight;
    [SerializeField, Range(0, 0.999f)] private float m_WindowWidth;
    [SerializeField, Range(3, 32)] private int m_WindowSides = 3;
    [SerializeField, Range(1, 10)] private int m_WindowColumns, m_WindowRows;
    [SerializeField, Range(-180, 180)] private float m_WindowAngle;
    [SerializeField] private bool m_WindowSmooth;
    [SerializeField] private bool m_IsWindowActive;
    [SerializeField, Range(1, 10)] private int m_WindowFrameColumns, m_WindowFrameRows;
    [SerializeField] private Vector3 m_WindowFrameScale;
    [SerializeField] private Material m_WindowPaneMaterial;
    [SerializeField] private Material m_WindowFrameMaterial;

    public float WindowAngle => m_WindowAngle;
    public float WindowHeight => m_WindowHeight;
    public float WindowWidth => m_WindowWidth;
    public int WindowColumns => m_WindowColumns;
    public int WindowRows => m_WindowRows;
    //---------- End of Windows Properties -----------


    // ----------- Extension ------------------
    [SerializeField, Range(1, 10)] private float m_ExtendDistance;
    [SerializeField, Range(0, 1)] private float m_ExtendHeight;
    [SerializeField, Range(0, 1)] private float m_ExtendWidth;
    // --------- End of Extension --------------

    List<IList<Vector3>> m_Points;

    // --------- Door Properties ---------------
    // Doorway
    [SerializeField, Range(0, 0.999f)] private float m_PedimentHeight;
    [SerializeField, Range(0, 0.999f)] private float m_SideWidth;
    [SerializeField, Range(-0.999f, 0.999f)] private float m_SideOffset;
    [SerializeField, Range(1, 10)] private int m_DoorColumns, m_DoorRows;
    [SerializeField] private float m_ArchHeight;
    [SerializeField] private int m_ArchSides;
    // Door
    [SerializeField] private bool m_IsDoorActive;
    [SerializeField] private Vector3 m_DoorScale;
    [SerializeField] private TransformPoint m_DoorHingePoint;
    [SerializeField] private Vector3 m_DoorHingeOffset;
    [SerializeField] private Vector3 m_DoorHingeEulerAngles;
    [SerializeField] private Material m_DoorMaterial;

    // Door Frame
    [SerializeField] private float m_DoorFrameDepth;
    [SerializeField] private float m_DoorFrameInsideScale;

    public Material DoorMaterial => m_DoorMaterial;
    public float PedimentHeight => m_PedimentHeight;
    public float SideWidth => m_SideWidth;

    public int DoorColumns => m_DoorColumns;
    public int DoorRows => m_DoorRows;

    // --------- End of Door Properties ---------------

    public WallSection Initialize(IEnumerable<Vector3> controlPoints, float wallDepth)
    {
        m_ControlPoints = controlPoints == null ? m_ControlPoints : controlPoints.ToArray();
        m_WallDepth = wallDepth;
        m_ProBuilderMesh = GetComponent<ProBuilderMesh>();

        // Window
        m_WindowHeight = 0.5f;
        m_WindowWidth = 0.5f;
        m_WindowColumns = 1;
        m_WindowRows = 1;
        m_WindowAngle = 0;
        m_IsWindowActive = true;
        m_WindowFrameRows = 2;
        m_WindowFrameColumns = 2;
        m_WindowFrameScale = Vector3.one * 0.95f;
        m_WindowPaneMaterial = BuiltinMaterials.defaultMaterial;
        m_WindowFrameMaterial = BuiltinMaterials.defaultMaterial;
        // End Window

        // Extension
        m_ExtendDistance = 2.5f;
        m_ExtendHeight = 0.75f;
        m_ExtendWidth = 0.75f;
        // End Extension

        // Door
        m_PedimentHeight = 0.75f;
        m_SideWidth = 0.5f;
        m_DoorColumns = 1;
        m_DoorRows = 1;
        m_DoorMaterial = BuiltinMaterials.defaultMaterial;
        m_ArchSides = 3;
        m_ArchHeight = 1;
        m_IsDoorActive = true;
        m_DoorScale = Vector3.one * 0.9f;
        m_DoorHingePoint = TransformPoint.Left;
        m_DoorHingeEulerAngles = Vector3.zero;
        // End Door


        return this;
    }

    public WallSection Build()
    {
        transform.DeleteChildren();
        switch(m_WallElement)
        {
            case WallElement.Wall:
                Rebuild(MeshMaker.Cube(m_ControlPoints, m_WallDepth));
                break;
            case WallElement.Doorway:
                
                List<List<Vector3>> doorGridControlPoints;

                if(m_ArchHeight > 0)
                {
                    ProBuilderMesh archedDoorGridA = MeshMaker.ArchedDoorHoleGrid(m_ControlPoints, m_SideWidth, m_DoorColumns, m_DoorRows, m_PedimentHeight, m_ArchHeight, m_ArchSides, out doorGridControlPoints, Vector3.right * m_SideOffset); // Outside
                    archedDoorGridA.Extrude(new Face[] { archedDoorGridA.faces[0] }, ExtrudeMethod.FaceNormal, m_WallDepth);
                    ProBuilderMesh archedDoorGridB = MeshMaker.ArchedDoorHoleGrid(m_ControlPoints, m_SideWidth, m_DoorColumns, m_DoorRows, m_PedimentHeight, m_ArchHeight, m_ArchSides, out doorGridControlPoints, Vector3.right * m_SideOffset, true); // Inside
                    CombineMeshes.Combine(new ProBuilderMesh[] { archedDoorGridA, archedDoorGridB }, archedDoorGridA);
                    Rebuild(archedDoorGridA);
                    DestroyImmediate(archedDoorGridB.gameObject);
                }
                else
                {
                    Vector3 doorScale = new Vector3(m_SideWidth, m_PedimentHeight);
                    ProBuilderMesh doorGridA = MeshMaker.NPolyHoleGrid(m_ControlPoints, doorScale, m_DoorColumns, m_DoorRows, 4, 0, out doorGridControlPoints, Vector3.right * m_SideOffset, new Vector3(0, -0.999f)); // Outside
                    doorGridA.Extrude(new Face[] { doorGridA.faces[0] }, ExtrudeMethod.FaceNormal, m_WallDepth);
                    ProBuilderMesh doorGridB = MeshMaker.NPolyHoleGrid(m_ControlPoints, doorScale, m_DoorColumns, m_DoorRows, 4, 0, out _, Vector3.right * m_SideOffset, new Vector3(0, -0.999f), true); // Inside
                    CombineMeshes.Combine(new ProBuilderMesh[] { doorGridA, doorGridB }, doorGridA);
                    Rebuild(doorGridA);
                    DestroyImmediate(doorGridB.gameObject);
                }

                if (!m_IsDoorActive)
                    return this;

                foreach(List<Vector3> controlPoints in doorGridControlPoints)
                {
                    ProBuilderMesh doorPro = ProBuilderMesh.Create();
                    doorPro.name = "Door";
                    doorPro.transform.SetParent(transform, true);
                    Door door = doorPro.AddComponent<Door>();
                    door.Initialize(controlPoints, m_WallDepth, m_DoorScale);
                    door.HingePoint = m_DoorHingePoint;
                    door.SetHingeOffset(m_DoorHingeOffset).SetHingeEulerAngles(m_DoorHingeEulerAngles).SetMaterial(DoorMaterial).Build();
                }

                break;
            case WallElement.Window:
                List<List<Vector3>> holePoints;
                Vector3 winScale = new Vector3(m_WindowWidth, m_WindowHeight);
                ProBuilderMesh polyHoleGridA = MeshMaker.NPolyHoleGrid(m_ControlPoints, winScale, m_WindowColumns, m_WindowRows, m_WindowSides, m_WindowAngle, out holePoints, false);
                polyHoleGridA.Extrude(polyHoleGridA.faces, ExtrudeMethod.FaceNormal, m_WallDepth);
                ProBuilderMesh polyHoleGridB = MeshMaker.NPolyHoleGrid(m_ControlPoints, winScale, m_WindowColumns, m_WindowRows, m_WindowSides, m_WindowAngle, out _, true);
                CombineMeshes.Combine(new ProBuilderMesh[] { polyHoleGridA, polyHoleGridB }, polyHoleGridA);
                Rebuild(polyHoleGridA);
                DestroyImmediate(polyHoleGridB.gameObject);

                if (!m_IsWindowActive)
                    return this;

                MeshMaker.PolyFrameGrid(holePoints[0], Vector3.one * 0.95f, m_WindowFrameColumns, m_WindowFrameRows);

                //float windowDepth = m_WallDepth * 0.5f;

                //foreach (Vector3[] hole in holeGridControlPoints)
                //{
                //    GameObject win = new GameObject("Window", typeof(Window));
                //    win.transform.SetParent(transform, true);
                //    Vector3 dir = hole[0].DirectionToTarget(hole[3]);
                //    Vector3 forward = Vector3.Cross(Vector3.up, dir);

                //    for(int i = 0; i < hole.Length; i++)
                //    {
                //        hole[i] += forward * windowDepth;
                //    }

                //    Window window = win.GetComponent<Window>();
                //    window.Initialize(hole, windowDepth);
                //    window.SetFrameGrid(m_WindowFrameColumns, m_WindowFrameRows);
                //    window.SetFrameScale(m_WindowFrameScale);
                //    window.SetMaterials(m_WindowFrameMaterial, m_WindowPaneMaterial).Build();
                //}

                break;
            case WallElement.Extension:
                List<List<Vector3>> wallHole;
                Vector3 scale = new Vector3(m_ExtendWidth, m_ExtendHeight);
                ProBuilderMesh extensionA = MeshMaker.NPolyHoleGrid(m_ControlPoints, scale, 1, 1, 4, 0, out wallHole, null, new Vector3(0, -0.999f)); // Outside
                Vector3 normal = extensionA.GetVertices(extensionA.faces[0].distinctIndexes)[0].normal;
                extensionA.Extrude(new Face[] { extensionA.faces[0] }, ExtrudeMethod.FaceNormal, m_WallDepth);
                ProBuilderMesh extensionB = MeshMaker.NPolyHoleGrid(m_ControlPoints, scale, 1, 1, 4, 0, out _, null, new Vector3(0, -0.999f), true); // Inside
                CombineMeshes.Combine(new ProBuilderMesh[] { extensionA, extensionB }, extensionA);
                Rebuild(extensionA);
                DestroyImmediate(extensionB.gameObject);

                ControlPoint[] points = new ControlPoint[4];
                points[0] = new ControlPoint(wallHole[0][0]);
                points[1] = new ControlPoint(wallHole[0][0] + normal * m_ExtendDistance);
                points[2] = new ControlPoint(wallHole[0][3] + normal * m_ExtendDistance);
                points[3] = new ControlPoint(wallHole[0][3]);

                for(int i = 0; i < points.Length; i++)
                {
                    int next = points.GetNext(i);
                    int previous = points.GetPrevious(i);

                    Vector3 nextForward = Vector3Extensions.DirectionToTarget(points[i].Position, points[next].Position);
                    Vector3 previousForward = Vector3Extensions.DirectionToTarget(points[i].Position, points[previous].Position);

                    points[i].SetForward(Vector3.Lerp(nextForward, previousForward, 0.5f));
                }

                float wallHeight = Vector3.Distance(wallHole[0][0], wallHole[0][1]);

                Storey wallSectionStorey;

                if(TryGetComponent(out Storey storey))
                {
                    wallSectionStorey = storey;
                }
                else
                {
                    wallSectionStorey = gameObject.AddComponent<Storey>().Initialize();
                }

                wallSectionStorey.SetControlPoints(points);
                wallSectionStorey.SetWallHeight(wallHeight);

                GameObject storeyGO = new GameObject("Storey", typeof(Storey));
                storeyGO.transform.SetParent(transform, true);
                storeyGO.GetComponent<Storey>().Initialize(wallSectionStorey).Build();

                Roof wallSectionRoof;

                if (TryGetComponent(out Roof roof))
                {
                    wallSectionRoof = roof;
                }
                else
                {
                    wallSectionRoof = gameObject.AddComponent<Roof>().Initialize();
                }

                wallSectionRoof.SetControlPoints(points);

                GameObject roofGO = new GameObject("Roof", typeof(Roof));
                roofGO.transform.SetParent(transform, true);
                roofGO.transform.localPosition = new Vector3(0, wallHeight, 0);
                Roof roofExtension = roofGO.GetComponent<Roof>().Initialize(wallSectionRoof.Data);
                roofExtension.SetControlPoints(points);
                roofExtension.BuildFrame();

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

        return this;
    }

    private WallSection Rebuild(ProBuilderMesh mesh)
    {
        m_ProBuilderMesh.RebuildWithPositionsAndFaces(mesh.positions, mesh.faces);
        m_ProBuilderMesh.ToMesh();
        m_ProBuilderMesh.Refresh();
        DestroyImmediate(mesh.gameObject);
        return this;
    }

    private void OnDrawGizmosSelected()
    {
        if (m_ProBuilderMesh == null)
            return;

        if (m_Points == null)
            return;

        for (int i = 0; i < m_Points.Count(); i++)
        {
            for (int j = 0; j < m_Points[i].Count(); j++)
            {
                Handles.DotHandleCap(0, m_Points[i][j], Quaternion.identity, 0.05f, EventType.Repaint);
            }

        }

        List<Vector3> positions = m_ProBuilderMesh.positions.ToList();

        GUIStyle style = new GUIStyle();
        style.fontSize = 18;
        style.normal.textColor = Color.red;

        for(int i = 0; i < m_ProBuilderMesh.faces[0].distinctIndexes.Count(); i++)
        {
            int index = m_ProBuilderMesh.faces[0].distinctIndexes[i];

            Handles.Label(positions[index], index.ToString(), style);
        }
    }
}
