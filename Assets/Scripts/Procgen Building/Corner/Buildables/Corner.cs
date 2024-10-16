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
            if (!m_Data.IsDirty)
                return;

            Vector3[] cornerPoints = PolygonMaker.WallCorner(CornerData.Angle);

            switch (CornerData.Type)
            {
                case CornerType.Point:
                    {
                        CornerData.SetShape(new PathShape(cornerPoints));
                    }
                    break;
                case CornerType.Round:
                    {
                        Vector3[] curveyPoints = Vector3Extensions.QuadraticLerpCollection(cornerPoints[^1], cornerPoints[0], cornerPoints[1], CornerData.Sides);

                        List<Vector3> points = new List<Vector3>();
                        points.Add(cornerPoints[2]);
                        points.AddRange(curveyPoints);

                        CornerData.SetShape(new PathShape(points));
                    }
                    break;
                case CornerType.Flat:
                    {
                        CornerData.SetShape(new PathShape(cornerPoints[1], cornerPoints[2], cornerPoints[3]));
                    }
                    break;
            }

            //Quaternion upRotation = Quaternion.FromToRotation(Vector3.forward, Vector3.up);

            //Matrix4x4 rotate = Matrix4x4.Rotate(upRotation);
            //Vector3 from = Vector3.zero.DirectionToTarget(cornerPoints[2]);

            //from = rotate.MultiplyPoint3x4(from);

            //Quaternion rotation = Quaternion.FromToRotation(from, CornerData.Forward) * upRotation;

            //CornerData.SetTransform(CornerData.Position, rotation.eulerAngles, CornerData.Scale);

            base.Build();
        }
    }
}
