using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UEColor = UnityEngine.Color;
using UnityEngine.ProBuilder;
using System;
using UnityEngine.Rendering;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class PolyPath
{
    [SerializeField] private List<ControlPoint> m_ControlPoints = new();
    [SerializeField] private bool m_IsClosed, m_IsGizmoDisplaying, m_IsDrawing;

    public PolyPath()
    {
    }

    public bool IsDrawing
    {
        get { return m_IsDrawing; }
        set { m_IsDrawing = value; }
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
            if(!IsClockwise())
            {
                m_ControlPoints.Reverse();
            }
            
            List<ControlPoint> controlPoints = new ();

            foreach(ControlPoint point in m_ControlPoints)
            {
                controlPoints.Add(new ControlPoint(point));
            }

            return controlPoints;
        }
    }

    public int ControlPointCount => m_ControlPoints.Count;
    public bool IsGizmoDisplaying => m_IsGizmoDisplaying;

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
                // Scale
                Vector3 point = localPoints[i] - t.localPosition;
                Vector3 v = Vector3.Scale(point, t.localScale) + t.localPosition;
                Vector3 offset = v - localPoints[i];
                localPoints[i] += offset;

                // Rotation
                Vector3 localEulerAngles = t.localEulerAngles;
                Vector3 v1 = Quaternion.Euler(localEulerAngles) * (localPoints[i] - t.localPosition) + t.localPosition;
                offset = v1 - localPoints[i];
                localPoints[i] += offset;

                // Position
                localPoints[i] += t.localPosition;
            }

            t = t.parent;
        }

        return localPoints;
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
    public void AddControlPoint(Vector3 position)
    {
        m_ControlPoints.Add(new ControlPoint(position));
    }

    public void ReverseControlPoints()
    {
        m_ControlPoints.Reverse();
    }

    public void RemoveControlPointAt(int index)
    {
        m_ControlPoints.RemoveAt(index);
    }

    public ControlPoint GetControlPointAt(int index)
    {
        return m_ControlPoints[index];
    }

    public void SetControlPointAt(int index, ControlPoint value)
    {
        m_ControlPoints[index] = value;
    }



    public void CalculateForwards()
    {
        if (!m_IsClosed)
            return;

        if (!IsClockwise())
            ReverseControlPoints();

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

    //private void OnDrawGizmosSelected()
    //{

    //}

    /// <summary>
    /// Do the control points move clockwise?
    /// </summary>
    /// <param name="polygonControlPoints"></param>
    /// <returns></returns>
    public bool IsClockwise()
    {
        Vector3[] points = Positions;

        float temp = 0;
        bool isClockwise = false;

        for (int i = 0; i < points.Length; i++)
        {
            if (i != points.Length - 1)
            {
                float mulA = points[i].x * points[i + 1].z;
                float mulB = points[i + 1].x * points[i].z;
                temp = temp + (mulA - mulB);
            }
            else
            {
                float mulA = points[i].x * points[i].z;
                float mulB = points[0].x * points[i].z;
                temp = temp + (mulA - mulB);
            }
        }
        temp /= 2;

        isClockwise = temp < 0 ? false : true;

        return isClockwise;
    }

}
