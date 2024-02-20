using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;

public class Polygon3D : MonoBehaviour, IBuildable
{
    protected ProBuilderMesh m_ProBuilderMesh;

    [SerializeField] Polygon3DData m_Data;

    public virtual IBuildable Initialize(IData data)
    {
        m_ProBuilderMesh = GetComponent<ProBuilderMesh>();
        m_Data = data as Polygon3DData;
        return this;
    }

    public virtual void Build()
    {
        m_ProBuilderMesh = ProBuilderMesh.Create();
        m_ProBuilderMesh.transform.SetParent(transform, false);
        m_ProBuilderMesh.CreateShapeFromPolygon(m_Data.ControlPoints, m_Data.Normal, m_Data.HolePoints);
        m_ProBuilderMesh.Solidify(m_Data.Depth);
    }

    public virtual void Demolish()
    {
        DestroyImmediate(m_ProBuilderMesh);
    }
}
