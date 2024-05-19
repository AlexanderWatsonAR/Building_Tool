using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OnlyInvalid.ProcGenBuilding.Common
{
    [System.Serializable]
    public class SectionData : Polygon3D.Polygon3DData
    {
        [SerializeField] int m_ID;
        [SerializeField] List<Opening> m_Openings;

        public List<Opening> Openings => m_Openings;

        public SectionData()
        {
            m_Openings = new List<Opening>();
        }
    }
}