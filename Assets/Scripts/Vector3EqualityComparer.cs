using System.Collections.Generic;
using UnityEngine;

public class Vector3EqualityComparer : IEqualityComparer<Vector3>
{
    private float tolerance;

    public Vector3EqualityComparer(float tolerance = 0.001f)
    {
        this.tolerance = tolerance;
    }

    public bool Equals(Vector3 v1, Vector3 v2)
    {
        return Vector3.Distance(v1, v2) < tolerance;
    }

    public int GetHashCode(Vector3 vector)
    {
        return vector.GetHashCode();
    }
}