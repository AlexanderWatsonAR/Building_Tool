using OnlyInvalid.ProcGenBuilding.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OnlyInvalid.ProcGenBuilding.Polygon3D;

namespace OnlyInvalid.ProcGenBuilding.Common
{
    public class Section : Polygon3D.Polygon3D
    {
        [SerializeField] SectionData m_SectionData;
        [SerializeField] List<Buildable> m_Buildables;

        public SectionData SectionData => m_SectionData;

        public override Buildable Initialize(DirtyData data)
        {
            m_SectionData = data as SectionData;
            m_SectionData.IsDirty = false;
            return base.Initialize(data);
        }
        public override void Build()
        {
            //base.Build();
        }
        public override void Demolish()
        {
            //base.Demolish();
        }
    }
}
