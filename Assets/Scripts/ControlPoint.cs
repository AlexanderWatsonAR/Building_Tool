using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
/// <summary>
/// Control Points Exists on the XZ plane
/// </summary>
[System.Serializable]
public class ControlPoint
{
    [SerializeField, HideInInspector] private Vector3 m_Position;
    [SerializeField, HideInInspector] private Vector3 m_Forward;
    [SerializeField, HideInInspector] private float m_Weight;

    public Vector3 Position => m_Position;
    public Vector3 Forward => m_Forward;
    public Vector3 Backward { get { return -Forward; } }
    public Vector3 Up => Vector3.up;
    public Vector3 Down => Vector3.down;
    public Vector3 Right { get { return Vector3.Cross(Up, Forward);}}
    public Vector3 Left { get { return -Right;}}
    public float Weight => m_Weight;

    public ControlPoint(ControlPoint controlPoint)
    {
        m_Position = controlPoint.Position;
        m_Forward = controlPoint.Forward;
        m_Weight = controlPoint.Weight;
    }

    public ControlPoint(Vector3 position, Vector3 forward)
    {
        m_Position = position;
        m_Forward = forward;
    }

    public ControlPoint(Vector3 position, Vector3 forward, float weight)
    {
        m_Position = position;
        m_Forward = forward;
        m_Weight = weight;
    }

    public ControlPoint(Vector3 position)
    {
        m_Position = position;
    }

    public void SetForward(Vector3 forward)
    {
        m_Forward = forward;
    }

    public void SetPosition(Vector3 position)
    {
        m_Position = position;
    }
    public void SetWeight(float weight)
    {
        m_Weight = weight;
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
