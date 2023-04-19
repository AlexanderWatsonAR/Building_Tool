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
    [SerializeField, HideInInspector] private Vector3[,] m_SubPoints; // Grid points, based on control points, columns & rows.
    [SerializeField, Range(1, 10)] private int m_Columns, m_Rows;
    [SerializeField, HideInInspector] private float m_Height, m_Depth;
    [SerializeField] Material m_Material;

    ProBuilderMesh[,] m_WallSections;

    private Vector3[,] SubPoints
    {
        get
        {
            if (m_Columns <= 0 && m_Rows <= 0) return null;

            if (m_SubPoints == null)
                m_SubPoints = CalculateGrid(m_Points, m_Columns, m_Rows);

            if (m_SubPoints.GetLength(0) == m_Columns &&
                m_SubPoints.GetLength(1) == m_Rows)
                return m_SubPoints;


            m_SubPoints = CalculateGrid(m_Points, m_Columns, m_Rows);

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
        m_WallSections = new ProBuilderMesh[m_Columns, m_Rows];

        return this;
    }

    public Wall Build()
    {
        Vector3[,] subPoints = SubPoints;

        transform.DeleteChildren();

        for (int i = 0; i < m_Columns; i++)
        {
            for (int j = 0; j < m_Rows; j++)
            {
                Vector3 first = subPoints[i + 0, j + 0];
                Vector3 second = subPoints[i + 0, j + 1];
                Vector3 third = subPoints[i + 1, j + 1];
                Vector3 fourth = subPoints[i + 1, j + 0];

                Vector3[] points = new Vector3[] { first, second, third, fourth };

                ProBuilderMesh wallSection = ProBuilderMesh.Create();
                wallSection.name = "Wall Section " + i.ToString() + " " + j.ToString();
                wallSection.GetComponent<Renderer>().sharedMaterial = m_Material;
                
                wallSection.transform.SetParent(transform, true);
                wallSection.AddComponent<WallSection>().Initialize(points, m_Depth).Build();
            }
        }

        return this;
    }

    private Vector3[,] CalculateGrid(Vector3[] controlPoints, int cols, int rows)
    {
        Vector3[,] calculateGrid = new Vector3[cols + 1, rows + 1];

        Vector3[] leftPoints = Vector3Extensions.LerpCollection(controlPoints[0], controlPoints[1], rows + 1).ToArray(); // row start points
        Vector3[] rightPoints = Vector3Extensions.LerpCollection(controlPoints[3], controlPoints[2], rows + 1).ToArray(); // row end points

        for (int i = 0; i < m_Rows + 1; i++)
        {
            Vector3[] points = Vector3Extensions.LerpCollection(leftPoints[i], rightPoints[i], cols + 1);

            for (int j = 0; j < points.Length; j++)
            {
                calculateGrid[j, i] = points[j];
            }
        }

        return calculateGrid;
    }

    private void OnDrawGizmosSelected()
    {
        if (m_Points == null)
            return;

        Handles.DrawAAPolyLine(m_Points);
        Handles.DrawAAPolyLine(m_Points[0], m_Points[^1]);

        if (m_Rows != 0 && m_Columns != 0)
        {
            Vector3[,] subPoints = SubPoints; // cols by rows

            for (int i = 0; i < m_Columns; i++)
            {
                for (int j = 0; j < m_Rows; j++)
                {
                    Vector3 first = subPoints[i + 0, j + 0];
                    Vector3 second = subPoints[i + 0, j + 1];
                    Vector3 third = subPoints[i + 1, j + 1];
                    Vector3 fourth = subPoints[i + 1, j + 0];

                    Handles.DrawAAPolyLine(first, second, third, fourth);

                }
            }
        }
    }
}
