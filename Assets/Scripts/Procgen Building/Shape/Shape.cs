using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

/// <summary>
/// The shape class contains data displayed in the inspector.
/// The user will adjust this data to configure the shape of the opening
/// </summary>

namespace OnlyInvalid.ProcGenBuilding.Common
{

    [System.Serializable]
    public class Shape
    {
        // Do we want columns & rows?

        [SerializeField] float m_Height, m_Width;
        [SerializeField] float m_Angle;
        [SerializeField] Vector2 m_Position;

        public float Height { get { return m_Height; } set { m_Height = value; } }
        public float Width { get { return m_Width; } set { m_Width = value; } }
        public Vector2 Position { get { return m_Position; } set { m_Position = value; } }

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
}
