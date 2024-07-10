using UnityEngine;
using OnlyInvalid.ProcGenBuilding.Common;
using OnlyInvalid.ProcGenBuilding.Polygon3D;
using UnityEngine.Events;

namespace OnlyInvalid.ProcGenBuilding.Window
{
    public class OuterFrame : Frame
    {
        public OuterFrameData OuterFrameData => m_Data as OuterFrameData;

        public override void Build()
        {
            if (!OuterFrameData.IsDirty)
                return;

            base.Build();

            OuterFrameData.InnerPolygon3D.Polygon3DData.SetPolygon(OuterFrameData.Holes[0].ControlPoints, OuterFrameData.Normal);
            OuterFrameData.InnerPolygon3D.Polygon3DData.IsDirty = true;
            //BuildInnerPolygon();
        }

        private void BuildInnerPolygon()
        {
            OuterFrameData.InnerPolygon3D.Build();
        }

        public override void Demolish()
        {
            OuterFrameData.InnerPolygon3D.Demolish();
            base.Demolish();
        }

    }
}
