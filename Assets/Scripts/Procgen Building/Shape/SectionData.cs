using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OnlyInvalid.ProcGenBuilding.Common
{
    [System.Serializable]
    public class SectionData : Polygon3D.Polygon3DData
    {
        [SerializeField] int m_ID;
        [SerializeField] OpeningDataList m_Openings;

        public OpeningDataList Openings => m_Openings;

        public int ID { get { return m_ID; } set { m_ID = value; } }

        public SectionData()
        {
            m_Openings = new OpeningDataList();
        }

        public SectionData(SectionData sectionData) : base(sectionData)
        {
            m_Openings = sectionData.Openings;
        }

        public void AddOpening(OpeningData opening)
        {
            m_Openings.Add(opening);
        }
    }
}