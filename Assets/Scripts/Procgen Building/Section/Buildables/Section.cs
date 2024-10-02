using OnlyInvalid.ProcGenBuilding.Polygon3D;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OnlyInvalid.ProcGenBuilding.Common
{
    public class Section : Polygon3D.Polygon3D
    {
        public SectionData SectionData => m_Data as SectionData;

        #region Settings
        [SerializeField] bool m_IsHorizontal, m_IsVertical, m_IsDirectionReversed;
        public int Columns
        {
            get
            {
                return m_IsVertical ? 1 : SectionData.Openings.Count;
            }
            
        }
        public int Rows
        {
            get
            {
                return m_IsHorizontal ? 1 : SectionData.Openings.Count;
            }

        }
        public List<Vector3[]> Grid
        {
            get
            {
                return MeshMaker.CreateGridFromControlPoints(SectionData.ControlPoints, Columns, Rows);
            }
        }
        public float Height
        {
            get
            {
                return 1;
                //return m_IsHorizontal ? SectionData.Height : SectionData.Height / SectionData.Openings.Count;
            }
        }
        public float Width
        {
            get
            {
                return 1;
                //return m_IsVertical ? SectionData.Width : SectionData.Width / SectionData.Openings.Count;
            }
        }
        #endregion

        public override Buildable Initialize(DirtyData data)
        {
            m_IsHorizontal = true;
            m_IsVertical = false;
            m_IsDirectionReversed = false;

            return base.Initialize(data);
        }
        public override void Build()
        {
            if (!m_Data.IsDirty)
                return;

            CalculateOpenings();

            base.Build();

            //BuildContent();
        }
        private void CalculateOpenings()
        {
            if (SectionData.Openings == null)
            {
                SectionData.ClearInterior();
                return;
            }

            if (SectionData.Openings.Count() == 0)
            {
                SectionData.ClearInterior();
                return;
            }

            IList<IList<Vector3>> holePoints = new List<IList<Vector3>>();

            int index = 0;
            int i = m_IsDirectionReversed ? Columns-1 : 0;
            int iterator = m_IsDirectionReversed ? -1 : 1;

            for (; m_IsDirectionReversed ? i > -1 : i < Columns; i += iterator)
            {
                int j = m_IsDirectionReversed ? Rows - 1 : 0;

                for (; m_IsDirectionReversed ? j > -1 : j < Rows; j += iterator)
                {
                    OpeningData opening = SectionData.Openings[index];

                    index++;

                    if (!opening.IsActive)
                        continue;

                    Vector3 bl = Grid[j][i];
                    Vector3 tl = Grid[j + 1][i];
                    Vector3 tr = Grid[j+ 1][i + 1];
                    Vector3 br = Grid[j][i + 1];

                    IList<Vector3> hole = CalculateOpening(opening, new Vector3[] { bl, tl, tr, br });

                    holePoints.Add(hole);

                    if (!opening.HasContent)
                        continue;

                    Polygon3D.Polygon3D polygon3D = opening.Polygon3D;
                    Polygon3DData polygon3DData = polygon3D.Data as Polygon3DData;
                    polygon3DData.SetPolygon(hole.ToArray(), SectionData.Normal());
                    polygon3DData.IsDirty = true;
                }
            }
            
            foreach(var hole in holePoints)
            {
                SectionData.AddInteriorShape(new PathShape(hole));
            }
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

            float height = Height;
            float width = Width;

            Vector3 position = controlPoints.Centroid();
            Quaternion rotation = Vector3Extensions.Approximately(SectionData.Normal(), Vector3.back) ? Quaternion.identity : Quaternion.FromToRotation(Vector3.forward, SectionData.Normal());
            Vector3 sectionScale = new Vector3(width * 0.5f, height * 0.5f);

            Vector3 offset = new Vector3(opening.Position.x, opening.Position.y);
            Quaternion openingRotation = Quaternion.Euler(Vector3.forward * opening.Angle);
            Vector3 openingScale = new Vector3(opening.Scale.x, opening.Scale.y);

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
    }
}
