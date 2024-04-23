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
    public class Pillar : Buildable
    {
        [SerializeField] ProBuilderMesh m_ProBuilderMesh;
        [SerializeReference] PillarData m_PillarData;

        public override Buildable Initialize(DirtyData data)
        {
            base.Initialize(data);
            m_PillarData = new PillarData(data as PillarData);
            m_ProBuilderMesh = GetComponent<ProBuilderMesh>();
            return this;
        }

        private void CreateControlPoints()
        {
            float halfWidth = m_PillarData.Width * 0.5f;
            float halfDepth = m_PillarData.Depth * 0.5f;

            Vector3[] controlPoints = new Vector3[]
            {
            new Vector3(-halfWidth, -halfDepth),
            new Vector3(-halfWidth, halfDepth),
            new Vector3(halfWidth, halfDepth),
            new Vector3(halfWidth, -halfDepth)
            };

            Quaternion rotation = Quaternion.FromToRotation(Vector3.forward, Vector3.up);

            if (m_PillarData.Sides != 4)
            {
                controlPoints = MeshMaker.CreateNPolygon(m_PillarData.Sides, halfWidth, halfDepth);
            }
            // Orientate points to the XZ plane.
            for (int i = 0; i < controlPoints.Length; i++)
            {
                Vector3 euler = rotation.eulerAngles;
                Vector3 v = Quaternion.Euler(euler) * controlPoints[i];
                controlPoints[i] = v;
            }

            m_PillarData.ControlPoints = controlPoints;
        }

        public override void Build()
        {
            CreateControlPoints();
            m_ProBuilderMesh.CreateShapeFromPolygon(m_PillarData.ControlPoints, 0, false);
            m_ProBuilderMesh.ToMesh();
            Face[] faces = m_ProBuilderMesh.Extrude(m_ProBuilderMesh.faces, ExtrudeMethod.FaceNormal, m_PillarData.Height);
            m_ProBuilderMesh.ToMesh();

            if (m_PillarData.IsSmooth)
            {
                Smoothing.ApplySmoothingGroups(m_ProBuilderMesh, faces, 360f / m_PillarData.Sides);
                m_ProBuilderMesh.ToMesh();
            }

            GetComponent<Renderer>().material = m_PillarData.Material;
            m_ProBuilderMesh.Refresh();
        }

        public override void Demolish()
        {

        }
    }
}
