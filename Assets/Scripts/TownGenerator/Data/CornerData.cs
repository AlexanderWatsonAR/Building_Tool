using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CornerData : IData
{
    [SerializeField] private CornerType m_Type;
    [SerializeField] private int m_Sides;

    public CornerType Type => m_Type;
    public int Sides => m_Sides;

    public CornerData() : this (CornerType.Point, 4)
    {

    }

    public CornerData(CornerType type, int sides)
    {
        m_Type = type;
        m_Sides = sides;
    }

    public CornerData(CornerData data) : this(data.Type, data.Sides)
    {

    }
}
