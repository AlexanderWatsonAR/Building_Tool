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

    public FrameData(PolygonData polygon, PolygonData[] holes, Vector3 normal, float height, float width, float depth, Vector3 position, float scale) : base (polygon, holes, normal, height, width, depth, position)
    {
        m_Scale = scale;
    }

    public FrameData(FrameData data) : this(data.Polygon, data.Holes, data.Normal, data.Height, data.Width, data.Depth, data.Position, data.Scale)
    {

    }

    public override bool Equals(object obj)
    {
        FrameData other = obj as FrameData;

        if (other == null)
            return false;

        if(m_Scale == other.Scale && base.Equals(obj))
        {
            return true;
        }

        return false;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
