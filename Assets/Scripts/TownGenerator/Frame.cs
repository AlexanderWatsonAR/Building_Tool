using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder;

public class Frame : Polygon3D
{
    [SerializeField] FrameData m_Data;

    public FrameData Data => m_Data;

    public override IBuildable Initialize(IData data)
    {
        m_Data = data as FrameData;
        base.Initialize(data);
        return this;
    }

    public override void Build()
    {
        m_Data.HolePoints = new Vector3[1][];
        m_Data.HolePoints[0] = m_Data.ControlPoints.ScalePolygon(m_Data.Scale, m_Data.Position);
        base.Build();
    }
}
