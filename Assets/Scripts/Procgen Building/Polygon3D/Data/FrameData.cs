using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace OnlyInvalid.ProcGenBuilding.Polygon3D
{
    [System.Serializable]
    public class FrameData : Polygon3DAData
    {
        [SerializeField] protected float m_InsideScale;
        [SerializeField] bool m_IsHoleDirty;
        public float InsideScale { get { return m_InsideScale; } set { m_InsideScale = value; } }
        public bool IsHoleDirty { get { return m_IsHoleDirty; } set { m_IsHoleDirty = value; } }

        #region Constructors
        public FrameData() : this(0.95f)
        {
            m_IsHoleDirty = false;
        }
        public FrameData(float scale) : base()
        {
            m_InsideScale = scale;
            m_IsHoleDirty = false;

        }
        public FrameData(Shape exteriorShape, List<Polygon2DData> interiorShapes, float frameScale, float depth, Vector3 position, Vector3 eulerAngle, Vector3 scale) : base(position, eulerAngle, scale, exteriorShape, interiorShapes, depth)
        {
            m_InsideScale = frameScale;
        }
        public FrameData(FrameData data) : this(data.ExteriorShape, data.InteriorShapes, data.InsideScale, data.Depth, data.Position, data.EulerAngle, data.Scale)
        {
            m_IsHoleDirty = false;
        }
        #endregion

        public new object Clone()
        {
            FrameData clone = base.Clone() as FrameData;
            clone.InsideScale = m_InsideScale;
            clone.IsDirty = m_IsHoleDirty;
            clone.IsHoleDirty = m_IsHoleDirty;
            return clone;
        }

        #region overrides

        public override Vector3 Normal()
        {
            return Vector3.forward;
        }

        public override bool Equals(object obj)
        {
            FrameData other = obj as FrameData;

            if (other == null)
                return false;

            if (m_InsideScale == other.InsideScale && base.Equals(obj))
            {
                return true;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion
    }
}
