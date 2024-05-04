using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UIElements;


[System.Serializable]
public struct ControlPoint
{
    #region Member Variables
    [SerializeField] Vector3 m_Position;
    [SerializeField] Vector3 m_Forward;
    [SerializeField] Vector3 m_Up;
    [SerializeField] float m_Weight;
    #endregion

    #region Accessors
    public Vector3 Position { get { return m_Position; } set { m_Position = value; } }
    public Vector3 Forward { get { return m_Forward; } set { m_Position = value; } }
    public Vector3 Backward { get { return -Forward; } }
    public Vector3 Up { get{ return m_Up; } set { m_Up = value; } }
    public Vector3 Down => Vector3.down;
    public Vector3 Right { get { return Vector3.Cross(Up, Forward);}}
    public Vector3 Left { get { return -Right;}}
    public float Weight => m_Weight;
    #endregion

    #region Constructors
    public ControlPoint(ControlPoint controlPoint)
    {
        m_Position = controlPoint.Position;
        m_Forward = controlPoint.Forward;
        m_Up = controlPoint.Up;
        m_Weight = controlPoint.Weight;
    }
    public ControlPoint(Vector3 position, Vector3 forward, Vector3 up, float weight)
    {
        m_Position = position;
        m_Forward = forward;
        m_Up = up;
        m_Weight = weight;
    }
    public ControlPoint(Vector3 pos)
    {
        m_Position = pos;
        m_Forward = Vector3.forward;
        m_Up = Vector3.up;
        m_Weight = 1;
    }
    #endregion

    #region Operators
    public static ControlPoint operator +(ControlPoint a) => a;
    public static ControlPoint operator -(ControlPoint a)
    {
        return new ControlPoint(-a.Position, a.Forward, a.Up, a.Weight);
    }
    public static ControlPoint operator +(ControlPoint a, ControlPoint b)
    {
        return new ControlPoint(a.Position + b.Position, a.Forward, a.Up, a.Weight);
    }
    public static ControlPoint operator +(ControlPoint a, Vector3 b)
    {
        return new ControlPoint(a.Position + b, a.Forward, a.Up, a.Weight);
    }
    public static ControlPoint operator -(ControlPoint a, ControlPoint b)
    {
        return new ControlPoint(a.Position - b.Position, a.Forward, a.Up, a.Weight);
    }
    public static ControlPoint operator -(ControlPoint a, Vector3 b)
    {
        return new ControlPoint(a.Position - b, a.Forward, a.Up, a.Weight);
    }
    public static ControlPoint operator *(ControlPoint a, ControlPoint b)
    {
        return new ControlPoint(Vector3.Scale(a.Position, b.Position), a.Forward, a.Up, a.Weight);
    }
    public static ControlPoint operator *(ControlPoint a, Vector3 b)
    {
        return new ControlPoint(Vector3.Scale(a.Position, b), a.Forward, a.Up, a.Weight);
    }
    #endregion
}
