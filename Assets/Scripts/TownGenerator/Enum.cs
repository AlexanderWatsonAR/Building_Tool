using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoofType
{
    OpenGable, Mansard, Flat, Dormer, MShaped, Pyramid, PyramidHip
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
    Wall, Door, Window
}
