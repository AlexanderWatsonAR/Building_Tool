using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NPolygon : CalculatedShape
{
    [SerializeField] int m_Sides;

    public int Sides { get { return m_Sides; } set { m_Sides = value; } }

    public NPolygon() : this(4)
    {

    }

    public NPolygon(int sides) : base()
    {
        m_Sides = sides;
    }

    public override Vector3[] ControlPoints()
    {
        return MeshMaker.CalculateNPolygon(m_Sides, 1, 1);
    }

}
