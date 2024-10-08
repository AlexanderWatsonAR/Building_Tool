using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;
using OnlyInvalid.ProcGenBuilding.Common;

namespace OnlyInvalid.ProcGenBuilding.Polygon3D
{
    public abstract class Polygon3D : Polygon2D
    {
        public Polygon3DAData Polygon3DData => m_Data as Polygon3DAData;

        public override void Build()
        {
            if (!m_Data.IsDirty)
                return;

            base.Build();

            m_ProBuilderMesh.Solidify(Polygon3DData.Depth);
        }
    }
}
