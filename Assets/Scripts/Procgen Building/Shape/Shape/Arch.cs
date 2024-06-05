using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Arch : CalculatedShape
{
    [SerializeField, Range(0, 1)] float m_Height;
    [SerializeField, Range(3, 16)] int m_Sides;

    public float Height => m_Height;
    public int Sides => m_Sides;

    public Arch() : this(0.75f, 5)
    {

    }

    public Arch(float archHeight, int archSides)
    {
        m_Height = archHeight;
        m_Sides = archSides;
    }

    public override Vector3[] ControlPoints()
    {
        List<Vector3> controlPoints = new List<Vector3>();

        // TODO: apply height

        //controlPoints.Add(new Vector3(-0.5f, -0.5f));
        //controlPoints.AddRange(Vector3Extensions.QuadraticLerpCollection(new Vector3(-0.5f, 0.75f), new Vector3(0, 1), new Vector3(0.5f, 0.75f), m_ArchSides));
        //controlPoints.Add(new Vector3(0.5f, -0.5f));

        return controlPoints.ToArray();
    }
}


