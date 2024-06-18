using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

namespace OnlyInvalid.ProcGenBuilding.Common
{
    [System.Serializable]
    public class OpeningData : DirtyData, ICloneable
    {
        [SerializeReference] Shape m_Shape;
        [SerializeReference] Polygon3D.Polygon3D m_Polygon3D;

        [SerializeField] string m_Name;
        [SerializeField, Range(0, 1)] float m_Height, m_Width;
        [SerializeField, Range(0, 180)] float m_Angle;
        [SerializeField] Vector2 m_Position;
        
        public string Name { get => m_Name; set => m_Name = value; }
        public float Height { get { return m_Height; } set { m_Height = value; } }
        public float Width { get { return m_Width; } set { m_Width = value; } }
        public float Angle { get { return m_Angle; } set { m_Angle = value; } }
        public Vector2 Position { get { return m_Position; } set { m_Position = value; } }
        public Shape Shape { get { return m_Shape; } set { m_Shape = value; } }
        public bool HasContent => m_Polygon3D != null;
        public Polygon3D.Polygon3D Polygon3D { get { return m_Polygon3D; } set { m_Polygon3D = value; } }

        public OpeningData (Shape shape)
        {
            m_Name = "Opening";
            m_Shape = shape;
            m_Height = 0.5f;
            m_Width = 0.5f;
            m_Angle = 0;
            m_Position = new Vector2();
        }
        public OpeningData (Shape shape, Polygon3D.Polygon3D buildable) : this (shape)
        {
            m_Polygon3D = buildable;
        }

        /// <summary>
        /// Shallow copy
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            OpeningData clone = MemberwiseClone() as OpeningData;
            return clone;
        }
    }
}
