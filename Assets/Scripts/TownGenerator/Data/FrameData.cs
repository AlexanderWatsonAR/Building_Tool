using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class FrameData : Polygon3DData
{
    [SerializeField, Range(0, 0.999f)] float m_Scale;

    public float Scale { get { return m_Scale; } set { m_Scale = value; } }

    public FrameData() : this(0.95f)
    {
    }

    public FrameData(float scale) : base()
    {
        m_Scale = scale;
    }

    public FrameData(Vector3[] controlPoints, Vector3[][] holePoints, Vector3 normal, float height, float width, float depth, Vector3 position, float scale) : base (controlPoints, holePoints, normal, height, width, depth, position)
    {
        m_Scale = scale;
    }

    public FrameData(FrameData data) : this(data.ControlPoints, data.HolePoints, data.Normal, data.Height, data.Width, data.Depth, data.Position, data.Scale)
    {

    }
}
