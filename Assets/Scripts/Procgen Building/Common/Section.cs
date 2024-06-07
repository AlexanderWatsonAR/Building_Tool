using OnlyInvalid.ProcGenBuilding.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OnlyInvalid.ProcGenBuilding.Polygon3D;
using UnityEngine.UIElements;
using System.Linq;
using Unity.VisualScripting;
using System;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

namespace OnlyInvalid.ProcGenBuilding.Common
{
    public class Section : Polygon3D.Polygon3D
    {
        public SectionData SectionData => m_Data as SectionData;

        public override Buildable Initialize(DirtyData data)
        {
            return base.Initialize(data);
        }
        public override void Build()
        {
            if (SectionData.Openings == null)
                return;

            if (SectionData.Openings.Count == 0)
                return;

            BuildWithMatrix();

            //OpeningData opening = SectionData.Openings[0];

            //Vector3[] hole = opening.Shape.ControlPoints();

            //Vector3 position = SectionData.Position;
            //Vector3 offset = new Vector3(opening.Position.x, opening.Position.y);

            //position += offset;

            //Quaternion rotation = Quaternion.FromToRotation(Vector3.forward, SectionData.Normal);

            //Vector3[] scaledControlPoints = new Vector3[SectionData.Polygon.ControlPoints.Length];

            //Array.Copy(SectionData.Polygon.ControlPoints, scaledControlPoints, SectionData.Polygon.ControlPoints.Length);

            //for (int i = 0; i < hole.Length; i++)
            //{
            //    // Scale
            //    Vector3 point = hole[i];
            //    Vector3 v = Vector3.Scale(point, new Vector3(SectionData.Width * 0.5f, SectionData.Height * 0.5f));
            //    hole[i] = v;

            //    // Position
            //    hole[i] += position;

            //    // Scale
            //    Vector3 point1 = hole[i] - position;
            //    Vector3 v2 = Vector3.Scale(point1, new Vector3(opening.Width, opening.Height)) + position;
            //    hole[i] = v2;

            //    // Rotate
            //    Vector3 euler0 = Quaternion.AngleAxis(opening.Angle, Vector3.forward).eulerAngles;
            //    Vector3 v3 = Quaternion.Euler(euler0) * (hole[i] - position) + position;
            //    hole[i] = v3;

            //    // Rotate
            //    Vector3 euler = rotation.eulerAngles;
            //    Vector3 v1 = Quaternion.Euler(euler) * (hole[i] - position) + position;
            //    hole[i] = v1;
            //}

            //for(int i = 0; i < scaledControlPoints.Length; i++)
            //{
            //    // Scale
            //    Vector3 point = scaledControlPoints[i] - position;
            //    Vector3 v = Vector3.Scale(point, Vector3.one * 0.999f) + position;
            //    scaledControlPoints[i] = v;
            //}

            //hole = MeshMaker.ConstrainPolygonToQuad(scaledControlPoints, hole);

            //IList<IList<Vector3>> holePoints = new List<IList<Vector3>>();
            //holePoints.Add(hole.ToList());

            //SectionData.SetHoles(holePoints);

            base.Build();
        }

        private void BuildWithMatrix()
        {
            OpeningData opening = SectionData.Openings[0];

            Vector3[] hole = opening.Shape.ControlPoints();
            Vector3[] scaledControlPoints = new Vector3[SectionData.Polygon.ControlPoints.Length];

            Array.Copy(SectionData.Polygon.ControlPoints, scaledControlPoints, SectionData.Polygon.ControlPoints.Length);

            float height = SectionData.Height;
            float width = SectionData.Width;

            Vector3 position = SectionData.Position;
            Quaternion rotation = Quaternion.FromToRotation(Vector3.forward, SectionData.Normal);
            Vector3 sectionScale = new Vector3(width * 0.5f, height * 0.5f);

            Vector3 offset = new Vector3(opening.Position.x, opening.Position.y);
            Vector3 eulerAngle = Vector3.forward * opening.Angle;
            Vector3 openingScale = new Vector3(opening.Width, opening.Height);

            Matrix4x4 sectionTranslationMatrix = Matrix4x4.Translate(position);
            Matrix4x4 sectionScaleMatrix = Matrix4x4.Scale(sectionScale);
            Matrix4x4 sectionRotationMatrix = Matrix4x4.Rotate(rotation);

            Matrix4x4 openingOffsetMatrix = Matrix4x4.Translate(offset);
            Matrix4x4 openingScaleMatrix = Matrix4x4.Scale(openingScale);
            Matrix4x4 openingRotationMatrix = Matrix4x4.Rotate(Quaternion.Euler(eulerAngle));

            Matrix4x4 openingTransformation = openingOffsetMatrix * openingRotationMatrix * openingScaleMatrix;
            Matrix4x4 sectionTransformation = sectionTranslationMatrix * sectionRotationMatrix * sectionScaleMatrix;

            Matrix4x4 combinedMatrix =  sectionTransformation * openingTransformation;

            for (int i = 0; i < hole.Length; i++)
            {
                hole[i] = combinedMatrix.MultiplyPoint3x4(hole[i]);
            }

            for (int i = 0; i < scaledControlPoints.Length; i++)
            {
                // Scale
                Vector3 point = scaledControlPoints[i] - position;
                Vector3 v = Vector3.Scale(point, Vector3.one * 0.999f) + position;
                scaledControlPoints[i] = v;
            }

            hole = MeshMaker.ClampToQuad(scaledControlPoints, hole);

            IList<IList<Vector3>> holePoints = new List<IList<Vector3>>();
            holePoints.Add(hole.ToList());

            SectionData.SetHoles(holePoints);

        }


        public override void Demolish()
        {
            //base.Demolish();
        }
    }
}
