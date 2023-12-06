using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder;

[System.Serializable]
public class PillarData : IData
{
    [SerializeField, HideInInspector] private int m_ID;
    [SerializeField, HideInInspector] private Vector3[] m_ControlPoints;
    [SerializeField, Range(1, 50)] private float m_Height;
    [SerializeField, Range(0, 10)] private float m_Width;
    [SerializeField, Range(0, 10)] private float m_Depth;
    [SerializeField, Range(3, 32)] private int m_Sides;
    [SerializeField] private bool m_IsSmooth;
    [SerializeField, HideInInspector] private Material m_Material;

    public int ID { get { return m_ID; } set { m_ID = value; } }
    public Vector3[] ControlPoints { get { return m_ControlPoints; } set { m_ControlPoints = value; } }
    public float Width { get { return m_Width; } set { m_Width = value; } }
    public float Height { get { return m_Height; } set { m_Height = value; } }
    public float Depth { get { return m_Depth; } set { m_Depth = value; } }
    public int Sides { get { return m_Sides; } set { m_Sides = value; } }
    public bool IsSmooth { get { return m_IsSmooth; } set { m_IsSmooth = value; } }
    public Material Material { get { return m_Material; } set { m_Material = value; } }

    public PillarData() : this(null, 0.5f, 0.5f, 0.5f, 4, null)
    {
    }
    public PillarData(PillarData data):this (data.ControlPoints, data.Width, data.Height, data.Depth, data.Sides, data.Material, data.IsSmooth)
    {
    }
    public PillarData(IEnumerable<Vector3> controlPoints, float width, float height, float depth, int sides, Material material, bool isSmooth = false)
    {
        m_ControlPoints = controlPoints == null ? new Vector3[0] : controlPoints.ToArray();
        m_Width = width;
        m_Height = height;
        m_Depth = depth;
        m_Sides = sides;
        m_IsSmooth = isSmooth;
        m_Material = material;
    }
}
