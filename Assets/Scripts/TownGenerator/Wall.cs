using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.ProBuilder;
using System.Linq;
using UnityEditor;

public class Wall : MonoBehaviour
{
    [SerializeField] WallData m_Data;
    private List<Vector3[]> m_SubPoints; // Grid points, based on control points, columns & rows.

    private List<Vector3[]> SubPoints
    {
        get
        {
            if (m_Data.Columns <= 0 && m_Data.Rows <= 0) return null;

            m_SubPoints = MeshMaker.CreateGridFromControlPoints(m_Data.ControlPoints, m_Data.Columns, m_Data.Rows);

            return m_SubPoints;
        }
    }

    public WallData WallData => m_Data;

    public Wall Initialize(WallData data)
    {
        m_Data = new WallData(data);
        return this;
    }

    public Wall Build()
    {
        List<Vector3[]> subPoints = SubPoints;

        transform.DeleteChildren();

        for (int i = 0; i < m_Data.Columns; i++)
        {
            for (int j = 0; j < m_Data.Rows; j++)
            {
                Vector3 first = subPoints[j][i];
                Vector3 second = subPoints[j + 1][i];
                Vector3 third = subPoints[j + 1][i + 1];
                Vector3 fourth = subPoints[j][i + 1];

                Vector3[] points = new Vector3[] { first, second, third, fourth };

                ProBuilderMesh wallSection = ProBuilderMesh.Create();
                wallSection.name = "Wall Section " + i.ToString() + " " + j.ToString();
                wallSection.GetComponent<Renderer>().sharedMaterial = m_Data.Material;
                
                wallSection.transform.SetParent(transform, false);
                wallSection.AddComponent<WallSection>().Initialize(points, m_Data.Depth).Build();
            }
        }

        return this;
    }

    private void OnDrawGizmosSelected()
    {
        //if (m_Points == null)
        //    return;

        //if (m_Rows != 0 && m_Columns != 0)
        //{
        //    List<Vector3[]> subPoints = SubPoints; // cols by rows

        //    for (int i = 0; i < m_Columns; i++)
        //    {
        //        for (int j = 0; j < m_Rows; j++)
        //        {
        //            Vector3 first = subPoints[i + 0][j + 0];
        //            Vector3 second = subPoints[i + 0][j + 1];
        //            Vector3 third = subPoints[i + 1][j + 1];
        //            Vector3 fourth = subPoints[i + 1][j + 0];

        //            Vector3 dir = first.DirectionToTarget(fourth);
        //            Vector3 cross = Vector3.Cross(Vector3.up, dir) * m_Depth;

        //            first += cross;
        //            second += cross;
        //            third += cross;
        //            fourth += cross;

        //            first += transform.position;
        //            second += transform.position;
        //            third += transform.position;
        //            fourth += transform.position;

        //            Handles.DrawAAPolyLine(first, second, third, fourth);
        //            Handles.DrawAAPolyLine(first, fourth);

        //        }
        //    }
        //}
    }
}
