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
