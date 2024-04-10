using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;
using OnlyInvalid.ProcGenBuilding.Common;

namespace OnlyInvalid.ProcGenBuilding.Polygon3D
{
    public abstract class Polygon3D : MonoBehaviour, IBuildable
    {
        [SerializeField] protected ProBuilderMesh m_ProBuilderMesh;
        [SerializeReference] Polygon3DData m_Data;

        public virtual Polygon3DData Data => m_Data;

        public virtual IBuildable Initialize(DirtyData data)
        {
            m_ProBuilderMesh = GetComponent<ProBuilderMesh>();
            m_Data = data as Polygon3DData;

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

        public virtual void Build()
        {
            if (!m_Data.IsDirty)
                return;

            IList<IList<Vector3>> holePoints = m_Data.GetHoles();
            m_ProBuilderMesh.CreateShapeFromPolygon(m_Data.Polygon.ControlPoints, m_Data.Polygon.Normal, holePoints);
            m_ProBuilderMesh.Solidify(m_Data.Depth);

            m_Data.IsDirty = false;
        }

        public virtual void Demolish()
        {
            DestroyImmediate(gameObject);
        }
    }
}
