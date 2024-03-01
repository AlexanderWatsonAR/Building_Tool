using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CornerData : IData
{
    [SerializeField, HideInInspector] int m_ID;
    [SerializeField, HideInInspector] Vector3[] m_CornerPoints;
    [SerializeField, HideInInspector] Vector3[] m_FlatPoints;
    [SerializeField, HideInInspector] float m_Height;
    [SerializeField, HideInInspector] bool m_IsInside;
    [SerializeField] CornerType m_Type;
    [SerializeField, Range(3, 18)] int m_Sides;

    public int ID { get { return m_ID; } set { m_ID = value; } }
    public Vector3[] CornerPoints { get { return m_CornerPoints; } set { m_CornerPoints = value; } }
    public Vector3[] FlatPoints { get { return m_FlatPoints; } set { m_FlatPoints = value; } }
    public float Height { get { return m_Height; } set { m_Height = value; } }
    public bool IsInside { get { return m_IsInside; } set { m_IsInside = value; } }
    public CornerType Type { get { return m_Type; } set { m_Type = value; } }
    public int Sides { get { return m_Sides; } set { m_Sides = value; } }

    public CornerData() : this (new Vector3[0], new Vector3[0], CornerType.Point, 4)
    {

    }

    public CornerData(Vector3[] controlPoints, Vector3[] flatPoints, CornerType type, int sides)
    {
        m_CornerPoints = controlPoints;
        m_FlatPoints = flatPoints;
        m_Type = type;
        m_Sides = sides;
    }

    public CornerData(CornerData data) : this(data.CornerPoints, data.FlatPoints, data.Type, data.Sides)
    {

    }

    public override bool Equals(object obj)
    {
        if (obj is not CornerData)
            return false;

        CornerData corner = obj as CornerData;

        if(m_Sides == corner.Sides &&
           m_Type == corner.Type)
            return true;

        return false;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
