using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class OpeningData : ICloneable
{
    [SerializeField, Range(0, 0.999f)] protected float m_Height, m_Width;
    [SerializeField, Range(1, 5)] protected int m_Columns, m_Rows;

    public float Height { get { return m_Height; } set { m_Height = value; } }
    public float Width { get { return m_Width; } set { m_Width = value; } }
    public int Columns { get { return m_Columns; } set { m_Columns = value; } }
    public int Rows { get { return m_Rows; } set { m_Rows = value; } }

    public OpeningData() : this (0.5f, 0.5f, 1, 1)
    {

    }
    public OpeningData(float height, float width, int columns, int rows)
    {
        m_Height = height;
        m_Width = width;
        m_Columns = columns;
        m_Rows = rows;
    }

    public OpeningData(OpeningData data) : this(data.Height, data.Width, data.Columns, data.Rows)
    {

    }

    public object Clone()
    {
        return MemberwiseClone();
    }

}
