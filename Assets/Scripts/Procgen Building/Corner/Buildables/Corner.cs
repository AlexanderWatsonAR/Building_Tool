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
    public class Corner : Polygon3D.Polygon3D
    {
        public CornerData CornerData => m_Data as CornerData;

        public override void Build()
        {
            base.Build();
            //Vector3[] cornerPoints = CornerData.CornerPoints;

            //switch (CornerData.Type)
            //{
            //    case CornerType.Point:
            //        cornerPoints = cornerPoints.SortPointsClockwise().ToArray();
            //        corner.CreateShapeFromPolygon(cornerPoints, CornerData.Height, false);
            //        break;
            //    case CornerType.Round:
            //        int numberOfSamples = CornerData.Sides + 1;

            //        List<Vector3> curveyPoints = Vector3Extensions.QuadraticLerpCollection(cornerPoints[1], cornerPoints[3], cornerPoints[2], numberOfSamples).ToList();
            //        curveyPoints.Insert(0, cornerPoints[0]);

            //        if (CornerData.IsInside)
            //        {
            //            curveyPoints = Vector3Extensions.QuadraticLerpCollection(cornerPoints[2], cornerPoints[3], cornerPoints[0], numberOfSamples).ToList();
            //            curveyPoints.Insert(0, cornerPoints[1]);
            //        }

            //        curveyPoints = curveyPoints.SortPointsClockwise().ToList();

            //        corner.CreateShapeFromPolygon(curveyPoints, 0, false);
            //        Face[] extrudeFaces = corner.Extrude(new Face[] { corner.faces[0] }, ExtrudeMethod.FaceNormal, CornerData.Height);
            //        //Smoothing.ApplySmoothingGroups(post, extrudeFaces, 360);
            //        break;
            //    case CornerType.Flat:
            //        corner.CreateShapeFromPolygon(CornerData.FlatPoints, m_CornerData.Height, false);
            //        break;
            //}

            //corner.ToMesh();
            //corner.Refresh();
            //Rebuild(corner);
        }
    }
}
