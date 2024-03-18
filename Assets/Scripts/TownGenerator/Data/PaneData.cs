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

    public PaneData(PolygonData polygon, PolygonData[] holes, Vector3 normal, Vector3 up, float height, float width, float depth, Vector3 position): base(polygon, holes, normal, up, height, width, depth, position)
    {
    }

    public PaneData(PaneData data) : base(data)
    {

    }

}
