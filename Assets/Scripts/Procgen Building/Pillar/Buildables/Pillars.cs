using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OnlyInvalid.ProcGenBuilding.Common;

namespace OnlyInvalid.ProcGenBuilding.Pillar
{
    public class Pillars : Buildable
    {
        [SerializeField] PillarsData m_PillarsData;
        [SerializeField] Pillar[] m_Pillars;

        public override Buildable Initialize(DirtyData data)
        {
            m_PillarsData = data as PillarsData;
            m_Pillars = new Pillar[m_PillarsData.Pillars.Length];
            return this;
        }
        public override void Build()
        {
            m_Pillars.BuildCollection();
        }
        public override void Demolish()
        {
            m_Pillars.DemolishCollection();
        }
    }
}
