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

namespace OnlyInvalid.ProcGenBuilding.Common
{ 
    [System.Serializable]
    public class PlanarPath
    {
        #region Members
        // The stored positions should use global coords.
        [SerializeField] protected List<ControlPoint> m_ControlPoints;
        [SerializeField] protected Plane m_Plane;
        [SerializeField] protected float m_MinPointDistance;
        [SerializeField] protected bool m_IsPathValid;
        #endregion

        #region Events
        public UnityEvent OnPointAdded = new UnityEvent();
        public UnityEvent OnPointRemoved = new UnityEvent();
        public UnityEvent OnPointMoved = new UnityEvent();
        #endregion

        #region Accessors
        public Plane Plane => m_Plane;
        public bool IsPathValid => m_IsPathValid;
        public int PathPointsCount => m_ControlPoints.Count;
        public Vector3 Average
        {
            get
            {
                Vector3 centroid = Vector3.zero;
                foreach (ControlPoint pos in m_ControlPoints)
                {
                    centroid += pos.Position;
                }
                centroid /= m_ControlPoints.Count;
                return centroid;
            }
        }
        public Vector3[] Positions
        {
            get
            {
                Vector3[] positions = new Vector3[m_ControlPoints.Count];
                for (int i = 0; i < positions.Length; i++)
                {
                    positions[i] = m_ControlPoints[i].Position;
                }
                return positions;
            }
        }
        public ControlPoint[] ControlPoints => m_ControlPoints.ToArray();
        public Vector3 Normal => m_Plane.normal;
        public float MinimumPointDistance => m_MinPointDistance;
        #endregion

        #region Constructors
        public PlanarPath(Vector3 planeNormal, float minimumPointDistance = 1) : this (new Plane(planeNormal, 0), minimumPointDistance)
        {

        }
        public PlanarPath(Plane plane, float minimumPointDistance = 1)
        {
            m_Plane = plane;
            m_ControlPoints = new List<ControlPoint>();
            m_MinPointDistance = minimumPointDistance;
            m_IsPathValid = true;
        }
        public PlanarPath(List<ControlPoint> controlPoints, Plane plane, float minimumPointDistance = 1)
        {
            m_ControlPoints = controlPoints;
            m_Plane = plane;
            m_MinPointDistance = minimumPointDistance;
            m_IsPathValid = true;
        }
        #endregion

        /// <summary>
        /// Will return false if the point is invalid
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool AddPointToPath(Vector3 point)
        {
            if (!CanPointBeAdded(point))
                return false;

            m_ControlPoints.Add(new ControlPoint(point));
            OnPointAdded.Invoke();
            return true;
        }
        /// <summary>
        /// Will return false if point is invalid.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool InsertPointInPath(Vector3 point, int index)
        {
            if (!CanPointBeInserted(point, index))
                return false;

            m_ControlPoints.Insert(index, new ControlPoint(point));
            return true;
        }
        public void RemovePointAt(int index)
        {
            m_ControlPoints.RemoveAt(index);
            OnPointRemoved.Invoke();
        }
        public void RemoveLastPoint()
        {
            RemovePointAt(PathPointsCount - 1);
        }
        public virtual bool CanPointBeAdded(Vector3 point)
        {
            if (!IsPointOnPlane(point))
                return false;

            int count = PathPointsCount;

            if (count == 0)
                return true;

            for (int i = 0; i < count; i++)
            {
                float dis = Vector3.Distance(point, GetPositionAt(i));

                if (dis < m_MinPointDistance)
                {
                    return false;
                }
            }

            return true;
        }
        public virtual bool CanPointBeInserted(Vector3 point, int index)
        {
            if (index < 0 || index > m_ControlPoints.Count)
                return false;

            if (!IsPointOnPlane(point))
                return false;

            int lastIndex = m_ControlPoints.Count - 1;

            if (index == 0 && Vector3.Distance(GetFirstPosition(), point) < m_MinPointDistance)
                return false;

            if (index == lastIndex && Vector3.Distance(GetLastPosition(), point) < m_MinPointDistance)
                return false;

            if (Vector3.Distance(m_ControlPoints[index - 1].Position, point) < m_MinPointDistance ||
                Vector3.Distance(m_ControlPoints[index + 1].Position, point) < m_MinPointDistance)
                return false;

            return true;
        }
        public virtual bool CanPointBeUpdated(Vector3 point, int index)
        {
            int count = PathPointsCount;

            if (count == 0)
                return false;

            if (index < 0 || index > count)
                return false;

            if (!IsPointOnPlane(point))
                return false;

            for (int i = 0; i < count; i++)
            {
                if (i == index)
                    continue;

                float dis = Vector3.Distance(point, GetPositionAt(i));

                if (dis < m_MinPointDistance)
                {
                    return false;
                }
            }

            return true;
        }
        public bool IsPointOnPlane(Vector3 point)
        {
            float dotProduct = Vector3.Dot(m_Plane.normal, point);
            return Mathf.Approximately(dotProduct + m_Plane.distance, 0);
        }
        public void SetPositionAt(Vector3 position, int index, bool ignoreValidity = true)
        {
            m_IsPathValid = CanPointBeUpdated(position, index);

            if (!m_IsPathValid && !ignoreValidity)
                return;

            if (ignoreValidity)
            {
                m_ControlPoints[index] = new ControlPoint(position);
                OnPointMoved.Invoke();
            }
        }
        public Vector3 GetPositionAt(int index)
        {
            return m_ControlPoints[index].Position;
        }
        public Vector3 GetLastPosition()
        {
            return m_ControlPoints[^1].Position;
        }
        public Vector3 GetFirstPosition()
        {
            return m_ControlPoints[0].Position;
        }
    }
}






