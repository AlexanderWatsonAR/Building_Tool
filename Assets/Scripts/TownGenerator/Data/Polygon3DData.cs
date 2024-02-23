using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public class Polygon3DData : IData
{
    [SerializeField] Vector3[] m_ControlPoints;
    [SerializeField] Vector3[][] m_HolePoints;
    [SerializeField] Vector3 m_Normal;
    [SerializeField, Range(0, 0.999f)] float m_Depth;

    [SerializeField] float m_Height, m_Width;
    [SerializeField] Vector3 m_Position;

    public Vector3[] ControlPoints { get { return m_ControlPoints; } set { m_ControlPoints = value; } }
    public Vector3 Normal { get { return m_Normal; } set { m_Normal = value; } }
    public float Depth { get { return m_Depth; } set { m_Depth = value; } }
    public Vector3[][] HolePoints { get { return m_HolePoints; } set { m_HolePoints = value; } }
    public float Height { get { return m_Height; } set { m_Height = value; } }
    public float Width { get { return m_Width; } set { m_Width = value; } }
    public Vector3 Position { get { return m_Position; } set { m_Position = value; } }

    public Polygon3DData() : this(null, null, Vector3.forward, 0, 0, 0.1f, Vector3.zero) // dummy data
    {

    }

    public Polygon3DData(Vector3[] controlPoints, Vector3[][] holePoints, Vector3 normal, float height, float width, float depth, Vector3 position)
    {
        m_ControlPoints = controlPoints;
        m_HolePoints = holePoints;
        m_Normal = normal;
        m_Height = height;
        m_Width = width;
        m_Depth = depth;
        m_Position = position;
    }

    public Polygon3DData(Polygon3DData data)
    {
        m_ControlPoints = data.ControlPoints;
        m_HolePoints = data.HolePoints;
        m_Normal = data.Normal;
        m_Height = data.Height;
        m_Width = data.Width;
        m_Depth = data.Depth;
        m_Position = data.Position;
    }

    
}
