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
using UnityEditor;

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
            if (!m_Data.IsDirty)
                return;



            //foreach(OpeningData opening in SectionData.Openings)
            //{
            if (SectionData.Openings != null && SectionData.Openings.Count > 0)
                BuildWithMatrix(SectionData.Openings[0]);
            //}

            base.Build();
        }

        private void BuildWithMatrix(OpeningData opening)
        {
            Vector3[] hole = opening.Shape.ControlPoints();
            Vector3[] scaledControlPoints = new Vector3[SectionData.Polygon.ControlPoints.Length];

            Array.Copy(SectionData.Polygon.ControlPoints, scaledControlPoints, SectionData.Polygon.ControlPoints.Length);

            float height = SectionData.Height;
            float width = SectionData.Width;

            Vector3 position = SectionData.Position;
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

            IList<IList<Vector3>> holePoints = new List<IList<Vector3>>();
            holePoints.Add(hole.ToList());

            SectionData.SetHoles(holePoints);

            Polygon3D.Polygon3D polygon3D = opening.Polygon3D;
            Polygon3DData polygon3DData = polygon3D.Data as Polygon3DData;
            polygon3DData.SetPolygon(hole, SectionData.Normal);

        }


        public override void Demolish()
        {
            //base.Demolish();
        }
    }
}
