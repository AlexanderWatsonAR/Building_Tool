using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace OnlyInvalid.ProcGenBuilding.Polygon3D
{
    [System.Serializable]
    public class FrameData : Polygon3DAData
    {
        [SerializeField] float m_FrameScale;
        [SerializeField] bool m_IsHoleDirty;
        public float FrameScale { get { return m_FrameScale; } set { m_FrameScale = value; } }
        public bool IsHoleDirty { get { return m_IsHoleDirty; } set { m_IsHoleDirty = value; } }

        public FrameData() : this(0.95f)
        {
            m_IsHoleDirty = false;
        }

        public FrameData(float scale) : base()
        {
            m_FrameScale = scale;
            m_IsHoleDirty = false;
        }

        public FrameData(Shape exteriorShape, List<Shape> interiorShapes, float frameScale, float depth, Vector3 position, Vector3 eulerAngle, Vector3 scale) : base(position, eulerAngle, scale, exteriorShape, interiorShapes, depth)
        {
            m_FrameScale = frameScale;
        }

        public FrameData(FrameData data) : this(data.ExteriorShape, data.InteriorShapes, data.FrameScale, data.Depth, data.Position, data.EulerAngle, data.Scale)
        {
            m_IsHoleDirty = false;
        }

        public new object Clone()
        {
            FrameData clone = base.Clone() as FrameData;
            clone.FrameScale = m_FrameScale;
            clone.IsDirty = m_IsHoleDirty;
            clone.IsHoleDirty = m_IsHoleDirty;
            return clone;
        }

        public override bool Equals(object obj)
        {
            FrameData other = obj as FrameData;

            if (other == null)
                return false;

            if (m_FrameScale == other.FrameScale && base.Equals(obj))
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
