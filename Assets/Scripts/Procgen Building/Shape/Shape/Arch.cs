using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Arch : CalculatedShape
{
    [SerializeField, Range(0, 1)] float m_ArchHeight, m_BaseHeight;
    [SerializeField, Range(2, 16)] int m_Sides;

    public float ArchHeight { get => m_ArchHeight; set => m_ArchHeight = value; }
    public float BaseHeight { get => m_BaseHeight; set => m_BaseHeight = value; }
    public int Sides { get => m_Sides; set => m_Sides = value; }

    public Arch() : this(0.75f, 0.5f, 5)
    {

    }

    public Arch(float archHeight, float bottomHeight, int archSides)
    {
        m_ArchHeight = archHeight;
        m_BaseHeight = bottomHeight;
        m_Sides = archSides;
    }

    public override Vector3[] ControlPoints()
    {
        List<Vector3> controlPoints = new List<Vector3>();

        // TODO: apply height

        controlPoints.Add(new Vector3(-1, -1)); // bl
        controlPoints.AddRange(Vector3Extensions.QuadraticLerpCollection(new Vector3(-1, -1 + m_BaseHeight), new Vector3(0, -1 + m_BaseHeight + m_ArchHeight), new Vector3(1, -1 + m_BaseHeight), m_Sides + 1));
        controlPoints.Add(new Vector3(1, -1)); // br

        //Vector3[] points = m_Height == 0 ? MeshMaker.Square() : controlPoints.ToArray();

        return controlPoints.ToArray();
    }
}


