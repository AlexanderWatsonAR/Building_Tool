using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;

public abstract class Polygon3D : MonoBehaviour, IBuildable
{
    [SerializeField] protected ProBuilderMesh m_ProBuilderMesh;
    [SerializeReference] Polygon3DData m_Poly3DData;

    public Polygon3DData Poly3DData => m_Poly3DData;

    public virtual IBuildable Initialize(IData data)
    {
        m_ProBuilderMesh = GetComponent<ProBuilderMesh>();
        m_Poly3DData = data as Polygon3DData;

        AssignDefaultMaterial();

        if (m_Poly3DData.Polygon.ControlPoints == null || m_Poly3DData.Polygon.ControlPoints.Length == 0)
            return this;

        Extensions.MinMax(m_Poly3DData.Polygon.ControlPoints, out Vector3 min, out Vector3 max);
        // this assumes the up vector is always 0,1,0
        m_Poly3DData.Height = max.y - min.y;
        m_Poly3DData.Width = max.x - min.x + (max.z - min.z);
        m_Poly3DData.Position = Vector3.Lerp(min, max, 0.5f);
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
        if (!m_Poly3DData.IsDirty)
            return;

        IList<IList<Vector3>> holePoints = m_Poly3DData.GetHoles();
        m_ProBuilderMesh.CreateShapeFromPolygon(m_Poly3DData.Polygon.ControlPoints, m_Poly3DData.Polygon.Normal, holePoints);
        m_ProBuilderMesh.Solidify(m_Poly3DData.Depth);
        m_Poly3DData.IsDirty = false;
    }

    public virtual void Demolish()
    {
        DestroyImmediate(gameObject);
    }
}
