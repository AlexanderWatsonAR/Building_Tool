using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

[System.Serializable]
public class Extrudable : MonoBehaviour
{
    [SerializeField, HideInInspector] private ExtrudeMethod m_Method;
    [SerializeField, HideInInspector] private float m_Distance;
    [SerializeField, HideInInspector] private List<Vector3> m_OriginalVertices;
    [SerializeField, HideInInspector] private List<Face> m_OriginalFaces;
    [SerializeField, HideInInspector] private Vector3 m_OriginalPivotPoint; // World Coords

    public event EventHandler OnHaveExtrusionPointsChanged;

    [SerializeField, HideInInspector] protected List<int[]> m_ExtrusionEdgeIndices;
    [SerializeField, HideInInspector] protected ProBuilderMesh m_ProBuilderMesh;
    [SerializeField, HideInInspector] protected bool m_IsInitialized;
    [SerializeField, HideInInspector] protected int m_Steps;

    /// <summary>
    /// Returns extrusion positions in world space.
    /// </summary>
    public Vector3[] ExtrusionPositions
    {
        get
        {
            return GetExtrusionPositions();
        }
    }

    public List<int[]> ExtrusionIndices
    {
        get
        {
            return m_ExtrusionEdgeIndices;
        }
        private set
        {
            m_ExtrusionEdgeIndices = value;
            OnHaveExtrusionPointsChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private Vector3[] GetExtrusionPositions()
    {
        ProBuilderMesh mesh = GetComponent<ProBuilderMesh>();
        IList<Vector3> positions = mesh.positions;

        List<Vector3> extrusionPoints = new List<Vector3>();

        foreach (int[] edgeIndex in m_ExtrusionEdgeIndices)
        {
            List<Vector3> vertices = new List<Vector3>();

            foreach (int i in edgeIndex)
            {
                vertices.Add(transform.TransformPoint(positions[i]));
            }

            extrusionPoints.Add(UnityEngine.ProBuilder.Math.Average(vertices));
        }

        // Gets the position of the start face.
        List<Vector3> verts = new List<Vector3>();

        for (int i = 0; i < m_ProBuilderMesh.faces[0].distinctIndexes.Count; i++)
        {
            verts.Add(transform.TransformPoint(positions[i]));
        }

        extrusionPoints.Add(UnityEngine.ProBuilder.Math.Average(verts));

        return extrusionPoints.ToArray();
    }

    protected virtual void Initialize()
    {
        if (GetComponent<ProBuilderMesh>() == null)
        {
            new MeshImporter(gameObject).Import();
        }

        m_ProBuilderMesh = m_ProBuilderMesh != null ? m_ProBuilderMesh : GetComponent<ProBuilderMesh>();
        m_IsInitialized = true;
    }

    // Generate LOD Group?
    protected void ResetMeshToDefault()
    {
        m_ProBuilderMesh.SetPivot(m_OriginalPivotPoint);
        m_ProBuilderMesh.RebuildWithPositionsAndFaces(m_OriginalVertices, m_OriginalFaces);
        m_ProBuilderMesh.ToMesh();
        m_ProBuilderMesh.Refresh();
    }

    public void ResetToDefualt()
    {
        ResetMeshToDefault();
    }

    public void Extrude()
    {
        ResetMeshToDefault();

        Extrude(m_OriginalFaces, m_Method, m_Distance, m_Steps);
    }

    public List<int[]> Extrude(ExtrudeMethod method, float distance)
    {
        if(!m_IsInitialized)
            this.Initialize();
        return Extrude(m_ProBuilderMesh.faces, method, distance, 0);
    }

    public List<int[]> Extrude(IEnumerable<Face> faces, ExtrudeMethod method, float distance)
    {
        return Extrude(faces, method, distance, 0);
    }

    public List<int[]> Extrude(IEnumerable<Face> faces, ExtrudeMethod method, float distance, int steps)
    {
        if (!m_IsInitialized)
            this.Initialize();
        //m_Faces = faces.ToListPooled();
        m_Method = method;
        m_Distance = distance;
        m_Steps = steps;

        if (m_OriginalVertices != null && m_OriginalVertices.Count == 0)
        {
            m_OriginalVertices = m_ProBuilderMesh.positions.ToListPooled();
            m_OriginalFaces = m_ProBuilderMesh.faces.ToListPooled();
            m_OriginalPivotPoint = m_ProBuilderMesh.transform.position;
        }

        ExtrusionIndices = m_ProBuilderMesh.Extrude(faces, method, distance, steps);

        return m_ExtrusionEdgeIndices;
    }
}
