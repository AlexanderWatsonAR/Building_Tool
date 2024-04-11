using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OnlyInvalid.ProcGenBuilding.Common;

namespace OnlyInvalid.ProcGenBuilding.Corner
{
    public class Corners : MonoBehaviour, IBuildable
    {
        [SerializeField] CornersData m_Data;
        [SerializeField] Corner[] m_Corners;

        public IBuildable Initialize(DirtyData data)
        {
            m_Data = data as CornersData;
            m_Corners = new Corner[m_Data.Corners.Length];
            return this;
        }

        public void Build()
        {
            m_Corners.BuildCollection();
        }

        public void Demolish()
        {
            m_Corners.DemolishCollection();
        }
    }
}
