using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

public class Pillar : MonoBehaviour, IBuildable
{
    [SerializeField] private ProBuilderMesh m_ProBuilderMesh;
    [SerializeField] private PillarData m_Data;

    public event Action<PillarData> OnDataChange;

    public void OnDataChange_Invoke()
    {
        OnDataChange?.Invoke(m_Data);
    }

    public IBuildable Initialize(IData data)
    {
        m_ProBuilderMesh = GetComponent<ProBuilderMesh>();
        m_Data = data as PillarData;
        return this;
    }

    private void CreateControlPoints()
    {
        float halfWidth = m_Data.Width * 0.5f;
        float halfDepth = m_Data.Depth * 0.5f;

        Vector3[] controlPoints = new Vector3[]
        {
            new Vector3(-halfWidth, -halfDepth),
            new Vector3(-halfWidth, halfDepth),
            new Vector3(halfWidth, halfDepth),
            new Vector3(halfWidth, -halfDepth)
        };

        Quaternion rotation = Quaternion.FromToRotation(Vector3.forward, Vector3.up);

        if (m_Data.Sides != 4)
        {
            controlPoints = MeshMaker.CreateNPolygon(m_Data.Sides, halfWidth, halfDepth);
        }
        // Orientate points to the XZ plane.
        for (int i = 0; i < controlPoints.Length; i++)
        {
            Vector3 euler = rotation.eulerAngles;
            Vector3 v = Quaternion.Euler(euler) * controlPoints[i];
            controlPoints[i] = v;
        }

        m_Data.ControlPoints = controlPoints;
    }

    public void Build()
    {
        CreateControlPoints();
        m_ProBuilderMesh.CreateShapeFromPolygon(m_Data.ControlPoints, 0, false);
        m_ProBuilderMesh.ToMesh();
        Face[] faces = m_ProBuilderMesh.Extrude(m_ProBuilderMesh.faces, ExtrudeMethod.FaceNormal, m_Data.Height);
        m_ProBuilderMesh.ToMesh();

        if (m_Data.IsSmooth)
        {
            Smoothing.ApplySmoothingGroups(m_ProBuilderMesh, faces, 360f / m_Data.Sides);
            m_ProBuilderMesh.ToMesh();
        }

        GetComponent<Renderer>().material = m_Data.Material;
        m_ProBuilderMesh.Refresh();
    }
}
