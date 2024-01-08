using System.Collections.Generic;
using UnityEngine;

public class Vector3EqualityComparer : IEqualityComparer<Vector3>
{
    private float m_Tolerance;

    public Vector3EqualityComparer(float tolerance = 0.001f)
    {
        m_Tolerance = tolerance;
    }
    public bool Equals(Vector3 a, Vector3 b)
    {
        float distance = Vector3.Distance(a, b);
        bool result = distance < m_Tolerance;
        Debug.Log(distance);
        return result;
    }
    public int GetHashCode(Vector3 vector)
    {
        return vector.GetHashCode();
    }

}