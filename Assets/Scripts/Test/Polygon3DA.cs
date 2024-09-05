using OnlyInvalid.ProcGenBuilding.Common;
using OnlyInvalid.ProcGenBuilding.Polygon3D;
using System.Collections;
using System.Collections.Generic;
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

        //IList<IList<Vector3>> holePoints = Polygon3DData.GetHoles();
        //m_ProBuilderMesh.CreateShapeFromPolygon(Polygon3DData.Polygon.ControlPoints, Polygon3DData.Polygon.Normal, holePoints);
        //m_ProBuilderMesh.Solidify(Polygon3DData.Depth);
        Polygon3DData.IsDirty = false;
    }
}

[System.Serializable]
public class Polygon3DAData : DirtyData
{
    // TRS
    Vector3 m_Position, m_Scale;
    Quaternion m_Rotation;
    Shape m_ExteriorShape;
    Shape[] m_InteriorShapes;

    public Vector3[] ControlPoints
    {
        get
        {
            Vector3[] controlPoints = m_ExteriorShape.ControlPoints();
            Matrix4x4 trs = Matrix4x4.TRS(m_Position, m_Rotation, m_Scale);

            for(int i = 0; i < controlPoints.Length; i++)
            {
                controlPoints[i] = trs.MultiplyPoint3x4(controlPoints[i]);
            }

            return controlPoints;
        }
    }

}
