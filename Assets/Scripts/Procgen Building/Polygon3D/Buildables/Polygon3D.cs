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

        public Polygon3DData Polygon3DData => m_Data as Polygon3DData;

        public override Buildable Initialize(DirtyData data)
        {
            m_ProBuilderMesh = GetComponent<ProBuilderMesh>();

            AssignDefaultMaterial();

            return base.Initialize(data);
        }
        private void AssignDefaultMaterial()
        {
            Renderer renderer = GetComponent<Renderer>();

            if (renderer.sharedMaterials.Length == 1 && renderer.sharedMaterials[0] == null)
            {
                renderer.sharedMaterial = Rendering.BuiltInMaterials.Defualt;
            }
        }
        public override void Build()
        {
            if (!m_Data.IsDirty)
                return;

            IList<IList<Vector3>> holePoints = Polygon3DData.GetHoles();
            m_ProBuilderMesh.CreateShapeFromPolygon(Polygon3DData.Polygon.ControlPoints, Polygon3DData.Polygon.Normal, holePoints);
            m_ProBuilderMesh.Solidify(Polygon3DData.Depth);
            Polygon3DData.IsDirty = false;
        }

        public override void Demolish()
        {
            DestroyImmediate(gameObject);
        }
    }
}
