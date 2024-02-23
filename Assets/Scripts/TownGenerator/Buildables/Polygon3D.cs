using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;

public abstract class Polygon3D : MonoBehaviour, IBuildable
{
    protected ProBuilderMesh m_ProBuilderMesh;

    [SerializeField] Polygon3DData m_Polygon3DData;

    public virtual IBuildable Initialize(IData data)
    {
        m_ProBuilderMesh = GetComponent<ProBuilderMesh>();
        m_Polygon3DData = data as Polygon3DData;

        if (m_Polygon3DData.ControlPoints == null || m_Polygon3DData.ControlPoints.Length == 0)
            return this;

        Extensions.MinMax(m_Polygon3DData.ControlPoints, out Vector3 min, out Vector3 max);
        // this assumes the up vector is always 0,1,0
        m_Polygon3DData.Height = max.y - min.y;
        m_Polygon3DData.Width = max.x - min.x + (max.z - min.z);
        m_Polygon3DData.Position = Vector3.Lerp(min, max, 0.5f);
        return this;
    }

    public virtual void Build()
    {
        m_ProBuilderMesh.CreateShapeFromPolygon(m_Polygon3DData.ControlPoints, m_Polygon3DData.Normal, m_Polygon3DData.HolePoints);
        m_ProBuilderMesh.Solidify(m_Polygon3DData.Depth);
    }

    public virtual void Demolish()
    {
        m_ProBuilderMesh.Clear();
        DestroyImmediate(m_ProBuilderMesh);
    }
}
