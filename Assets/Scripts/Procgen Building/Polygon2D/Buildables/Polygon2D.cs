using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;
using OnlyInvalid.ProcGenBuilding.Common;

public abstract class Polygon2D : Buildable
{
    [SerializeField] protected ProBuilderMesh m_ProBuilderMesh;

    public Polygon2DData Polygon2DData => m_Data as Polygon2DData;

    public override Buildable Initialize(DirtyData data)
    {
        m_ProBuilderMesh = GetComponent<ProBuilderMesh>();
        //m_ProBuilderMesh.transform.position = Polygon2DData.Position;
        //m_ProBuilderMesh.transform.eulerAngles = Polygon2DData.EulerAngle;
        //m_ProBuilderMesh.transform.localScale = Polygon2DData.Scale;

        AssignDefaultMaterial();

        return base.Initialize(data);
    }
    private void AssignDefaultMaterial()
    {
        Renderer renderer = GetComponent<Renderer>();

        if (renderer.sharedMaterials.Length == 1 && renderer.sharedMaterials[0] == null)
        {
            renderer.sharedMaterial = OnlyInvalid.Rendering.BuiltInMaterials.Defualt;
        }
    }
    public override void Build()
    {
        if (!m_Data.IsDirty)
            return;

        m_ProBuilderMesh.CreateShapeFromPolygon(Polygon2DData.ControlPoints, Polygon2DData.Normal(), Polygon2DData.Holes);
        Polygon2DData.IsDirty = false;
    }

    public override void Demolish()
    {
        DestroyImmediate(gameObject);
    }
}
