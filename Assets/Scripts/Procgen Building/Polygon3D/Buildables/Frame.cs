using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder;

namespace OnlyInvalid.ProcGenBuilding.Polygon3D
{
    public class Frame : BaseFrame
    {
        public FrameData FrameData => m_Data as FrameData;

        public override void Build()
        {
            if (!FrameData.IsDirty)
                return;

            FrameData.IsHoleDirty = true;

            if(FrameData.IsHoleDirty)
                CalculateInside();

            base.Build();
        }

        protected override void CalculateInside()
        {
            FrameData.ClearInterior();

            Vector3[] controlPoints = FrameData.ExteriorShape.ControlPoints();

            Matrix4x4 scale = Matrix4x4.Scale(Vector3.one * FrameData.FrameScale);

            for (int i = 0; i < controlPoints.Length; i++)
            {
                controlPoints[i] = scale.MultiplyPoint3x4(controlPoints[i]);
            }

            FrameData.AddInteriorShape(new PathShape(controlPoints));
        }
    }
}
