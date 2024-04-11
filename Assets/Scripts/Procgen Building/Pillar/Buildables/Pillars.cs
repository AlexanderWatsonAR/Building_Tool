using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OnlyInvalid.ProcGenBuilding.Common;

namespace OnlyInvalid.ProcGenBuilding.Pillar
{
    public class Pillars : Buildable
    {
        [SerializeField] PillarsData m_Data;
        [SerializeField] Pillar[] m_Pillars;

        public override Buildable Initialize(DirtyData data)
        {
            m_Data = data as PillarsData;
            m_Pillars = new Pillar[m_Data.Pillars.Length];
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
