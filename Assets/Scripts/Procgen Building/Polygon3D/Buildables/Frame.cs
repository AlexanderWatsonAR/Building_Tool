using OnlyInvalid.ProcGenBuilding.Common;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.Shapes;

namespace OnlyInvalid.ProcGenBuilding.Polygon3D
{
    public class Frame : BaseFrame
    {
        public FrameData FrameData => m_Data as FrameData;

        public override Buildable Initialize(DirtyData data)
        {
            base.Initialize(data);

            FrameData.AddToInterior(new Polygon2DData(Vector3.zero, Vector3.zero, Vector3.one * FrameData.InsideScale, FrameData.ExteriorShape, null));

            return this;
        }

        public override void Build()
        {
            if (!m_Data.IsDirty)
                return;

            FrameData.InteriorShapes[0].SetScale(FrameData.InsideScale);

            base.Build();

        }
    }
}
