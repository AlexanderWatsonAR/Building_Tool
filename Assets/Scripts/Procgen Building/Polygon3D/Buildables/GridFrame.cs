using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using OnlyInvalid.ProcGenBuilding.Common;

namespace OnlyInvalid.ProcGenBuilding.Polygon3D
{
    public class GridFrame : Polygon3D
    {
        public GridFrameData GridFrameData => m_Data as GridFrameData;

        public override Buildable Initialize(DirtyData data)
        {
            return base.Initialize(data);
        }

        public override void Build()
        {
            if (!GridFrameData.IsDirty)
                return;

            GridFrameData.IsHoleDirty = true;

            if (GridFrameData.IsHoleDirty)
                CalculateHole();

            base.Build();
        }

        private void CalculateHole()
        {
            Vector3[][] holePoints = MeshMaker.SpiltPolygon(GridFrameData.Polygon.ControlPoints, GridFrameData.Width, GridFrameData.Height, GridFrameData.Columns, GridFrameData.Rows, GridFrameData.Position, GridFrameData.Normal).Select(list => list.ToArray()).ToArray();
            GridFrameData.Holes = new PolygonData[holePoints.Length];

            for (int i = 0; i < holePoints.Length; i++)
            {
                GridFrameData.Holes[i] = new PolygonData(holePoints[i].ScalePolygon(GridFrameData.Scale).ToArray());
            }

            GridFrameData.IsHoleDirty = false;
        }
    }
}
