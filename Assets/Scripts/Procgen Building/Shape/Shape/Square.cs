using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Square : Shape
{
    public override Vector3[] ControlPoints()
    {
        return MeshMaker.Square();
    }
}
