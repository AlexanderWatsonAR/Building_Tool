using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using OnlyInvalid.ProcGenBuilding.Common;

namespace OnlyInvalid.ProcGenBuilding.Floor
{
    public class FloorSection : Buildable
    {
        [SerializeField] FloorSectionData m_FloorSectionData;
        [SerializeField] ProBuilderMesh m_ProBuilderMesh;

        public override Buildable Initialize(DirtyData data)
        {
            m_FloorSectionData = data as FloorSectionData;
            m_ProBuilderMesh = GetComponent<ProBuilderMesh>();
            return this;
        }

        public override void Build()
        {
            m_ProBuilderMesh.CreateShapeFromPolygon(m_FloorSectionData.ControlPoints, m_FloorSectionData.Height, false);
            m_ProBuilderMesh.ToMesh();
            m_ProBuilderMesh.Refresh();
        }
        public override void Demolish()
        {
        }
    }
}
