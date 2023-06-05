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
    [SerializeField, Range(0, 1)] private float m_WindowHeight;
    [SerializeField, Range(0, 1)] private float m_WindowWidth;
    [SerializeField, Range(1, 10)] private int m_WindowColumns, m_WindowRows;
    [SerializeField] private bool m_IsWindowActive;
    [SerializeField, Range(1, 10)] private int m_WindowFrameColumns, m_WindowFrameRows;
    [SerializeField] private Vector3 m_WindowFrameScale;

    public float WindowHeight => m_WindowHeight;
    public float WindowWidth => m_WindowWidth;
    public int WindowColumns => m_WindowColumns;
    public int WindowRows => m_WindowRows;
    //---------- End of Windows Properties -----------


    // --------- Door Properties ---------------
    [SerializeField, Range(0, 1)] private float m_PedimentHeight;
    [SerializeField, Range(0, 1)] private float m_SideWidth;
    [SerializeField, Range(-1, 1)] private float m_SideOffset;
    [SerializeField, Range(1, 10)] private int m_DoorColumns, m_DoorRows; 
    [SerializeField] private bool m_IsDoorActive;
    [SerializeField] private Vector3 m_DoorScale;
    [SerializeField] private TransformPoint m_DoorHingePoint;
    [SerializeField] private Vector3 m_DoorHingeOffset;
    [SerializeField] private Vector3 m_DoorHingeEulerAngles;
    [SerializeField] private Material m_DoorMaterial;

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
        m_IsWindowActive = true;
        m_WindowFrameRows = 2;
        m_WindowFrameColumns = 2;
        m_WindowFrameScale = Vector3.one * 0.95f;
        // End Window

        // Door
        m_PedimentHeight = 0.75f;
        m_SideWidth = 0.5f;
        m_DoorColumns = 1;
        m_DoorRows = 1;
        m_DoorMaterial = BuiltinMaterials.defaultMaterial;
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
            case WallElement.Door:
                Vector3 doorScale = new Vector3(m_SideWidth, m_PedimentHeight, m_SideWidth);
                List<Vector3[]> doorGridControlPoints;
                ProBuilderMesh doorGridA = MeshMaker.DoorGrid(m_ControlPoints, doorScale, m_DoorColumns, m_DoorRows, m_SideOffset, out doorGridControlPoints);
                doorGridA.Extrude(new Face[] { doorGridA.faces[0] }, ExtrudeMethod.FaceNormal, m_WallDepth);
                ProBuilderMesh doorGridB = MeshMaker.DoorGrid(m_ControlPoints, doorScale, m_DoorColumns, m_DoorRows, m_SideOffset, out _, true);
                CombineMeshes.Combine(new ProBuilderMesh[] { doorGridA, doorGridB }, doorGridA);
                Rebuild(doorGridA);
                DestroyImmediate(doorGridB.gameObject);

                if (!m_IsDoorActive)
                    return this;

                foreach(Vector3[] controlPoints in doorGridControlPoints)
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
                Vector3 windowScale = new Vector3(m_WindowWidth, m_WindowHeight, m_WindowWidth);
                List<Vector3[]> holeGridControlPoints;
                ProBuilderMesh holeGridA = MeshMaker.HoleGrid(m_ControlPoints, Vector3.zero, 0, windowScale, m_WindowColumns, m_WindowRows, out holeGridControlPoints);
                holeGridA.Extrude(new Face[] { holeGridA.faces[0] }, ExtrudeMethod.FaceNormal, m_WallDepth);
                ProBuilderMesh holeGridB = MeshMaker.HoleGrid(m_ControlPoints, Vector3.zero, 0, windowScale, m_WindowColumns, m_WindowRows, out _, true);
                CombineMeshes.Combine(new ProBuilderMesh[] { holeGridA, holeGridB }, holeGridA);
                Rebuild(holeGridA);
                DestroyImmediate(holeGridB.gameObject);

                if (!m_IsWindowActive)
                    return this;

                float windowDepth = m_WallDepth * 0.5f;

                foreach (Vector3[] hole in holeGridControlPoints)
                {
                    GameObject win = new GameObject("Window", typeof(Window));
                    win.transform.SetParent(transform, true);
                    Vector3 dir = hole[0].DirectionToTarget(hole[3]);
                    Vector3 forward = Vector3.Cross(Vector3.up, dir);

                    for(int i = 0; i < hole.Length; i++)
                    {
                        hole[i] += forward * windowDepth;
                    }

                    Window window = win.GetComponent<Window>();
                    window.Initialize(hole, windowDepth);
                    window.SetFrameGrid(m_WindowFrameColumns, m_WindowFrameRows);
                    window.SetFrameScale(m_WindowFrameScale).Build();
                }

                break;
            case WallElement.Empty:
                m_ProBuilderMesh.Clear();
                m_ProBuilderMesh.ToMesh();
                m_ProBuilderMesh.Refresh();
                break;
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
