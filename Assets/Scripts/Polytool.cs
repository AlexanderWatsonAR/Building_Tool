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

// TODO: Duplicate polyshape functionality
[System.Serializable]
public class Polytool : MonoBehaviour
{
    [SerializeField] private List<ControlPoint> m_ControlPoints;
    [SerializeField] private bool m_IsDrawing;
    [SerializeField] private bool m_Show;

    /// <summary>
    /// TODO: Invoke when a control point's position has changed
    /// </summary>
    public event Action<Vector3, int> OnPositionChanged;

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

            return m_ControlPoints;
        }
    }

    public List<Vector3> LocalPositions
    {
        get
        {
            List<Vector3> localPoints = new List<Vector3>();

            foreach (Vector3 pos in Positions)
            {
                localPoints.Add(pos);
            }

            //Traverse the parents.
            Transform current = transform;
            while (current != null)
            {
                for (int i = 0; i < localPoints.Count; i++)
                {
                    // Scale
                    Vector3 point = localPoints[i] - current.localPosition;
                    Vector3 v = Vector3.Scale(point, current.localScale) + current.localPosition;
                    Vector3 offset = v - localPoints[i];
                    localPoints[i] += offset;

                    // Rotation
                    Vector3 localEulerAngles = current.localEulerAngles;
                    Vector3 v1 = Quaternion.Euler(localEulerAngles) * (localPoints[i] - current.localPosition) + current.localPosition;
                    offset = v1 - localPoints[i];
                    localPoints[i] += offset;

                    // Position
                    localPoints[i] += current.localPosition;
                }

                current = current.parent;
            }

            return localPoints;
        }
    }

    private void Reset()
    {
        m_Show = true;
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

        Vector3 v = transform.InverseTransformPoint(position);

        if (m_ControlPoints.Count <= 2)
        {
            m_ControlPoints.Add(new ControlPoint(v));
        }
        else
        {
            Vector3 a = transform.TransformPoint(m_ControlPoints[0].Position);

            float distance = Vector3.Distance(a, position);

            if(distance <= 1)
            {
                m_IsDrawing = false;
            }
            else
            {
                m_ControlPoints.Add(new ControlPoint(v));
            }
        }
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

    public int ControlPointCount()
    {
        return m_ControlPoints.Count;
    }

    public void CalculateForwards()
    {
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

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (m_ControlPoints == null || m_Show == false)
            return;
        
        for (int i = 0; i < LocalPositions.Count; i++)
        {
            //if (i == 0)
            //    Handles.color = UEColor.red;
            //else
                Handles.color = UEColor.white;

            float size = UnityEditor.HandleUtility.GetHandleSize(m_ControlPoints[i].Position) * 0.04f;

            Handles.DotHandleCap(i, LocalPositions[i], Quaternion.identity, size, Event.current.type);
        }

        if (m_ControlPoints.Count <= 1)
            return;

        Handles.DrawAAPolyLine(LocalPositions.ToArray());

        if(!m_IsDrawing && m_ControlPoints.Count >= 3)
        {
            Handles.DrawAAPolyLine(LocalPositions[0], LocalPositions[^1]);
        }
    }
#endif
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
