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
    [SerializeField, HideInInspector] private ProBuilderMesh m_WallMesh;
    [SerializeField] private Vector3[] m_Points; // control points.
    [SerializeField] private float m_Height, m_Width, m_Depth;
    [SerializeField] private List<Vector3> m_Holes;
    [SerializeField] private bool m_FlipFace;

    public void Init(ProBuilderMesh wallMesh)
    {
        m_Points = wallMesh.positions.ToArray();
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
