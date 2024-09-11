using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OnlyInvalid.Polygon.Clipper_API;
using UnityEngine.ProBuilder;
using UnityEditor;

public class AGrid : Polygon3DA
{
    public AGridData GridData => m_Data as AGridData;

    private void Reset()
    {
        if(m_Data == null)
            m_Data = new AGridData();

        m_ProBuilderMesh = GetComponent<ProBuilderMesh>();
    }

    public override void Build()
    {
        if (!GridData.IsDirty)
            return;

        CalculateInterior();

        base.Build();
    }

    private void CalculateInterior()
    {
        GridData.ClearInterior();

        var split = Clipper.Split(GridData.ExteriorShape.ControlPoints(), GridData.Dimensions, GridData.Scale);

        foreach (var square in split)
        {
            GridData.AddInteriorShape(new PathShape(square));
        }
    }
}