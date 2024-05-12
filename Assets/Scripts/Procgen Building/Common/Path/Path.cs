using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace OnlyInvalid.ProcGenBuilding.Common
{
    [System.Serializable]
    public class Path
    {
        #region Members
        [SerializeField] protected List<ControlPoint> m_ControlPoints;
        [SerializeField] protected float m_MinPointDistance;
        [SerializeField] protected bool m_IsPathValid;
        #endregion

        #region Events
        public UnityEvent OnPointAdded = new UnityEvent();
        public UnityEvent OnPointRemoved = new UnityEvent();
        public UnityEvent OnPointMoved = new UnityEvent();
        #endregion

        #region Accessors
        public bool IsPathValid => m_IsPathValid;
        public int PathPointsCount => m_ControlPoints.Count;
        public Vector3 Average
        {
            get
            {
                return Positions.Average();
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
        public float MinimumPointDistance => m_MinPointDistance;
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
        #endregion

        #region Constructors
        public Path(float minimumPointDistance = 1)
        {
            m_ControlPoints = new List<ControlPoint>();
            m_MinPointDistance = minimumPointDistance;
            m_IsPathValid = true;
        }
        public Path(IEnumerable<ControlPoint> controlPoints, float minimumPointDistance = 1)
        {
            m_ControlPoints = controlPoints.ToList();
            m_MinPointDistance = minimumPointDistance;
            m_IsPathValid = true;
        }
        public Path(IEnumerable<Vector3> positions, float minimumPointDistance = 1)
        {
            m_MinPointDistance = minimumPointDistance;

            foreach (Vector3 pos in positions)
            {
                m_ControlPoints.Add(new ControlPoint(pos));
            }
            m_IsPathValid = true;
        }
        #endregion

        #region Virtual Functions
        public virtual bool CanPointBeAdded(Vector3 point)
        {
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
        public virtual bool Raycast(Ray ray, out Vector3 impactPoint)
        {
            RaycastHit hitInfo;
            impactPoint = Vector3.zero;
            if (Physics.Raycast(ray, out hitInfo))
            {
                impactPoint = hitInfo.point;
                return true;
            }
            return false;
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
    }

}
