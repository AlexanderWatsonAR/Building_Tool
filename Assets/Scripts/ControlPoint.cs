using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ControlPoint
{
    [SerializeField, HideInInspector] private Vector3 m_Position; // World coordinate from cursor pos.
    [SerializeField, HideInInspector] private Vector3 m_Forward; // Forward can be calculated based on its position relative to 2 other control point positions 
    [SerializeField, HideInInspector] private Vector3 m_Right; // Vector3.Cross(up, forward)

    public Vector3 Position => m_Position;
    public Vector3 Forward => m_Forward;
    public Vector3 Up => Vector3.up;
    public Vector3 Right => m_Right;


    public ControlPoint(Vector3 position)
    {
        m_Position = position;
    }

    public void SetForward(Vector3 forward)
    {
        m_Forward = forward;
        m_Right = Vector3.Cross(Up, Forward);
    }

}
