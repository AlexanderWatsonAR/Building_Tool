using OnlyInvalid.ProcGenBuilding.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Points : ScriptableObject
{
    [SerializeField] XZPolygonPath m_Path;

    public XZPolygonPath Path => m_Path;
}
