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
            CalculateHole();
            return this;
        }

        public override void Build()
        {
            if (!m_GridFrameData.IsDirty)
                return;

            if (m_GridFrameData.IsHoleDirty)
                CalculateHole();

            base.Build();
        }

        private void CalculateHole()
        {
            Vector3[][] holePoints = MeshMaker.SpiltPolygon(m_GridFrameData.Polygon.ControlPoints, m_GridFrameData.Width, m_GridFrameData.Height, m_GridFrameData.Columns, m_GridFrameData.Rows, m_GridFrameData.Position, m_GridFrameData.Normal).Select(list => list.ToArray()).ToArray();
            m_GridFrameData.Holes = new PolygonData[holePoints.Length];

            for (int i = 0; i < holePoints.Length; i++)
            {
                m_GridFrameData.Holes[i] = new PolygonData(holePoints[i].ScalePolygon(m_GridFrameData.Scale).ToArray());
            }

            m_GridFrameData.IsHoleDirty = false;
        }
    }
}
