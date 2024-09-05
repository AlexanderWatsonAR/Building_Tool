using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OnlyInvalid.ProcGenBuilding.Polygon3D;


namespace OnlyInvalid.ProcGenBuilding.Layout
{
    [System.Serializable]
    public class LayoutGroupData : Polygon3DData
    {
        [SerializeField] protected List<Polygon3D.Polygon3D> m_Polygons;

        public List<Polygon3D.Polygon3D> Polygons => m_Polygons;

        public LayoutGroupData() : base() 
        {
            m_Polygons = new List<Polygon3D.Polygon3D>();
        }

    }
}
