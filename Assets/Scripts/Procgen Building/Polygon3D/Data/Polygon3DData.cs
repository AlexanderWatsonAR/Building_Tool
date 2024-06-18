using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using OnlyInvalid.ProcGenBuilding.Common;
using Unity.VisualScripting;

namespace OnlyInvalid.ProcGenBuilding.Polygon3D
{
    [System.Serializable]
    public class Polygon3DData : DirtyData, ICloneable
    {
        [SerializeField] PolygonData m_Polygon;
        [SerializeField] PolygonData[] m_Holes;
        [SerializeField] Vector3 m_Normal, m_Up;
        [SerializeField, Range(0, 0.999f)] float m_Depth;

        [SerializeField] float m_Height, m_Width;
        [SerializeField] Vector3 m_Position;

        public PolygonData Polygon { get { return m_Polygon; } set { m_Polygon = value; if (m_Polygon != null) { CalcualateInternal(); } } }
        public PolygonData[] Holes { get { return m_Holes; } set { m_Holes = value; } }
        public Vector3 Normal { get { return m_Normal; } set { m_Normal = value; } }
        public Vector3 Up { get { return m_Up; } set { m_Up = value; } }
        public Vector3 Right { get { return Vector3.Cross(m_Up, m_Normal); } }
        public float Height { get { return m_Height; } set { m_Height = value; } }
        public float Width { get { return m_Width; } set { m_Width = value; } }
        public float Depth { get { return m_Depth; } set { m_Depth = value; } }
        public Vector3 Position { get { return m_Position; } set { m_Position = value; } }

        public Polygon3DData() : this(null, null, Vector3.forward, Vector3.up, 0.1f)
        {

        }

        public Polygon3DData(PolygonData polygon, PolygonData[] holes, Vector3 normal, Vector3 up, float depth)
        {
            m_Polygon = polygon;
            m_Holes = holes;
            m_Normal = normal;
            m_Depth = depth;
            m_Up = up;

            CalcualateInternal();
        }


        public Polygon3DData(Polygon3DData data)
        {
            m_Polygon = data.Polygon;
            m_Holes = data.Holes;
            m_Normal = data.Normal;
            m_Height = data.Height;
            m_Width = data.Width;
            m_Depth = data.Depth;
            m_Position = data.Position;
        }

        private void CalcualateInternal()
        {
            if (m_Polygon == null || m_Polygon.ControlPoints == null || m_Polygon.ControlPoints.Length == 0)
                return;

            m_Polygon.ControlPoints.MinMax(out Vector3 min, out Vector3 max);
            m_Height = max.y - min.y;
            m_Width = max.x - min.x + (max.z - min.z);
            m_Position = Vector3.Lerp(min, max, 0.5f);
        }

        public void SetPolygon(Vector3[] controlPoints, Vector3 normal)
        {
            m_Polygon ??= new PolygonData(controlPoints, normal);
            m_Polygon.ControlPoints = controlPoints;
            m_Polygon.Normal = normal;
            m_Normal = normal;

            CalcualateInternal();
        }


        public void SetHoles(IList<IList<Vector3>> holePoints)
        {
            m_Holes = new PolygonData[holePoints.Count];

            for (int i = 0; i < holePoints.Count; i++)
            {
                IList<Vector3> hole = holePoints[i];
                m_Holes[i] = new PolygonData(hole.ToArray());
            }
        }

        public IList<IList<Vector3>> GetHoles()
        {
            if (m_Holes == null)
                return null;

            IList<IList<Vector3>> holePoints = new List<IList<Vector3>>();

            foreach (PolygonData hole in m_Holes)
            {
                holePoints.Add(hole.ControlPoints);
            }

            return holePoints;
        }

        public override bool Equals(object obj)
        {
            Polygon3DData other = obj as Polygon3DData;

            if (other == null)
                return false;

            if (m_Depth == other.Depth)
                return true;

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public object Clone()
        {
            Polygon3DData clone = MemberwiseClone() as Polygon3DData;
            clone.Polygon = this.Polygon.Clone() as PolygonData;
            if (Holes != null)
                clone.Holes = this.Holes.Clone() as PolygonData[];
            return clone;
        }

    }
}
