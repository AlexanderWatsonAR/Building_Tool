using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

namespace OnlyInvalid.ProcGenBuilding.Common
{
    [System.Serializable]
    public class OpeningData
    {
        [SerializeReference] Shape m_Shape;

        // Perhaps put this in a transform class?
        [SerializeField, Range(0, 1)] float m_Height, m_Width;
        [SerializeField, Range(0, 180)] float m_Angle;
        [SerializeField] Vector2 m_Position;
        
        public float Height { get { return m_Height; } set { m_Height = value; } }
        public float Width { get { return m_Width; } set { m_Width = value; } }
        public float Angle { get { return m_Angle; } set { m_Angle = value; } }
        public Vector2 Position { get { return m_Position; } set { m_Position = value; } }
        public Shape Shape { get { return m_Shape; } set { m_Shape = value; } }

        public OpeningData (Shape shape)
        {
            m_Shape = shape;
            m_Height = 0.5f;
            m_Width = 0.5f;
            m_Angle = 0;
            m_Position = new Vector2();
        }

    }
}
