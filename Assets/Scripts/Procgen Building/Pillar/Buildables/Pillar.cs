using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using OnlyInvalid.ProcGenBuilding.Common;


namespace OnlyInvalid.ProcGenBuilding.Pillar
{
    public class Pillar : Polygon3D.Polygon3D
    {
        public PillarData PillarData => m_Data as PillarData;

        public override void Build()
        {
            //if (!PillarData.IsDirty)
            //    return;

            base.Build();


            //if (PillarData.IsSmooth)
            //{
            //    Smoothing.ApplySmoothingGroups(m_ProBuilderMesh, faces, 360f / PillarData.Sides);
            //    m_ProBuilderMesh.ToMesh();
            //}

        }
    }
}
