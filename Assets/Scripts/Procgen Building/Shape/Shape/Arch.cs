using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Arch : CalculatedShape
{
    [SerializeField, Range(0, 1)] float m_TopHeight, m_BottomHeight;
    [SerializeField, Range(3, 16)] int m_Sides;

    public float TopHeight => m_TopHeight;
    public float BottomHeight => m_BottomHeight;
    public int Sides => m_Sides;

    public Arch() : this(0.75f, 0.5f, 5)
    {

    }

    public Arch(float archHeight, float bottomHeight, int archSides)
    {
        m_TopHeight = archHeight;
        m_BottomHeight = bottomHeight;
        m_Sides = archSides;
    }

    public override Vector3[] ControlPoints()
    {
        List<Vector3> controlPoints = new List<Vector3>();

        // TODO: apply height

        controlPoints.Add(new Vector3(-1, -1)); // bl
        controlPoints.AddRange(Vector3Extensions.QuadraticLerpCollection(new Vector3(-1, -1 + m_BottomHeight), new Vector3(0, -1 + m_BottomHeight + m_TopHeight), new Vector3(1, -1 + m_BottomHeight), m_Sides));
        controlPoints.Add(new Vector3(1, -1)); // br

        //Vector3[] points = m_Height == 0 ? MeshMaker.Square() : controlPoints.ToArray();

        return controlPoints.ToArray();
    }
}


