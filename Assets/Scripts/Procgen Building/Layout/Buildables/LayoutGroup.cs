using OnlyInvalid.ProcGenBuilding.Common;
using OnlyInvalid.ProcGenBuilding.Polygon3D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OnlyInvalid.ProcGenBuilding.Layout
{
    public abstract class LayoutGroup : Polygon3D.Polygon3D
    {
        public LayoutGroupData LayoutGroupData => m_Data as LayoutGroupData;


        public override void Build()
        {
            Layout();
        }

        public abstract void Layout();

    }
}