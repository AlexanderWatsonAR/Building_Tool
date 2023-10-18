using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GableData
{
    // How the arrays work:

    // "new int[] { 0, 1, 0, 1 }"
    // In the above example, the first & last points reference the control points / path points of the building.
    // The middle 2 points reference the one line points.

    // Indices
    public static readonly int[][] lIndices = new int[][]
    {
        new int[] { 0, 1, 0, 1 },
        new int[] { 2, 0, 1, 3 },
        new int[] { 5, 2, 1, 0 },
        new int[] { 3, 1, 2, 4 },
        new int[] { 4, 2, 2, 5 },
        new int[] { 1, 0, 0, 2 },
    };
    public static readonly int[][] arrowIndices = new int[][]
    {
        new int[] { 8, 0, 3, 0 },
        new int[] { 6, 3, 0, 7 },
        new int[] { 5, 2, 3, 6 },
        new int[] { 3, 3, 2, 4 },
        new int[] { 2, 1, 3, 3 },
        new int[] { 0, 3, 1, 1 },
        new int[] { 7, 0, 0, 8 },
        new int[] { 1, 1, 1, 2 },
        new int[] { 4, 2, 2, 5 }
    };
    public static readonly int[][] simpleNIndices = new int[][]
    {
        new int[] { 0, 1, 2, 1 },
        new int[] { 1, 2, 3, 2 },
        new int[] { 3, 3, 2, 4 },
        new int[] { 4, 2, 1, 5 },
        new int[] { 5, 1, 0, 6 },
        new int[] { 7, 0, 1, 0 },
        new int[] { 2, 3, 3, 3 },
        new int[] { 6, 0, 0, 7 }
    };
    public static readonly int[][] tIndices = new int[][]
    {
        new int[] { 0, 3, 0, 1 },
        new int[] { 2, 0, 3, 3 },
        new int[] { 3, 3, 2, 4 },
        new int[] { 5, 2, 1, 6 },
        new int[] { 7, 1, 3, 0 },
        new int[] { 1, 0, 0, 2 },
        new int[] { 6, 1, 1, 7 },
        new int[] { 4, 2, 2, 5 }
    };
    public static readonly int[][] uIndices = new int[][]
    {
        new int[] { 0, 1, 2, 1 },
        new int[] { 1, 2, 3, 2 },
        new int[] { 3, 3, 2, 4 },
        new int[] { 4, 2, 1, 5 },
        new int[] { 5, 1, 0, 6 },
        new int[] { 7, 0, 1, 0 },
        new int[] { 2, 3, 3, 3 },
        new int[] { 6, 0, 0, 7 }
    };
    public static readonly int[][] yIndices = new int[][]
    {
        new int[] { 0, 3, 0, 1 },
        new int[] { 2, 0, 3, 3 },
        new int[] { 3, 3, 1, 4 },
        new int[] { 5, 1, 3, 6 },
        new int[] { 6, 3, 2, 7 },
        new int[] { 8, 2, 3, 0 },
        new int[] { 1, 0, 0, 2 },
        new int[] { 4, 1, 1, 5 },
        new int[] { 7, 2, 2, 8 }
    };
    public static readonly int[][] fIndices = new int[][]
    {
        new int[] { 9, 0, 1, 0 },
        new int[] { 0, 1, 2, 1 },
        new int[] { 2, 2, 1, 3 },
        new int[] { 3, 1, 3, 4 },
        new int[] { 4, 3, 4, 5 },
        new int[] { 6, 4, 3, 7 },
        new int[] { 7, 3, 0, 8 },
        new int[] { 8, 0, 0, 9 },
        new int[] { 1, 2, 2, 2 },
        new int[] { 5, 4, 4, 6 }
    };
    public static readonly int[][] simpleMIndices = new int[][]
    {
        new int[] { 0, 1, 2, 1 },
        new int[] { 1, 2, 3, 2 },
        new int[] { 2, 3, 4, 3 },
        new int[] { 4, 4, 3, 5 },
        new int[] { 5, 3, 2, 6 },
        new int[] { 6, 2, 1, 7 },
        new int[] { 7, 1, 0, 8 },
        new int[] { 9, 0, 1, 0 },
        new int[] { 3, 4, 4, 4 },
        new int[] { 8, 0, 0, 9 }
    };
    public static readonly int[][] nIndices = new int[][]
    {
        new int[] { 0, 1, 2, 0 },
        new int[] { 0, 2, 3, 1 },
        new int[] { 1, 3, 4, 2 },
        new int[] { 2, 4, 5, 3 },
        new int[] { 4, 5, 4, 5 },
        new int[] { 5, 3, 2, 6 },
        new int[] { 5, 4, 3, 5 },
        new int[] { 6, 2, 1, 7 },
        new int[] { 7, 1, 0, 8 },
        new int[] { 9, 0, 1, 0 },
        new int[] { 8, 0, 0, 9 },
        new int[] { 3, 5, 5, 4 }
    };
    public static readonly int[][] eIndices = new int[][]
    {
        new int[] { 0, 1, 4, 1 },
        new int[] { 1, 4, 5, 2 },
        new int[] { 3, 5, 4, 4 },
        new int[] { 4, 4, 2, 5 },
        new int[] { 5, 2, 3, 6 },
        new int[] { 7, 3, 2, 8 },
        new int[] { 8, 2, 1, 9 },
        new int[] { 9, 1, 0, 10 },
        new int[] { 11, 0, 1, 0 },
        new int[] { 10, 0, 0, 11 },
        new int[] { 2, 5, 5, 3 },
        new int[] { 6, 3, 3, 7 }
    };
    public static readonly int[][] hIndices = new int[][]
    {
        new int[] { 11, 0, 1, 0 },
        new int[] { 0, 1, 4, 1 },
        new int[] { 1, 4, 5, 2 },
        new int[] { 3, 5, 3, 4 },
        new int[] { 5, 3, 4, 6 },
        new int[] { 6, 4, 1, 7 },
        new int[] { 7, 1, 2, 8 },
        new int[] { 9, 2, 0, 10 },
        new int[] { 10, 0, 0, 11 },
        new int[] { 8, 2, 2, 9 },
        new int[] { 4, 3, 3, 5 },
        new int[] { 2, 5, 5, 3 }
    };
    public static readonly int[][] kIndices = new int[][]
    {
        new int[] { 11, 0, 1, 0 },
        new int[] { 0, 1, 5, 1 },
        new int[] { 1, 5, 4, 2 },
        new int[] { 3, 4, 5, 4 },
        new int[] { 4, 5, 3, 5 },
        new int[] { 6, 3, 1, 7 },
        new int[] { 7, 1, 2, 8 },
        new int[] { 9, 2, 0, 10 },
        new int[] { 10, 0, 0, 11 },
        new int[] { 8, 2, 2, 9 },
        new int[] { 5, 3, 3, 6 },
        new int[] { 2, 4, 4, 3 }
    };
    public static readonly int[][] mIndices = new int[][]
    {
        new int[] { 0, 1, 2, 0 },
        new int[] { 0, 2, 3, 1 },
        new int[] { 1, 3, 4, 2 },
        new int[] { 2, 4, 5, 2 },
        new int[] { 2, 5, 6, 3 },
        new int[] { 4, 6, 5, 5 },
        new int[] { 5, 5, 4, 6 },
        new int[] { 6, 4, 3, 7 },
        new int[] { 7, 3, 2, 8 },
        new int[] { 8, 2, 1, 9 },
        new int[] { 9, 1, 0, 10 },
        new int[] { 11, 0, 1, 0 },
        new int[] { 3, 6, 6, 4 },
        new int[] { 10, 0, 0, 11 }
    };

    public static readonly int[][] interYIndices = new int[][]
    {
        new int[] { 7, 0, 1, 8},
        new int[] { 5, 1, 0, 6},
        new int[] { 8, 1, 2, 9},
        new int[] { 0, 2, 3, 1},
        new int[] { 1, 3, 4, 2},
        new int[] { 3, 4, 3, 4},
        new int[] { 5, 1, 3, 4},
        new int[] { 9, 2, 5, 10},
        new int[] { 11, 5, 2, 0},
        new int[] { 6, 0, 0, 7},
        new int[] { 2, 4, 4, 3},
        new int[] { 10, 5, 5, 11}
    };

    // Extensions
    // Height Start, Height End, Width Start, Width End
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

}
