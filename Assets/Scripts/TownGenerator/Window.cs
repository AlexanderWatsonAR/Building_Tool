using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

public class Window : MonoBehaviour
{
    [SerializeField] private Vector3[] m_ControlPoints;
    [SerializeField] private ProBuilderMesh m_OuterFrame;
    [SerializeField] private ProBuilderMesh m_GridFrame;
    [SerializeField] private ProBuilderMesh m_Pane;
    [SerializeField, Range(1, 5)] private int m_Columns, m_Rows;
    [SerializeField] private float m_OuterFrameDepth, m_GridFrameDepth, m_PaneDepth;
    [SerializeField, Range(0, 0.999f)] private float m_OuterFrameScale, m_GridFrameScale;
    [SerializeField] private Material m_FrameMaterial, m_PaneMaterial;

    [SerializeField] private Vector3[] m_PanePoints;
    [SerializeField] private Vector3 m_Forward;

    [SerializeField] private float m_Height, m_Width;
    [SerializeField] private Vector3 m_Min, m_Max;

    public Window Initialize(IEnumerable<Vector3> controlPoints)
    {
        return Initialize(controlPoints, m_GridFrameDepth);
    }
    public Window Initialize(IEnumerable<Vector3> controlPoints, float depth)
    {
        m_ControlPoints = controlPoints == null ? m_ControlPoints : controlPoints.ToArray();
        m_Columns = 2;
        m_Rows = 2;
        m_GridFrameScale = 0.95f;
        m_GridFrameDepth = depth;
        m_PaneDepth = depth*0.5f;
        m_FrameMaterial = BuiltinMaterials.defaultMaterial;
        m_PaneMaterial = BuiltinMaterials.defaultMaterial;
        m_PanePoints = new Vector3[m_ControlPoints.Length];

        MeshMaker.MinMax(controlPoints, out m_Min, out m_Max);
        m_Height = m_Max.y - m_Min.y;
        m_Width = m_Max.x - m_Min.x + (m_Max.z - m_Min.z);

        return this;
    }

    public Window SetMaterials(Material frame, Material pane)
    {
        frame = frame != null ? frame : BuiltinMaterials.defaultMaterial;
        pane = pane != null ? pane : BuiltinMaterials.defaultMaterial;

        m_FrameMaterial = frame;
        m_PaneMaterial = pane;
        return this;
    }

    public void SetFramePosition(Vector3 position, bool localiseVertices = true)
    {
        if(m_GridFrame != null)
        {
            m_GridFrame.transform.position = position;

            if(localiseVertices)
            {
                m_GridFrame.LocaliseVertices();
            }
        }
    }

    public Window SetFrameGrid(int cols, int rows)
    {
        m_Columns = cols;
        m_Rows = rows;
        return this;
    }

    public Window SetFrameScale(float scale)
    {
        m_GridFrameScale = scale;
        return this;
    }

    public Window SetControlPoints(IEnumerable<Vector3> controlPoints)
    {
        m_ControlPoints = controlPoints == null ? m_ControlPoints : controlPoints.ToArray();
        return this;
    }

    public Window Build()
    {
        transform.DeleteChildren();

        // Grid Frame
        m_GridFrame = MeshMaker.PolyFrameGrid(m_ControlPoints, m_Height, m_Width, m_GridFrameScale, m_Columns, m_Rows);
        ProBuilderMesh gridCover = Instantiate(m_GridFrame);
        gridCover.faces[0].Reverse();
        m_GridFrame.Extrude(new Face[] { m_GridFrame.faces[0] }, ExtrudeMethod.FaceNormal, m_GridFrameDepth);
        CombineMeshes.Combine(new ProBuilderMesh[] { m_GridFrame, gridCover }, m_GridFrame);
        DestroyImmediate(gridCover.gameObject);
        m_GridFrame.transform.SetParent(transform, true);
        m_GridFrame.name = "Inside Frame";
        m_GridFrame.GetComponent<Renderer>().sharedMaterial = m_FrameMaterial;

        //// Pane
        //m_Pane = MeshMaker.Cube(m_PanePoints, m_GridFrameDepth * 0.5f);
        //m_Pane.transform.SetParent(transform, true);
        //m_Pane.name = "Pane";
        //m_Pane.GetComponent<Renderer>().sharedMaterial = m_PaneMaterial;

        return this;
    }
}
