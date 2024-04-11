using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace OnlyInvalid.ProcGenBuilding.Polygon3D
{
    [System.Serializable]
    public class FrameData : Polygon3DData
    {
        [SerializeField, Range(0, 0.999f)] float m_Scale;

        public float Scale { get { return m_Scale; } set { m_Scale = value; } }

        public FrameData() : this(0.95f)
        {
        }

        public FrameData(float scale) : base()
        {
            m_Scale = scale;
        }

        public FrameData(PolygonData polygon, PolygonData[] holes, Vector3 normal, Vector3 up, float depth, float scale) : base(polygon, holes, normal, up, depth)
        {
            m_Scale = scale;
        }

        public FrameData(FrameData data) : this(data.Polygon, data.Holes, data.Normal, data.Up, data.Depth, data.Scale)
        {

        }

        public override bool Equals(object obj)
        {
            FrameData other = obj as FrameData;

            if (other == null)
                return false;

            if (m_Scale == other.Scale && base.Equals(obj))
            {
                return true;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
