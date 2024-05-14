using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

/// <summary>
/// The shape class contains data displayed in the inspector.
/// The user will adjust this data to configure the shape of the opening
/// </summary>


public class Shape
{
    [SerializeField] float m_Height, m_Width;
    [SerializeField] Vector2 m_Position;

    public float Height { get { return m_Height; } set { m_Height = value; } }
    public float Width { get { return m_Width; } set { m_Width = value; } }

    public Shape() : this(0.5f)
    {

    }

    public Shape(float size)
    {
        m_Height = size;
        m_Width = size;
        m_Position = Vector2.zero;
    }

    public Shape(float height, float width)
    {
        m_Height = height;
        m_Width = width;
        m_Position = Vector2.zero;
    }
}

public class NPolygon : Shape
{
    [SerializeField] int m_Sides;

    public int Sides { get { return m_Sides } set { m_Sides = value; } }

    public NPolygon() : this(4)
    {

    }

    public NPolygon(int sides) : base()
    {
        m_Sides = sides;
    }
}

public class Arch : Shape
{
    [SerializeField] float m_ArchHeight;
    [SerializeField] int m_ArchSides;
}

public enum ShapeType
{
    Polygon, Arch
}

public class Opening
{
    [SerializeField] Shape m_Shape;
    [SerializeField] ShapeType m_ShapeType;
}