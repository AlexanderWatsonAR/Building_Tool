using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OnlyInvalid.ProcGenBuilding.Common;

namespace OnlyInvalid.ProcGenBuilding.Corner
{
    [System.Serializable]
    public class CornerData : Polygon3DAData
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

        public CornerData() : this(CornerType.Point, 4, Vector3.zero, Vector3.zero, Vector3.zero)
        {

        }

        public CornerData(CornerType type, float height, Vector3 position, Vector3 eulerAngle, Vector3 scale) : base(position, eulerAngle, scale, null, null, height)
        {
            // TODO: Like with the pillar, we need to rotate the shape so it faces up.

            m_Type = type;

            switch(m_Type)
            {
                case CornerType.Point:
                    m_ExteriorShape = new Square();
                    break;
                case CornerType.Round:
                    m_ExteriorShape = new PathShape(PolygonMaker.Quatercircle(m_Sides));
                    break;
                case CornerType.Flat:
                    m_ExteriorShape = new NPolygon(3);
                    break;
            }
        }

        public CornerData(CornerData data) : this(data.Type, data.Height, data.Position, data.EulerAngle, data.Scale)
        {

        }

        public override bool Equals(object obj)
        {
            if (obj is not CornerData)
                return false;

            CornerData corner = obj as CornerData;

            if (m_Sides == corner.Sides &&
               m_Type == corner.Type)
                return true;

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
