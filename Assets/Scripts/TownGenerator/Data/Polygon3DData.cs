using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Polygon3DData : IData
{
    [SerializeField] Vector3[] m_ControlPoints;
    [SerializeField] Vector3[][] m_HolePoints;
    [SerializeField] Vector3 m_Normal;
    [SerializeField, Range(0, 0.999f)] float m_Depth;

    public Vector3[] ControlPoints { get { return m_ControlPoints; } set { m_ControlPoints = value; } }
    public Vector3 Normal { get { return m_Normal; } set { m_Normal = value; } }
    public float Depth { get { return m_Depth; } set { m_Depth = value; } }
    public Vector3[][] HolePoints { get { return m_HolePoints; } set { m_HolePoints = value; } }

    public Polygon3DData() : this(null, null, Vector3.forward, 0.1f) // dummy data
    {

    }

    public Polygon3DData(Vector3[] controlPoints, Vector3[][] insidePoints, Vector3 normal, float depth)
    {
        m_ControlPoints = controlPoints;
        m_HolePoints = insidePoints;
        m_Normal = normal;
        m_Depth = depth;
    }

    public Polygon3DData(Polygon3DData data)
    {
        m_ControlPoints = data.ControlPoints;
        m_HolePoints = data.HolePoints;
        m_Normal = data.Normal;
        m_Depth = data.Depth;
    }

    
}
