using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.ProBuilder;
using OnlyInvalid.ProcGenBuilding.Common;

namespace OnlyInvalid.ProcGenBuilding.Pillar
{
    [System.Serializable]
    public class PillarData : Polygon3D.Polygon3DData
    {
        [SerializeField, HideInInspector] int m_ID;
        [SerializeField, Range(3, 32)] int m_Sides;
        [SerializeField] bool m_IsSmooth;

        public int ID { get { return m_ID; } set { m_ID = value; } }
        public int Sides { get { return m_Sides; } set { m_Sides = value; } }
        public bool IsSmooth { get { return m_IsSmooth; } set { m_IsSmooth = value; } }

        public PillarData() : base()
        {
            m_Sides = 4;
            m_IsSmooth = false;
        }
        public PillarData(PillarData data) : base(data)
        {
            m_Sides = data.Sides;
            m_IsSmooth = data.IsSmooth;
        }

        public override bool Equals(object obj)
        {
            if (obj is not PillarData) return false;

            //PillarData pillar = obj as PillarData;

            //if (pillar.Height == m_Height &&
            //    pillar.Width == m_Width &&
            //    pillar.Depth == m_Depth &&
            //    pillar.Sides == m_Sides &&
            //    pillar.IsSmooth == m_IsSmooth)
            //{
            //    return true;
            //}

            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
