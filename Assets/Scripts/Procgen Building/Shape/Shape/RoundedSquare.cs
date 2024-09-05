using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoundedSquare : CalculatedShape
{
    [SerializeField] int m_NumberOfSides = 20;
    [SerializeField] float m_CurveSize = 0.5f;

    public int Sides { get => m_NumberOfSides; set => m_NumberOfSides = value; }
    public float CurveSize { get => m_CurveSize; set => m_CurveSize = value; }

    public RoundedSquare(int numberOfSides, float curveSize)
    {
        m_NumberOfSides = numberOfSides;
        m_CurveSize = curveSize;
    }

    public RoundedSquare() : this(5, 10)
    {
    }

    public override Vector3[] ControlPoints()
    {
        return PolygonMaker.RoundedSquare(m_CurveSize, m_NumberOfSides);
    }
}
