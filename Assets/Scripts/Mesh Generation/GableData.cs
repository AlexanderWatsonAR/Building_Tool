using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using Unity.VisualScripting;

public static class GablePersistentData
{
    // How the arrays work:

    // "new int[] { 0, 1, 0, 1 }"
    // In the above example, the first & last points reference the control points / path points of the building.
    // The middle 2 points reference the one line points.

    #region Indices
    public static readonly ushort[][] squareIndices = new ushort[][]
    {
        new ushort[] { 3, 1, 0, 0 },
        new ushort[] { 1, 0, 1, 2 },
        new ushort[] { 0, 0, 0, 1 },
        new ushort[] { 2, 1, 1, 3 },
    }; //
    public static readonly ushort[][] lIndices = new ushort[][]
    {
        new ushort[] { 0, 1, 0, 1 },
        new ushort[] { 2, 0, 1, 3 },
        new ushort[] { 5, 2, 1, 0 },
        new ushort[] { 3, 1, 2, 4 },
        new ushort[] { 4, 2, 2, 5 },
        new ushort[] { 1, 0, 0, 2 },
    }; //
    public static readonly ushort[][] arrowIndices = new ushort[][]
    {
        new ushort[] { 8, 0, 3, 0 },
        new ushort[] { 6, 3, 0, 7 },
        new ushort[] { 5, 2, 3, 6 },
        new ushort[] { 3, 3, 2, 4 },
        new ushort[] { 2, 1, 3, 3 },
        new ushort[] { 0, 3, 1, 1 },
        new ushort[] { 7, 0, 0, 8 },
        new ushort[] { 1, 1, 1, 2 },
        new ushort[] { 4, 2, 2, 5 }
    }; //
    public static readonly ushort[][] simpleNIndices = new ushort[][]
    {
        new ushort[] { 1, 0, 1, 0 },//
        new ushort[] { 3, 1, 0, 2 },//
        new ushort[] { 4, 2, 1, 3 },//
        new ushort[] { 5, 3, 2, 4 },//
        new ushort[] { 7, 2, 3, 6 },//
        new ushort[] { 0, 1, 2, 7 },//
        new ushort[] { 2, 0, 0, 1 },//
        new ushort[] { 6, 3, 3, 5 }//
    }; //
    public static readonly ushort[][] tIndices = new ushort[][]
    {
        new ushort[] { 0, 3, 0, 1 },
        new ushort[] { 2, 0, 3, 3 },
        new ushort[] { 3, 3, 2, 4 },
        new ushort[] { 5, 2, 1, 6 },
        new ushort[] { 7, 1, 3, 0 },
        new ushort[] { 1, 0, 0, 2 },
        new ushort[] { 6, 1, 1, 7 },
        new ushort[] { 4, 2, 2, 5 }
    };//
    public static readonly ushort[][] uIndices = new ushort[][]
    {
        new ushort[] { 0, 1, 2, 1 },
        new ushort[] { 1, 2, 3, 2 },
        new ushort[] { 3, 3, 2, 4 },
        new ushort[] { 4, 2, 1, 5 },
        new ushort[] { 5, 1, 0, 6 },
        new ushort[] { 7, 0, 1, 0 },
        new ushort[] { 2, 3, 3, 3 },
        new ushort[] { 6, 0, 0, 7 }
    };//
    public static readonly ushort[][] yIndices = new ushort[][]
    {
        new ushort[] { 0, 3, 0, 1 },
        new ushort[] { 2, 0, 3, 3 },
        new ushort[] { 3, 3, 1, 4 },
        new ushort[] { 5, 1, 3, 6 },
        new ushort[] { 6, 3, 2, 7 },
        new ushort[] { 8, 2, 3, 0 },
        new ushort[] { 1, 0, 0, 2 },
        new ushort[] { 4, 1, 1, 5 },
        new ushort[] { 7, 2, 2, 8 }
    };//
    public static readonly ushort[][] fIndices = new ushort[][]
    {
        new ushort[] { 1, 0, 1, 0 },
        new ushort[] { 3, 3, 0, 2 },
        new ushort[] { 4, 4, 3, 3 },
        new ushort[] { 6, 3, 4, 5 },
        new ushort[] { 7, 1, 3, 6 },
        new ushort[] { 8, 2, 1, 7 },
        new ushort[] { 0, 1, 2, 9 },
        new ushort[] { 2, 0, 0, 1 },
        new ushort[] { 9, 2, 2, 8 },
        new ushort[] { 5, 4, 4, 4 }
    }; //
    public static readonly ushort[][] simpleMIndices = new ushort[][]
    {
        new ushort[] { 0, 1, 2, 1 },
        new ushort[] { 1, 2, 3, 2 },
        new ushort[] { 2, 3, 4, 3 },
        new ushort[] { 4, 4, 3, 5 },
        new ushort[] { 5, 3, 2, 6 },
        new ushort[] { 6, 2, 1, 7 },
        new ushort[] { 7, 1, 0, 8 },
        new ushort[] { 9, 0, 1, 0 },
        new ushort[] { 3, 4, 4, 4 },
        new ushort[] { 8, 0, 0, 9 }
    }; //
    public static readonly ushort[][] nIndices = new ushort[][]
    {
        new ushort[] { 1, 0, 1, 0 },
        new ushort[] { 3, 1, 0, 2 },
        new ushort[] { 4, 2, 1, 3 },
        new ushort[] { 5, 3, 2, 4 },
        new ushort[] { 5, 4, 3, 5 },
        new ushort[] { 6, 5, 4, 5 },
        new ushort[] { 8, 4, 5, 7 },
        new ushort[] { 9, 3, 4, 8 },
        new ushort[] { 0, 2, 3, 9 },
        new ushort[] { 0, 1, 2, 0 },
        new ushort[] { 2, 0, 0, 1 },
        new ushort[] { 7, 5, 5, 6 }
    };
    public static readonly ushort[][] eIndices = new ushort[][]
    {
        new ushort[] { 0, 1, 4, 1 },
        new ushort[] { 1, 4, 5, 2 },
        new ushort[] { 3, 5, 4, 4 },
        new ushort[] { 4, 4, 2, 5 },
        new ushort[] { 5, 2, 3, 6 },
        new ushort[] { 7, 3, 2, 8 },
        new ushort[] { 8, 2, 1, 9 },
        new ushort[] { 9, 1, 0, 10 },
        new ushort[] { 11, 0, 1, 0 },
        new ushort[] { 10, 0, 0, 11 },
        new ushort[] { 2, 5, 5, 3 },
        new ushort[] { 6, 3, 3, 7 }
    };//
    public static readonly ushort[][] hIndices = new ushort[][]
    {
        new ushort[] { 11, 0, 1, 0 },
        new ushort[] { 0, 1, 4, 1 },
        new ushort[] { 1, 4, 5, 2 },
        new ushort[] { 3, 5, 3, 4 },
        new ushort[] { 5, 3, 4, 6 },
        new ushort[] { 6, 4, 1, 7 },
        new ushort[] { 7, 1, 2, 8 },
        new ushort[] { 9, 2, 0, 10 },
        new ushort[] { 10, 0, 0, 11 },
        new ushort[] { 8, 2, 2, 9 },
        new ushort[] { 4, 3, 3, 5 },
        new ushort[] { 2, 5, 5, 3 }
    };//
    public static readonly ushort[][] kIndices = new ushort[][]
    {
        new ushort[] { 11, 0, 1, 0 },
        new ushort[] { 0, 1, 5, 1 },
        new ushort[] { 1, 5, 4, 2 },
        new ushort[] { 3, 4, 5, 4 },
        new ushort[] { 4, 5, 3, 5 },
        new ushort[] { 6, 3, 1, 7 },
        new ushort[] { 7, 1, 2, 8 },
        new ushort[] { 9, 2, 0, 10 },
        new ushort[] { 10, 0, 0, 11 },
        new ushort[] { 8, 2, 2, 9 },
        new ushort[] { 5, 3, 3, 6 },
        new ushort[] { 2, 4, 4, 3 }
    };
    public static readonly ushort[][] mIndices = new ushort[][]
    {
        new ushort[] { 0, 1, 2, 0 },
        new ushort[] { 0, 2, 3, 1 },
        new ushort[] { 1, 3, 4, 2 },
        new ushort[] { 2, 4, 5, 2 },
        new ushort[] { 2, 5, 6, 3 },
        new ushort[] { 4, 6, 5, 5 },
        new ushort[] { 5, 5, 4, 6 },
        new ushort[] { 6, 4, 3, 7 },
        new ushort[] { 7, 3, 2, 8 },
        new ushort[] { 8, 2, 1, 9 },
        new ushort[] { 9, 1, 0, 10 },
        new ushort[] { 11, 0, 1, 0 },
        new ushort[] { 3, 6, 6, 4 },
        new ushort[] { 10, 0, 0, 11 }
    }; 
    public static readonly ushort[][] interYIndices = new ushort[][]
    {
        new ushort[] { 7, 0, 1, 8},
        new ushort[] { 5, 1, 0, 6},
        new ushort[] { 8, 1, 2, 9},
        new ushort[] { 0, 2, 3, 1},
        new ushort[] { 1, 3, 4, 2},
        new ushort[] { 3, 4, 3, 4},
        new ushort[] { 5, 1, 3, 4},
        new ushort[] { 9, 2, 5, 10},
        new ushort[] { 11, 5, 2, 0},
        new ushort[] { 6, 0, 0, 7},
        new ushort[] { 2, 4, 4, 3},
        new ushort[] { 10, 5, 5, 11}
    };
    public static readonly ushort[][] xIndices = new ushort[][]
    {
        new ushort[] { 1, 0, 4, 0},
        new ushort[] { 3, 4, 0, 2},
        new ushort[] { 4, 1, 4, 3},
        new ushort[] { 6, 4, 1, 5},
        new ushort[] { 7, 2, 4, 6},
        new ushort[] { 9, 4, 2, 8},
        new ushort[] { 10, 3, 4, 9},
        new ushort[] { 0, 4, 3, 11},
        new ushort[] { 2, 0, 0, 1},
        new ushort[] { 4, 1, 1, 5},
        new ushort[] { 8, 2, 2, 7},
        new ushort[] { 11, 3, 3, 10},
    }; //
    #endregion

    #region Extend
    // Height Start, Height End, Width Start, Width End
    public static readonly bool[][] squareExtend = new bool[][]
    {
        new bool[] { false, true, true, true },
        new bool[] { false, true, true, true },
        new bool[] { false, true, false, false },
        new bool[] { false, true, false, false }
    };
    public static readonly bool[][] lExtend = new bool[][]
    {
        new bool[] { false, true, false, true },
        new bool[] { false, true, true, false },
        new bool[] { false, true, true, false },
        new bool[] { false, true, false, true },
        new bool[] { false, true, true, false },
        new bool[] { false, true, false, true }
    };
    public static readonly bool[][] arrowExtend = new bool[][]
    {
        new bool[] { false, true, true, false },
        new bool[] { false, true, false, true },
        new bool[] { false, true, true, false },
        new bool[] { false, true, false, true },
        new bool[] { false, true, true, false },
        new bool[] { false, true, false, true },
        new bool[] { false, true, false, false },
        new bool[] { false, true, false, false },
        new bool[] { false, true, false, false }
    };
    public static readonly bool[][] simpleNExtend = new bool[][]
    {
        new bool[] { false, true, false, false },
        new bool[] { false, true, false, true },
        new bool[] { false, true, true, false },
        new bool[] { false, true, false, false },
        new bool[] { false, true, false, true },
        new bool[] { false, true, true, false },
        new bool[] { false, true, false, false },
        new bool[] { false, true, false, false }
    };
    public static readonly bool[][] tExtend = new bool[][]
    {
        new bool[] { false, true, false, true },
        new bool[] { false, true, true, false },
        new bool[] { false, true, false, true },
        new bool[] { false, true, true, true },
        new bool[] { false, true, true, false },
        new bool[] { false, true, false, false },
        new bool[] { false, true, false, false },
        new bool[] { false, true, false, false }
    };
    public static readonly bool[][] uExtend = new bool[][]
    {
        new bool[] { false, true, false, false },
        new bool[] { false, true, false, true },
        new bool[] { false, true, true, false },
        new bool[] { false, true, false, false },
        new bool[] { false, true, false, true },
        new bool[] { false, true, true, false },
        new bool[] { false, true, false, false },
        new bool[] { false, true, false, false }
    };
    public static readonly bool[][] yExtend = new bool[][]
    {
        new bool[] { false, true, false, true },
        new bool[] { false, true, true, false },
        new bool[] { false, true, false, true },
        new bool[] { false, true, true, false },
        new bool[] { false, true, false, true },
        new bool[] { false, true, true, false },
        new bool[] { false, true, false, false },
        new bool[] { false, true, false, false },
        new bool[] { false, true, false, false }
    };
    public static readonly bool[][] fExtend = new bool[][]
    {
        new bool[] { false, true, true, false },
        new bool[] { false, true, false, true },
        new bool[] { false, true, true, false },
        new bool[] { false, true, false, false },
        new bool[] { false, true, false, true },
        new bool[] { false, true, true, false },
        new bool[] { false, true, false, true },
        new bool[] { false, true, true, true },
        new bool[] { false, true, true, true },
        new bool[] { false, true, true, true }
    };
    public static readonly bool[][] simpleMExtend = new bool[][]
    {
        new bool[] { false, true, false, false },
        new bool[] { false, true, false, false },
        new bool[] { false, true, false, true },
        new bool[] { false, true, true, false },
        new bool[] { false, true, false, false },
        new bool[] { false, true, false, false },
        new bool[] { false, true, false, true },
        new bool[] { false, true, true, false },
        new bool[] { false, true, false, false },
        new bool[] { false, true, false, false }
    };
    public static readonly bool[][] nExtend = new bool[][]
    {
        new bool[] { false, false, false, false },
        new bool[] { false, true, false, false },
        new bool[] { false, true, false, false },
        new bool[] { false, true, false, true },
        new bool[] { false, true, true, false },
        new bool[] { false, true, false, false },
        new bool[] { false, false, false, false },
        new bool[] { false, true, false, false },
        new bool[] { false, true, false, true },
        new bool[] { false, true, true, false },
        new bool[] { false, true, false, false },
        new bool[] { false, true, false, false }
    };
    public static readonly bool[][] eExtend = new bool[][]
    {
        new bool[] { false, true, false, false },
        new bool[] { false, true, false, true },
        new bool[] { false, true, true, false },
        new bool[] { false, true, false, false },
        new bool[] { false, true, false, true },
        new bool[] { false, true, true, false },
        new bool[] { false, true, false, false },
        new bool[] { false, true, false, true },
        new bool[] { false, true, true, false },
        new bool[] { false, true, false, false },
        new bool[] { false, true, false, false },
        new bool[] { false, true, false, false }
    };
    public static readonly bool[][] hExtend = new bool[][]
    {
        new bool[] { false, true, true, false },
        new bool[] { false, true, false, false },
        new bool[] { false, true, false, true },
        new bool[] { false, true, true, true },
        new bool[] { false, true, true, false },
        new bool[] { false, true, false, false },
        new bool[] { false, true, false, true },
        new bool[] { false, true, true, true },
        new bool[] { false, true, true, true },
        new bool[] { false, true, true, true },
        new bool[] { false, true, true, true },
        new bool[] { false, true, true, true }
    };
    public static readonly bool[][] kExtend = new bool[][]
    {
        new bool[] { false, true, true, false },
        new bool[] { false, true, false, false },
        new bool[] { false, true, false, true },
        new bool[] { false, true, true, false },
        new bool[] { false, true, false, true },
        new bool[] { false, true, true, false },
        new bool[] { false, true, false, true },
        new bool[] { false, true, true, true },
        new bool[] { false, true, true, true },
        new bool[] { false, true, true, true },
        new bool[] { false, true, true, true },
        new bool[] { false, true, true, true }
    };
    public static readonly bool[][] mExtend = new bool[][]
    {
        new bool[] { false, false, false, false },
        new bool[] { false, true, false, false },
        new bool[] { false, true, false, false },
        new bool[] { false, false, false, false },
        new bool[] { false, true, false, true },
        new bool[] { false, true, true, false },
        new bool[] { false, true, false, false },
        new bool[] { false, true, false, false },
        new bool[] { false, true, false, false },
        new bool[] { false, true, false, false },
        new bool[] { false, true, false, true },
        new bool[] { false, true, true, false },
        new bool[] { false, true, false, false },
        new bool[] { false, true, false, false }
    };
    public static readonly bool[][] interYExtend = new bool[][]
    {
        new bool[] {  false, true, true, false },
        new bool[] { false, true, false, true },
        new bool[] { false, true, false, false },
        new bool[] {  false, true, false, false },
        new bool[] {  false, true, false, true },
        new bool[] {  false, true, true, false },
        new bool[] {  false, true, false, false },
        new bool[] {  false, true, false, true },
        new bool[] {  false, true, true, false },
        new bool[] {  false, true, false, true },
        new bool[] {  false, true, false, true },
        new bool[] {  false, true, false, true }
    };
    public static readonly bool[][] xExtend = new bool[][]
    {
        new bool[] {  false, true, true, false },
        new bool[] { false, true, false, true },
        new bool[] { false, true, true, false },
        new bool[] {  false, true, false, true },
        new bool[] {  false, true, true, false },
        new bool[] {  false, true, false, true },
        new bool[] {  false, true, true, false },
        new bool[] {  false, true, false, true },
        new bool[] {  false, true, true, true },
        new bool[] {  false, true, true, true },
        new bool[] {  false, true, true, true },
        new bool[] {  false, true, true, true }
    };
    #endregion

    #region Wall Indices
    public static readonly ushort[] squareWallIndices = new ushort[]{ 2, 3 };
    public static readonly ushort[] lWallIndices = new ushort[] { 4, 5 };
    public static readonly ushort[] arrowWallIndices = new ushort[] { 6, 7, 8 };
    public static readonly ushort[] simpleNWallIndices = new ushort[] { 6, 7 };
    public static readonly ushort[] tWallIndices = new ushort[] { 5, 6, 7 };
    public static readonly ushort[] uWallIndices = new ushort[] { 6, 7 };
    public static readonly ushort[] yWallIndices = new ushort[] { 6, 7, 8 };
    public static readonly ushort[] fWallIndices = new ushort[] { 7, 8, 9 };
    public static readonly ushort[] simpleMWallIndices = new ushort[] { 8, 9 };
    public static readonly ushort[] nWallIndices = new ushort[] { 10, 11 };
    public static readonly ushort[] eWallIndices = new ushort[] { 9, 10, 11 };
    public static readonly ushort[] hWallIndices = new ushort[] { 8, 9, 10, 11 };
    public static readonly ushort[] kWallIndices = new ushort[] { 8, 9, 10, 11 };
    public static readonly ushort[] mWallIndices = new ushort[] { 12, 13 };
    public static readonly ushort[] interYWallIndices = new ushort[] { 9, 10, 11 };
    public static readonly ushort[] xWallIndices = new ushort[] { 8, 9, 10, 11 };
    #endregion

    /// <summary>
    /// Returns false if the OneLineShape is a non-static shape.
    /// For non-static shapes, use 'CalculateGableData' function.
    /// </summary>
    /// <param name="shape"></param>
    /// <param name="shapeIndices"></param>
    /// <param name="shapeExtend"></param>
    /// <returns></returns>
    public static bool GetGableData(this OneLineShape shape, out GableData data)
    {
        data = new GableData();

        switch (shape)
        {
            case OneLineShape.Arrow:
                data.indices = GablePersistentData.arrowIndices;
                data.extend = GablePersistentData.arrowExtend;
                data.wallIndices = GablePersistentData.arrowWallIndices;
                return true;
            case OneLineShape.Square:
                data.indices = GablePersistentData.squareIndices;
                data.extend = GablePersistentData.squareExtend;
                data.wallIndices = GablePersistentData.squareWallIndices;
                return true;
            case OneLineShape.E:
                data.indices = GablePersistentData.eIndices;
                data.extend = GablePersistentData.eExtend;
                data.wallIndices = GablePersistentData.eWallIndices;
                return true;
            case OneLineShape.F:
                data.indices = GablePersistentData.fIndices;
                data.extend = GablePersistentData.fExtend;
                data.wallIndices = GablePersistentData.fWallIndices;
                return true;
            case OneLineShape.H:
                data.indices = GablePersistentData.hIndices;
                data.extend = GablePersistentData.hExtend;
                data.wallIndices = GablePersistentData.hWallIndices;
                return true;
            case OneLineShape.InterlockY:
                data.indices = GablePersistentData.interYIndices;
                data.extend = GablePersistentData.interYExtend;
                data.wallIndices = GablePersistentData.interYWallIndices;
                return true;
            case OneLineShape.K:
                data.indices = GablePersistentData.kIndices;
                data.extend = GablePersistentData.kExtend;
                data.wallIndices = GablePersistentData.kWallIndices;
                return true;
            case OneLineShape.L:
                data.indices = GablePersistentData.lIndices;
                data.extend = GablePersistentData.lExtend;
                data.wallIndices = GablePersistentData.lWallIndices;
                return true;
            case OneLineShape.M:
                data.indices = GablePersistentData.mIndices;
                data.extend = GablePersistentData.mExtend;
                data.wallIndices = GablePersistentData.mWallIndices;
                return true;
            case OneLineShape.N:
                data.indices = GablePersistentData.nIndices;
                data.extend = GablePersistentData.nExtend;
                data.wallIndices = GablePersistentData.nWallIndices;
                return true;
            case OneLineShape.SimpleM:
                data.indices = GablePersistentData.simpleMIndices;
                data.extend = GablePersistentData.simpleMExtend;
                data.wallIndices = GablePersistentData.simpleMWallIndices;
                return true;
            case OneLineShape.SimpleN:
                data.indices = GablePersistentData.simpleNIndices;
                data.extend = GablePersistentData.simpleNExtend;
                data.wallIndices = GablePersistentData.simpleNWallIndices;
                return true;
            case OneLineShape.T:
                data.indices = GablePersistentData.tIndices;
                data.extend = GablePersistentData.tExtend;
                data.wallIndices = GablePersistentData.tWallIndices;
                return true;
            case OneLineShape.U:
                data.indices = GablePersistentData.uIndices;
                data.extend = GablePersistentData.uExtend;
                data.wallIndices = GablePersistentData.uWallIndices;
                return true;
            case OneLineShape.Y:
                data.indices = GablePersistentData.yIndices;
                data.extend = GablePersistentData.yExtend;
                data.wallIndices = GablePersistentData.yWallIndices;
                return true;
            case OneLineShape.X:
                data.indices = GablePersistentData.xIndices;
                data.extend = GablePersistentData.xExtend;
                data.wallIndices = GablePersistentData.xWallIndices;
                return true;
        }

        return false;
    }

    /// <summary>
    /// Returns false if the OneLineShape is a static shape.
    /// For static static shapes, use GetGableData function.
    /// </summary>
    /// <param name="shape"></param>
    /// <param name="shapeIndices"></param>
    /// <param name="shapeExtend"></param>
    /// <returns></returns>
    public static bool CalculateGableData(this OneLineShape shape, Vector3[] controlPoints, Vector3[] oneLine, out GableData data)
    {
        data = new GableData();

        switch (shape)
        {
            case OneLineShape.Antenna:
                {
                    if (!controlPoints.IsPolygonAntennaShaped(out int[] antPointsIndices))
                        return false;

                    List<List<int>> indices = new ();
                    List<List<bool>> extend = new ();
                    List<int> wallIndices = new();

                    // Middle Vertical
                    int current = controlPoints.GetControlPointIndex(antPointsIndices[0] - 4);
                    for(int i = 3; i < oneLine.Length-3; i+= 3)
                    {
                        int next = controlPoints.GetControlPointIndex(current + 1);
                        int nextTwo = controlPoints.GetControlPointIndex(current + 2);
                        int nextThree = controlPoints.GetControlPointIndex(current + 3);
                        
                        indices.Add(new List<int> { current, i+1, i, next });
                        extend.Add(new List<bool> { false, true, false, true });
                        //
                        indices.Add(new List<int> { nextTwo, i, i + 1, nextThree });
                        extend.Add(new List<bool> { false, true, true, false });
                        //
                        indices.Add(new List<int> { next, i, i, nextTwo });
                        extend.Add(new List<bool> { false, true, false, true });
                        wallIndices.Add(indices.Count - 1);

                        current = controlPoints.GetControlPointIndex(current - 4);
                    }

                    current = controlPoints.GetControlPointIndex(current - 3);

                    for (int i = oneLine.Length - 4; i > 3; i -= 3)
                    {
                        int previous = controlPoints.GetControlPointIndex(current - 1);
                        int previousTwo = controlPoints.GetControlPointIndex(current - 2);
                        int previousThree = controlPoints.GetControlPointIndex(current - 3);

                        indices.Add(new List<int> { previous, i, i - 1, current });
                        extend.Add(new List<bool> { false, true, true, false });
                        //
                        indices.Add(new List<int> { previousThree, i - 1, i, previousTwo });
                        extend.Add(new List<bool> { false, true, false, true });
                        //
                        indices.Add(new List<int> { previousTwo, i, i, previous });
                        extend.Add(new List<bool> { false, true, false, true });
                        wallIndices.Add(indices.Count - 1);
                        //

                        current = controlPoints.GetControlPointIndex(current - 4);
                    }

                    // Middle Horizontal
                    current = controlPoints.GetControlPointIndex(antPointsIndices[0]);
                    int nextFive = controlPoints.GetControlPointIndex(current + 5);
                    int index = 1;
                    int loop = (oneLine.Length / 3) - 1;
                    for (int i = 0; i < loop; i++)
                    {
                        int previous = controlPoints.GetControlPointIndex(current - 1);
                        int next = controlPoints.GetControlPointIndex(nextFive + 1);

                        indices.Add(new List<int> { previous, index + 3, index, current });
                        extend.Add(new List<bool> { false, true, false, false });
                        //
                        indices.Add(new List<int> { nextFive, index, index + 3, next });
                        extend.Add(new List<bool> { false, true, false, false });
                        //

                        current = controlPoints.GetControlPointIndex(current - 4);
                        nextFive = controlPoints.GetControlPointIndex(nextFive + 4);
                        index += 3;
                    }

                    // Ends
                    current = controlPoints.GetControlPointIndex(antPointsIndices[0]);
                    index = 0;

                    for (int i = 0; i < 2; i++)
                    {
                        int next = controlPoints.GetControlPointIndex(current + 1);
                        int nextTwo = controlPoints.GetControlPointIndex(current + 2);
                        int nextThree = controlPoints.GetControlPointIndex(current + 3);
                        int nextFour = controlPoints.GetControlPointIndex(current + 4);
                        nextFive = controlPoints.GetControlPointIndex(current + 5);

                        if (i == 0)
                        {
                            indices.Add(new List<int> { current, index + 1, index, next });
                            extend.Add(new List<bool> { false, true, false, true });
                            //
                            indices.Add(new List<int> { nextTwo, index, index + 2, nextThree });
                            extend.Add(new List<bool> { false, true, true, true });
                            //
                            indices.Add(new List<int> { nextFour, index + 2, index + 1, nextFive });
                            extend.Add(new List<bool> { false, true, true, false });
                            //
                            indices.Add(new List<int> { next, index, index, nextTwo });
                            extend.Add(new List<bool> { false, true, false, false });
                            wallIndices.Add(indices.Count - 1);
                            //
                            indices.Add(new List<int> { nextThree, index + 2, index + 2, nextFour });
                            extend.Add(new List<bool> { false, true, true, false });
                            wallIndices.Add(indices.Count - 1);
                            //
                        }
                        else
                        {
                            indices.Add(new List<int> { current, index + 1, index + 2, next });
                            extend.Add(new List<bool> { false, true, false, true });
                            //
                            indices.Add(new List<int> { nextTwo, index + 2, index, nextThree });
                            extend.Add(new List<bool> { false, true, true, true });
                            //
                            indices.Add(new List<int> { nextFour, index, index + 1, nextFive });
                            extend.Add(new List<bool> { false, true, true, false });
                            //
                            indices.Add(new List<int> { next, index + 2, index + 2, nextTwo });
                            extend.Add(new List<bool> { false, true, false, false });
                            wallIndices.Add(indices.Count - 1);
                            //
                            indices.Add(new List<int> { nextThree, index, index, nextFour });
                            extend.Add(new List<bool> { false, true, true, false });
                            wallIndices.Add(indices.Count - 1);
                            //
                        }

                        index += oneLine.Length - 3;
                        current = controlPoints.GetControlPointIndex(current + (controlPoints.Length / 2));
                    }

                    data.indices = indices.Select(list => list.ToUShort()).ToArray();
                    data.extend = extend.Select(list => list.ToArray()).ToArray();
                    data.wallIndices = wallIndices.ToUShort();
                }
                return true;
            case OneLineShape.Asterisk:
                {
                    if (!controlPoints.IsPolygonAsteriskShaped(out int[] asteriskPointIndices))
                        return false;

                    List<List<int>> indices = new();
                    List<List<bool>> extend = new();
                    List<int> wallIndices = new();

                    int count = oneLine.Length - 2;
                    for (ushort i = 0; i < asteriskPointIndices.Length; i++)
                    {
                        int current = asteriskPointIndices[i];
                        int previous = controlPoints.GetControlPointIndex(current - 1);
                        int next = controlPoints.GetControlPointIndex(current + 1);

                        indices.Add(new List<int> { current, oneLine.Length - 1, i, next });
                        extend.Add(new List<bool> { false, true, false, true });
                        //
                        indices.Add(new List<int> { previous, count, oneLine.Length - 1, current });
                        extend.Add(new List<bool> { false, true, true, false });
                        //
                        count++;

                        if (count > oneLine.Length - 2)
                        {
                            count = 0;
                        }

                        int nextTwo = controlPoints.GetControlPointIndex(current + 2);
                        indices.Add(new List<int> { next, i, i, nextTwo });
                        extend.Add(new List<bool> { false, true, false, true });
                        wallIndices.Add(indices.Count - 1);
                        //
                    }

                    data.indices = indices.Select(list => list.ToUShort()).ToArray();
                    data.extend = extend.Select(list => list.ToArray()).ToArray();
                }
                return true;
            case OneLineShape.Crenel:
                {
                    if (!controlPoints.IsPolygonCrenelShaped(out int[] crenelIndices))
                        return false;

                    List<List<int>> indices = new();
                    List<List<bool>> extend = new();
                    List<int> wallIndices = new();

                    int[] startIndices = controlPoints.RelativeIndices(crenelIndices[0]);
                    int[] endIndices = controlPoints.RelativeIndices(crenelIndices[^1]);

                    indices.Add(new List<int> { startIndices[^1], 0, 1, startIndices[0] });
                    extend.Add(new List<bool> { false, true, false, false });
                    //
                    indices.Add(new List<int> { startIndices[^3], 1, 0, startIndices[^2] });
                    extend.Add(new List<bool> { false, true, false, false });
                    //
                    indices.Add(new List<int> { startIndices[^4], oneLine.Length-2, 1, startIndices[^3] });
                    extend.Add(new List<bool> { false, true, false, false });
                    //
                    indices.Add(new List<int> { startIndices[^5], oneLine.Length - 1, oneLine.Length -2, startIndices[^4] });
                    extend.Add(new List<bool> { false, true, false, false });
                    //
                    indices.Add(new List<int> { endIndices[0], oneLine.Length - 2, oneLine.Length - 1, endIndices[1] });
                    extend.Add(new List<bool> { false, true, false, false });
                    //
                    indices.Add(new List<int> { startIndices[0], 1, 2, startIndices[1] });
                    extend.Add(new List<bool> { false, true, false, false });
                    //
                    indices.Add(new List<int> { startIndices[^2], 0, 0, startIndices[^1] });
                    extend.Add(new List<bool> { false, true, false, false });
                    wallIndices.Add(indices.Count-1);
                    //
                    indices.Add(new List<int> { endIndices[1], oneLine.Length - 1, oneLine.Length - 1, endIndices[2] });
                    extend.Add(new List<bool> { false, true, false, false });
                    wallIndices.Add(indices.Count - 1);
                    //

                    int fourCount = 0;
                    int twoCount = 0;

                    int length = ((crenelIndices.Length / 2) + 1) - 2;

                    for (int i = 0; i < length; i++)
                    {
                        indices.Add(new List<int> { startIndices[4 + fourCount], 2 + twoCount, 4 + twoCount, startIndices[5 + fourCount] });
                        extend.Add(new List<bool> { false, true, false, false });
                        //
                        indices.Add(new List<int> { startIndices[1 + fourCount], 2 + twoCount, 3 + twoCount, startIndices[2 + fourCount] });
                        extend.Add(new List<bool> { false, true, false, false });
                        //
                        indices.Add(new List<int> { startIndices[3 + fourCount], 3 + twoCount, 2 + twoCount, startIndices[4 + fourCount] });
                        extend.Add(new List<bool> { false, true, false, false });
                        //
                        indices.Add(new List<int> { startIndices[2 + fourCount], 3 + twoCount, 3 + twoCount, startIndices[3 + fourCount] });
                        extend.Add(new List<bool> { false, true, false, false });
                        wallIndices.Add(indices.Count - 1);
                        //

                        fourCount += 4;
                        twoCount += 2;
                    }

                    data.indices = indices.Select(list => list.ToUShort()).ToArray();
                    data.extend = extend.Select(list => list.ToArray()).ToArray();
                    data.wallIndices = wallIndices.ToUShort();
                }
                return true;
            case OneLineShape.ZigZag:
                {
                    if (!controlPoints.IsPolygonZigZagShaped(out int[] zigZagIndices))
                        return false;

                    List<List<int>> indices = new();
                    List<List<bool>> extend = new();
                    List<int> wallIndices = new();

                    int[] relIndices = controlPoints.RelativeIndices(zigZagIndices[0]);
                    int previous = relIndices[^1];
                    int start = relIndices[^3];
                    int end = relIndices[^2];

                    for (ushort i = 0; i < oneLine.Length - 1; i++)
                    {
                        if (i == 0)
                        {
                            indices.Add(new List<int> { previous, i, i + 1, relIndices[i] });
                            extend.Add(new List<bool> { false, true, true, false });
                            //
                            indices.Add(new List<int> { start, i + 1, i, end });
                            extend.Add(new List<bool> { false, true, false, true });
                            //
                            indices.Add(new List<int> { end, i, i, previous });
                            extend.Add(new List<bool> { false, true, false, false });
                            wallIndices.Add(wallIndices.Count-1);
                            //
                        }
                        else if (i == oneLine.Length - 2)
                        {
                            indices.Add(new List<int> { previous, i, i + 1, relIndices[i] });
                            extend.Add(new List<bool> { false, true, false, true });
                            //
                            indices.Add(new List<int> { start, i + 1, i, end });
                            extend.Add(new List<bool> { false, true, true, false });
                            //
                            indices.Add(new List<int> { previous, i + 1, i + 1, end - 1 });
                            extend.Add(new List<bool> { false, true, false, false });
                            wallIndices.Add(wallIndices.Count-1);
                            //
                        }
                        else
                        {
                            indices.Add(new List<int> { previous, i, i + 1, relIndices[i] });
                            extend.Add(new List<bool> { false, true, false, false });
                            //
                            indices.Add(new List<int> { start, i + 1, i, end });
                            extend.Add(new List<bool> { false, true, false, false });
                            //
                        }

                        previous = relIndices[i];
                        start--;
                        end--;
                    }

                    data.indices = indices.Select(list => list.ToUShort()).ToArray();
                    data.extend = extend.Select(list => list.ToArray()).ToArray();
                    data.wallIndices = wallIndices.ToUShort();
                }
                return true;
        }

        return false;
    }
}

[System.Serializable]
public struct GableData
{
    public ushort[][] indices;
    public bool[][] extend;
    public ushort[] wallIndices;

    public GableData(ushort[][] indices, bool[][] extend, ushort[] wallIndices)
    {
        this.indices = indices;
        this.extend = extend;
        this.wallIndices = wallIndices;

    }
}