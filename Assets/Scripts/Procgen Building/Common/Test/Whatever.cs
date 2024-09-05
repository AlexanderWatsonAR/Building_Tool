using OnlyInvalid.ProcGenBuilding.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Whatever : MonoBehaviour, IPolygon
{
    [SerializeField] Points m_Points;

    public Path Path => m_Points.Path;

    void Reset()
    {
        m_Points = ScriptableObject.CreateInstance<Points>();
    }

}

