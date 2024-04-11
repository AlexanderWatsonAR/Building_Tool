using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OnlyInvalid.ProcGenBuilding.Common;

namespace OnlyInvalid.ProcGenBuilding.Pillar
{
    public class Pillars : MonoBehaviour, IBuildable
    {
        [SerializeField] PillarsData m_Data;
        [SerializeField] Pillar[] m_Pillars;

        public IBuildable Initialize(DirtyData data)
        {
            m_Data = data as PillarsData;
            m_Pillars = new Pillar[m_Data.Pillars.Length];
            return this;
        }
        public void Build()
        {
            m_Pillars.BuildCollection();
        }
        public void Demolish()
        {
            m_Pillars.DemolishCollection();
        }
    }
}
