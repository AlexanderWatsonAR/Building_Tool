using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

public class RoofSection : MonoBehaviour
{
    [SerializeField] private RoofElement m_RoofElement;
    [SerializeField, HideInInspector] private Vector3[] m_ControlPoints;
    [SerializeField, HideInInspector] private Vector3[] m_TopPoints;
    [SerializeField, HideInInspector] private float m_SectionHeight;
    [SerializeField, HideInInspector] private ProBuilderMesh m_ProBuilderMesh;

    // --------- Window Properties -------------
    [SerializeField, Range(0, 0.999f)] private float m_WindowHeight;
    [SerializeField, Range(0, 0.999f)] private float m_WindowWidth;
    [SerializeField, Range(3, 32)] private int m_WindowSides = 3;
    [SerializeField, Range(1, 10)] private int m_WindowColumns, m_WindowRows;
    //---------- End of Windows Properties -----------
    public RoofElement RoofElement => m_RoofElement;

    public RoofSection Initialize(IEnumerable<Vector3> controlPoints, float sectionHeight)
    {
        m_ControlPoints = controlPoints.ToArray();
        m_SectionHeight = sectionHeight;
        m_ProBuilderMesh = GetComponent<ProBuilderMesh>();
        m_RoofElement = RoofElement.Tile;

        // Window
        m_WindowHeight = 0.5f;
        m_WindowWidth = 0.5f;
        m_WindowColumns = 1;
        m_WindowRows = 1;

        return this;
    }

    public RoofSection SetTopPoints(IEnumerable<Vector3> topPoints, bool rebuild = false)
    {
        m_TopPoints = topPoints.ToArray();
        return this;
    }

    public RoofSection Build()
    {
        transform.DeleteChildren();

        switch (m_RoofElement)
        {
            case RoofElement.Tile:
                if (m_ControlPoints[1] == m_ControlPoints[2])
                {
                    m_ControlPoints = new Vector3[] { m_ControlPoints[0], m_ControlPoints[1], m_ControlPoints[3] };
                    m_TopPoints = new Vector3[] { m_TopPoints[0], m_TopPoints[1], m_TopPoints[3] };
                }
                m_ProBuilderMesh.CreateShapeFromPolygon(m_ControlPoints, m_SectionHeight, false);
                m_ProBuilderMesh.ToMesh();
                SetCornerPoints();
                m_ProBuilderMesh.Refresh();
                break;
            case RoofElement.Window:
                //List<List<Vector3>> holePoints;
                Vector3 winScale = new Vector3(m_WindowWidth, m_WindowHeight);
                ProBuilderMesh polyHoleGrid = MeshMaker.NPolyHoleGrid(m_ControlPoints, winScale, m_WindowColumns, m_WindowRows, m_WindowSides, 0, out _, true);
                Rebuild(polyHoleGrid);
                SetCornerPoints();
                m_ProBuilderMesh.Refresh();
                break;
            case RoofElement.Empty:
                m_ProBuilderMesh.Clear();
                m_ProBuilderMesh.ToMesh();
                break;
        }

        return this;
    }

    private void SetCornerPoints()
    {
        Vertex[] vertices = m_ProBuilderMesh.GetVertices();

        //Vector3 normal = m_ProBuilderMesh.GetVertices(m_ProBuilderMesh.faces[0].distinctIndexes)[0].normal;
        
        Vector3[] top = new Vector3[m_ControlPoints.Length];
        Array.Copy(m_ControlPoints, top, m_ControlPoints.Length);

        for (int i = 0; i < top.Length; i++)
        {
            //top[i] += normal * m_SectionHeight; // This should be approximately where the extruded points are.

            // Note: No need to find the points. They are where you left them.
            List<int> shared = m_ProBuilderMesh.GetCoincidentVertices(new int[] { i });

            for(int j = 0; j < shared.Count; j++)
            {
                vertices[shared[j]].position = m_TopPoints[i];
            }
        }

        m_ProBuilderMesh.SetVertices(vertices);
        m_ProBuilderMesh.ToMesh();
    }

    private RoofSection Rebuild(ProBuilderMesh mesh)
    {
        m_ProBuilderMesh.RebuildWithPositionsAndFaces(mesh.positions, mesh.faces);
        m_ProBuilderMesh.ToMesh();
        m_ProBuilderMesh.Refresh();
        DestroyImmediate(mesh.gameObject);
        return this;
    }
}
