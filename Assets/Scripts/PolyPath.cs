using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UEColor = UnityEngine.Color;
using UnityEngine.ProBuilder;
using System;
using UnityEngine.Rendering;
using System.Drawing;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class PolyPath
{
    [SerializeField] private List<ControlPoint> m_ControlPoints;
    [SerializeField] private bool m_IsClosedLoop;
    [SerializeField] private PolyMode m_PolyMode;

    [SerializeField] private bool m_IsPathValid;

    public bool IsPathValid => m_IsPathValid;

    public event Action<List<ControlPoint>> OnControlPointsChanged;
    public event Action<PolyMode> OnPolyModeChanged;

    public PolyPath(bool isClosedLoop = true)
    {
        m_ControlPoints = m_ControlPoints == null ? new() : m_ControlPoints;
        m_IsClosedLoop = isClosedLoop;
    }

    public PolyMode PolyMode
    {
        get { return m_PolyMode; }
        set { m_PolyMode = value; OnPolyModeChanged?.Invoke(m_PolyMode); }
    }

    public Vector3[] Positions
    {
        get
        {
            Vector3[] positions = new Vector3[m_ControlPoints.Count];
            for(int i = 0; i < positions.Length; i++)
            {
                positions[i] = m_ControlPoints[i].Position;
            }
            return positions;
        }
    }

    public List<ControlPoint> ControlPoints
    {
        get 
        {
            List<ControlPoint> controlPoints = new ();

            foreach(ControlPoint point in m_ControlPoints)
            {
                controlPoints.Add(new ControlPoint(point));
            }

            return controlPoints;
        }
    }

    public int ControlPointCount => m_ControlPoints.Count;

    public List<Vector3> LocalPositions(Transform t)
    {
        List<Vector3> localPoints = new List<Vector3>();

        foreach (Vector3 pos in Positions)
        {
            localPoints.Add(pos);
        }

        //Traverse the parents.
        while (t != null)
        {
            for (int i = 0; i < localPoints.Count; i++)
            {
                // Position
                localPoints[i] += t.localPosition;

                // Scale
                Vector3 point = localPoints[i] - t.localPosition;
                Vector3 v = Vector3.Scale(point, t.localScale) + t.localPosition;
                Vector3 scaleOffset = v - localPoints[i];
                localPoints[i] += scaleOffset;

                // Rotation
                Vector3 localEulerAngles = t.localEulerAngles;
                Vector3 v1 = Quaternion.Euler(localEulerAngles) * (localPoints[i] - t.localPosition) + t.localPosition;
                Vector3 rotateOffset = v1 - localPoints[i];
                localPoints[i] += rotateOffset;
            }

            t = t.parent;
        }

        return localPoints;
    }

    public Vector3 LocalPosition(Transform t, Vector3 globalPoint)
    {
        Vector3 localPoint = globalPoint;

        //Traverse the parents.
        while (t != null)
        {
            // Position
            localPoint += t.localPosition;

            // Scale
            Vector3 point = localPoint - t.localPosition;
            Vector3 v = Vector3.Scale(point, t.localScale) + t.localPosition;
            Vector3 scaleOffset = v - localPoint;
            localPoint += scaleOffset;

            // Rotation
            Vector3 localEulerAngles = t.localEulerAngles;
            Vector3 v1 = Quaternion.Euler(localEulerAngles) * (localPoint - t.localPosition) + t.localPosition;
            Vector3 rotateOffset = v1 - localPoint;
            localPoint += rotateOffset;

            t = t.parent;
        }

        return localPoint;
    }

    public void ShiftControlPoints()
    {
        ControlPoint[] controlPoints = m_ControlPoints.ToArray();

        ControlPoint temp = controlPoints[0]; // Save the value at index 0

        // Shift each element to the right, starting from the end of the array
        for (int i = controlPoints.Length - 1; i >= 1; i--)
        {
            controlPoints[i] = controlPoints[i - 1];
        }

        controlPoints[1] = temp; // Set the new value at index 1

        SetControlPoints(controlPoints);
    }

    public void SetControlPoints(IEnumerable<ControlPoint> controlPoints)
    {
        m_ControlPoints = controlPoints.ToList();
    }

    public void SetControlPoints(IEnumerable<Vector3> positions)
    {
        if (positions.Count() != m_ControlPoints.Count)
            return;

        Vector3[] points = positions.ToArray();

        for(int i = 0; i < m_ControlPoints.Count; i++)
        {
            m_ControlPoints[i].SetPosition(points[i]);
        }
    }

    public void AddControlPoint(Vector3 position)
    {
        m_ControlPoints.Add(new ControlPoint(position));
    }

    public bool IsValidPath()
    {
        if (!IsClockwise())
            m_ControlPoints.Reverse(1, m_ControlPoints.Count - 1);

        m_IsPathValid = true;

        if (m_ControlPoints.Count < 3)
        {
            m_IsPathValid = false;
            return false;
        } 

        for(int i = 0; i < m_ControlPoints.Count; i++)
        {
            for (int j = 0; j < m_ControlPoints.Count; j++)
            {
                if (i == j)
                    continue;

                if(Vector3.Distance(GetPositionAt(i), GetPositionAt(j)) <= 1)
                {
                    m_IsPathValid = false;
                    return m_IsPathValid;
                }
            }
        }

        for (int i = 0; i < m_ControlPoints.Count; i++)
        {
            for (int j = 0; j < m_ControlPoints.Count; j++)
            {
                int next = m_ControlPoints.GetNext(j);
                Vector3 point = GetPositionAt(i);
                Vector3 lineStart = GetPositionAt(j);
                Vector3 lineEnd = GetPositionAt(next);

                if (point == lineStart || point == lineEnd)
                    continue;

                float distanceFromLine = UnityEditor.HandleUtility.DistancePointLine(GetPositionAt(i), GetPositionAt(j), GetPositionAt(next));

                if (distanceFromLine <= 1)
                {
                    m_IsPathValid = false;
                    return m_IsPathValid;
                }

            }
        }

        for (int i = 0; i < m_ControlPoints.Count; i++)
        {
            for (int j = 0; j < m_ControlPoints.Count; j++)
            {
                int previous = m_ControlPoints.GetPrevious(i);
                int next = m_ControlPoints.GetNext(j);

                Vector3 line1Start = GetPositionAt(previous);
                Vector3 line1End = GetPositionAt(i);
                Vector3 line2Start = GetPositionAt(j);
                Vector3 line2End = GetPositionAt(next);

                if (Extensions.DoLinesIntersect(line1Start, line1End, line2Start, line2End, out Vector3 intersection, false))
                {
                    float mag = intersection.magnitude;

                    if(Extensions.ApproximatelyEqual(mag, line1Start.magnitude) ||
                       Extensions.ApproximatelyEqual(mag, line1End.magnitude) ||
                       Extensions.ApproximatelyEqual(mag, line2Start.magnitude) ||
                       Extensions.ApproximatelyEqual(mag, line2End.magnitude))
                    {
                        continue;
                    }

                    m_IsPathValid = false;
                    return m_IsPathValid;
                }

            }
        }

        return m_IsPathValid;
    }
    public void RemoveControlPointAt(int index)
    {
        m_ControlPoints.RemoveAt(index);
    }
    public ControlPoint GetControlPointAt(int index)
    {
        return m_ControlPoints[index];
    }
    public void SetPositionAt(int index, Vector3 position)
    {
        if (index < 0 || index > m_ControlPoints.Count)
            return;

        m_ControlPoints[index].SetPosition(position);
        OnControlPointsChanged?.Invoke(m_ControlPoints);

        m_IsPathValid = true; // Assume the path is valid

        // Check if the path is valid for constructing a building
        for (int i = 0; i < ControlPointCount; i++)
        {
            if (i == index)
                continue;

            float dis = Vector3.Distance(position, GetPositionAt(i));

            if (dis <= 1)
            {
                m_IsPathValid = false;
                return;
            }
        }

        for (int i = 0; i < ControlPointCount - 1; i++)
        {
            int next = m_ControlPoints.GetNext(i);
            if (i == index || next == index)
                continue;

            float dis = UnityEditor.HandleUtility.DistancePointLine(position, GetPositionAt(i), GetPositionAt(i + 1));

            if (dis <= 1)
            {
                m_IsPathValid = false;
                return;
            }
        }

        int previousIndex = m_ControlPoints.GetPrevious(index);
        int nextIndex = m_ControlPoints.GetNext(index);

        for (int i = 0; i < ControlPointCount; i++)
        {
            if (i == index)
                continue;

            int previous = m_ControlPoints.GetPrevious(i);
            int next = m_ControlPoints.GetNext(i);

            // Notes on intersection:
            // the indexed point can be intersecting a line of which it is a part of.
            // I.E. it can only be between 2 other/different points.
            Vector3 intersection;

            if (Extensions.DoLinesIntersect(GetPositionAt(previousIndex), GetPositionAt(index), GetPositionAt(i), GetPositionAt(next), out intersection, false))
            {
                if (intersection != GetPositionAt(previousIndex) && intersection != position)
                    m_IsPathValid = false;
            }

            if (Extensions.DoLinesIntersect(GetPositionAt(index), GetPositionAt(nextIndex), GetPositionAt(i), GetPositionAt(next), out intersection, false))
            {
                if (intersection != GetPositionAt(nextIndex) && intersection != position && position != GetPositionAt(previous))
                    m_IsPathValid = false;
            }
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
    public void CalculateForwards()
    {
        if (!m_IsClosedLoop)
            return;

        if (!IsClockwise())
            IsValidPath();

        Vector3[] points = Positions;

        for(int i = 0; i < m_ControlPoints.Count; i++)
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

            m_ControlPoints[i].SetForward(inbetweenForward);
        }
    }

    ///// <summary>
    ///// Do the control points move clockwise?
    ///// </summary>
    ///// <param name="polygonControlPoints"></param>
    ///// <returns></returns>
    //public bool IsClockwise()
    //{
    //    Vector3[] points = Positions;

    //    float temp = 0;
    //    bool isClockwise = false;

    //    for (int i = 0; i < points.Length; i++)
    //    {
    //        if (i != points.Length - 1)
    //        {
    //            float mulA = points[i].x * points[i + 1].z;
    //            float mulB = points[i + 1].x * points[i].z;
    //            temp = temp + (mulA - mulB);
    //        }
    //        else
    //        {
    //            float mulA = points[i].x * points[i].z;
    //            float mulB = points[0].x * points[i].z;
    //            temp = temp + (mulA - mulB);
    //        }
    //    }
    //    temp /= 2;

    //    isClockwise = temp < 0 ? false : true;

    //    return isClockwise;
    //}

    /// <summary>
    /// Control points are required to be on the XZ plane.
    /// </summary>
    public bool IsClockwise()
    {
        Vector3[] points = Positions;

        float sum = 0f;

        for (int i = 0; i < points.Length; i++)
        {
            Vector3 current = points[i];
            Vector3 next = points[(i + 1) % points.Length]; // To handle the wrap-around for the last vertex
            Vector3 previous = points[(i - 1 + points.Length) % points.Length]; // To handle the wrap-around for the first vertex

            Vector3 edge1 = next - current;
            Vector3 edge2 = previous - current;

            sum += Vector3.Cross(edge1, edge2).y; // We use .y since we're on the XZ plane
        }

        //if(sum > 0)
        //{
        //    Debug.Log("Clockwise");
        //}
        //else
        //{
        //    Debug.Log("Counter-clockwise");
        //}

        return sum > 0f;

        // if sum is equal to 0 polygon is degenerate or collinear.
    }

}
