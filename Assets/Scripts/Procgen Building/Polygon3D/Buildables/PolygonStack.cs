using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace OnlyInvalid.ProcGenBuilding.Polygon3D
{
    public class PolygonStack : Polygon3D
    {
        public PolygonStackData PolygonStackData => m_Data as PolygonStackData;


        public override void Build()
        {
            if (!PolygonStackData.IsDirty)
                return;

            base.Build();
        }

    }
}