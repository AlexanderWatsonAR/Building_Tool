using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OnlyInvalid.ProcGenBuilding.Common
{
    [System.Serializable]
    public class NPolygon : Shape
    {
        [SerializeField] int m_Sides;

        public int Sides { get { return m_Sides; } set { m_Sides = value; } }

        public NPolygon() : this(4)
        {

        }

        public NPolygon(int sides) : base()
        {
            m_Sides = sides;
        }
    }
}
