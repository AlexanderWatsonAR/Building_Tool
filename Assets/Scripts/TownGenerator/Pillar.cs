using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

public class Pillar : MonoBehaviour 
{
    [SerializeField] private ProBuilderMesh m_ProBuilderMesh;
    [SerializeField] private Vector3[] m_ControlPoints;
    [SerializeField] private float m_Width;
    [SerializeField] private float m_Height;
    [SerializeField] private float m_Depth;
    [SerializeField] private int m_Sides;
    [SerializeField] private bool m_IsSmooth;

    public Pillar Initialize()
    {
        m_Height = 5;
        m_Width = 0.5f;
        m_Height = 0.5f;
        m_Sides = 4;
        return this;
    }

    private void CreateControlPoints()
    {
        float halfWidth = m_Width * 0.5f;
        float halfDepth = m_Depth * 0.5f;

        Vector3[] controlPoints = new Vector3[]
        {
            new Vector3(-halfWidth, -halfDepth),
            new Vector3(-halfWidth, halfDepth),
            new Vector3(halfWidth, halfDepth),
            new Vector3(halfWidth, -halfDepth)
        };

        Quaternion rotation = Quaternion.FromToRotation(Vector3.forward, Vector3.up);

        if (m_Sides != 4)
        {
            controlPoints = MeshMaker.CreateNPolygon(m_Sides, halfWidth, halfDepth);
        }

        for (int i = 0; i < controlPoints.Length; i++)
        {
            Vector3 euler = rotation.eulerAngles;
            Vector3 v = Quaternion.Euler(euler) * controlPoints[i];
            controlPoints[i] = v;
        }

        m_ControlPoints = controlPoints;
    }

    public Pillar Build()
    {
        CreateControlPoints();
        m_ProBuilderMesh.CreateShapeFromPolygon(m_ControlPoints, 0, false);
        m_ProBuilderMesh.ToMesh();
        Face[] faces = m_ProBuilderMesh.Extrude(m_ProBuilderMesh.faces, ExtrudeMethod.FaceNormal, m_Height);

        if (m_IsSmooth)
        {
            Smoothing.ApplySmoothingGroups(m_ProBuilderMesh, faces, 360f / m_Sides);
            m_ProBuilderMesh.ToMesh();
        }

        m_ProBuilderMesh.Refresh();

        return this;
    }
}
