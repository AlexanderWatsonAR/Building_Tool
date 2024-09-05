using OnlyInvalid.ProcGenBuilding.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class PolygonIdea : DirtyData
{
    [SerializeReference]  protected Shape m_Shape;
    [SerializeReference] List<Shape> holes;
    Matrix4x4 m_TRS;
    
    private void Translate()
    {
        m_TRS = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one);

        Vector3[] controlPoints = m_Shape.ControlPoints();

        for(int i = 0; i < controlPoints.Length; i++)
        {
            controlPoints[i] = m_TRS.MultiplyPoint3x4(controlPoints[i]);
        }
    }
}

public class WallIdea : PolygonIdea
{
    public WallIdea()
    {
        m_Shape = new Square();
    }
}
