using System.Collections;
using System.Drawing;
using System.Linq;
using UnityEngine;
using System;
using System.Collections.Generic;

public static class PolygonMaker
{
    public enum PivotPoint
    {
        Centre, BottomLeft
    }

    public static Vector3[] NPolygon(int sides, float width = 1, float height = 1)
    {
        float angle = 360f / sides;

        Vector3[] polygon = new Vector3[sides];

        float hWidth = width * 0.5f;
        float hHeight = height * 0.5f;

        for (int i = 0; i < sides; i++)
        {
            float x = Mathf.Sin(Mathf.Deg2Rad * (angle * i)) * hWidth;
            float y = Mathf.Cos(Mathf.Deg2Rad * (angle * i)) * hHeight;

            polygon[i] = new Vector3(x, y);
        }

        return polygon;
    }
    public static Vector3[] Square(PivotPoint pivot = PivotPoint.Centre, float width = 1, float height = 1)
    {
        float hWidth = width * 0.5f;
        float hHeight = height * 0.5f;

        switch (pivot)
        {
            case PivotPoint.Centre:
                return new Vector3[]
                {
                    new Vector3(-hWidth, -hHeight),
                    new Vector3(-hWidth, hHeight),
                    new Vector3(hWidth, hHeight),
                    new Vector3(hWidth, -hHeight)
                };
            case PivotPoint.BottomLeft:
                return new Vector3[]
                {
                    new Vector3(0, 0),
                    new Vector3(0, height),
                    new Vector3(width, height),
                    new Vector3(width, 0)
                };
        }

        return null;
    }
    public static IList<Vector3[]> Grid(Vector2Int dimensions)
    {
        return Grid(dimensions.x, dimensions.y);
    }
    public static IList<Vector3[]> Grid(int columns, int rows)
    {
        IList<Vector3[]> squares = new List<Vector3[]>();

        float xSize = 1 / (float)columns;
        float ySize = 1 / (float)rows;

        Vector3 offset = new Vector3(-0.5f, -0.5f);

        for (float x = 0; x < columns; x++)
        {
            for (float y = 0; y < rows; y++)
            {
                float xPos = x * xSize;
                float yPos = y * ySize;

                Vector3[] square = Square(PivotPoint.BottomLeft, xSize, ySize);

                Matrix4x4 translation = Matrix4x4.Translate(new Vector3(xPos, yPos)) * Matrix4x4.Translate(offset);

                for (int i = 0; i < square.Length; i++)
                {
                    square[i] = translation.MultiplyPoint3x4(square[i]);
                }

                squares.Add(square);
            }
        }

        return squares;
    }
    public static Vector3[] L()
    {
        return new Vector3[]
        {
            new Vector3(0, 0),
            new Vector3(0, 1),
            new Vector3(0.1f, 1),
            new Vector3(0.1f, 0.1f),
            new Vector3(1, 0.1f),
            new Vector3(1, 0)
        };
    }
    public static Vector3[] RoundedSquare(float curveSize = 0.1f, int numberOfCurvePoints = 5, float height = 1, float width = 1)
    {
        Vector3[] controlPoints = MeshMaker.Square();

        Vector3 aNext = Vector3.Lerp(controlPoints[0], controlPoints[1], curveSize);
        Vector3 bNext = Vector3.Lerp(controlPoints[1], controlPoints[2], curveSize);
        Vector3 cNext = Vector3.Lerp(controlPoints[2], controlPoints[3], curveSize);
        Vector3 dNext = Vector3.Lerp(controlPoints[3], controlPoints[0], curveSize);

        Vector3 aPrev = Vector3.Lerp(controlPoints[0], controlPoints[3], curveSize);
        Vector3 bPrev = Vector3.Lerp(controlPoints[1], controlPoints[0], curveSize);
        Vector3 cPrev = Vector3.Lerp(controlPoints[2], controlPoints[1], curveSize);
        Vector3 dPrev = Vector3.Lerp(controlPoints[3], controlPoints[2], curveSize);


        Vector3[] bl = Vector3Extensions.QuadraticLerpCollection(aPrev, controlPoints[0], aNext, numberOfCurvePoints);
        Vector3[] tl = Vector3Extensions.QuadraticLerpCollection(bPrev, controlPoints[1], bNext, numberOfCurvePoints);
        Vector3[] tr = Vector3Extensions.QuadraticLerpCollection(cPrev, controlPoints[2], cNext, numberOfCurvePoints);
        Vector3[] br = Vector3Extensions.QuadraticLerpCollection(dPrev, controlPoints[3], dNext, numberOfCurvePoints);

        Vector3 size = new Vector3(width, height);

        Matrix4x4 scale = Matrix4x4.Scale(size);

        return bl.Concat(tl, tr, br).ApplyTransform(scale).ToArray();
    }
    public static Vector3[] Arch(float baseHeight, float archHeight, int sides)
    {
        Vector3[] square = Square();

        int bl = 0;
        int tl = 1;
        int tr = 2;
        int br = 3;

        Vector3 bHeight = Vector3.up * baseHeight;
        Vector3 aHeight = Vector3.up * archHeight;
        Vector3 midPoint = Vector3.Lerp(square[tl],  square[tr], 0.5f);

        List<Vector3> controlPoints = new List<Vector3>();
        controlPoints.Add(square[bl]);
        controlPoints.AddRange(Vector3Extensions.QuadraticLerpCollection(square[tl] + bHeight, midPoint + bHeight + aHeight, square[tr] + bHeight, sides + 1));
        controlPoints.Add(square[br]);

        return controlPoints.ToArray();
    }
    public static Vector3[] Semicircle()
    {
        return Vector3Extensions.QuadraticLerpCollection(new Vector3(-0.5f, 0), new Vector3(0, 0.5f), new Vector3(0, 0.5f), 10);
    }
    public static Vector3[] Quatercircle()
    {
        Vector3[] square = Square();

        int bl = 0;
        int tl = 1;
        int tr = 2;
        int br = 3;

        List<Vector3> controlPoints = new List<Vector3>();
        controlPoints.AddRange(Vector3Extensions.QuadraticLerpCollection(square[bl], square[tl], square[tr], 10));
        controlPoints.Add(square[br]);

        return controlPoints.ToArray();
    }

    

}