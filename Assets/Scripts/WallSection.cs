using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEditor;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using System.Linq;

public class WallSection : MonoBehaviour
{
    [SerializeField] private WallElement m_WallElement;
    [SerializeField, HideInInspector] private IEnumerable<Vector3> m_ControlPoints;
    [SerializeField, HideInInspector] private float m_WallDepth;
    [SerializeField, HideInInspector] private ProBuilderMesh m_ProBuilderMesh;
    [SerializeField, HideInInspector] private MeshFilter m_MeshFilter;

    public WallElement WallElement => m_WallElement;

    // --------- Window Properties -------------
    [SerializeField, Range(0, 1)] private float m_WindowHeight;
    [SerializeField, Range(0, 1)] private float m_WindowWidth;
    [SerializeField, Range(1, 10)] private int m_WindowColumns, m_WindowRows;

    public float WindowHeight => m_WindowHeight;
    public float WindowWidth => m_WindowWidth;
    public int WindowColumns => m_WindowColumns;
    public int WindowRows => m_WindowRows;
    //---------- End of Windows Properties -----------


    // --------- Door Properties ---------------
    [SerializeField, Range(0, 1)] private float m_PedimentHeight;
    [SerializeField, Range(0, 1)] private float m_SideWidth;
    [SerializeField, Range(1, 10)] private int m_DoorColumns, m_DoorRows;

    public float PedimentHeight => m_PedimentHeight;
    public float SideWidth => m_SideWidth;

    public int DoorColumns => m_DoorColumns;
    public int DoorRows => m_DoorRows;

    // --------- End of Door Properties ---------------

    public WallSection Initialize(IEnumerable<Vector3> controlPoints, float wallDepth)
    {
        m_ControlPoints = controlPoints;
        m_WallDepth = wallDepth;
        m_ProBuilderMesh = GetComponent<ProBuilderMesh>();
        m_MeshFilter = GetComponent<MeshFilter>();

        // Window
        m_WindowHeight = 0.5f;
        m_WindowWidth = 0.5f;
        m_WindowColumns = 1;
        m_WindowRows = 1;
        // End Window

        // Door
        m_PedimentHeight = 0.75f;
        m_SideWidth = 0.5f;
        m_DoorColumns = 1;
        m_DoorRows = 1;
        // End Door


        return this;
    }

    public WallSection Build()
    {
        switch(m_WallElement)
        {
            case WallElement.Wall:
                Rebuild(MeshMaker.Cube(m_ControlPoints, m_WallDepth));
                break;
            case WallElement.Door:
                Vector3 doorScale = new Vector3(m_SideWidth, m_PedimentHeight, m_SideWidth);
                ProBuilderMesh doorGridA = MeshMaker.DoorGrid(m_ControlPoints, doorScale, m_DoorColumns, m_DoorRows);
                doorGridA.Extrude(new Face[] { doorGridA.faces[0] }, ExtrudeMethod.FaceNormal, m_WallDepth);
                ProBuilderMesh doorGridB = MeshMaker.DoorGrid(m_ControlPoints, doorScale, m_DoorColumns, m_DoorRows, true);
                CombineMeshes.Combine(new ProBuilderMesh[] { doorGridA, doorGridB }, doorGridA);
                Rebuild(doorGridA);
                DestroyImmediate(doorGridB.gameObject);
                break;
            case WallElement.Window:
                Vector3 windowScale = new Vector3(m_WindowWidth, m_WindowHeight, m_WindowWidth);
                ProBuilderMesh holeGridA = MeshMaker.HoleGrid(m_ControlPoints, Vector3.zero, 0, windowScale, m_WindowColumns, m_WindowRows);
                holeGridA.Extrude(new Face[] { holeGridA.faces[0] }, ExtrudeMethod.FaceNormal, m_WallDepth);
                ProBuilderMesh holeGridB = MeshMaker.HoleGrid(m_ControlPoints, Vector3.zero, 0, windowScale, m_WindowColumns, m_WindowRows, true);
                CombineMeshes.Combine(new ProBuilderMesh[] { holeGridA, holeGridB }, holeGridA);
                Rebuild(holeGridA);
                DestroyImmediate(holeGridB.gameObject);
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

        for (int i = 0; i < positions.Count; i++)
        {
            Handles.Label(positions[i], i.ToString(), style);
        }
    }
}
