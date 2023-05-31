using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.UIElements;
using ProMaths = UnityEngine.ProBuilder.Math;

public class Door : MonoBehaviour
{
    [SerializeField, HideInInspector] private Vector3[] m_ControlPoints;
    private Vector3[] m_ScaledControlPoints;
    [SerializeField, HideInInspector] private ProBuilderMesh m_ProBuilderMesh;
    [SerializeField, HideInInspector] private float m_Depth;
    [SerializeField, HideInInspector] private Vector3 m_Centre;
    [SerializeField, HideInInspector] private Vector3 m_Forward;
    [SerializeField] private Material m_Material;
    [SerializeField, Range(0, 1)] private float m_HeightScale, m_WidthScale, m_DepthScale;


    public Door SetMaterial(Material material)
    {
        m_Material = material;
        GetComponent<Renderer>().material = m_Material;
        return this;
    }

    public Door Initialize(IEnumerable<Vector3> controlPoints, float depth)
    {
        m_ControlPoints = controlPoints == null ? m_ControlPoints : controlPoints.ToArray();
        m_ProBuilderMesh = GetComponent<ProBuilderMesh>();
        m_Depth = depth;

        Vector3 a = m_ControlPoints[0];
        Vector3 b = m_ControlPoints[3];
        Vector3 dir = a.DirectionToTarget(b);
        Vector3 forward = Vector3.Cross(Vector3.up, dir);
        Vector3 c = ProMaths.Average(m_ControlPoints);
        m_Centre = c + (forward * (depth * 0.5f));

        m_HeightScale = 0.9f;
        m_WidthScale = 0.9f;
        m_DepthScale = 0.9f;
        m_Forward = forward;
        return this;
    }

    public Door Build()
    {
        Rebuild(MeshMaker.Cube(m_ControlPoints, m_Depth));

        //float height = Vector3.Distance(m_ControlPoints[0], m_ControlPoints[1]);
        //float width = Vector3.Distance(m_ControlPoints[0], m_ControlPoints[3]);

        Vector3 scale = new Vector3(m_Forward.x * m_WidthScale, m_HeightScale, m_Forward.z * m_DepthScale);

        for (int i = 0; i < m_ProBuilderMesh.faceCount; i++)
        {
            m_ProBuilderMesh.ScaleVertices(m_ProBuilderMesh.faces[i].distinctIndexes, m_Centre, scale);
        }
        return this;
    }


    private void Rebuild(ProBuilderMesh mesh)
    {
        m_ProBuilderMesh.RebuildWithPositionsAndFaces(mesh.positions, mesh.faces);
        m_ProBuilderMesh.ToMesh();
        m_ProBuilderMesh.Refresh();
        DestroyImmediate(mesh.gameObject);
    }

}
