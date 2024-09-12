using OnlyInvalid.ProcGenBuilding.Common;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using OnlyInvalid.Polygon.Clipper_API;

namespace OnlyInvalid.ProcGenBuilding.Layout
{
    public class HorizontalLayoutGroup : LayoutGroup
    {
        public override void Layout()
        {
            if (!LayoutGroupData.IsDirty)
                return;

            var polygons = Clipper.Split(LayoutGroupData.ControlPoints, new Vector2Int(LayoutGroupData.Polygons.Count, 1), Vector3.one);

            for (int i = 0; i < polygons.Count; i++)
            {
                Vector3[] controlPoints = polygons[i].ToArray();

                LayoutGroupData.Polygons[i].Polygon3DData.SetExteriorShape(new PathShape(controlPoints));
                LayoutGroupData.Polygons[i].Polygon3DData.IsDirty = true;
            }

            LayoutGroupData.IsDirty = false;
        }
    }
}

