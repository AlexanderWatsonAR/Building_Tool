using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using HandleUtil = UnityEditor.HandleUtility;
using UnityEngine.Events;
using System.Linq;
using System;

namespace OnlyInvalid.ProcGenBuilding.Common
{

    [System.Serializable]
    public class PlanarPath : Path
    {
        #region Members
        [SerializeField] protected Plane m_Plane;
        #endregion

        #region Accessors
        public Plane Plane => m_Plane;
        #endregion

        #region Constructors
        public PlanarPath(Vector3 planeNormal, float minimumPointDistance = 1) : this(new Plane(planeNormal, 0), minimumPointDistance)
        {

        }
        public PlanarPath(Plane plane, float minimumPointDistance = 1) : base(minimumPointDistance)
        {
            m_Plane = plane;
        }
        public PlanarPath(List<ControlPoint> controlPoints, Plane plane, float minimumPointDistance = 1) : base(controlPoints, minimumPointDistance)
        {
            m_Plane = plane;
        }
        #endregion

        public override bool CanPointBeAdded(Vector3 point)
        {
            if (!base.CanPointBeAdded(point))
                return false;

            return IsPointOnPlane(point);
        }
        public override bool CanPointBeInserted(Vector3 point, int index)
        {
            if (!base.CanPointBeInserted(point, index))
                return false;

            return IsPointOnPlane(point);
        }
        public override bool CanPointBeUpdated(Vector3 point, int index)
        {
            if (!base.CanPointBeUpdated(point, index))
                return false;

            return IsPointOnPlane(point);
        }
        public override bool Raycast(Ray ray, out Vector3 impactPoint)
        {
            impactPoint = Vector3.zero;
            if (m_Plane.Raycast(ray, out float enter))
            {
                impactPoint = ray.GetPoint(enter);
                return true;
            }
            return false;
        }

        public bool IsPointOnPlane(Vector3 point)
        {
            float dotProduct = Vector3.Dot(m_Plane.normal, point);
            return Mathf.Abs(dotProduct) < Mathf.Epsilon;
        }
    }
}






