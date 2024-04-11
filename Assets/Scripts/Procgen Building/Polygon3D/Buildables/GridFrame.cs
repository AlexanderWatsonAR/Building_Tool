using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using OnlyInvalid.ProcGenBuilding.Common;

namespace OnlyInvalid.ProcGenBuilding.Polygon3D
{
    public class GridFrame : Polygon3D
    {
        [SerializeReference] GridFrameData m_GridFrameData;

        public override Buildable Initialize(DirtyData data)
        {
            base.Initialize(data);
            m_GridFrameData = data as GridFrameData;

            CalculateHoleData(m_GridFrameData);
            return this;
        }

        public override void Build()
        {
            if (!m_GridFrameData.IsDirty)
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
}
