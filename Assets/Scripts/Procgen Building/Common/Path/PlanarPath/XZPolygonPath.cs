using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HandleUtil = UnityEditor.HandleUtility;

namespace OnlyInvalid.ProcGenBuilding.Common
{
    [System.Serializable]
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

            // The commented out section is supposed to resolve line intersection.
            // At one time this worked perfectly but now it does not.

            //int previousIndex = index == 0 ? count - 1 : index - 1;
            //int nextIndex = (index + 1) % count;

            //for (int i = 0; i < count; i++)
            //{
            //    if (i == index)
            //        continue;

            //    int previous = i == 0 ? count - 1 : i - 1;
            //    int next = (i + 1) % count;

            //    // Notes on intersection:
            //    // the indexed point can be intersecting a line of which it is a part of.
            //    // I.E. it can only be between 2 other/different points.
            //    Vector3 intersection;

            //    if (Extensions.DoLinesIntersect(GetPositionAt(previousIndex), GetPositionAt(index), GetPositionAt(i), GetPositionAt(next), out intersection, false))
            //    {
            //        if (intersection != GetPositionAt(previousIndex) && intersection != point)
            //            return false;
            //    }

            //    if (Extensions.DoLinesIntersect(GetPositionAt(index), GetPositionAt(nextIndex), GetPositionAt(i), GetPositionAt(next), out intersection, false))
            //    {
            //        if (intersection != GetPositionAt(nextIndex) && intersection != point && point != GetPositionAt(previous))
            //            return false;
            //    }
            //}

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

                        if (Mathf.Approximately(mag, line1Start.magnitude) ||
                            Mathf.Approximately(mag, line1End.magnitude) ||
                            Mathf.Approximately(mag, line2Start.magnitude) ||
                            Mathf.Approximately(mag, line2End.magnitude))
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

        public void CalculateForwards()
        {
            m_IsPathValid = CheckPath();

            Vector3[] points = Positions;

            for (int i = 0; i < m_ControlPoints.Count; i++)
            {
                int previousPoint = points.GetPreviousControlPoint(i);
                int nextPoint = points.GetNextControlPoint(i);

                Vector3 nextForward = Vector3Extensions.DirectionToTarget(points[i], points[nextPoint]);
                Vector3 previousForward = Vector3Extensions.DirectionToTarget(points[i], points[previousPoint]);
                Vector3 inbetweenForward = Vector3.Lerp(nextForward, previousForward, 0.5f);

                Vector3 v = points[i] + inbetweenForward;

                if (!points.IsPointInsidePolygon(v))
                {
                    inbetweenForward = -inbetweenForward;
                }

                ControlPoint a = m_ControlPoints[i];
                a.Forward = inbetweenForward;
                m_ControlPoints[i] = a;
            }
        }
    }
}
