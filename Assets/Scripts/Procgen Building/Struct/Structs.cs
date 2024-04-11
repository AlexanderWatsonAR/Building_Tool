using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct WallPoints
{
    public Vector3 Start;
    public Vector3 End;

    public WallPoints(Vector3 start, Vector3 end)
    {
        Start = start;
        End = end;
    }
}
[System.Serializable]
public struct LerpPoint
{
    public Vector3 Start, End;
    public float T;

    public LerpPoint(Vector3 start, Vector3 end, float t)
    {
        this.Start = start;
        this.End = end;
        this.T = t;
    }

    public LerpPoint(Vector3 start)
    {
        this.Start = start;
        this.End = Vector3.zero;
        this.T = 0;
    }
}