using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using OnlyInvalid.ProcGenBuilding.Common;

namespace OnlyInvalid.ProcGenBuilding.Floor
{
    [System.Serializable]
    public class FloorSectionData : DirtyData
    {
        [SerializeField] Vector3[] m_ControlPoints;
        [SerializeField] float m_Height;


        public Vector3[] ControlPoints { get { return m_ControlPoints; } set { m_ControlPoints = value; } }
        public float Height { get { return m_Height; } set { m_Height = value; } }


        public FloorSectionData() : this(new Vector3[0], 0.1f)
        {

        }

        public FloorSectionData(IEnumerable<Vector3> controlPoints, float height)
        {
            m_ControlPoints = controlPoints.ToArray();
            m_Height = height;
        }

        public FloorSectionData(FloorSectionData data)
        {
            m_ControlPoints = data.ControlPoints;
            m_Height = data.Height;
        }
    }
}
