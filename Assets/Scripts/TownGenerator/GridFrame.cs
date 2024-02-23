using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridFrame : Polygon3D
{
    [SerializeField] GridFrameData m_Data;

    public GridFrameData Data => m_Data;

    public override IBuildable Initialize(IData data)
    {
        m_Data = data as GridFrameData;
        base.Initialize(data);
        return this;
    }

    public override void Build()
    {
        Vector3[][] holePoints = MeshMaker.SpiltPolygon(m_Data.ControlPoints, m_Data.Width, m_Data.Height, m_Data.Columns, m_Data.Rows, m_Data.Position, m_Data.Normal).Select(list => list.ToArray()).ToArray();

        for(int i = 0; i < holePoints.Length; i++)
        {
            holePoints[i] = holePoints[i].ScalePolygon(m_Data.Scale);

        }

        m_Data.HolePoints = holePoints;

        base.Build();
    }
}
