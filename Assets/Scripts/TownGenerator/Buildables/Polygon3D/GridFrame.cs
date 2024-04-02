using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridFrame : Polygon3D
{
    [SerializeReference] GridFrameData m_Data;

    public GridFrameData Data => m_Data;

    public override IBuildable Initialize(DirtyData data)
    {
        m_Data = data as GridFrameData;
        base.Initialize(data);
        CalculateHoleData(m_Data);
        return this;
    }

    public override void Build()
    {
        if (!m_Data.IsDirty)
            return;

        base.Build();
    }

    public static void CalculateHoleData(GridFrameData data)
    {
        Vector3[][] holePoints = MeshMaker.SpiltPolygon(data.Polygon.ControlPoints, data.Width, data.Height, data.Columns, data.Rows, data.Position, data.Normal).Select(list => list.ToArray()).ToArray();
        data.Holes = new PolygonData[holePoints.Length];

        for (int i = 0; i < holePoints.Length; i++)
        {
            data.Holes[i] = new PolygonData(holePoints[i].ScalePolygon(data.Scale).ToArray());
        }
    }
}
