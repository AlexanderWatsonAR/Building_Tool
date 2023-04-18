using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

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


    public WallSection Initialize(IEnumerable<Vector3> controlPoints, float wallDepth)
    {
        m_ControlPoints = controlPoints;
        m_WallDepth = wallDepth;
        m_ProBuilderMesh = GetComponent<ProBuilderMesh>();
        m_MeshFilter = GetComponent<MeshFilter>();

        // Windows
        m_WindowHeight = 0.5f;
        m_WindowWidth = 0.5f;
        m_WindowColumns = 1;
        m_WindowRows = 1;
        // End Windows
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
                break;
            case WallElement.Window:
                Vector3 scale = new Vector3(m_WindowWidth, m_WindowHeight, m_WindowWidth);
                ProBuilderMesh holeGridA = MeshMaker.HoleGrid(m_ControlPoints, Vector3.zero, 0, scale, m_WindowColumns, m_WindowRows);
                holeGridA.Extrude(new Face[] { holeGridA.faces[0] }, ExtrudeMethod.FaceNormal, m_WallDepth);
                ProBuilderMesh holeGridB = MeshMaker.HoleGrid(m_ControlPoints, Vector3.zero, 0, scale, m_WindowColumns, m_WindowRows, true);
                CombineMeshes.Combine(new ProBuilderMesh[] { holeGridA, holeGridB }, holeGridA);
                Rebuild(holeGridA);
                DestroyImmediate(holeGridB.gameObject);
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
}
