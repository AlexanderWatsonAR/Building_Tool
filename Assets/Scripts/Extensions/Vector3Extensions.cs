using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder;

public static class Vector3Extensions
{

    public static Vector3 Average(this IEnumerable<Vector3> collection)
    {
        Vector3 average = Vector3.zero;
        foreach (Vector3 vector in collection)
        {
            average += vector;
        }
        average /= collection.Count();
        return average;
    }

    public static bool Approximately(Vector3 vector1, Vector3 vector2, float tolerance = 0.001f)
    {
        float sqrMagnitudeDifference = (vector1 - vector2).sqrMagnitude;
        float sqrTolerance = tolerance * tolerance;

        return sqrMagnitudeDifference <= sqrTolerance;
    }

    /// <summary>
    /// Returns a normalized vector to the target.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="targetPosition"></param>
    /// <returns></returns>
    public static Vector3 DirectionToTarget(this Vector3 position, Vector3 targetPosition)
    {
        return (targetPosition - position).normalized;
    }

    public static Vector3 DirectionToTarget(this ControlPoint controlPoint, ControlPoint targetControlPoint)
    {
        return (targetControlPoint.Position - controlPoint.Position).normalized;
    }

    /// <summary>
    /// Returns lerp points from 0 & 1 inclusive.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="numberOfSamples"></param>
    /// <returns></returns>
    public static Vector3[] LerpCollection(Vector3 a, Vector3 b, int numberOfSamples)
    {
        if (numberOfSamples < 2)
            numberOfSamples = 2;

        Vector3[] samplePoints = new Vector3[numberOfSamples];

        float index = 0;
        float count = (float)numberOfSamples / (numberOfSamples - 1);

        for (int i = 0; i < numberOfSamples; i++)
        {
            float t = index / (float)numberOfSamples;
            index += count;

            samplePoints[i] = Vector3.Lerp(a, b, t);
        }

        return samplePoints;
    }

    public static Vector3[] Concat(this Vector3[] collection, params Vector3[][] other)
    {
        List<Vector3> result = new List<Vector3>();

        result.AddRange(collection);

        foreach(var item in other)
        {
            result.AddRange(item);
        }

        return result.ToArray();
    }

    public static IEnumerable<Vector3> ApplyTransform(this IEnumerable<Vector3> points, Matrix4x4 matrix)
    {
        return points.Select(point => point = matrix.MultiplyPoint3x4(point));
    }

    public static Vector3[] Curve(int numberOfPoints)
    {
        return QuadraticLerpCollection(Vector3.zero, Vector3.right, Vector3.right + Vector3.up, numberOfPoints);
    }

    public static Vector3[] QuadraticLerpCollection(Vector3 a, Vector3 b, Vector3 c, int numberOfSamples)
    {
        Vector3[] ab = LerpCollection(a, b, numberOfSamples);
        Vector3[] bc = LerpCollection(b, c, numberOfSamples);

        Vector3[] samplePoints = new Vector3[numberOfSamples];

        float index = 0;
        float count = (float)numberOfSamples / (numberOfSamples - 1);

        for (int i = 0; i < numberOfSamples; i++)
        {
            float t = index / (float)numberOfSamples;
            index += count;

            samplePoints[i] = Vector3.Lerp(ab[i], bc[i], t);
        }

        return samplePoints;
    }

    public static float InverseLerp(Vector3 a, Vector3 b, Vector3 value)
    {
        return Vector3.Distance(a, value) / Vector3.Distance(a, b);
    }

    public static Vector3[] ShiftIndices(this IEnumerable<Vector3> controlPoints)
    {
        Vector3[] points = controlPoints.ToArray();
        Vector3[] temp = points.Clone() as Vector3[];

        for (int i = 0; i < points.Length; i++)
        {
            int index = points.GetNextControlPoint(i);
            temp[i] = points[index];
        }

        return temp;
    }

    public static IEnumerable<Vector3> Distinct(this IEnumerable<Vector3> points, float tolerance = 0.001f)
    {
        List<Vector3> distinctList = new List<Vector3>();
        foreach (Vector3 vector in points)
        {
            bool isDuplicate = false;
            foreach (Vector3 distinctVector in distinctList)
            {
                if (Vector3.Distance(vector, distinctVector) < tolerance)
                {
                    isDuplicate = true;
                    break;
                }
            }
            if (!isDuplicate)
            {
                distinctList.Add(vector);
            }
        }
        return distinctList;
    }
}
