using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OnlyInvalid.ProcGenBuilding.Common
{
    [System.Serializable]
    public class SectionData : Polygon3DAData
    {
        [SerializeField] int m_ID;
        [SerializeField] OpeningDataList m_Openings;

        public OpeningDataList Openings => m_Openings;

        public int ID { get { return m_ID; } set { m_ID = value; } }

        public SectionData()
        {
            m_Openings = new OpeningDataList();
        }

        public SectionData(Shape exteriorShape, List<Polygon2DData> interiorShapes, float depth, Vector3 position, Vector3 eulerAngle, Vector3 scale) : base (position, eulerAngle, scale, exteriorShape, interiorShapes, depth)
        {

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