using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UEColor = UnityEngine.Color;
using UnityEngine.ProBuilder;
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

    public List<Vector3> ControlPoints => m_ControlPoints;
    public List<Vector3> LocalControlPoints
    {
        get
        {
            List<Vector3> controlPoints = new List<Vector3>();
            foreach(Vector3 controlPoint in m_ControlPoints)
            {
                controlPoints.Add(transform.InverseTransformPoint(controlPoint));
            }
            return controlPoints;
        }
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
        if(m_ControlPoints.Count <= 2)
        {
            m_ControlPoints.Add(controlPoint);
        }
        else
        {
            float distance = Vector3.Distance(m_ControlPoints[0], controlPoint);

            if(distance <= 1)
            {
                m_IsDrawing = false;
            }
            else
            {
                m_ControlPoints.Add(controlPoint);
            }
        }
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
        foreach (Vector3 controlPoint in m_ControlPoints)
        {
            Handles.DotHandleCap(-1, controlPoint, Quaternion.identity, 0.1f, Event.current.type);
        }

        if (m_ControlPoints.Count <= 1)
            return;

        Handles.DrawAAPolyLine(m_ControlPoints.ToArray());

        if(!m_IsDrawing && m_ControlPoints.Count >= 3)
        {
            Handles.DrawAAPolyLine(m_ControlPoints[0], m_ControlPoints[^1]);
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

}
