using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DoorFrameData : Polygon3DData
{
    [SerializeField] float m_Scale;

    public float Scale { get { return m_Scale; } set { m_Scale = value; } }

    public DoorFrameData() : base()
    {
        m_Scale = 0.95f;
    }

    public DoorFrameData(Vector3[] controlPoints, float insideScale, Vector3 normal, float depth) : base (controlPoints, null, normal, depth)
    {
        m_Scale = insideScale;
    }

    public DoorFrameData(DoorFrameData data) : this(data.ControlPoints, data.Scale, data.Normal, data.Depth)
    {

    }
}
