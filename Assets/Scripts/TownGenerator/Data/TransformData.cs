using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TransformData 
{
    [SerializeField] RelativePosition m_RelativePosition;
    [SerializeField] Vector3 m_AbsolutePosition;
    [SerializeField] Vector3 m_PositionOffset;
    [SerializeField] Vector3 m_EulerAngle;
    [SerializeField] Vector3 m_Scale;

    public RelativePosition RelativePosition { get { return m_RelativePosition; } set { m_RelativePosition = value; } }
    public Vector3 EulerAngle { get { return m_EulerAngle; } set { m_EulerAngle = value; } }
    public Vector3 Scale { get { return m_Scale; } set { m_Scale = value; } }
    public Vector3 PositionOffset { get { return m_PositionOffset; } set { m_PositionOffset = value; } }
    public Vector3 AbsolutePosition { get { return m_AbsolutePosition; } set { m_AbsolutePosition = value; } }

    public TransformData() :this (RelativePosition.Middle, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.one)
    {

    }

    public TransformData(RelativePosition relativePosition, Vector3 absolutePosition, Vector3 positionOffset, Vector3 eulerAngle, Vector3 scale)
    {
        m_RelativePosition = relativePosition;
        m_AbsolutePosition = absolutePosition;
        m_PositionOffset = positionOffset;
        m_EulerAngle = eulerAngle;
        m_Scale = scale;
    }

    public TransformData(TransformData data) : this (data.RelativePosition, data.AbsolutePosition, data.PositionOffset, data.EulerAngle, data.Scale)
    {

    }


}
