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
            FrameData.Holes = new PolygonData[1];
            Matrix4x4 scaleMatrix = Matrix4x4.Translate(FrameData.Position) * Matrix4x4.Scale(Vector3.one * FrameData.Scale) * Matrix4x4.Translate(-FrameData.Position);

            Vector3[] controlPoints = new Vector3[FrameData.Polygon.ControlPoints.Length];
            System.Array.Copy(FrameData.Polygon.ControlPoints, controlPoints, controlPoints.Length);

            for(int i = 0; i < controlPoints.Length; i++)
            {
                controlPoints[i] = scaleMatrix.MultiplyPoint3x4(controlPoints[i]);
            }

            FrameData.Holes[0] = new PolygonData(controlPoints, FrameData.Polygon.Normal);

            FrameData.IsHoleDirty = false;
        }
    }
}
