using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace OnlyInvalid.ProcGenBuilding.Common
{
    [System.Serializable]
    public class OpeningData : DirtyData, ICloneable
    {
        #region Members
        [SerializeReference] Shape m_Shape;
        [SerializeReference] Polygon3D.Polygon3D m_Polygon3D;

        [SerializeField] string m_Name;
        [SerializeField] bool m_IsActive;
        
        [SerializeField] Vector2 m_Position;
        [SerializeField] float m_Angle;
        [SerializeField] Vector2 m_Scale;
        #endregion

        #region Accessors
        public bool IsActive { get => m_IsActive; set => m_IsActive = value; }
        public string Name { get => m_Name; set => m_Name = value; }
        public Vector2 Position { get { return m_Position; } set { m_Position = value; } }
        public float Angle { get { return m_Angle; } set { m_Angle = value; } }
        public Vector2 Scale { get { return m_Scale; } set { m_Scale = value; } }
        public Shape Shape { get { return m_Shape; } set { m_Shape = value; } }
        public bool HasContent => m_Polygon3D != null;
        public Polygon3D.Polygon3D Polygon3D { get { return m_Polygon3D; } set { m_Polygon3D = value; } }
        #endregion

        public OpeningData (Shape shape)
        {
            m_Name = "Opening";
            m_Shape = shape.Clone() as Shape;
            m_Position = Vector2.zero;
            m_Angle = 0;
            m_Scale = Vector2.one;
            m_IsActive = true;
        }
        public OpeningData (Shape shape, Polygon3D.Polygon3D buildable) : this (shape)
        {
            m_Polygon3D = buildable;
        }

        public void RemoveContent()
        {
            m_Polygon3D.Demolish();
            m_Polygon3D = null;
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
