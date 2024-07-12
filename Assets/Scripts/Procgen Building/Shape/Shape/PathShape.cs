using OnlyInvalid.ProcGenBuilding.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PathShape : Shape, IPolygon
{
    [SerializeField] PlanarPath m_Path;

    public Path Path => m_Path;

    public PathShape()
    {
        m_Path = new PlanarPath(Vector3.forward, 0.01f);
    }
    public override Vector3[] ControlPoints()
    {
        return Path.Positions;
    }
}
