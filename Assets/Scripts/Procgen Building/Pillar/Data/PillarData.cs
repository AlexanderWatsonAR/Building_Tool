using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.ProBuilder;
using OnlyInvalid.ProcGenBuilding.Common;

namespace OnlyInvalid.ProcGenBuilding.Pillar
{
    [System.Serializable]
    public class PillarData : DirtyData
    {
        [SerializeField, HideInInspector] int m_ID;
        [SerializeField, HideInInspector] Vector3[] m_ControlPoints;
        [SerializeField, Range(1, 50)] float m_Height;
        [SerializeField, Range(0, 10)] float m_Width;
        [SerializeField, Range(0, 10)] float m_Depth;
        [SerializeField, Range(3, 32)] int m_Sides;
        [SerializeField] bool m_IsSmooth;
        [SerializeField, HideInInspector] Material m_Material;

        public int ID { get { return m_ID; } set { m_ID = value; } }
        public Vector3[] ControlPoints { get { return m_ControlPoints; } set { m_ControlPoints = value; } }
        public float Width { get { return m_Width; } set { m_Width = value; } }
        public float Height { get { return m_Height; } set { m_Height = value; } }
        public float Depth { get { return m_Depth; } set { m_Depth = value; } }
        public int Sides { get { return m_Sides; } set { m_Sides = value; } }
        public bool IsSmooth { get { return m_IsSmooth; } set { m_IsSmooth = value; } }
        public Material Material { get { return m_Material; } set { m_Material = value; } }

        public PillarData() : this(null, 0.5f, 4f, 0.5f, 4, null)
        {
        }
        public PillarData(PillarData data) : this(data.ControlPoints, data.Width, data.Height, data.Depth, data.Sides, data.Material, data.IsSmooth)
        {
        }
        public PillarData(IEnumerable<Vector3> controlPoints, float width, float height, float depth, int sides, Material material, bool isSmooth = false)
        {
            m_ControlPoints = controlPoints == null ? new Vector3[0] : controlPoints.ToArray();
            m_Width = width;
            m_Height = height;
            m_Depth = depth;
            m_Sides = sides;
            m_IsSmooth = isSmooth;
            m_Material = material;
        }
        public override bool Equals(object obj)
        {
            if (obj is not PillarData) return false;

            PillarData pillar = obj as PillarData;

            if (pillar.Height == m_Height &&
                pillar.Width == m_Width &&
                pillar.Depth == m_Depth &&
                pillar.Sides == m_Sides &&
                pillar.IsSmooth == m_IsSmooth)
            {
                return true;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
