using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class etes : MonoBehaviour
{
    //[SerializeField, Range(4, 32)] private int m_Sides = 4;
    ////[SerializeField, Range(0, 1)] private float m_Scale = 0.5f;
    //[SerializeField, Range(0, 0.999f)] private float m_Width = 0.5f;
    //[SerializeField, Range(0, 0.999f)] private float m_Height = 0.5f;
    //[SerializeField, Range(0, 45)] private float m_Angle = 0;
    //[SerializeField, Range(1, 10)] private int m_Columns = 1, m_Rows = 1;

    private void OnDrawGizmosSelected()
    {
        //Vector3[] controlPoints = new Vector3[] { new Vector3(-10.5f, 0, -10.5f), new Vector3(-10.5f, 0, 10.5f), new Vector3(10.5f, 0, 10.5f), new Vector3(10.5f, 0, -10.5f) };

        //List<Vector3[]> points = MeshMaker.HoleGrid0(controlPoints, Vector3.one * scale, 1, 1, side);

        //Handles.color = Color.white;
        //foreach (Vector3[] vertices in points)
        //{
        //    Handles.DrawPolyLine(vertices);
        //    Handles.DrawPolyLine(vertices[0], vertices[^1]);
        //    for(int i = 0; i < vertices.Length;i++)
        //    {
        //        Handles.Label(vertices[i], (4 + i).ToString());
        //    }
        //}

        //Handles.color = Color.red;
        //Handles.DrawPolyLine(controlPoints);
        //Handles.DrawPolyLine(controlPoints[0], controlPoints[^1]);

        //for (int i = 0; i < controlPoints.Length; i++)
        //{
        //    Handles.Label(controlPoints[i], i.ToString());
        //}

    }

    public void DoTest()
    {
        //Vector3[] controlPoints = new Vector3[] { new Vector3(-10.5f, -10.5f, 0), new Vector3(-10.5f, 10.5f, 0), new Vector3(10.5f, 10.5f, 0), new Vector3(10.5f, -10.5f, 0) };

        //for(int i = 0; i < controlPoints.Length; i++)
        //{
        //    controlPoints[i] += transform.position;
        //}

        //MeshMaker.NPolyHoleGrid(controlPoints, new Vector3(m_Width, m_Height, 0), m_Columns, m_Rows, m_Sides, m_Angle);
    }

}
