using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

[System.Serializable]
public class FloorData
{
    [SerializeField] private ControlPoint[] m_ControlPoints;
    [SerializeField] private float m_Height;
    [SerializeField, HideInInspector] private Material m_Material;

    public Material Material => m_Material;

    public ControlPoint[] ControlPoints { get { return m_ControlPoints; } set { m_ControlPoints = value; }}
    public float Height => m_Height;

    public FloorData() : this (new ControlPoint[0], 0.1f, null)
    {

    }
    public FloorData(ControlPoint[] controlPoints, float height, Material material)
    {
        m_ControlPoints = controlPoints;
        m_Height = height;
        m_Material = material;
    }

    public FloorData(FloorData data) : this (data.ControlPoints, data.Height, data.Material)
    {

    }



}
