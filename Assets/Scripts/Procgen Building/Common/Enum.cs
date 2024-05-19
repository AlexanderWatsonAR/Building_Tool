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
public enum DoorwayElement
{
    Nothing = 0, Door = 1, Handle = 2, Frame = 4, Everything = ~0
}

[System.Flags]
public enum DoorElement
{
    Nothing = 0, Door = 1, Handle = 2, Everything = ~0
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

public enum RelativePosition
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
public enum DrawState
{
    Hide, Draw, Edit
}
public enum OneLineShape
{
    Unknown, Antenna, Arrow, Asterisk, Square, Crenel, E, F, H, InterlockY, K, L, M, N, SimpleM, SimpleN, T, U, Y, X, ZigZag 
}

public enum ShapeType
{
    Polygon, Arch, Custom
}

public static class EnumExtensions
{

    public static bool IsElementActive(this WindowElement windowElement, WindowElement comparison)
    {
        return windowElement == WindowElement.Nothing ? false : (windowElement & comparison) != 0;
    }

    public static bool IsElementActive(this DoorwayElement doorwayElement, DoorwayElement comparison)
    {
        return doorwayElement == DoorwayElement.Nothing ? false : (doorwayElement & comparison) != 0;
    }

    public static bool IsElementActive(this StoreyElement storeyElement, StoreyElement comparison)
    {
        return storeyElement == StoreyElement.Nothing ? false : (storeyElement & comparison) != 0;
    }

    public static bool IsElementActive(this DoorElement doorElement, DoorElement comparison)
    {
        return doorElement == DoorElement.Nothing ? false : (doorElement & comparison) != 0;
    }

    public static DoorElement ToDoorElement(this DoorwayElement doorway)
    {
        if (doorway == DoorwayElement.Nothing)
            return DoorElement.Nothing;

        if (doorway == DoorwayElement.Everything)
            return DoorElement.Everything;

        if (doorway.IsElementActive(DoorwayElement.Door))
            return DoorElement.Door;

        if (doorway.IsElementActive(DoorwayElement.Handle))
            return DoorElement.Handle;

        return DoorElement.Nothing;
    }
}