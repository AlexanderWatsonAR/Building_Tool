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
            Vector3[] controlPoints = PillarData.Sides == 4 ? MeshMaker.Square() : MeshMaker.CalculateNPolygon(PillarData.Sides, 1, 1);

            Quaternion rotation = Quaternion.FromToRotation(Vector3.forward, Vector3.up);
            //Vector3 scale = new Vector3(PillarData.Height * 0.5f, PillarData.Width * 0.5f, 1);

            Matrix4x4 trs = Matrix4x4.TRS(Vector3.zero, rotation, Vector3.one);

            // Orientate points to the XZ plane.
            for (int i = 0; i < controlPoints.Length; i++)
            {
                controlPoints[i] = trs.MultiplyPoint3x4(controlPoints[i]);
            }

            PillarData.SetPolygon(controlPoints, PillarData.Up);

            // Issues with using polygon3d as a base:
            // 1. Extrude is using depth, we want to extrude by height.
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
