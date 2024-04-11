using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OnlyInvalid.ProcGenBuilding.Common;

namespace OnlyInvalid.ProcGenBuilding.Corner
{
    public class Corners : Buildable
    {
        [SerializeField] CornersData m_Data;
        [SerializeField] Corner[] m_Corners;

        public override Buildable Initialize(DirtyData data)
        {
            base.Initialize(data);
            m_Data = data as CornersData;
            m_Corners = new Corner[m_Data.Corners.Length];
            return this;
        }

        public override void Build()
        {
            m_Corners.BuildCollection();
        }

        public override void Demolish()
        {
            m_Corners.DemolishCollection();
        }
    }
}
