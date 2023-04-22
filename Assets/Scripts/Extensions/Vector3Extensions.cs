using System.Collections;
using System.Collections.Generic;
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
    /// <summary>
    /// Returns a normalized vector to the target.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="targetPosition"></param>
    /// <returns></returns>
    public static Vector3 GetDirectionToTarget(this Vector3 position, Vector3 targetPosition)
    {
        return (targetPosition - position).normalized;
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

}
