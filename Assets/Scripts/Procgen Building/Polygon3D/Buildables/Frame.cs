using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder;
using OnlyInvalid.ProcGenBuilding.Common;
using UnityEngine.UIElements;

namespace OnlyInvalid.ProcGenBuilding.Polygon3D
{
    public class Frame : Polygon3D
    {
        public FrameData FrameData => m_Data as FrameData;

        public override Buildable Initialize(DirtyData data)
        {
            return base.Initialize(data);
        }

        public override void Build()
        {
            if (!FrameData.IsDirty)
                return;

            if(FrameData.IsHoleDirty)
                CalculateHoleWithMatrix();

            base.Build();
        }

        private void CalculateHole()
        {
            FrameData.Holes = new PolygonData[1];
            FrameData.Holes[0] = new PolygonData(FrameData.Polygon.ControlPoints.ScalePolygon(FrameData.Scale, FrameData.Position), FrameData.Polygon.Normal);
            FrameData.IsHoleDirty = false;
        }

        private void CalculateHoleWithMatrix()
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
        }
    }
}
