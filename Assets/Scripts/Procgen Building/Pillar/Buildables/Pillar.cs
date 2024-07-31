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
        PillarData PillarData => m_Data as PillarData;

        private void CreateControlPoints()
        {
            Vector3[] controlPoints = PillarData.Sides == 4 ? MeshMaker.Square() : MeshMaker.CalculateNPolygon(PillarData.Sides);

            Quaternion rotation = Quaternion.FromToRotation(Vector3.forward, Vector3.up);

            Matrix4x4 trs = Matrix4x4.TRS(Vector3.zero, rotation, Vector3.one * 0.25f);
            for (int i = 0; i < controlPoints.Length; i++)
            {
                controlPoints[i] = trs.MultiplyPoint3x4(controlPoints[i]);
            }

            PillarData.SetPolygon(controlPoints, Vector3.up);
        }

        public override void Build()
        {
            if (!PillarData.IsDirty)
                return;

            CreateControlPoints();

            base.Build();


            //if (PillarData.IsSmooth)
            //{
            //    Smoothing.ApplySmoothingGroups(m_ProBuilderMesh, faces, 360f / PillarData.Sides);
            //    m_ProBuilderMesh.ToMesh();
            //}

        }
    }
}
