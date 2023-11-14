using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Flags]
public enum StoreyElement
{
    Nothing = 0, Walls = 1, Floor = 2, Pillars = 4, Everything = ~0
}

[System.Flags]
public enum WindowElement
{
    Nothing = 0, OuterFrame = 1, InnerFrame = 2, Pane = 4, Shutters = 8, Everything = ~0
}

[System.Flags]
public enum DoorElement
{
    Nothing = 0, Door = 1, Handle = 2, Frame = 4, Everything = ~0
}

public enum FloorMode
{
    Hide, Show
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

public enum RoofElement
{
    Tile, Window, Empty
}

public enum WallElement
{
    Wall, Doorway, Archway, Window, Extension, Empty
}


public enum PolyMode
{
    Draw, Edit, Hide
}

public enum CornerType
{
    Point, Round, Flat
}

public static class EnumExtensions
{
    public static bool IsWindowElementActive(this WindowElement windowElement, WindowElement comparison)
    {
        return windowElement == WindowElement.Nothing ? false : (windowElement & comparison) != 0;
    }

    public static bool IsDoorElementActive(this DoorElement doorElement, DoorElement comparison)
    {
        return doorElement == DoorElement.Nothing ? false : (doorElement & comparison) != 0;
    }
}