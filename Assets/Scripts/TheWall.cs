using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class TheWall : MonoBehaviour
{
    [SerializeField, HideInInspector] private Vector3[] m_Points; // control points.

    [SerializeField, Range(0, 5)] private float m_Columns, m_Rows;
    [SerializeField, HideInInspector] private float m_Height, m_Depth;
    [SerializeField] Material m_Material;

    List<IWallElement> m_WallElements;

    public TheWall Initialize(IEnumerable<Vector3> controlPoints)
    {
        m_Points = controlPoints.ToArray();
        return this;
    }

    public TheWall Build()
    {
        if (m_WallElements.Count == 0 || m_Columns == 0 || m_Rows == 0)
        {
            ProBuilderMesh outside = MeshMaker.Cube(m_Points, m_Depth);
            outside.name = "Outside";
            outside.GetComponent<Renderer>().material = m_Material;
            outside.transform.SetParent(transform, true);

            ProBuilderMesh inside = MeshMaker.Quad(m_Points, true);
            inside.name = "Inside";
            inside.GetComponent<Renderer>().material = m_Material;
            inside.transform.SetParent(outside.transform, true);

            return this;
        }

        //for(int i = 0; i < m_WallElements.Count; i++)
        //{
        //    for(int j = 0; j < m_Columns; j++)
        //    {
        //        for(int k = 0; k < m_Rows; k++)
        //        {

        //        }
        //    }
        //}

        return this;
    }

    private void OnDrawGizmosSelected()
    {
        if (m_Points == null)
            return;

        GUIStyle style = new GUIStyle();
        style.fontSize = 18;
        style.normal.textColor = Color.red;

        //for (int i = 0; i < m_Holes.Count; i++)
        //{
        //    Handles.Label(m_Holes[i], i.ToString(), style);
        //}

        //style.normal.textColor = Color.black;

        for (int i = 0; i < m_Points.Length; i++)
        {
            Handles.Label(m_Points[i] + transform.localPosition, i.ToString(), style);
        }

        //Vector3 dirA = m_Holes[0].GetDirectionToTarget(m_Holes[1]);
        //Vector3 dirB = m_Holes[3].GetDirectionToTarget(m_Holes[2]);
        //Vector3 dir = dirA + dirB;
        //Vector3 centre = (m_Holes[0] + m_Holes[1]) / 2;
        //Vector3 forward = Vector3.Cross(Vector3.up, dir);

        //Vector3[] points = new Vector3[] { centre, Vector3.Scale(centre, forward) };

        //Handles.DrawDottedLine(centre, centre + forward, 0.1f);

    }
}
