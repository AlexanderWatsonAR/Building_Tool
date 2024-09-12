using OnlyInvalid.ProcGenBuilding.Common;
using OnlyInvalid.ProcGenBuilding.Polygon3D;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.Rendering;


public class Polygon3DA : Buildable
{
    [SerializeField, HideInInspector] protected ProBuilderMesh m_ProBuilderMesh;

    public Polygon3DAData Polygon3DData => m_Data as Polygon3DAData;

    public override void Build()
    {
        if (!m_Data.IsDirty)
            return;

        IList<IList<Vector3>> holePoints = Polygon3DData.Holes;
        m_ProBuilderMesh.CreateShapeFromPolygon(Polygon3DData.ControlPoints, Polygon3DData.Normal, holePoints);
        m_ProBuilderMesh.Solidify(Polygon3DData.Depth);
        Polygon3DData.IsDirty = false;
    }
}


