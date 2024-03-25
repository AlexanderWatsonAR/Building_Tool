using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using ProMaths = UnityEngine.ProBuilder.Math;

public class RoofSection : MonoBehaviour, IBuildable
{
    [SerializeReference] private RoofSectionData m_Data;
    public RoofSectionData Data => m_Data;

    [SerializeField, HideInInspector] private ProBuilderMesh m_ProBuilderMesh;

    // --------- Window Properties--------------------
    [SerializeField] private WindowData m_WindowData;
    //---------- End of Window Properties -----------

    public IBuildable Initialize(DirtyData data)
    {
        m_Data = data as RoofSectionData;

        m_ProBuilderMesh = GetComponent<ProBuilderMesh>();

        return this;
    }

    public void Build()
    {
        transform.DeleteChildren();

        Vector3[] controlPoints = m_Data.ControlPoints.Clone() as Vector3[];

        if (controlPoints[1] == controlPoints[2])
        {
            m_Data.ControlPoints = new Vector3[] { m_Data.ControlPoints[0], m_Data.ControlPoints[1], m_Data.ControlPoints[3] };
            m_Data.TopPoints = new Vector3[] { m_Data.TopPoints[0], m_Data.TopPoints[1], m_Data.TopPoints[3] };
        }
        else if (controlPoints[0] == controlPoints[3])
        {
            m_Data.ControlPoints = new Vector3[] { m_Data.ControlPoints[0], m_Data.ControlPoints[1], m_Data.ControlPoints[2] };
            m_Data.TopPoints = new Vector3[] { m_Data.TopPoints[0], m_Data.TopPoints[1], m_Data.TopPoints[2] };
        }

        switch (m_Data.RoofElement)
        {
            case RoofElement.Tile:
                m_ProBuilderMesh.CreateShapeFromPolygon(m_Data.ControlPoints, m_Data.SectionHeight, false);
                m_ProBuilderMesh.ToMesh();
                SetCornerPoints();
                m_ProBuilderMesh.Refresh();
                break;
            case RoofElement.Window:
                //List<List<Vector3>> holePoints;
                //Vector3 winScale = new Vector3(m_Data.WindowWidth, m_Data.WindowHeight);
                //ProBuilderMesh polyHoleGrid = MeshMaker.NPolyHoleGrid(m_Data.ControlPoints, winScale, m_Data.WindowColumns, m_Data.WindowRows, m_Data.WindowSides, 0, out holePoints, true);
                //polyHoleGrid.ToMesh();
                //polyHoleGrid.Refresh();
                //Vector3 calculatedNormal = m_Data.ControlPoints.CalculatePolygonFaceNormal();
                //polyHoleGrid.MatchFaceToNormal(calculatedNormal);
                //polyHoleGrid.Extrude(polyHoleGrid.faces, ExtrudeMethod.FaceNormal, m_Data.SectionHeight);
                //polyHoleGrid.ToMesh();
                //Rebuild(polyHoleGrid);
                //SetCornerPoints();
                //m_ProBuilderMesh.Refresh();

                //WindowData windowData = new WindowData(m_WindowData);
                //windowData.SetControlPoints(holePoints[0]);

                //ProBuilderMesh win = ProBuilderMesh.Create();
                //win.name = "Window";
                //win.transform.SetParent(transform, true);
                //Window window = win.AddComponent<Window>();
                //window.Initialize(windowData).Build();
                //win.transform.localPosition = Vector3.zero;
                break;
            case RoofElement.Empty:
                m_ProBuilderMesh.Clear();
                m_ProBuilderMesh.ToMesh();
                break;
        }
    }

    private void SetCornerPoints()
    {
        Vertex[] vertices = m_ProBuilderMesh.GetVertices();
        
        for (int i = 0; i < m_Data.TopPoints.Length; i++)
        {
            // Note: No need to find the points. They are where you left them.
            List<int> shared = m_ProBuilderMesh.GetCoincidentVertices(new int[] { i });

            for(int j = 0; j < shared.Count; j++)
            {
                vertices[shared[j]].position = m_Data.TopPoints[i];
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

    public void Demolish()
    {

    }
}
