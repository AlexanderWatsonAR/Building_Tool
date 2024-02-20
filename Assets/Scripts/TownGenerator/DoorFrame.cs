using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder;

public class DoorFrame : Polygon3D
{
    private DoorFrameData m_Data;

    public DoorFrameData Data => m_Data;

    public override IBuildable Initialize(IData data)
    {
        name = "Frame";
        m_Data = data as DoorFrameData;
        base.Initialize(data);
        return this;
    }

    public override void Build()
    {
        m_Data.HolePoints = new Vector3[1][];
        m_Data.HolePoints[0] = m_Data.ControlPoints.ScalePolygon(m_Data.Scale);
        base.Build();
    }
}
