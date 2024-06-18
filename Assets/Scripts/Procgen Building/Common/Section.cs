using OnlyInvalid.ProcGenBuilding.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OnlyInvalid.ProcGenBuilding.Polygon3D;
using System.Linq;
using Unity.VisualScripting;
using System;

namespace OnlyInvalid.ProcGenBuilding.Common
{
    public class Section : Polygon3D.Polygon3D
    {
        [SerializeField] SectionData m_PreviousSectionData;

        public SectionData SectionData => m_Data as SectionData;

        public override Buildable Initialize(DirtyData data)
        {
            return base.Initialize(data);
        }
        public override void Build()
        {
            if (!m_Data.IsDirty)
                return;

            CalculateOpenings();

            base.Build();

            BuildContent();
        }
        private void CalculateOpenings()
        {
            if (SectionData.Openings == null)
                return;

            if (SectionData.Openings.Count() == 0)
                return;

            IList<IList<Vector3>> holePoints = new List<IList<Vector3>>();

            List<Vector3[]> sectionGrid = MeshMaker.CreateGridFromControlPoints(SectionData.Polygon.ControlPoints, SectionData.Openings.Count, 1);

            for (int i = 0; i < SectionData.Openings.Count(); i++)
            {
                OpeningData opening = SectionData.Openings[i];

                Vector3 bl = sectionGrid[0][i];
                Vector3 tl = sectionGrid[1][i];
                Vector3 tr = sectionGrid[1][i + 1];
                Vector3 br = sectionGrid[0][i + 1];

                IList<Vector3> hole = CalculateOpening(opening, new Vector3[] { bl, tl, tr, br });

                holePoints.Add(hole);

                if (!opening.HasContent)
                    continue;

                Polygon3D.Polygon3D polygon3D = opening.Polygon3D;
                Polygon3DData polygon3DData = polygon3D.Data as Polygon3DData;
                polygon3DData.SetPolygon(hole.ToArray(), SectionData.Normal);
                polygon3DData.IsDirty = true;
            }

            SectionData.SetHoles(holePoints);
        }
        private void BuildContent()
        {
            foreach(OpeningData opening in SectionData.Openings)
            {
                if (!opening.HasContent)
                    continue;

                opening.Polygon3D.Build();
            }
        }
        private IList<Vector3> CalculateOpening(OpeningData opening, Vector3[] controlPoints)
        {
            Vector3[] hole = opening.Shape.ControlPoints();
            Vector3[] scaledControlPoints = new Vector3[controlPoints.Length];

            Array.Copy(controlPoints, scaledControlPoints, controlPoints.Length);

            float height = SectionData.Height;
            float width = SectionData.Width / SectionData.Openings.Count;

            Vector3 position = controlPoints.Centroid();
            Quaternion rotation = Quaternion.FromToRotation(Vector3.forward, SectionData.Normal);
            Vector3 sectionScale = new Vector3(width * 0.5f, height * 0.5f);

            Vector3 offset = new Vector3(opening.Position.x, opening.Position.y);
            Quaternion openingRotation = Quaternion.Euler(Vector3.forward * opening.Angle);
            Vector3 openingScale = new Vector3(opening.Width, opening.Height);

            Matrix4x4 sectionTRS = Matrix4x4.TRS(position, rotation, sectionScale);
            Matrix4x4 openingTRS = Matrix4x4.TRS(offset, openingRotation, openingScale);

            Matrix4x4 combinedMatrix = sectionTRS * openingTRS;

            for (int i = 0; i < hole.Length; i++)
            {
                hole[i] = combinedMatrix.MultiplyPoint3x4(hole[i]);
            }

            Matrix4x4 scaleMatrix = Matrix4x4.Translate(position) * Matrix4x4.Scale(Vector3.one * 0.999f) * Matrix4x4.Translate(-position);

            for (int i = 0; i < scaledControlPoints.Length; i++)
            {
                scaledControlPoints[i] = scaleMatrix.MultiplyPoint3x4(scaledControlPoints[i]);
            }

            hole = MeshMaker.ClampToQuad(scaledControlPoints, hole);
            
            opening.IsDirty = false;

            return hole.ToList();
        }
        public override void Demolish()
        {
            //base.Demolish();
        }
    }
}
