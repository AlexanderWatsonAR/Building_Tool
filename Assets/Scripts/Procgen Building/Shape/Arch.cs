using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OnlyInvalid.ProcGenBuilding.Common
{
    [CreateAssetMenu(fileName = "Arch", menuName = "Shape/Arch")]
    public class Arch : Shape
    {
        [SerializeField, Range(0, 1)] float m_ArchHeight;
        [SerializeField, Range(3, 16)] int m_ArchSides;

        public Arch(float archHeight, int archSides)
        {
            m_ArchHeight = archHeight;
            m_ArchSides = archSides;
        }

        public override Vector3[] ControlPoints()
        {
            List<Vector3> controlPoints = new List<Vector3>();

            // TODO: apply height

            controlPoints.Add(new Vector3(-0.5f, -0.5f));
            controlPoints.AddRange(Vector3Extensions.QuadraticLerpCollection(new Vector3(-0.5f, 0.75f), new Vector3(0, 1), new Vector3(0.5f, 0.75f), m_ArchSides));
            controlPoints.Add(new Vector3(0.5f, -0.5f));

            return controlPoints.ToArray();
        }
    }


}

