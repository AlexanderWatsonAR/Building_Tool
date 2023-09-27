using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ControlPoint
{
    [SerializeField, HideInInspector] private Vector3 m_Position; // World coordinate from cursor pos.
    [SerializeField, HideInInspector] private Vector3 m_Forward;
    [SerializeField, HideInInspector] private Vector3 m_Right; // Vector3.Cross(up, forward)

    public Vector3 Position => m_Position;
    public Vector3 Forward => m_Forward;
    public Vector3 Up => Vector3.up;
    public Vector3 Right => m_Right;

    public ControlPoint(ControlPoint controlPoint)
    {
        m_Position = controlPoint.Position;
        m_Forward = controlPoint.Forward;
        m_Right = controlPoint.Right;
    }

    public ControlPoint(Vector3 position, Vector3 forward)
    {
        m_Position = position;
        SetForward(forward);
    }

    public ControlPoint(Vector3 position)
    {
        m_Position = position;
    }

    public void SetForward(Vector3 forward)
    {
        m_Forward = forward;
        m_Right = Vector3.Cross(Up, Forward);
    }

    public void SetPosition(Vector3 position)
    {
        m_Position = position;
    }

    // Operators
    public static ControlPoint operator +(ControlPoint a) => a;
    public static ControlPoint operator -(ControlPoint a)
    {
        return new ControlPoint(-a.Position, a.Forward);
    }
    public static ControlPoint operator +(ControlPoint a, ControlPoint b)
    {
        return new ControlPoint(a.Position + b.Position, a.Forward);
    }
    public static ControlPoint operator +(ControlPoint a, Vector3 b)
    {
        return new ControlPoint(a.Position + b, a.Forward);
    }
    public static ControlPoint operator -(ControlPoint a, ControlPoint b)
    {
        return new ControlPoint(a.Position - b.Position, a.Forward);
    }
    public static ControlPoint operator -(ControlPoint a, Vector3 b)
    {
        return new ControlPoint(a.Position - b, a.Forward);
    }
    public static ControlPoint operator *(ControlPoint a, ControlPoint b)
    {
        return new ControlPoint(Vector3.Scale(a.Position, b.Position), a.Forward);
    }
    public static ControlPoint operator *(ControlPoint a, Vector3 b)
    {
        return new ControlPoint(Vector3.Scale(a.Position, b), a.Forward);
    }
    // End Operators
}
