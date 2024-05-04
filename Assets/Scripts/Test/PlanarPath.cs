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

[System.Serializable]
public class LinePath : PlanarPath
{
    public LinePath(Plane plane, float minimumPointDistance) : base(plane, minimumPointDistance)
    {
    }
    public LinePath(List<ControlPoint> controlPoints, Plane plane, float minimumPointDistance) : base(controlPoints, plane, minimumPointDistance)
    {

    }
    public override bool CanPointBeAdded(Vector3 point)
    {
        if (!base.CanPointBeAdded(point))
            return false;

        return true;
    }

}


[System.Serializable]
public class PolygonPath : PlanarPath
{
    public PolygonPath(Vector3 normal, float minimumPointDistance = 1) : base (new Plane(normal, 0), minimumPointDistance) 
    {

    }
    public PolygonPath(Plane plane, float minimumPointDistance = 1) : base(plane, minimumPointDistance)
    {

    }
    public PolygonPath(List<ControlPoint> controlPoints, Plane plane, float minimumPointDistance = 1) : base(controlPoints, plane, minimumPointDistance)
    {

    }

}

public class XZPolygonPath : PolygonPath
{
    public XZPolygonPath(float minimumPointDistance = 1) : base(Vector3.up, minimumPointDistance)
    {
    }
    public XZPolygonPath(List<ControlPoint> controlPoints, float minimumPointDistance = 1) : base(controlPoints, new Plane(Vector3.up, 0), minimumPointDistance)
    {

    }
    public override bool CanPointBeAdded(Vector3 point)
    {
        int count = PathPointsCount;

        if (count == 0)
            return true;

        if (count >= 3)
        {
            if (Vector3.Distance(point, GetPositionAt(0)) <= m_MinPointDistance)
            {
                return true;
            }
        }

        for (int i = 0; i < count; i++)
        {
            float dis = Vector3.Distance(point, GetPositionAt(i));

            if (dis <= m_MinPointDistance)
            {
                return false;
            }
        }

        for (int i = 0; i < count - 1; i++)
        {
            float dis = HandleUtil.DistancePointLine(point, GetPositionAt(i), GetPositionAt(i + 1));

            if (dis <= m_MinPointDistance)
            {
                return false;
            }
        }

        for (int i = 0; i < count; i++)
        {
            int next = (i + 1) % count;

            if (Extensions.DoLinesIntersect(line1Start: GetLastPosition(),
                line1End: point, line2Start: GetPositionAt(i),
                line2End: GetPositionAt(next),
                out Vector3 intersection, false))
            {
                if (intersection == GetLastPosition())
                    return true;

                return false;
            }
        }

        return true;
    }
    public override bool CanPointBeUpdated(Vector3 point, int index)
    {
        if (!base.CanPointBeUpdated(point, index))
            return false;


        int count = PathPointsCount;

        for (int i = 0; i < count - 1; i++)
        {
            int next = (i + 1) % count;

            if (i == index || next == index)
                continue;

            float dis = HandleUtil.DistancePointLine(point, GetPositionAt(i), GetPositionAt(i + 1));

            if (dis <= m_MinPointDistance)
            {
                return false;
            }
        }

        int previousIndex = index == 0 ? count - 1 : index - 1;
        int nextIndex = (index + 1) % count;

        for (int i = 0; i < count; i++)
        {
            if (i == index)
                continue;

            int previous = i == 0 ? count - 1 : i - 1;
            int next = (i + 1) % count;

            // Notes on intersection:
            // the indexed point can be intersecting a line of which it is a part of.
            // I.E. it can only be between 2 other/different points.
            Vector3 intersection;

            // Here we are assuming the intersection is happening on the XZ plane.

            if (Extensions.DoLinesIntersect(GetPositionAt(previousIndex), GetPositionAt(index), GetPositionAt(i), GetPositionAt(next), out intersection, false))
            {
                if (intersection != GetPositionAt(previousIndex) && intersection != point)
                    return false;
            }

            if (Extensions.DoLinesIntersect(GetPositionAt(index), GetPositionAt(nextIndex), GetPositionAt(i), GetPositionAt(next), out intersection, false))
            {
                if (intersection != GetPositionAt(nextIndex) && intersection != point && point != GetPositionAt(previous))
                    return false;
            }
        }

        return true;
    }
    public bool CheckPath()
    {
        int count = PathPointsCount;

        if (!IsClockwise())
            m_ControlPoints.Reverse(1, count - 1);

        if (count < 3) // Minimum number of points to form a polygon is 3.
        {
            return false;
        }

        for (int i = 0; i < count; i++)
        {
            if (!IsPointOnPlane(GetPositionAt(i)))
                return false;
        }

        for (int i = 0; i < count; i++)
        {
            for (int j = 0; j < count; j++)
            {
                if (i == j)
                    continue;

                if (Vector3.Distance(GetPositionAt(i), GetPositionAt(j)) <= m_MinPointDistance)
                {
                    return false;
                }
            }
        }

        for (int i = 0; i < count; i++)
        {
            for (int j = 0; j < count; j++)
            {
                int next = (j + 1) % count;
                Vector3 point = GetPositionAt(i);
                Vector3 lineStart = GetPositionAt(j);
                Vector3 lineEnd = GetPositionAt(next);

                if (point == lineStart || point == lineEnd)
                    continue;

                float distanceFromLine = HandleUtil.DistancePointLine(GetPositionAt(i), GetPositionAt(j), GetPositionAt(next));

                if (distanceFromLine <= m_MinPointDistance)
                {
                    return false;
                }

            }
        }

        for (int i = 0; i < count; i++)
        {
            for (int j = 0; j < count; j++)
            {
                int previous = i == 0 ? count - 1 : i - 1;
                int next = (i + 1) % count;

                Vector3 line1Start = GetPositionAt(previous);
                Vector3 line1End = GetPositionAt(i);
                Vector3 line2Start = GetPositionAt(j);
                Vector3 line2End = GetPositionAt(next);

                if (Extensions.DoLinesIntersect(line1Start, line1End, line2Start, line2End, out Vector3 intersection, false))
                {
                    float mag = intersection.magnitude;

                    if (Extensions.ApproximatelyEqual(mag, line1Start.magnitude) ||
                       Extensions.ApproximatelyEqual(mag, line1End.magnitude) ||
                       Extensions.ApproximatelyEqual(mag, line2Start.magnitude) ||
                       Extensions.ApproximatelyEqual(mag, line2End.magnitude))
                    {
                        continue;
                    }

                    return false;
                }

            }
        }

        return true;
    }
    public bool IsClockwise()
    {
        Vector3[] points = Positions;
        int length = points.Length;

        float sum = 0f;

        for (int i = 0; i < length; i++)
        {
            Vector3 current = points[i];
            Vector3 next = points[(i + 1) % length]; // To handle the wrap-around for the last vertex
            Vector3 previous = points[(i - 1 + length) % length]; // To handle the wrap-around for the first vertex

            Vector3 edge1 = next - current;
            Vector3 edge2 = previous - current;

            sum += Vector3.Cross(edge1, edge2).y; // We use .y since we're on the XZ plane
        }

        return sum > 0f;
    }
    public bool IsPointInside(Vector3 point)
    {
        return Positions.IsPointInsidePolygon(point);
    }
}

public enum DrawState
{
    Hide, Draw, Edit
}
