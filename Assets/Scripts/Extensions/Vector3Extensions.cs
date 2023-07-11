using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder;

public static class Vector3Extensions
{
    // https://answers.unity.com/questions/950927/compare-vector3.html
    // With absolute value
    public static bool Approximately(this Vector3 vector, Vector3 other, float allowedDifference)
    {
        var dx = vector.x - other.x;
        if (Mathf.Abs(dx) > allowedDifference)
            return false;

        var dy = vector.y - other.y;
        if (Mathf.Abs(dy) > allowedDifference)
            return false;

        var dz = vector.z - other.z;

        return Mathf.Abs(dz) >= allowedDifference;
    }

    public static bool Approximately0(Vector3 vector1, Vector3 vector2, float tolerance)
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
}
