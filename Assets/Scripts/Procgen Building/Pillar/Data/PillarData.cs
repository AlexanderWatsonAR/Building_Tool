using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.ProBuilder;
using OnlyInvalid.ProcGenBuilding.Common;

namespace OnlyInvalid.ProcGenBuilding.Pillar
{
    [System.Serializable]
    public class PillarData : Polygon3DAData
    {
        #region Members
        [SerializeField] int m_ID;
        [SerializeField] bool m_IsSmooth;
        #endregion

        #region Accessors
        public int ID { get { return m_ID; } set { m_ID = value; } }
        public bool IsSmooth { get { return m_IsSmooth; } set { m_IsSmooth = value; } }
        #endregion

        #region Constructors
        public PillarData() : base()
        {
            m_ExteriorShape = new NPolygon(4);
            m_InteriorShapes = null;

        }
        public PillarData(Vector3 position, Vector3 eulerAngle, Vector3 scale, float height, int sides = 4) : base(position, Vector3.zero, scale, new NPolygon(sides), null, height)
        {
            Quaternion upRotation = Quaternion.FromToRotation(Vector3.forward, Vector3.up);

            m_EulerAngle = (Quaternion.Euler(eulerAngle) * upRotation).eulerAngles;
        }
        public PillarData(PillarData data) : base(data)
        {
            m_IsSmooth = data.IsSmooth;
        }
        #endregion

        public void SetDirection(Vector3 direction)
        {
            Quaternion eulerAngle = Quaternion.FromToRotation(Vector3.forward, direction);
            Quaternion upRotation = Quaternion.FromToRotation(Vector3.forward, Vector3.up);

            m_EulerAngle = (eulerAngle * upRotation).eulerAngles;
        }

        public void SetEulerAngle(Vector3 angle)
        {
            Quaternion upRotation = Quaternion.FromToRotation(Vector3.forward, Vector3.up);
            m_EulerAngle = (Quaternion.Euler(angle) * upRotation).eulerAngles;
        }

        public void SetPosition(Vector3 position)
        {
            m_Position = position;
        }

        public override bool Equals(object obj)
        {
            if (obj is not PillarData) return false;
            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
