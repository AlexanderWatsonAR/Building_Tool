using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OnlyInvalid.ProcGenBuilding.Layout
{
    public class VerticalLayoutGroup : LayoutGroup
    {
        public override void Layout()
        {
            if (!LayoutGroupData.IsDirty)
                return;

            var polygons = MeshMaker.SpiltPolygon(LayoutGroupData.Polygon.ControlPoints, LayoutGroupData.Width, LayoutGroupData.Height, 1, LayoutGroupData.Polygons.Count, LayoutGroupData.Position, LayoutGroupData.Normal);

            for (int i = 0; i < polygons.Count; i++)
            {
                Vector3[] controlPoints = polygons[i].ToArray();

                LayoutGroupData.Polygons[i].Polygon3DData.SetPolygon(controlPoints, LayoutGroupData.Normal);
                LayoutGroupData.Polygons[i].Polygon3DData.IsDirty = true;
            }

            LayoutGroupData.IsDirty = false;
        }
    }
}
