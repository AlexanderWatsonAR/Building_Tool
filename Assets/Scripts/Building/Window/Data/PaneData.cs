using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.UIElements;

[System.Serializable]
public class PaneData : Polygon3DData
{
    public PaneData() : base()
    {

    }

    public PaneData(PolygonData polygon, PolygonData[] holes, Vector3 normal, Vector3 up, float depth): base(polygon, holes, normal, up, depth)
    {
    }

    public PaneData(PaneData data) : base(data)
    {

    }

}
