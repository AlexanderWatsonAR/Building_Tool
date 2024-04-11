using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using OnlyInvalid.ProcGenBuilding.Common;

namespace OnlyInvalid.ProcGenBuilding.Corner
{
    public class Corner : Buildable
    {
        [SerializeReference] CornerData m_Data;

        [SerializeField] ProBuilderMesh m_ProBuilderMesh;

        public override Buildable Initialize(DirtyData data)
        {
            base.Initialize(data);
            m_Data = data as CornerData;
            m_ProBuilderMesh = GetComponent<ProBuilderMesh>();
            return this;
        }

        public override void Build()
        {
            ProBuilderMesh corner = ProBuilderMesh.Create();

            Vector3[] cornerPoints = m_Data.CornerPoints;

            switch (m_Data.Type)
            {
                case CornerType.Point:
                    cornerPoints = cornerPoints.SortPointsClockwise().ToArray();
                    corner.CreateShapeFromPolygon(cornerPoints, m_Data.Height, false);
                    break;
                case CornerType.Round:
                    int numberOfSamples = m_Data.Sides + 1;

                    List<Vector3> curveyPoints = Vector3Extensions.QuadraticLerpCollection(cornerPoints[1], cornerPoints[3], cornerPoints[2], numberOfSamples).ToList();
                    curveyPoints.Insert(0, cornerPoints[0]);

                    if (m_Data.IsInside)
                    {
                        curveyPoints = Vector3Extensions.QuadraticLerpCollection(cornerPoints[2], cornerPoints[3], cornerPoints[0], numberOfSamples).ToList();
                        curveyPoints.Insert(0, cornerPoints[1]);
                    }

                    curveyPoints = curveyPoints.SortPointsClockwise().ToList();

                    corner.CreateShapeFromPolygon(curveyPoints, 0, false);
                    Face[] extrudeFaces = corner.Extrude(new Face[] { corner.faces[0] }, ExtrudeMethod.FaceNormal, m_Data.Height);
                    //Smoothing.ApplySmoothingGroups(post, extrudeFaces, 360);
                    break;
                case CornerType.Flat:
                    corner.CreateShapeFromPolygon(m_Data.FlatPoints, m_Data.Height, false);
                    break;
            }

            corner.ToMesh();
            corner.Refresh();
            Rebuild(corner);
        }

        public override void Demolish()
        {
            DestroyImmediate(gameObject);
        }

        private void Rebuild(ProBuilderMesh mesh)
        {
            m_ProBuilderMesh.RebuildWithPositionsAndFaces(mesh.positions, mesh.faces);
            m_ProBuilderMesh.ToMesh();
            m_ProBuilderMesh.Refresh();
            DestroyImmediate(mesh.gameObject);
        }
    }
}
