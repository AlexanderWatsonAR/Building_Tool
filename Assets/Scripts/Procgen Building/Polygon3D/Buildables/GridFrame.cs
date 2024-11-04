using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using OnlyInvalid.ProcGenBuilding.Common;
using OnlyInvalid.Polygon.Clipper_API;

namespace OnlyInvalid.ProcGenBuilding.Polygon3D
{
    public class GridFrame : BaseFrame
    {
        public GridFrameData GridFrameData => m_Data as GridFrameData;

        public override void Build()
        {
            if (!GridFrameData.IsDirty)
                return;

            GridFrameData.IsHoleDirty = true;

            if (GridFrameData.IsHoleDirty)
                CalculateInside();

            base.Build();
        }

        protected void CalculateInside()
        {
            GridFrameData.ClearInterior();

            var split = Clipper.Split(GridFrameData.ExteriorShape.ControlPoints(), GridFrameData.Columns, GridFrameData.Rows, GridFrameData.InsideScale);

            foreach (var section in split)
            {
                GridFrameData.AddToInterior(new Polygon2DData(section));
            }

            GridFrameData.IsHoleDirty = false;
        }
    }
}
