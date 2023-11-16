using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder;

[System.Serializable]
public class PillarData : IData
{
    [SerializeField, HideInInspector] private Vector3[] m_ControlPoints;
    [SerializeField, Range(0, 100)] private float m_Height;
    [SerializeField, Range(0, 10)] private float m_Width;
    [SerializeField, Range(0, 10)] private float m_Depth;
    [SerializeField, Range(3, 32)] private int m_Sides;
    [SerializeField] private bool m_IsSmooth;
    [SerializeField,HideInInspector] private Material m_Material;

    public Vector3[] ControlPoints { get { return m_ControlPoints; } set { m_ControlPoints = value; } }
    public float Width => m_Width;
    public float Height { get { return m_Height; } set { m_Height = value; } }
    public float Depth => m_Depth;
    public int Sides => m_Sides;
    public bool IsSmooth => m_IsSmooth;
    public Material Material { get { return m_Material; } set { m_Material = value; } }

    public PillarData() : this(null, 0.5f, 0.5f, 0.5f, 4, null)
    {
    }
    public PillarData(PillarData data):this (data.ControlPoints, data.Width, data.Height, data.Depth, data.Sides, data.Material, data.IsSmooth)
    {
    }
    public PillarData(IEnumerable<Vector3> controlPoints, float width, float height, float depth, int sides, Material material, bool isSmooth = false)
    {
        m_ControlPoints = controlPoints == null ? new Vector3[0] : ControlPoints.ToArray();
        m_Width = width;
        m_Height = height;
        m_Depth = depth;
        m_Sides = sides;
        m_IsSmooth = isSmooth;
        m_Material = material;
    }
}
