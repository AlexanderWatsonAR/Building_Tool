using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PolygonData
{
    [SerializeField] Vector3[] m_ControlPoints;
    [SerializeField] Vector3 m_Normal;

    public Vector3[] ControlPoints { get { return m_ControlPoints; } set { m_ControlPoints = value; } }
    public Vector3 Normal { get { return m_Normal; } set { m_Normal = value; } }

    public PolygonData(Vector3[] controlPoints, Vector3 normal)
    {
        m_ControlPoints = controlPoints;
        m_Normal = normal;
    }
    public PolygonData(Vector3[] controlPoints)
    {
        m_ControlPoints = controlPoints;
        m_Normal = controlPoints.CalculatePolygonFaceNormal();
    }
    public PolygonData(PolygonData data) : this (data.ControlPoints, data.Normal)
    {

    }


}
