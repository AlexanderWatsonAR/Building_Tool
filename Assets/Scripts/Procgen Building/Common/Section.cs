using OnlyInvalid.ProcGenBuilding.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OnlyInvalid.ProcGenBuilding.Polygon3D;
using UnityEngine.UIElements;
using System.Linq;

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

            OpeningData opening = SectionData.Openings[0];

            Vector3[] hole = opening.Shape.ControlPoints();

            Quaternion rotation = Quaternion.FromToRotation(Vector3.forward, SectionData.Normal);

            for (int i = 0; i < hole.Length; i++)
            {
                // Scale
                Vector3 point = hole[i];
                Vector3 v = Vector3.Scale(point, new Vector3(SectionData.Width * 0.5f, SectionData.Height * 0.5f));
                hole[i] = v;

                // Position
                hole[i] += SectionData.Position;

                // Scale
                Vector3 point1 = hole[i] - SectionData.Position;
                Vector3 v2 = Vector3.Scale(point1, new Vector3(0.5f, 0.5f, 0.5f)) + SectionData.Position;
                hole[i] = v2;

                // Rotate
                Vector3 euler = rotation.eulerAngles;
                Vector3 v1 = Quaternion.Euler(euler) * (hole[i] - SectionData.Position) + SectionData.Position;
                hole[i] = v1;
            }


            IList<IList<Vector3>> holePoints = new List<IList<Vector3>>();
            holePoints.Add(hole.ToList());

            SectionData.SetHoles(holePoints);
            base.Build();

        }
        public override void Demolish()
        {
            //base.Demolish();
        }
    }
}
