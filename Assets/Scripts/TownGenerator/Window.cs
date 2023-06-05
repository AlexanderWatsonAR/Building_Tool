using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

public class Window : MonoBehaviour
{
    [SerializeField] private Vector3[] m_ControlPoints;
    [SerializeField] private ProBuilderMesh m_Frame;
    [SerializeField] private ProBuilderMesh m_Pane;
    [SerializeField, Range(1, 10)] private int m_Columns, m_Rows;
    [SerializeField] private float m_FrameDepth, m_PaneDepth;
    [SerializeField] private Vector3 m_FrameScale;
    [SerializeField] private Material m_FrameMaterial, m_PaneMaterial;

    [SerializeField] private Vector3[] m_PanePoints;
    [SerializeField] private Vector3 m_Forward;

    public Window Initialize(IEnumerable<Vector3> controlPoints, float depth)
    {
        m_ControlPoints = controlPoints == null ? m_ControlPoints : controlPoints.ToArray();
        m_Columns = 2;
        m_Rows = 2;
        m_FrameScale = Vector3.one * 0.95f;
        m_FrameDepth = depth;
        m_PaneDepth = depth*0.5f;
        m_FrameMaterial = BuiltinMaterials.defaultMaterial;
        m_PaneMaterial = BuiltinMaterials.defaultMaterial;
        m_PanePoints = new Vector3[m_ControlPoints.Length];

        Vector3 dir = m_ControlPoints[0].DirectionToTarget(m_ControlPoints[3]);
        Vector3 forward = Vector3.Cross(Vector3.up, dir);

        for (int i = 0; i < m_ControlPoints.Length; i++)
        {
            m_PanePoints[i] = m_ControlPoints[i] + (forward * (m_PaneDepth*0.5f));
        }

        return this;
    }

    public Window SetFrameGrid(int cols, int rows)
    {
        m_Columns = cols;
        m_Rows = rows;
        return this;
    }

    public Window SetFrameScale(Vector3 scale)
    {
        m_FrameScale = scale;
        return this;
    }

    public Window Build()
    {
        transform.DeleteChildren();
        // Frame
        m_Frame = MeshMaker.HoleGrid(m_ControlPoints, Vector3.zero, 0, m_FrameScale, m_Columns, m_Rows, out _);
        m_Frame.Extrude(new Face[] { m_Frame.faces[0] }, ExtrudeMethod.FaceNormal, m_FrameDepth);
        ProBuilderMesh holeGrid = MeshMaker.HoleGrid(m_ControlPoints, Vector3.zero, 0, m_FrameScale, m_Columns, m_Rows, out _, true);
        CombineMeshes.Combine(new ProBuilderMesh[] { m_Frame, holeGrid }, m_Frame);
        DestroyImmediate(holeGrid.gameObject);
        m_Frame.transform.SetParent(transform, true);
        m_Frame.name = "Frame";
        m_Frame.GetComponent<Renderer>().sharedMaterial = m_FrameMaterial;

        // Pane
        m_Pane = MeshMaker.Cube(m_PanePoints, m_FrameDepth * 0.5f);
        m_Pane.transform.SetParent(transform, true);
        m_Pane.name = "Pane";
        m_Pane.GetComponent<Renderer>().sharedMaterial = m_PaneMaterial;

        return this;
    }
    private void Rebuild(ProBuilderMesh member, ProBuilderMesh mesh)
    {
        member.RebuildWithPositionsAndFaces(mesh.positions, mesh.faces);
        member.ToMesh();
        member.Refresh();
        DestroyImmediate(mesh.gameObject);
    }
}
