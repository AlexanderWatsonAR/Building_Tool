using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder;
using OnlyInvalid.ProcGenBuilding.Common;

namespace OnlyInvalid.ProcGenBuilding.Polygon3D
{
    public class Frame : Polygon3D
    {
        [SerializeReference] FrameData m_Data;

        public new FrameData Data => m_Data;

        public override IBuildable Initialize(DirtyData data)
        {
            m_Data = data as FrameData;
            base.Initialize(data);

            m_Data.Holes = new PolygonData[1];
            m_Data.Holes[0] = new PolygonData(m_Data.Polygon.ControlPoints.ScalePolygon(m_Data.Scale, m_Data.Position), m_Data.Polygon.Normal);
            return this;
        }

        public override void Build()
        {
            if (!m_Data.IsDirty)
                return;

            base.Build();
        }
    }
}
