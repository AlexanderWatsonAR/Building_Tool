using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridFrame : Polygon3D
{
    [SerializeReference] GridFrameData m_Data;

    public GridFrameData Data => m_Data;

    public override IBuildable Initialize(IData data)
    {
        m_Data = data as GridFrameData;
        base.Initialize(data);
        return this;
    }

    public override void Build()
    {
        Vector3[][] holePoints = MeshMaker.SpiltPolygon(m_Data.Polygon.ControlPoints, m_Data.Width, m_Data.Height, m_Data.Columns, m_Data.Rows, m_Data.Position, m_Data.Normal).Select(list => list.ToArray()).ToArray();
        m_Data.Holes = new PolygonData[holePoints.Length];

        for (int i = 0; i < holePoints.Length; i++)
        {
            m_Data.Holes[i] = new PolygonData(holePoints[i].ScalePolygon(m_Data.Scale).ToArray());
        }

        base.Build();
    }
}
