using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PathShape : Shape
{
    [SerializeField] Vector3[] m_Vertices;

    public PathShape()
    {
        m_Vertices = new Vector3[0];
    }

    public PathShape(Vector3[] vertices)
    {
        m_Vertices = vertices;
    }


    public override Vector3[] ControlPoints()
    {
        return m_Vertices;
    }
}
