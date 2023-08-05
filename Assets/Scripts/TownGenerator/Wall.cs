using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.ProBuilder;
using System.Linq;
using UnityEditor;

[System.Serializable]
public class Wall : MonoBehaviour
{
    [SerializeField, HideInInspector] private Vector3[] m_Points; // control points.
    [SerializeField, HideInInspector] private List<Vector3[]> m_SubPoints; // Grid points, based on control points, columns & rows.
    [SerializeField, Range(1, 10)] private int m_Columns, m_Rows;
    [SerializeField, HideInInspector] private float m_Height, m_Depth;
    [SerializeField] Material m_Material;

    public Vector3[] ControlPoints => m_Points;
    public Material Material => m_Material;
    public float Height => m_Height;
    public float Depth => m_Depth;

    private List<Vector3[]> SubPoints
    {
        get
        {
            if (m_Columns <= 0 && m_Rows <= 0) return null;

            m_SubPoints = MeshMaker.CreateGridFromControlPoints(m_Points, m_Columns, m_Rows);

            return m_SubPoints;
        }
    }

    public int Columns => m_Columns;
    public int Rows => m_Rows;

    public Wall Initialize(IEnumerable<Vector3> controlPoints, float depth, Material material)
    {
        m_Columns = 1;
        m_Rows = 1;
        m_Depth = depth;
        m_Material = material;
        m_Points = controlPoints.ToArray();
        return this;
    }

    public Wall Build()
    {
        List<Vector3[]> subPoints = SubPoints;

        transform.DeleteChildren();

        for (int i = 0; i < m_Columns; i++)
        {
            for (int j = 0; j < m_Rows; j++)
            {
                Vector3 first = subPoints[j][i];
                Vector3 second = subPoints[j + 1][i];
                Vector3 third = subPoints[j + 1][i + 1];
                Vector3 fourth = subPoints[j][i + 1];

                Vector3[] points = new Vector3[] { first, second, third, fourth };

                ProBuilderMesh wallSection = ProBuilderMesh.Create();
                wallSection.name = "Wall Section " + i.ToString() + " " + j.ToString();
                wallSection.GetComponent<Renderer>().sharedMaterial = m_Material;
                
                wallSection.transform.SetParent(transform, false);
                wallSection.AddComponent<WallSection>().Initialize(points, m_Depth).Build();
            }
        }

        return this;
    }

    private void OnDrawGizmosSelected()
    {
        if (m_Points == null)
            return;

        if (m_Rows != 0 && m_Columns != 0)
        {
            List<Vector3[]> subPoints = SubPoints; // cols by rows

            for (int i = 0; i < m_Columns; i++)
            {
                for (int j = 0; j < m_Rows; j++)
                {
                    Vector3 first = subPoints[i + 0][j + 0];
                    Vector3 second = subPoints[i + 0][j + 1];
                    Vector3 third = subPoints[i + 1][j + 1];
                    Vector3 fourth = subPoints[i + 1][j + 0];

                    Vector3 dir = first.DirectionToTarget(fourth);
                    Vector3 cross = Vector3.Cross(Vector3.up, dir) * m_Depth;

                    first += cross;
                    second += cross;
                    third += cross;
                    fourth += cross;

                    first += transform.position;
                    second += transform.position;
                    third += transform.position;
                    fourth += transform.position;

                    Handles.DrawAAPolyLine(first, second, third, fourth);
                    Handles.DrawAAPolyLine(first, fourth);

                }
            }
        }
    }
}
