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
        [SerializeReference] FrameData m_FrameData;

        public override Buildable Initialize(DirtyData data)
        {
            base.Initialize(data);
            m_FrameData = data as FrameData;
            m_FrameData.Holes = new PolygonData[1];
            m_FrameData.Holes[0] = new PolygonData(m_FrameData.Polygon.ControlPoints.ScalePolygon(m_FrameData.Scale, m_FrameData.Position), m_FrameData.Polygon.Normal);
            return this;
        }

        public override void Build()
        {
            if (!m_FrameData.IsDirty)
                return;

            base.Build();
        }
    }
}
