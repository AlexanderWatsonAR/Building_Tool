using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OnlyInvalid.ProcGenBuilding.Polygon3D
{
    public class PolygonStackData : Polygon3DData
    {
        [SerializeReference] List<Polygon3D> m_Stack; // e.g. contents

        public List<Polygon3D> Stack => m_Stack;

        public PolygonStackData() : base() { }

    }
}