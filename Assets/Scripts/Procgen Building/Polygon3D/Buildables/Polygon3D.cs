using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;
using OnlyInvalid.ProcGenBuilding.Common;

namespace OnlyInvalid.ProcGenBuilding.Polygon3D
{
    public abstract class Polygon3D : Buildable
    {
        [SerializeField] protected ProBuilderMesh m_ProBuilderMesh;
        [SerializeReference] Polygon3DData m_PolyData;

        public override Buildable Initialize(DirtyData data)
        {
            base.Initialize(data);
            m_PolyData = data as Polygon3DData;
            m_ProBuilderMesh = GetComponent<ProBuilderMesh>();
            
            AssignDefaultMaterial();

            return this;
        }

        private void AssignDefaultMaterial()
        {
            Renderer renderer = GetComponent<Renderer>();

            if (renderer.sharedMaterials.Length == 1 && renderer.sharedMaterials[0] == null)
            {
                renderer.sharedMaterial = BuiltinMaterials.defaultMaterial;
            }
        }

        public override void Build()
        {
            if (!m_PolyData.IsDirty)
                return;

            IList<IList<Vector3>> holePoints = m_PolyData.GetHoles();
            m_ProBuilderMesh.CreateShapeFromPolygon(m_PolyData.Polygon.ControlPoints, m_PolyData.Polygon.Normal, holePoints);
            m_ProBuilderMesh.Solidify(m_PolyData.Depth);

            m_PolyData.IsDirty = false;
        }

        public override void Demolish()
        {
            DestroyImmediate(gameObject);
        }
    }
}
