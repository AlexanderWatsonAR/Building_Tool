using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UEColor = UnityEngine.Color;
using UnityEngine.ProBuilder;
using System;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
#endif

// TODO: Duplicate polyshape functionality
[System.Serializable]
public class Polytool : MonoBehaviour
{
    [SerializeField] private List<Vector3> m_ControlPoints; // world points.
    [SerializeField] private bool m_IsDrawing;
    [SerializeField] private bool m_Show;

    public List<Vector3> ControlPoints
    {
        get 
        {
            if(!IsClockwiseAlternative(m_ControlPoints))
            {
                m_ControlPoints.Reverse();
            }

            return m_ControlPoints;
        }
    }

    public List<Vector3> LocalControlPoints
    {
        get
        {
            List<Vector3> controlPoints = new List<Vector3>();

            foreach (Vector3 controlPoint in m_ControlPoints)
            {
                controlPoints.Add(controlPoint);
            }

            //Traverse the parents.
            Transform current = transform;
            while (current != null)
            {
                for (int i = 0; i < controlPoints.Count; i++)
                {
                    // Scale
                    Vector3 point = controlPoints[i] - current.localPosition;
                    Vector3 v = Vector3.Scale(point, current.localScale) + current.localPosition;
                    Vector3 offset = v - controlPoints[i];
                    controlPoints[i] += offset;

                    // Rotation
                    Vector3 localEulerAngles = current.localEulerAngles;
                    Vector3 v1 = Quaternion.Euler(localEulerAngles) * (controlPoints[i] - current.localPosition) + current.localPosition;
                    offset = v1 - controlPoints[i];
                    controlPoints[i] += offset;

                    // Position
                    controlPoints[i] += current.localPosition;
                }

                current = current.parent;
            }

            return controlPoints;
        }
    }

    public void ShiftControlPoints()
    {
        Vector3[] controlPointsArray = m_ControlPoints.ToArray();

        Vector3 temp = controlPointsArray[0]; // Save the value at index 0

        // Shift each element to the right, starting from the end of the array
        for (int i = controlPointsArray.Length - 1; i >= 1; i--)
        {
            controlPointsArray[i] = controlPointsArray[i - 1];
        }

        controlPointsArray[1] = temp; // Set the new value at index 1

        SetControlPoints(controlPointsArray);
    }

    public void SetControlPoints(IEnumerable<Vector3> controlPoints)
    {
        m_ControlPoints = controlPoints.ToList();
    }

    public void SetControlPoints(List<Vector3> controlPoints)
    {
        m_ControlPoints = controlPoints;
    }

    public void AddControlPoint(Vector3 controlPoint)
    {
        Vector3 v = transform.InverseTransformPoint(controlPoint);

        if (m_ControlPoints.Count <= 2)
        {
            m_ControlPoints.Add(v);
        }
        else
        {
            Vector3 a = transform.TransformPoint(m_ControlPoints[0]);

            float distance = Vector3.Distance(a, controlPoint);

            if(distance <= 1)
            {
                m_IsDrawing = false;
            }
            else
            {
                m_ControlPoints.Add(v);
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

    public Vector3 GetControlPointAt(int index)
    {
        return m_ControlPoints[index];
    }

    public void SetControlPointAt(int index, Vector3 value)
    {
        m_ControlPoints[index] = value;
    }

    public int GetNumberOfControlPoints()
    {
        return m_ControlPoints.Count;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (m_ControlPoints == null || m_Show == false)
            return;
        
        for (int i = 0; i < LocalControlPoints.Count; i++)
        {
            if (i == 0)
                Handles.color = UEColor.red;
            else
                Handles.color = UEColor.white;

            Handles.DotHandleCap(-1, LocalControlPoints[i], Quaternion.identity, 0.1f, Event.current.type);
        }

        if (m_ControlPoints.Count <= 1)
            return;

        Handles.DrawAAPolyLine(LocalControlPoints.ToArray());

        if(!m_IsDrawing && m_ControlPoints.Count >= 3)
        {
            Handles.DrawAAPolyLine(LocalControlPoints[0], LocalControlPoints[^1]);
        }
    }
#endif

    // Helper functions

    /// <summary>
    /// Do the control points move clockwise?
    /// </summary>
    /// <returns></returns>
    public bool IsClockwise()
    {
        return IsClockwise(m_ControlPoints);
    }
    /// <summary>
    /// Do the control points move clockwise?
    /// </summary>
    /// <param name="polygonControlPoints"></param>
    /// <returns></returns>
    public bool IsClockwise(IEnumerable<Vector3> polygonControlPoints)
    {
        Vector3[] points = polygonControlPoints.ToArray();
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

    public bool IsClockwiseAlternative()
    {
        return IsClockwiseAlternative(m_ControlPoints);

    }

    public bool IsClockwiseAlternative(IEnumerable<Vector3> polygonControlPoints)
    {
        Vector3[] controlPointsArray = polygonControlPoints.ToArray();

        // Calculate the sum of the cross products of consecutive edges
        float sum = 0f;
        for (int i = 0; i < controlPointsArray.Length; i++)
        {
            Vector3 current = controlPointsArray[i];
            Vector3 next = controlPointsArray[(i + 1) % controlPointsArray.Length];
            sum += (next.x - current.x) * (next.z + current.z);
        }

        // If the sum is negative, the polygon is clockwise
        bool isClockwise = sum < 0 ? true : false;
        return isClockwise;
    }

}
