using OnlyInvalid.ProcGenBuilding.Common;
using OnlyInvalid.ProcGenBuilding.Polygon3D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ContentScriptableObject : ScriptableObject
{
    public abstract Polygon3D Create3DPolygon();
}
