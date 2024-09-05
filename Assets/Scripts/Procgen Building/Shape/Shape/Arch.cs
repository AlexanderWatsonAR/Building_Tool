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

    public Arch(float archHeight, float baseHeight, int archSides)
    {
        m_ArchHeight = archHeight;
        m_BaseHeight = baseHeight;
        m_Sides = archSides;
    }

    public override Vector3[] ControlPoints()
    {
        return PolygonMaker.Arch(m_BaseHeight, m_ArchHeight, m_Sides);
    }
}


