using UnityEngine;
using OnlyInvalid.ProcGenBuilding.Common;

namespace OnlyInvalid.ProcGenBuilding.Corner
{
    public class Corners : Buildable
    {
        [SerializeField] CornersData m_CornersData;
        [SerializeField] Corner[] m_Corners;

        public override Buildable Initialize(DirtyData data)
        {
            base.Initialize(data);
            m_CornersData = data as CornersData;
            m_Corners = new Corner[m_CornersData.Corners.Length];
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
