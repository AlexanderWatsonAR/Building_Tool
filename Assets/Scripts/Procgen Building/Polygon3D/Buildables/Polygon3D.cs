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
        [SerializeReference] Polygon3DData m_Polygon3DData;

        public override Buildable Initialize(DirtyData data)
        {
            m_Polygon3DData = data as Polygon3DData;
            m_ProBuilderMesh = GetComponent<ProBuilderMesh>();
            
            AssignDefaultMaterial();

            return base.Initialize(data);
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
            if (!m_Polygon3DData.IsDirty)
                return;

            IList<IList<Vector3>> holePoints = m_Polygon3DData.GetHoles();
            m_ProBuilderMesh.CreateShapeFromPolygon(m_Polygon3DData.Polygon.ControlPoints, m_Polygon3DData.Polygon.Normal, holePoints);
            m_ProBuilderMesh.Solidify(m_Polygon3DData.Depth);

            m_Polygon3DData.IsDirty = false;
        }

        public override void Demolish()
        {
            DestroyImmediate(gameObject);
        }
    }
}
