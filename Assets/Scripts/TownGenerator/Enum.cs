using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Flags]
public enum StoreyElement
{
    Nothing = 0, Walls = 1, Floor = 2, Pillar = 4, Everything = ~0
}
public enum RoofType
{
    Gable, Mansard, Dormer, MShaped, Pyramid, PyramidHip
}
public enum Axis
{
    Z, MinusZ, X, MinusX, Y, MinusY
}

public enum TransformPoint
{
    Middle, Top, Bottom, Left, Right
}

public enum TransformType
{
    Translation, Rotation, Scale
}

public enum WallElement
{
    Wall, Door, Window, Empty
}

public enum PolyMode
{
    Draw, Edit, Show, Hide
}